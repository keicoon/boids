using System.Numerics;

namespace boids
{
    internal static class Utils
    {
        static Random Random = new Random();

        public static Int32 Next(Int32 maxValue) => Random.Next(maxValue);
        public static Single NextSingle() => Random.NextSingle();
    }

    internal static class ExtensionMethod
    {
        internal static Vector3 GetLocation(this Boid a) => a.Actor.Location;
        internal static void SetLocation(this Boid a, Vector3 location)
        {
            a.Actor.Location = location;
        }

        internal static Vector3 GetLocation(this Avoid a) => a.Actor.Location;

        internal static Vector3 GetDirection(this Boid a) => a.Actor.Direction;
        internal static void SetDirection(this Boid a, Vector3 direction)
        {
            a.Actor.Direction = direction;
        }

        internal static Int32 GetGroupId(this Boid a) => a.Actor.GroupId;
        internal static void SetGroupId(this Boid a, Int32 id)
        {
            a.Actor.GroupId = id;
        }

        internal static Single GetDistance(this Boid a, Boid b)
        {
            return (a.GetLocation() - b.GetLocation()).Length();
        }

        internal static Single GetDistance(this Boid a, Avoid b)
        {
            return (a.GetLocation() - b.GetLocation()).Length();
        }

        internal static void UpdateMovement(this Boid a, Vector3 delta)
        {
            a.SetLocation(a.GetLocation() + delta);
        }
    }
}
