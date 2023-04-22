using System.Numerics;
using boids;

namespace Example.Boids
{
    public class Actor : IBoidHandler
    {
        public static Random Random = new Random();

        public IDrawHandler DrawHandler { get; }

        private Vector3 _location;
        public Vector3 Location
        {
            get { return _location; }
            set
            {
                // Wrap position to WorldSize
                _location = new Vector3(
                    (value.X + Simulation.WorldSize.Width) % Simulation.WorldSize.Width,
                    (value.Y + Simulation.WorldSize.Height) % Simulation.WorldSize.Height,
                    0f
                );
            }
        }
        public Vector3 Direction { get; set; }
        public Int32 GroupId { get; set; }

        public Actor(IDrawHandler drawHandler, Vector3 initLocation)
        {
            DrawHandler = drawHandler;

            Location = initLocation;

            var radian = MathF.PI * Random.NextSingle();
            Direction = new Vector3(MathF.Cos(radian), MathF.Sin(radian), 0f); 
            
            GroupId = Random.Next(255);
        }

        public void Draw(Graphics gra)
        {
            DrawHandler.Draw(gra, this);
        }
    }
}
