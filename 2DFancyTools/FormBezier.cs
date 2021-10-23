using AffineTransformations;

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
    public enum PointType
    {
        MAIN,
        HELP1,
        HELP2
    }

    public class BezPoint
    {
        public Point main;
        public Point help1;
        public Point help2;

        public BezPoint(Point point)
        {
            main = point;
            help1 = new Point(main.X - 50, main.Y);
            help2 = new Point(main.X + 50, main.Y);
        }

        void changeMainPointLocation(Point newLocation)
        {
            help1.Offset(newLocation.X - main.X, newLocation.Y - main.Y);
            help2.Offset(newLocation.X - main.X, newLocation.Y - main.Y);
            main = newLocation;
        }

        private double degreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        void changeHelp1PointLocation(Point newLocation)
        {
            //double savedDistance = distance(main, help2);
            //help1 = newLocation;
            //var v = new PointF(help1.X - main.X, help1.Y - main.Y);
            ////Длина вектора
            //var l = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            ////Нормирование
            //v = new PointF(v.X / l, v.Y / l);
            ////Новые координаты отрезка
            //help2 = new Point((int)(main.X - v.X * savedDistance), (int)(main.Y - v.Y * savedDistance));\
            //rotateAroundMainPoint(ref help2, findRotationAngle(help1, newLocation));
            help2.X -= newLocation.X - help1.X;
            help2.Y -= newLocation.Y - help1.Y;
            help1 = newLocation;
        }

        void changeHelp2PointLocation(Point newLocation)
        {
            //double savedDistance = distance(main, help1);
            //help2 = newLocation;
            //var v = new PointF(help2.X - main.X, help2.Y - main.Y);
            ////Длина вектора
            //var l = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            ////Нормирование
            //v = new PointF(v.X / l, v.Y / l);
            ////Новые координаты отрезка
            //help1 = new Point((int)Math.Round(main.X - v.X * savedDistance), (int)Math.Round(main.Y - v.Y * savedDistance));
            help1.X -= newLocation.X - help2.X;
            help1.Y -= newLocation.Y - help2.Y;
            help2 = newLocation;
        }

        public void replacePoint(PointType type, Point newPoint)
        {
            switch (type)
            {
                case PointType.MAIN: changeMainPointLocation(newPoint); break;
                case PointType.HELP1: changeHelp1PointLocation(newPoint); break;
                case PointType.HELP2: changeHelp2PointLocation(newPoint); break;
            }
        }
     
    }
    public partial class FormBezier : Form
    {
        List<BezPoint> points = new List<BezPoint>();
        BezPoint selectedPoint;
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Pen blackPen = new Pen(Color.LightGray, 3);
        Graphics g;
        Timer timer;

        public FormBezier()
        {
            InitializeComponent();
            g = canvas.CreateGraphics();
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        bool shouldRedraw = false;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (shouldRedraw)
            {
                redrawCanvas();
                shouldRedraw = false;
            }
        }

        private void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        void redrawCanvas()
        {
            g.Clear(Color.White);
            foreach (var p in points)
            {
                g.DrawLine(blackPen, p.help1, p.help2);
                g.FillEllipse(new SolidBrush(Color.Green), p.help1.X - 3, p.help1.Y - 3, 7, 7);
                g.FillEllipse(new SolidBrush(Color.Blue), p.help2.X - 3, p.help2.Y - 3, 7, 7);
                g.FillEllipse(new SolidBrush(Color.Red), p.main.X - 5, p.main.Y - 5, 9, 9);
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                drawBezierLine(points[i].main, points[i].help2, points[i + 1].help1, points[i + 1].main);
            }
        }

       

        void drawBezierLine(Point p1, Point p2, Point p3, Point p4)
        {

            //Matrix bezier = new Matrix(4, 4).fill(1, -3, 3, 1, 0, 3, 6, 3, 0, 0, 3, -3, 0, 0, 0, 1);
            //Matrix pX = new Matrix(1, 4).fill(p1.X, p2.X, p3.X, p4.X) * bezier;
            //Matrix pY = new Matrix(1, 4).fill(p1.Y, p2.Y, p3.Y, p4.Y) * bezier;
            //double t = 0.05;
            //Matrix zero = new Matrix(4, 1).fill(1, 0, 0, 0);
            //Point lastPoint = new Point((int)(pX * zero)[0, 0], (int)(pY * zero)[0, 0]);
            //for (t = 0.1; t <= 1; t += 0.05)
            //{
            //    Matrix mt = new Matrix(4, 1).fill(1, t, t * t, t * t * t);
            //    g.DrawLine(new Pen(Color.Black, 4), lastPoint, lastPoint = new Point((int)(pX * mt)[0, 0], (int)(pY * mt)[0, 0]));
            //}
            Point lastPoint = p1;
            for (double t = 0.05; t <= 1; t += 0.05)
            {
                int x = (int)(Math.Pow(1 - t, 3) * p1.X + 3 * Math.Pow(1 - t, 2) * t * p2.X + 3 * (1 - t) * Math.Pow(t, 2) * p3.X + Math.Pow(t, 3) * p4.X);
                int y = (int)(Math.Pow(1 - t, 3) * p1.Y + 3 * Math.Pow(1 - t, 2) * t * p2.Y + 3 * (1 - t) * Math.Pow(t, 2) * p3.Y + Math.Pow(t, 3) * p4.Y);
                g.DrawLine(new Pen(Color.Black, 4), lastPoint, lastPoint = new Point(x,y));
            }
            g.DrawLine(new Pen(Color.Black, 4), lastPoint, p4);

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Location = new Point(-100, -100);
            btnDelete.Enabled = false;
            points.Remove(selectedPoint);
            //redrawCanvas();
            shouldRedraw = true;
        }

        int draggingPointIndex = -1;
        PointType draggingPointType;
        bool isDragging = false;

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if ((Math.Abs(points[i].main.X - e.X) < 10) && (Math.Abs(points[i].main.Y - e.Y) < 10))
                    {
                        draggingPointIndex = i;
                        draggingPointType = PointType.MAIN;
                        isDragging = true;
                        return;
                    }
                    if ((Math.Abs(points[i].help1.X - e.X) < 10) && (Math.Abs(points[i].help1.Y - e.Y) < 10))
                    {
                        draggingPointIndex = i;
                        draggingPointType = PointType.HELP1;
                        isDragging = true;
                        return;
                    }
                    if ((Math.Abs(points[i].help2.X - e.X) < 10) && (Math.Abs(points[i].help2.Y - e.Y) < 10))
                    {
                        draggingPointIndex = i;
                        draggingPointType = PointType.HELP2;
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
                points[draggingPointIndex].replacePoint(draggingPointType, e.Location);
                //redrawCanvas();
                shouldRedraw = true;
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                foreach (var point in points)
                {
                    if ((Math.Abs(point.main.X - e.X) < 10) && (Math.Abs(point.main.Y - e.Y) < 10))
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
                    points.Add(new BezPoint(e.Location));
                    //redrawCanvas();
                    shouldRedraw = true;
                    return;
                }
                isDragging = false;
                draggingPointIndex = -1;
            }
        }
    }
}
