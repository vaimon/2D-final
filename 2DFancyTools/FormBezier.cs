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
    /// <summary>
    /// https://ru.stackoverflow.com/questions/672647/Как-найти-угол-между-векторами
    /// </summary>
    struct Vector
    {
        public double X { get; }
        public double Y { get; }

        public Vector(double x, double y)
        {
            X = x; Y = y;
        }

        public static readonly Vector Reference = new Vector(1, 0);

        public static double AngleOfReference(Vector v)
            => NormalizeAngle(Math.Atan2(v.Y, v.X) / Math.PI * 180);

        public static double AngleOfVectors(Vector first, Vector second)
            => NormalizeAngle(AngleOfReference(first) - AngleOfReference(second));

        private static double NormalizeAngle(double angle)
        {
            bool CheckBottom(double a) => a >= 0;
            bool CheckTop(double a) => a < 360;

            double turn = CheckBottom(angle) ? -360 : 360;
            while (!(CheckBottom(angle) && CheckTop(angle))) angle += turn;
            return angle;
        }
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

        double findRotationAngle(Point from, Point to)
        {
            return Vector.AngleOfVectors(new Vector(from.X - main.X, from.Y - main.Y), new Vector(to.X - main.X, to.Y - main.Y));
        }

        static double distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        private double degreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        void rotateAroundMainPoint(ref Point point, double angle)
        {
                var shift1 = new Matrix(3, 3).fillAffine(1, 0, 0, 1, -main.X, -main.Y);
                var rotation = new Matrix(3, 3).fillAffine(Math.Cos(degreesToRadians(angle)), -Math.Sin(degreesToRadians(angle)), Math.Sin(degreesToRadians(angle)), Math.Cos(degreesToRadians(angle)), 0, 0);
                var shift2 = new Matrix(3, 3).fillAffine(1, 0, 0, 1, main.X, main.Y);
                var vals = new Matrix(1, 3).fill(point.X, point.Y, 1);
                var prom = (shift1 * rotation * shift2);
                var res = vals * prom;
                point = new Point((int)res[0, 0], (int)(res[0, 1]));
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
            rotateAroundMainPoint(ref help2, findRotationAngle(help1, newLocation));
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
            rotateAroundMainPoint(ref help1, findRotationAngle(help1, newLocation));
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
            timer.Interval = 100;
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
                g.FillEllipse(new SolidBrush(Color.Gray), p.help1.X - 3, p.help1.Y - 3, 7, 7);
                g.FillEllipse(new SolidBrush(Color.Gray), p.help2.X - 3, p.help2.Y - 3, 7, 7);
                g.FillEllipse(new SolidBrush(Color.Red), p.main.X - 5, p.main.Y - 5, 9, 9);
            }
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
