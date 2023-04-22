using System.Numerics;

namespace boids
{
    internal static class ExtensionMethod
    {
        internal static Vector3 GetLocation(this Boid a) => a.Handler.Location;
        internal static void SetLocation(this Boid a, Vector3 location)
        {
            a.Handler.Location = location;
        }

        internal static Vector3 GetLocation(this Avoid a) => a.Handler.Location;

        internal static Vector3 GetDirection(this Boid a) => a.Handler.Direction;
        internal static void SetDirection(this Boid a, Vector3 direction)
        {
            a.Handler.Direction = direction;
        }

        internal static Int32 GetGroupId(this Boid a) => a.Handler.GroupId;
        internal static void SetGroupId(this Boid a, Int32 id)
        {
            a.Handler.GroupId = id;
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
