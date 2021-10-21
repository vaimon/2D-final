using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2DFancyTools
{
    public partial class FormBezier : Form
    {
        List<Point> mainPoints = new List<Point>();
        Point selectedPoint;
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Pen blackPen = new Pen(Color.Black, 3);
        Graphics g;

        public FormBezier()
        {
            InitializeComponent();
            g = canvas.CreateGraphics();
        }

        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        void redrawCanvas()
        {
            g.Clear(Color.White);
            foreach (var p in mainPoints)
            {
                g.FillEllipse(new SolidBrush(Color.Red), p.X - 3, p.Y - 3, 7, 7);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Location = new Point(-100, -100);
            btnDelete.Enabled = false;
            mainPoints.Remove(selectedPoint);
            redrawCanvas();
        }

        int draggingPointIndex = -1;
        bool isDragging = false;

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (int i = 0; i < mainPoints.Count; i++)
                {
                    if ((Math.Abs(mainPoints[i].X - e.X) < 10) && (Math.Abs(mainPoints[i].Y - e.Y) < 10))
                    {
                        draggingPointIndex = i;
                        isDragging = true;
                        return;
                    }
                }

            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(isDragging)
            {
                mainPoints[draggingPointIndex] = e.Location;
                redrawCanvas();
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (var point in mainPoints)
                {
                    if ((Math.Abs(point.X - e.X) < 10) && (Math.Abs(point.Y - e.Y) < 10))
                    {
                        btnDelete.Location = new Point(e.X + 10, e.Y + 10);
                        btnDelete.Enabled = true;
                        selectedPoint = point;
                        return;
                    }
                }
            }
            else
            {
                if (!isDragging)
                {
                    mainPoints.Add(e.Location);
                    redrawCanvas();
                }
                isDragging = false;
                draggingPointIndex = -1;
            }
        }
    }
}
