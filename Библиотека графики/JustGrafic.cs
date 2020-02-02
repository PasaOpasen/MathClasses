using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;
using System.IO;
using System.Diagnostics;


namespace Библиотека_графики
{
    public partial class JustGrafic : Form
    {
        public int step;
        public enum Mode : byte { Time, Tick };
        internal Mode MeMode = Mode.Tick;
        internal bool Normalize;

        public JustGrafic(string title = "График")
        {
            InitializeComponent();
            groupBox3.Hide();

            this.Text = title;
            step = numericUpDown1.Value.ToInt32();

            this.FormClosing += new FormClosingEventHandler((object o, FormClosingEventArgs f) =>
            {
                arr = null;
                xticks = null;
                xtime = null;
                arr2 = null;
                GC.Collect();
            });
        }

        public JustGrafic(string[] names, string[] filenames, string title = "График", double dt = 0, int beforecount = 0, bool normalize = false) : this(title)
        {
            fnames = filenames;

            this.chart1.Series.Clear();

            arr = new double[names.Length][];
            arr2 = new double[names.Length][];

            // ReadDataOld();
            ReadData();
            xticks = new double[arr[0].Length];
            xtime = new double[arr[0].Length];
            for (int i = 0; i < xtime.Length; i++)
            {
                xticks[i] = i + 1;
                xtime[i] = -(beforecount - i) * dt;
            }
            xmas = xticks;
            if (dt != 0 || beforecount != 0)
                groupBox3.Show();

            for (int i = 0; i < names.Length; i++)
            {
                this.chart1.Series.Add(names[i]);
                this.chart1.Series[i].BorderWidth = 1;
                this.chart1.Series[i].Color = colors[i];
                this.chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                this.chart1.Series[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                this.chart1.Series[i].Font = new Font("Arial", 16);

                for (int k = 0; k < arr[i].Length; k += step)
                    this.chart1.Series[i].Points.AddXY(xmas[k], arr[i][k]);
            }

            this.CreateCheckBoxes();
            this.ReDraw();

            //string s = "";
            xmin = 1;
            xmax = arr[0].Length;
            //double x, y, xold = double.NaN, yold = double.NaN;

            Stopwatch sp = new Stopwatch();
            sp.Start();

            //this.chart1.MouseMove += new MouseEventHandler((object o, MouseEventArgs arg) =>
            //{
            //    if (arg.Location.X < chart1.Size.Width * 0.95 && arg.Location.Y < chart1.Size.Height * 0.95)
            //        if (sp.ElapsedMilliseconds > 600)
            //        {
            //            x = chart1.ChartAreas[0].AxisX.PixelPositionToValue(arg.Location.X);
            //            y = chart1.ChartAreas[0].AxisY.PixelPositionToValue(arg.Location.Y);
            //            if (x >= xmin && x <= xmax && y >= ymin && y <= ymax)
            //            {
            //                if (x != xold || y != yold)
            //                {
            //                    if (MeMode == Mode.Tick)
            //                        s = $"n = {(int)x}  val = {y.ToString(4)}";//s.Show();
            //                    else
            //                        s = $"t = {x.ToString()}  val = {y.ToString(4)}";//s.Show();
            //                    toolTip1.SetToolTip(chart1, s);
            //                    xold = x;
            //                    yold = y;
            //                }
            //            }
            //            else
            //                toolTip1.SetToolTip(chart1, "");

            //        }

            //});

            Normalize = normalize;
        }

        private void ReadDataOld()
        {
            Parallel.For(0, fnames.Length, (int i) =>
             {
                 using (StreamReader f = new StreamReader(fnames[i]))
                 {
                     var p = (f.ReadToEnd()).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                     arr[i] = new double[p.Length];
                     arr2[i] = new double[p.Length];
                     for (int k = 0; k < arr[i].Length; k++)
                     {
                         arr[i][k] = p[k].Replace('.', ',').ToDouble();
                         arr2[i][k] = arr[i][k];
                     }
                 }
             });
        }

        private void ReadData()
        {
            Parallel.For(0, fnames.Length, (int i) =>
            {
                var collection = File.ReadLines(fnames[i]).Select(p => Convert.ToDouble(p.Replace('.', ',')));

                arr[i] = collection.ToArray();
                arr2[i] = collection.ToArray();
            });
        }

        /// <summary>
        /// Создаёт форму по массиву названий. Предполагается, что данные хранятся в файлах вида $"{s[i]}.txt"
        /// </summary>
        /// <param name="names"></param>
        public JustGrafic(string[] names, string title = "График", double dt = 0, int beforecount = 0,bool normalize=false) : this(names, Expendator.Map(names, (string s) => s + ".txt"), title, dt, beforecount,normalize)
        {

        }
        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Black, Color.Yellow, Color.Violet, Color.SkyBlue, Color.HotPink };


