using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineTraceMouse
{
    public partial class Form1 : Form
    {
        private Point? mousePosition = null;
        Control sourse;
        Point soursePoint;
        Control target;
        bool setting = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            foreach (Control childControl in this.Controls)
            {
                if (!(childControl is Label))
                    continue;
                childControl.MouseClick += (sender, e) =>
                {
                    if (!setting)
                        return;
                    target = childControl;
                    sourse.Text = target.Text;
                    setting = false;
                };
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!setting)
                return;
            base.OnPaint(e);
            if (mousePosition.HasValue)
            {
                using (Pen p = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawLine(p, soursePoint.X, soursePoint.Y, mousePosition.Value.X, mousePosition.Value.Y);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousePosition = e.Location;
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sourse = label1;
            soursePoint = new Point(label1.Location.X + (label1.Width / 2), label1.Location.Y + (label1.Height / 2));
            setting = true;
        }
    }
}
