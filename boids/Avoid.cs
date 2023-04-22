namespace boids
{
    public class Avoid
    {
        public IBoidHandler Handler { get; }

        public Avoid(IBoidHandler handler)
        {
            Handler = handler;
        }
    }
}
