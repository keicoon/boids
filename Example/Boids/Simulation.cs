using System.Numerics;
using boids;
using static Example.Boids.IDrawHandler;

namespace Example.Boids
{
    public class Simulation
    {
        internal static Size WorldSize = new Size(800, 400);

        private BoidParameter _configuration;

        private List<Boid> _boids = new ();
        private List<Avoid> _avoid = new ();

        public Simulation()
        {
            _configuration = new BoidParameter()
            {
                FriendRadius = 60f,
                CrowdRadius = 60f / 1.3f,
                AvoidRadius = 40f,
                CoheseRadius = 40f,
                MaxSpeed = 2.1f,

                UsingFriend = true,
                UsingCrowd = true,
                UsingAvoid = true,
                UsingNoise = false,
                UsingCohese = true
            };

            Setup();
        }

        // @hack : Ignore polymorphism issue
        public IEnumerable<Actor> Actors => 
            _boids.Select(boid => (Actor)boid.Handler).Concat(_avoid.Select(avoid => (Actor)avoid.Handler));

        public void Update()
        {
            foreach(var boid in _boids)
            {
                boid.Update(_boids, _avoid);
            }
        }

        public void Setup()
        {
            SetupWalls();
        }

        public void AddBoid(Vector3 location)
        {
            var actor = new Actor(new BoidDrawHandler() { Radius = 15 }, location);

            _boids.Add(new Boid(_configuration, actor));
        }

        public void AddAvoid(Vector3 location)
        {
            var actor = new Actor(new AvoidCircleDrawHandler() { Radius = 8 }, location);

            _avoid.Add(new Avoid(actor));
        }

        void SetupWalls()
        {
            for (var x = 20; x < WorldSize.Width; x += 20)
            {
                AddAvoid(new Vector3(x, 20, 0));
                AddAvoid(new Vector3(x, WorldSize.Height - 20, 0));
            }
        }
    }
}