        private void button1_Click(object sender, EventArgs e)
        {
            SaveImage();
        }
        private void SaveImage()
        {
            Библиотека_графики.ForChart.SaveImageFromChart(chart1, $"Изображение от {DateTime.Now.ToString().Replace(':', ' ')}", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
        }

        public string[] fnames;
        public void CreateCheckBoxes()
        {
            ///с помощью этой функции определяются, какие чекбоксы соответствуют графикам функций
            bool str(string text) => Convert.ToInt32(text.Substring(8, text.Length - 8)) - 1 < chart1.Series.Count;

            void SetNullInTextBox(Control.ControlCollection control)
            {
                foreach (Control _control in control)
                {
                    if (_control is CheckBox)
                    {
                        if (str(_control.Name))
                        {
                            (_control as CheckBox).Checked = true;
                            _control.Show();
                        }
                        else
                        {
                            (_control as CheckBox).Checked = false;
                            _control.Hide();
                        }
                    }
                    else
                    if (_control.Controls.Count > 0)
                    {
                        SetNullInTextBox(_control.Controls);
                    }
                }
            }

            SetNullInTextBox(this.Controls);
        }
        public void Lims()
        {
            Библиотека_графики.ForChart.SetAxisesY(ref chart1);
            SetLimsY();
        }


        internal double[] xtime, xticks, xmas;
        public double[][] arr, arr2;
        private double xmin, xmax, ymin, ymax;

        public void ReSaveMas() => ReDraw();

        /// <summary>
        /// Отмена изменений, перерисовка под изначальный массив и копирование изначального массива в рабочий
        /// </summary>
        public void Cancel()
        {
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Points.Clear();

                for (int k = 0; k < arr[i].Length; k += step)
                {
                    this.chart1.Series[i].Points.AddXY(k, arr[i][k]);
                    arr2[i][k] = arr[i][k];
                }
            }

            Lims();
        }

        private void ReDraw()
        {
            ///возвращает номер чекбокса (начиная с 0) по названию
            int str(string text) => Convert.ToInt32(text.Substring(8, text.Length - 8)) - 1;

            void SetNullInTextBox(Control.ControlCollection control)
            {
                foreach (Control _control in control)
                {
                    if (_control is CheckBox)
                    {
                        int k = str(_control.Name);
                        if (k < chart1.Series.Count)
                        {
                            chart1.Series[k].Points.Clear();
                            if ((_control as CheckBox).Checked)
                            {
                                for (int i = 0; i < arr2[k].Length; i += step)
                                    chart1.Series[k].Points.AddXY(xmas[i], arr2[k][i]);
                                chart1.Series[k].IsVisibleInLegend = true;
                                if (step <= 30) this.Refresh();
                            }
                            else
                            {
                                chart1.Series[k].IsVisibleInLegend = false;
                            }
                        }
                    }
                    else
                    if (_control.Controls.Count > 0)
                    {
                        SetNullInTextBox(_control.Controls);
                    }
                }
            }
            SetNullInTextBox(this.Controls);

            Lims();
            //Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private void SetLimsY()
        {
            ymin = chart1.ChartAreas[0].AxisY.Minimum;
            ymax = chart1.ChartAreas[0].AxisY.Maximum;

            textBox1.Text = ymax.ToString(5);
            textBox2.Text = ymin.ToString(5);
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Maximum = textBox1.Text.ToDouble();
            chart1.ChartAreas[0].AxisY.Minimum = textBox2.Text.ToDouble();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            step = numericUpDown1.Value.ToInt32();
            ReDraw();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            MeMode = Mode.Time;
            xmas = xtime;
            xmin = xmas[0];
            xmax = xmas.Last();
            ReDraw();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            MeMode = Mode.Tick;
            xmas = xticks;
            xmin = 1;
            xmax = xmas.Length;
            ReDraw();
        }

        private void вернутьИсходныеМассивыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void сохранитьИзображениеВРабочуюПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage();
        }

        private void сохранитьНовыеМассивыВИсходныеФайлыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // for (int i = 0; i < fnames.Length; i++)
            if(Normalize)
                Parallel.For(0, fnames.Length, (int i) =>
                {
                    double max = arr2[i].Select(d=>Math.Abs(d)).Max();
                    using (StreamWriter t = new StreamWriter(fnames[i]))
                        for (int j = 0; j < arr2[i].Length; j++)
                            t.WriteLine((arr2[i][j]/max).ToString().Replace(',', '.'));
                });
            else
            Parallel.For(0, fnames.Length, (int i) =>
            {
                using (StreamWriter t = new StreamWriter(fnames[i]))
                    for (int j = 0; j < arr2[i].Length; j++)
                        t.WriteLine(arr2[i][j].ToString().Replace(',','.'));

            });
        }

        private void опцииToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            сохранитьНовыеМассивыВИсходныеФайлыToolStripMenuItem_Click(sender, e);

            this.Close();
        }

        private void открытьТрапецевидноеОкноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trapezi trap = new Trapezi(this);

            this.FormClosing += new FormClosingEventHandler((object o, FormClosingEventArgs a) =>
            {
                if (!trap.IsDisposed)
                    trap.Close();
            });
            //диалог сделан, чтобы не обрабатывать ситуацию, когда после запуска трапеции на основной форме меняется режим, так как надо тогда менять режим и на трапеции
            trap.ShowDialog();
        }

    }
}
