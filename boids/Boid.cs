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
        private static Random Random = new Random();

        private BoidParameter _configuration;

        public IBoidHandler Handler { get; }

        public Boid(BoidParameter configuration, IBoidHandler handler) 
        {
            _configuration = configuration;

            Handler = handler;
        }

        public void Update(IReadOnlyList<Boid> boids, IReadOnlyList<Avoid> avoids)
        {
            var nearby = new List<Boid>();

            foreach (var boid in boids)
            {
                if (boid == this) continue;
                if (MathF.Abs(boid.GetLocation().X - this.GetLocation().X) < _configuration.FriendRadius &&
                    MathF.Abs(boid.GetLocation().Y - this.GetLocation().Y) < _configuration.FriendRadius)
                {
                    nearby.Add(boid);
                }
            }

            UpdateGroupId(nearby);

            var delta = CalculateNextMovement(nearby, avoids);

            this.UpdateMovement(delta);
        }

        private Vector3 CalculateNextMovement(IReadOnlyList<Boid> friends, IReadOnlyList<Avoid> avoids)
        {
            var allign = GetAverageDir(friends);
            var avoidDir = GetAvoidDir(friends);
            var avoidObjects = GetAvoidAvoids(avoids);
            var cohese = GetCohesion(friends);

            // NOTE: Ignroe noise of 'z'.
            var noise = new Vector3(Random.NextSingle() * 2 - 1, Random.NextSingle() * 2 - 1, 0f);

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

            var nextMovement = this.GetDirection();
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

            var speedMag = 3.0f;
            return nextMovement * speedMag;
        }

        Vector3 GetAverageDir(IReadOnlyList<Boid> friends)
        {
            var sum = Vector3.Zero;

            foreach (var boid in friends)
            {
                var dist = this.GetDistance(boid);

                if (dist > 0f && (dist < _configuration.FriendRadius))
                {
                    var delta = boid.GetDirection();
                    delta /= dist;

                    sum += delta;
                }
            }

            return sum;
        }

        Vector3 GetAvoidDir(IReadOnlyList<Boid> friends)
        {
            var steer = Vector3.Zero;

            foreach (var boid in friends)
            {
                var dist = this.GetDistance(boid);

                if (dist > 0 && dist < _configuration.CrowdRadius)
                {
                    // Calculate vector pointing away from neighbor
                    var diff = this.GetLocation() - boid.GetLocation();
                    diff = Vector3.Normalize(diff);
                    diff /= dist; // Weight by distance

                    steer += diff;
                }
            }

            return steer;
        }

        Vector3 GetAvoidAvoids(IReadOnlyList<Avoid> avoids)
        {
            var steer = Vector3.Zero;

            foreach (var avoid in avoids)
            {
                var dist = this.GetDistance(avoid);

                if (dist > 0 && dist < _configuration.AvoidRadius)
                {
                    // Calculate vector pointing away from neighbor
                    var diff = this.GetLocation() - avoid.GetLocation();
                    diff = Vector3.Normalize(diff);
                    diff /= dist; // Weight by distance

                    steer += diff;
                }
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

                if (dist > 0 && dist < _configuration.CoheseRadius)
                {
                    sum += boid.GetLocation(); // Add location
                    count++;
                }
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

        private void UpdateGroupId(IReadOnlyList<Boid> friends)
        {
            static Single GetAverageColor(IReadOnlyList<Boid> friends, Int32 groupId)
            {
                var total = 0;
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

                return total / (Single)count;
            }

            Single groupId = this.GetGroupId();
            groupId += GetAverageColor(friends, (Int32)groupId) * 0.03f;
            // Give change to split new group
            groupId += (Random.NextSingle() * 2 - 1);
            this.SetGroupId((Int32)((groupId + 255) % 255));
        }
    }
}