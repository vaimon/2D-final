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
   
    public partial class FormMountains : Form
    {
        bool drawingMode;
        public FormMountains()
        //Реализовать алгоритм midpoint displacement для двумерной визуализации горного массива.
        {
            InitializeComponent();
            g = canvas.CreateGraphics();
           canvas.Image = new Bitmap(1300, 900);
            drawingMode = false;
        }
        List<Point> polygonPoints = new List<Point>();


        /// <summary>
        /// Для рисования
        /// </summary>
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Pen blackPen = new Pen(Color.Black, 3);
        Graphics g;
        
        void drawPoint(MouseEventArgs e)
        {
            
            
            polygonPoints.Add(e.Location);
            
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
            double r = double.Parse(textBox1.Text);//коэфф шероховатости зададим сами
            Point p1 = polygonPoints[0];
            Point p2 = polygonPoints[1];
            Random rnd = new Random();
            g.DrawLine(blackPen, p1, p2);
            int c = (p1.X + p2.X) / 2;
            double y = (p1.Y + p2.Y) / 2 + rnd.Next((int)(-r * 10), (int)(r * 10));
            Point p3=new Point(c,(int) y);
            g.DrawLine(blackPen,p1,p3);
            g.DrawLine(blackPen, p3, p2);
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
                textBox1.Clear();
                drawingMode = false;
            }

        }
    }
}
