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
        string fileName = "";
        int generation = 0, randFrom = 0, randTo = 0;
        List<string> systemRules = null;

        public FormFractal()
        {
            InitializeComponent();
        }

        private void ComboBoxLSystemChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = ComboBoxLSystemChange.SelectedIndex;
            switch(ind)
            {
                case 0:
                    fileName = "/L-systems/КриваяКоха.txt";
                    break;
                case 1:
                    fileName = "/L-systems/СнежинкаКоха.txt";
                    break;
                case 2:
                    fileName = "/L-systems/СнежинкаКоха.txt";
                    break;
                case 3:
                    fileName = "/L-systems/КоверСерпинского.txt";
                    break;
                case 4:
                    fileName = "/L-systems/НаконечникСерпинского.txt";
                    break;
                case 5:
                    fileName = "/L-systems/НаконечникСерпинского.txt";
                    break;
                case 6:
                    fileName = "/L-systems/КриваяДракона.txt";
                    break;
                case 7:
                    fileName = "/L-systems/ШестиугольаяКриваяГоспера.txt";
                    break;
            }
        }

        private void textBoxChangeGeneration_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void textBoxRandomFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void textBoxRandomTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void buttonDrawFractal_Click(object sender, EventArgs e)
        {
            generation = Int32.Parse(textBoxChangeGeneration.Text);
            randFrom = Int32.Parse(textBoxRandomFrom.Text);
            randTo = Int32.Parse(textBoxRandomTo.Text);

            //получаем данные из файла
            try
            {
                StreamReader sr = new StreamReader(fileName);
                string line = sr.ReadLine();
                string[] parseLine = line.Split(' ');
                string state = parseLine[0];
                int rotate = Int32.Parse(parseLine[1]);
                string direction = parseLine[2];

                line = sr.ReadLine();
                while (line != null)
                {
                    systemRules.Add(line);
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

            DrawFractal();
        }

        private void DrawFractal()
        {

        }
    }
}
