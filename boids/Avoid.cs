namespace boids
{
    public class Avoid
    {
        public IBoidHandler Actor { get; }

        public Avoid(IBoidHandler actor)
        {
            Actor = actor;
        }
    }
}
