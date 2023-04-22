using Timer = System.Windows.Forms.Timer;

namespace Example
{
    public partial class Form1 : Form
    {
        private enum SpawnerMode
        {
            Boid,
            Avoid
        }

        private Timer _timer = new Timer();
        private SpawnerMode _spawenerMode = SpawnerMode.Boid;

        public Form1()
        {
            InitializeComponent();

            _timer.Enabled = true;
            _timer.Interval = 33;  /* 33 millisec */
            _timer.Tick += TimerCallback;
        }

        private void TimerCallback(object? sender, EventArgs e)
        {
            Program.Simulation.Update();

            Invalidate(true);
            return;
        }

        /// <summary>
        /// draw event
        /// </summary>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(Color.Black);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            foreach (var actor in Program.Simulation.Actors)
            {
                actor.Draw(g);
            }
        }

        /// <summary>
        /// mouse event
        /// </summary>
        private void panel1_Click(object sender, EventArgs e)
        {
            var point = panel1.PointToClient(Cursor.Position);

            var position = new System.Numerics.Vector3(point.X, point.Y, 0f);
            switch (_spawenerMode)
            {
                case SpawnerMode.Boid:
                    Program.Simulation.AddBoid(position);
                    break;
                case SpawnerMode.Avoid:
                    Program.Simulation.AddAvoid(position);
                    break;
            }
        }

        /// <summary>
        /// key event
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                    _spawenerMode = SpawnerMode.Boid;
                    break;
                case Keys.D2:
                    _spawenerMode = SpawnerMode.Avoid;
                    break;
            }

            // update viewer
            label2.Text = _spawenerMode.ToString();
        }
    }
}