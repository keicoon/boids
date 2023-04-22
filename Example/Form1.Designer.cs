using Example.Boids;

namespace Example
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new DoubleBufferPanel();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // panel1
            // 

            panel1.Location = new Point(4, 36);
            panel1.Name = "panel1";
            panel1.Size = Simulation.WorldSize;
            panel1.TabIndex = 0;
            panel1.Click += panel1_Click;
            panel1.Paint += panel1_Paint;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(686, 8);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 2;
            label1.Text = "Spawner :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(749, 8);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 3;
            label2.Text = "Boid";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            KeyDown += Form1_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label2;
    }

    internal class DoubleBufferPanel : System.Windows.Forms.Panel
    {
        internal DoubleBufferPanel()
        {
            DoubleBuffered = true;
        }
    }
}