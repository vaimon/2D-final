using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2DFancyTools
{
    public partial class FormFractal : Form
    {
        private Graphics g;
        string fileName = "..\\..\\..\\L-systems\\КриваяКоха.txt";
        int generation = 0, randFrom = 0, randTo = 0;
        List<string> LSystem = new List<string>();
        List<Tuple<PointF,PointF>> points = new List<Tuple<PointF, PointF>>();

        public FormFractal()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
        }

        private void ComboBoxLSystemChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = ComboBoxLSystemChange.SelectedIndex;
            switch(ind)
            {
                case 0:
                    fileName = "..\\..\\..\\L-systems\\КриваяКоха.txt";
                    break;
                case 1:
                    fileName = "..\\..\\..\\L-systems\\СнежинкаКоха.txt";
                    break;
                case 2:
                    fileName = "..\\..\\..\\L-systems\\ТреугольникСерпинского.txt";
                    break;
                case 3:
                    fileName = "..\\..\\..\\L-systems\\КоверСерпинского.txt";
                    break;
                case 4:
                    fileName = "..\\..\\..\\L-systems\\КоверСерпинского.txt";
                    break;
                case 5:
                    fileName = "..\\..\\..\\L-systems\\КриваяГильберта.txt";
                    break;
                case 6:
                    fileName = "..\\..\\..\\L-systems\\КриваяДракона.txt";
                    break;
                case 7:
                    fileName = "..\\..\\..\\L-systems\\ШестиугольнаяКриваяГоспера.txt";
                    break;
            }
        }

        private void textBoxChangeGeneration_KeyPress(object sender, KeyPressEventArgs e)
        {
            char el = e.KeyChar;
            if (!Char.IsDigit(el) && el != (char)Keys.Back) // можно вводить только цифры и стирать
                e.Handled = true;
        }

        private void textBoxRandomFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            char el = e.KeyChar;
            if (!Char.IsDigit(el) && el != (char)Keys.Back && el != '-') // можно вводить только цифры, минус и стирать
                e.Handled = true;
        }

        private void textBoxRandomTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            char el = e.KeyChar;
            if (!Char.IsDigit(el) && el != (char)Keys.Back && el != '-') // можно вводить только цифры, минус и стирать
                e.Handled = true;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            points.Clear();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
        }

        private void buttonDrawFractal_Click(object sender, EventArgs e)
        {
            points.Clear();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);

            // если окна с изменением поколения и случайности пустые, то = 0
            generation = textBoxChangeGeneration.Text != "" ? Int32.Parse(textBoxChangeGeneration.Text) : 0;
            randFrom = textBoxRandomFrom.Text != "" ? Int32.Parse(textBoxRandomFrom.Text) : 0;
            randTo = textBoxRandomTo.Text != "" ? Int32.Parse(textBoxRandomTo.Text) : 0;

            string axiom = "", direction = "";
            float rotate = 0;
            //получаем данные из файла
            try
            {
                StreamReader sr = new StreamReader(fileName);
                string line = sr.ReadLine();
                string[] parseLine = line.Split(' ');
                axiom = parseLine[0];
                rotate = float.Parse(parseLine[1]);
                direction = parseLine[2];

                line = sr.ReadLine();
                while (line != null)
                {
                    LSystem.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            DrawFractal(axiom, rotate, direction);
        }

        private void DrawFractal(string rule, float rot, string dir)
        {
            // меняем местами значения диапазона, если пользователь ввёл неверно
            if (randFrom > randTo)
            {
                int temp = randFrom;
                randFrom = randTo;
                randTo = temp;
            }

            // разбиваем каждое правило
            Dictionary<char, string> systemRules = new Dictionary<char, string>();
            foreach (string line in LSystem)
                systemRules[line[0]] = line.Substring(2);

            // выводим одно общее правило построения для указанного поколения
            for (int i = 0; i < generation; ++i)
            {
                string seq = "";
                foreach (char lex in rule)
                    if (systemRules.ContainsKey(lex))
                        seq += systemRules[lex];
                    else
                        seq += lex;
                rule = seq;
            }

            float angle = 0;
            // находим угол начального направления
            switch (dir)
            {
                case "left":
                    angle = (float)Math.PI; // 180 градусов
                    break;
                case "right":
                    angle = 0; // 0 градусов
                    break;
                case "up":
                    angle = (float)(3 * Math.PI / 2); // 270(или -90) градусов
                    break;
            }

            rot = rot * (float)Math.PI / 180; // градусы в радианы
            PointF point = new PointF(0, 0);
            Random rand = new Random();
            int randRotate = 0;

            foreach (char lex in rule)
            {
                if (lex == 'F')
                {
                    // следующая точка фрактала переносится от старой по направлению синуса
                    float nextX = (float)(point.X + Math.Cos(angle)), nextY = (float)(point.Y + Math.Sin(angle)); 
                    points.Add(Tuple.Create(point, new PointF(nextX, nextY)));
                    point = new PointF(nextX, nextY);
                }
                else if (lex == '-')
                {                  
                    randRotate = rand.Next(randFrom, randTo+1);
                    angle -= rot + randRotate * (float)Math.PI / 180;
                }
                else if (lex == '+')
                {
                    randRotate = rand.Next(randFrom, randTo+1);
                    angle += rot + randRotate * (float)Math.PI / 180;
                }
            }

            // находим минимум и максимум полученных точек для масштабирования
            float minX = points.Min(point => Math.Min(point.Item1.X, point.Item2.X));
            float maxX = points.Max(point => Math.Max(point.Item1.X, point.Item2.X));

            float minY = points.Min(point => Math.Min(point.Item1.Y, point.Item2.Y));
            float maxY = points.Max(point => Math.Max(point.Item1.Y, point.Item2.Y));

            // центр окна
            PointF center = new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2);
            // центр полученного фрактала
            PointF fractal = new PointF(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
            // шаг для масштабирования
            float step = Math.Min(pictureBox1.Width / (maxX - minX), pictureBox1.Height / (maxY - minY));

            // масштабируем список точек
            for (int i = 0; i < points.Count(); i++)
            {
                float scaleX = center.X + (points[i].Item1.X - fractal.X) * step, scaleY = center.Y + (points[i].Item1.Y - fractal.Y) * step;
                float scaleNextX = center.X + (points[i].Item2.X - fractal.X) * step, scaleNextY = center.Y + (points[i].Item2.Y - fractal.Y) * step;
                points[i] = new Tuple<PointF, PointF>(new PointF(scaleX, scaleY), new PointF(scaleNextX, scaleNextY));
            }

            // выводим фрактал
            for (int i = 0; i < points.Count(); ++i)
            {            
                g.DrawLine(new Pen(Color.Black), points[i].Item1, points[i].Item2);
                pictureBox1.Invalidate();
            }

        }
    }  
}
