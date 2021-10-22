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
                    fileName = "..\\..\\..\\L-systems\\ШестиугольнаяКриваяГоспера.txt";
                    break;
                case 5:
                    fileName = "..\\..\\..\\L-systems\\КриваяГильберта.txt";
                    break;
                case 6:
                    fileName = "..\\..\\..\\L-systems\\КриваяДракона.txt";
                    break;
                case 7:
                    fileName = "..\\..\\..\\L-systems\\ВысокоеДерево.txt";
                    break;
                case 8:
                    fileName = "..\\..\\..\\L-systems\\ШирокоеДерево.txt";
                    break;
                case 9:
                    fileName = "..\\..\\..\\L-systems\\Куст.txt";
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
            generation = textBoxChangeGeneration.Text != "" ? Int32.Parse(textBoxChangeGeneration.Text) : 1;
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
            Stack<Tuple<PointF, float>> st = new Stack<Tuple<PointF, float>>();
            PointF point = new PointF(0, 0);
            Random rand = new Random();
            int randRotate = 0;
            int count = 0;
            int stepColor = generation == 0 ? 255 / generation : 255 / generation+1;
            Color color = Color.FromArgb(64, 0, 0);
            float width = 7;
            if (fileName == "..\\..\\..\\L-systems\\ШирокоеДерево.txt")
                width = 14;
            bool flagGr = false, flagBr = false;
            //List<int> indPlus = new List<int>(), indMinus = new List<int>();
            Dictionary<PointF, Tuple<Color, float>> gr = new Dictionary<PointF, Tuple<Color, float>>(), br = new Dictionary<PointF, Tuple<Color, float>>();

            foreach (char lex in rule)
            {
                if (lex == 'F')
                {
                    // следующая точка фрактала переносится от старой по направлению синуса
                    float nextX = (float)(point.X + Math.Cos(angle)), nextY = (float)(point.Y + Math.Sin(angle));
                    points.Add(Tuple.Create(point, new PointF(nextX, nextY)));

                    /* if (flagGr)
                         gr[point] = new Tuple<Color, float>(color, width);
                     else if (flagBr)
                         br[point] = new Tuple<Color, float>(color, width);
                     else if (fileName == "..\\..\\..\\L-systems\\Дерево.txt" || fileName == "..\\..\\..\\L-systems\\Куст.txt")
                         br[point] = new Tuple<Color, float>(Color.FromArgb(64, 0, 0), 10); */
                    if (fileName.Contains("Дерево") || fileName.Contains("Куст"))
                    {
                        if (count < 3)
                        {
                            //color = color.G + 20 < 0 ? Color.FromArgb(color.R + 3, 0, color.B) : Color.FromArgb(color.R + 3, color.G + 20, color.B);
                            width--;
                            count++;
                        }
                        gr[point] = new Tuple<Color, float>(color, width);
                    }

                    point = new PointF(nextX, nextY);
                }
                else if (lex == '[')
                {
                    st.Push(Tuple.Create(point, angle));
                    if (fileName.Contains("Дерево") || fileName.Contains("Куст"))
                    {
                        flagGr = true;
                        flagBr = false;
                        //color = color.G + stepColor > 255 ? Color.FromArgb(color.R - 3, 255, color.B) : Color.FromArgb(color.R - 3, color.G + stepColor, color.B);
                        color = color.G + 40 > 255 ? Color.FromArgb(color.R, 255, color.B) : Color.FromArgb(color.R, color.G + 40, color.B);
                        //width -= 2;
                        width--;
                    }
                    //count++;
                }
                else if (lex == ']')
                {
                    Tuple<PointF, float> tuple = st.Pop();
                    point = tuple.Item1;
                    angle = tuple.Item2;
                    if (fileName.Contains("Дерево") || fileName.Contains("Куст"))
                    {
                        flagGr = false;
                        flagBr = true;
                        //color = color.G - stepColor < 0 ? Color.FromArgb(color.R + 3, 0, color.B) : Color.FromArgb(color.R + 3, color.G - stepColor, color.B);
                        color = color.G - 40 < 0 ? Color.FromArgb(color.R, 0, color.B) : Color.FromArgb(color.R, color.G - 40, color.B);
                        //width += 2;
                        width++;
                    }
                }
                else if (lex == '-')
                {
                    randRotate = rand.Next(randFrom, randTo + 1);
                    angle -= rot + randRotate * (float)Math.PI / 180;
                }
                else if (lex == '+')
                {
                    randRotate = rand.Next(randFrom, randTo + 1);
                    angle += rot + randRotate * (float)Math.PI / 180;
                }
            }

            // находим минимум и максимум полученных точек для масштабирования
            float minX = points.Min(point => Math.Min(point.Item1.X, point.Item2.X)), maxX = points.Max(point => Math.Max(point.Item1.X, point.Item2.X));
            float minY = points.Min(point => Math.Min(point.Item1.Y, point.Item2.Y)), maxY = points.Max(point => Math.Max(point.Item1.Y, point.Item2.Y));

            // центр окна
            PointF center = new PointF(pictureBox1.Width / 2, pictureBox1.Height / 2);
            // центр полученного фрактала
            PointF fractal = new PointF(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
            // шаг для масштабирования
            float step = Math.Min(pictureBox1.Width / (maxX - minX), pictureBox1.Height / (maxY - minY));

            List<Tuple<PointF, PointF>> scalePoints = new List<Tuple<PointF, PointF>>(points);
            // масштабируем список точек
            for (int i = 0; i < points.Count(); i++)
            {
                float scaleX = center.X + (points[i].Item1.X - fractal.X) * step, scaleY = center.Y + (points[i].Item1.Y - fractal.Y) * step;
                float scaleNextX = center.X + (points[i].Item2.X - fractal.X) * step, scaleNextY = center.Y + (points[i].Item2.Y - fractal.Y) * step;
                scalePoints[i] = new Tuple<PointF, PointF>(new PointF(scaleX, scaleY), new PointF(scaleNextX, scaleNextY));
            }

            if (fileName.Contains("Дерево") || fileName.Contains("Куст"))
            {
                for (int i = 0; i < points.Count(); ++i)
                {
                    //if (gr.ContainsKey(points[i].Item1))
                        g.DrawLine(new Pen(gr[points[i].Item1].Item1, gr[points[i].Item1].Item2), scalePoints[i].Item1, scalePoints[i].Item2);
                 /*   else if (br.ContainsKey(points[i].Item1))
                        g.DrawLine(new Pen(br[points[i].Item1].Item1, br[points[i].Item1].Item2), points[i].Item1, points[i].Item2);*/
                    //else
                     //   g.DrawLine(new Pen(Color.Black), scalePoints[i].Item1, scalePoints[i].Item2);
                }
            }
            else
                for (int i = 0; i < points.Count(); ++i)
                    g.DrawLine(new Pen(Color.Black), scalePoints[i].Item1, scalePoints[i].Item2);

            pictureBox1.Invalidate();


            // выводим фрактал
            /*    if (fileName == "..\\..\\..\\L-systems\\Дерево.txt" || fileName == "..\\..\\..\\L-systems\\Куст.txt")
                {
                    Color color = Color.Brown;
                    float width = 10;
                    int c = points.Count() / count;
                    for (int i = 0; i < points.Count(); ++i)
                    {
                        c--;
                        if (c == 0)
                        {
                            c = points.Count() / count;
                            width--;
                            color = Color.FromArgb(color.R, color.G + c-1, color.B);
                        }
                        g.DrawLine(new Pen(color, width), points[i].Item1, points[i].Item2);
                    }
                }
                else
                {
                    for (int i = 0; i < points.Count(); ++i)
                    {
                        g.DrawLine(new Pen(Color.Black), points[i].Item1, points[i].Item2);
                        pictureBox1.Invalidate();
                    }
                } */
        }
    }  
}
