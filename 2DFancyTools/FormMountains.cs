﻿using System;
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


    public partial class FormMountains : Form
    {
        bool drawingMode;
        double n;
        double r;
        public FormMountains()
        //Реализовать алгоритм midpoint displacement для двумерной визуализации горного массива.
        {
            InitializeComponent();
            trackBar1.Scroll += trackBar1_Scroll;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            g = canvas.CreateGraphics();
            canvas.Image = new Bitmap(1300, 900);
            drawingMode = false;
        }
        List<Point> polygonPoints = new List<Point>();
        List<segment> Mountains = new List<segment>();

        /// <summary>
        /// Для рисования
        /// </summary>
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Pen blackPen = new Pen(Color.Black, 3);
        Pen blackPen2 = new Pen(Color.Black, 1);
        Graphics g;

        void drawPoint(MouseEventArgs e)
        {


            polygonPoints.Add(e.Location);
            if (polygonPoints.Count == 2)
                Mountains.Add(new segment(polygonPoints[0], polygonPoints[1]));
            g.FillEllipse(blackBrush, e.X - 3, e.Y - 3, 7, 7);

            //setFlags(isAffineTransformationsEnabled: true);
        }
        void canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawingMode)
            {

                drawPoint(e);

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawingMode = true;
        }
        void DrawMountain()
        {
            g.Clear(Color.White);
            r = double.Parse(comboBox1.Text);//коэфф шероховатости зададим сами
                                            // n= double.Parse(textBox2.Text);//глубина рекурсии задается пользователем
            n = trackBar1.Value;


            for (int j = 1; j <= (int)n; j++)
            {
                Random rnd = new Random();
                List<segment> drawpoints = new List<segment>();
                foreach (segment x in Mountains)
                {
                    Point left = x.l;
                    Point right = x.r;
                     double i = Math.Abs(left.X - right.X)* Math.Abs(left.X - right.X) + Math.Abs(left.Y - right.Y) * Math.Abs(left.Y - right.Y);
                    i = Math.Sqrt(i);
                    int c = (left.X + right.X) / 2;
                    double y = (left.Y + right.Y) / 2 + (rnd.NextDouble() - 0.5)*r * i;
                    //if (y > canvas.Size.Height)
                       // y = canvas.Size.Height;
                   // if (y < 0)
                       // y = 0;
                    Point center = new Point(c, (int)y);
                    drawpoints.Add(new segment(left, center));
                    drawpoints.Add(new segment(center, right));
                }
                Mountains = drawpoints;
            }
            DrawMountainsHelp();
            /*
            Point p1 = polygonPoints[0];
            Point p2 = polygonPoints[1];
            Random rnd = new Random();
            g.DrawLine(blackPen, p1, p2);
            int c = (p1.X + p2.X) / 2;
            int i = Math.Abs(p1.X - p2.X);
            double y = (p1.Y + p2.Y) / 2 + rnd.Next((int)(-r * i), (int)(r * i));
            Point p3=new Point(c,(int) y);
            g.DrawLine(blackPen2,p1,p3);
            g.DrawLine(blackPen2, p3, p2);*/
        }
        void DrawSegment(segment s)
        {
            g.DrawLine(blackPen2, s.l, s.r);
        }
        void DrawMountainsHelp()
        {
            foreach (segment x in Mountains)
                DrawSegment(x);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DrawMountain();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!drawingMode)
            {
                button3.Visible = false;
            }
            else
            {
                g.Clear(Color.White);
                polygonPoints.Clear();
                Mountains.Clear();
                //textBox1.Clear();

                drawingMode = false;
            }

        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = String.Format("Глубина рекурсии: {0}", trackBar1.Value);
        }
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
           // r = double.Parse(selectedState);
        }
    }
    class segment
    {
        public Point l;
        public Point r;
        public
        segment(Point p1, Point p2)
        {
            l = p1;
            r = p2;
        }

    };
}
