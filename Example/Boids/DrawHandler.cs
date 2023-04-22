using System.Numerics;

namespace Example.Boids
{
    public interface IDrawHandler
    {
        void Draw(Graphics gra, Actor actor);

        public class BoidDrawHandler : IDrawHandler
        {
            public Single Radius { get; init; }

            void IDrawHandler.Draw(Graphics gra, Actor actor)
            {
                static PointF GetPointF(Vector3 p) => new PointF(p.X, p.Y);
                var loc = actor.Location;
                var dir = actor.Direction;

                // Skip 'Z'
                var radian = MathF.Atan2(dir.Y, dir.X);
                var head = new Vector3(loc.X + Radius * MathF.Cos(radian), loc.Y + Radius * MathF.Sin(radian), 0f);
                // Rotate degree '60'
                var rot = MathF.PI / 180 * 60;
                var right = new Vector3(loc.X + Radius * 0.25f * MathF.Cos(radian + rot), loc.Y + Radius * 0.25f * MathF.Sin(radian + rot), 0f);
                var left = new Vector3(loc.X + Radius * 0.25f * MathF.Cos(radian - rot), loc.Y + Radius * 0.25f * MathF.Sin(radian - rot), 0f);

                var brush = new SolidBrush(Color.FromArgb(actor.GroupId, 90, 200));
                gra.FillPolygon(brush, new PointF[] { GetPointF(head), GetPointF(right), GetPointF(left) });
            }
        }

        public class AvoidRectDrawHandler : IDrawHandler
        {
            static Brush Brush = new SolidBrush(Color.Red);

            public Int32 Width { get; init; }
            public Int32 Hegiht { get; init; }

            void IDrawHandler.Draw(Graphics gra, Actor actor)
            {
                static Rectangle GetRectangle(Single x, Single y, Int32 width, Int32 height) =>
                    new Rectangle((Int32)x, (Int32)y, width, height);

                var loc = actor.Location;

                gra.FillRectangle(Brush, GetRectangle(loc.X - Width * 0.5f, loc.Y - Hegiht * 0.5f, Width, Hegiht));
            }
        }

        public class AvoidCircleDrawHandler : IDrawHandler
        {
            static Brush Brush = new SolidBrush(Color.Red);

            public Int32 Radius { get; init; }

            void IDrawHandler.Draw(Graphics gra, Actor actor)
            {
                static Rectangle GetRectangle(Single x, Single y, Int32 width, Int32 height) =>
                    new Rectangle((Int32)x, (Int32)y, width, height);

                var loc = actor.Location;

                gra.FillEllipse(Brush, GetRectangle(loc.X - Radius, loc.Y - Radius, Radius * 2, Radius * 2));
            }
        }
    }
}
