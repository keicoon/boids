/* 2023-04-22
 * Get algorithm from 'https://github.com/jackaperkins/boids'.
 * Thanks 'jackaperkins'.
 */

using System.Numerics;

namespace boids
{
    public readonly struct BoidParameter
    {
        /// <summary>
        /// Collect by radius that find nearest boids
        /// </summary>
        public Single FriendRadius { get; init; }
        /// <summary>
        /// Collect by radius that avoid other boids
        /// </summary>
        public Single CrowdRadius { get; init; }
        /// <summary>
        /// Radius that avoid other boids
        /// </summary>
        public Single AvoidRadius { get; init; }
        /// <summary>
        /// TODO:
        /// </summary>
        public Single CoheseRadius { get; init; }
        /// <summary>
        /// Limit boid movement size by one cycle
        /// </summary>
        public Single MaxSpeed { get; init; }

        // Using options
        public Boolean UsingFriend { get; init; }
        public Boolean UsingCrowd { get; init; }
        public Boolean UsingAvoid { get; init; }
        public Boolean UsingNoise { get; init; }
        public Boolean UsingCohese { get; init; }
    }

    public class Boid
    {
        BoidParameter _configuration;

        public IBoidHandler Actor { get; }

        public Boid(BoidParameter configuration, IBoidHandler actor) 
        {
            _configuration = configuration;

            Actor = actor;

            Actor.GroupId = Utils.Random.Next(255);
        }

        private IReadOnlyList<Boid> _friends = new List<Boid>();
        const Int32 SkipUpdateOfFriends = 5;
        private Int32 _counter = 0;

        public void Update(IReadOnlyList<Boid> boids, IReadOnlyList<Avoid> avoids)
        {
            Boolean IsSkipUpdateOfFriends()
            {
                _counter = (++_counter) % SkipUpdateOfFriends;
                return _counter == 0;
            }

            if (IsSkipUpdateOfFriends())
            {
                var newFriends = new List<Boid>();
                foreach (var boid in boids)
                {
                    if (boid == this) continue; // skip self
                    if (!this.InRange(boid, _configuration.FriendRadius)) continue;

                    newFriends.Add(boid);
                }
                _friends = newFriends;
            }

            var nearby = _friends;

            Single groupId = this.GetGroupId();
            groupId += GetAverageColor(nearby, (Int32)groupId) * 0.03f;
            groupId += Utils.Random.NextSingle();

            this.SetGroupId(((Int32)groupId + 255) % 255);

            var delta = CalculateNextMovement(nearby, avoids);
            this.UpdateMovement(delta * 10f);
        }

        private Vector3 CalculateNextMovement(IReadOnlyList<Boid> friends, IReadOnlyList<Avoid> avoids)
        {
            var allign = GetAverageDir(friends);
            var avoidDir = GetAvoidDir(friends);
            var avoidObjects = GetAvoidAvoids(avoids);
            var cohese = GetCohesion(friends);

            // NOTE: Ignroe noise of 'z'.
            var noise = new Vector3(Utils.Random.NextSingle(), Utils.Random.NextSingle(), 0f);

            allign *= 1f;
            if (!_configuration.UsingFriend) allign *= 0f;
            
            avoidDir *= 1f;
            if (!_configuration.UsingCrowd) avoidDir *= 0f;
            
            avoidObjects *= 3f;
            if (!_configuration.UsingAvoid) avoidObjects *= 0f;
            
            cohese *= 1;
            if (!_configuration.UsingCohese) cohese *= 0f;
            
            noise *= 0.1f;
            if (!_configuration.UsingNoise) noise *= 0f;

            var nextMovement = Vector3.Zero;
            nextMovement += allign;
            nextMovement += avoidDir;
            nextMovement += avoidObjects;
            nextMovement += cohese;
            nextMovement += noise;

            if (nextMovement.Length() > _configuration.MaxSpeed)
            {
                nextMovement = Vector3.Lerp(Vector3.Zero, nextMovement, _configuration.MaxSpeed / nextMovement.Length());
            }

            this.SetDirection(Vector3.Zero == nextMovement ? Vector3.Zero : Vector3.Normalize(nextMovement));

            return nextMovement;
        }

        Vector3 GetAverageDir(IReadOnlyList<Boid> friends)
        {
            var sum = Vector3.Zero;

            foreach (var boid in friends)
            {
                var dist = this.GetDistance(boid);

                // Skip that check 'FriendRadius'.

                var delta = boid.GetDirection();
                delta /= dist;

                sum += delta;
            }

            return sum;
        }

        Vector3 GetAvoidDir(IReadOnlyList<Boid> friends)
        {
            var steer = Vector3.Zero;

            foreach (var boid in friends)
            {
                var dist = this.GetDistance(boid);

                if (dist > _configuration.CrowdRadius) continue;

                // Calculate vector pointing away from neighbor
                var diff = this.GetLocation() - boid.GetLocation();
                diff = Vector3.Normalize(diff);
                diff /= dist; // Weight by distance

                steer += diff;
            }

            return steer;
        }

        Vector3 GetAvoidAvoids(IReadOnlyList<Avoid> avoids)
        {
            var steer = Vector3.Zero;

            foreach (var avoid in avoids)
            {
                var dist = this.GetDistance(avoid);

                if (dist > _configuration.AvoidRadius) continue;

                // Calculate vector pointing away from neighbor
                var diff = this.GetLocation() - avoid.GetLocation();
                diff = Vector3.Normalize(diff);
                diff /= dist; // Weight by distance

                steer += diff;
            }

            return steer;
        }

        Vector3 GetCohesion(IReadOnlyList<Boid> friends)
        {
            var sum = Vector3.Zero;   // Start with empty vector to accumulate all locations
            var count = 0;

            foreach (var boid in friends)
            {
                var dist = this.GetDistance(boid);

                if (dist > _configuration.CoheseRadius) continue;

                sum += boid.GetLocation(); // Add location
                count++;
            }

            if (count > 0)
            {
                sum /= count;

                var desired = sum - this.GetLocation();
                
                var normal = Vector3.Normalize(desired);
                return normal * 0.05f;
            }
            else
            {
                return Vector3.Zero;
            }
        }
        private Single GetAverageColor(IReadOnlyList<Boid> friends, Int32 groupId)
        {
            var total = 0f;
            var count = 0;

            foreach (var boid in friends)
            {
                var otherGroupId = boid.GetGroupId();

                if (otherGroupId - groupId < -128)
                {
                    total += otherGroupId + 255 - groupId;
                }
                else if (otherGroupId - groupId > 128)
                {
                    total += otherGroupId - 255 - groupId;
                }
                else
                {
                    total += otherGroupId - groupId;
                }

                count++;
            }

            if (count == 0) return 0;

            return total / count;
        }
    }
}