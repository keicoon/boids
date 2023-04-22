using System.Numerics;

namespace boids
{
    public interface IBoidHandler
    {
        /// <summary>
        /// Position
        /// </summary>
        Vector3 Location { get; set; }
        /// <summary>
        /// Forward direction
        /// </summary>
        Vector3 Direction { get; set; }
        /// <summary>
        /// Group ( friend ) id
        /// </summary>
        Int32 GroupId { get; set; }
    }
}
