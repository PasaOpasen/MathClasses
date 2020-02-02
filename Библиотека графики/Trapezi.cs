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

namespace Библиотека_графики
{
    public partial class Trapezi : Form
    {
        int mantis = 7;
        double beg = 0.01, end = 0.58, s = 0.04;
        int h;
        double t => (100 - trackBar1.Value - trackBar3.Value - 2 * trackBar2.Value);
        JustGrafic gr;
        double dt, len;

        public Trapezi(JustGrafic g)
        {
            InitializeComponent();

            gr = g;
            chart1.Series[0].IsVisibleInLegend = false;
            h = g.arr[0].Length;
            dt = g.xmas[1] - g.xmas[0];
            len = g.xmas.Last() - g.xmas[0];
            ReDraw();
            Библиотека_графики.ForChart.SetToolTips(ref chart1);

            GetParamsFromFile();
            GetValFunc();
            GetTracks();
            GetTimer();
        }
        private void GetValFunc()
        {
            if (gr.MeMode == JustGrafic.Mode.Tick)
            {
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0,}K";
                ValueForBox = ValueForBoxInt;
            }
            else
            {
                //    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0,4}";
                ValueForBox = ValueForBoxDouble;
            }
        }
        private void GetTracks()
        {
            trackBar1.Value = (int)(100 * beg);
            trackBar2.Value = (int)(100 * s);
            trackBar3.Value = (int)(100 * end);
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar2.Scroll += trackBar2_Scroll;
            trackBar3.Scroll += trackBar3_Scroll;
            trackBar1_Scroll(new object(), new EventArgs());
            trackBar2_Scroll(new object(), new EventArgs());
            trackBar3_Scroll(new object(), new EventArgs());
        }
        private void GetTimer()
        {
            bool NonEqu(string s, int i)
            {
                try
                {
                    return
                   Math.Abs(Convert.ToDouble(s) / len - i / 100.0) > Math.Pow(10.0, -(mantis - 1));
                }
                catch
                {
                    return false;
                }

            }
            bool NonEqu2(string s, int i)
            {
                try
                {
                    return
                   Math.Abs(Convert.ToDouble(s) / h - i / 100.0) > 0;
                }
                catch
                {
                    return false;
                }

            }

            void act(Func<string, int, bool> NQ)
            {
                void SetTL(TextBox t, string s) => toolTip1.SetToolTip(t, s);
                string s1 = "Поле загорается красным, когда его содержание не соответствует значению на trackBar", s2 = "";

                if (NQ(textBox1.Text, trackBar1.Value))
                {
                    textBox1.BackColor = Color.Red;
                    SetTL(textBox1, s1);
                }
                else
                {
                    textBox1.BackColor = Color.White;
                    SetTL(textBox1, s2);
                }

                if (NQ(textBox2.Text, trackBar2.Value))
                {
                    textBox2.BackColor = Color.Red;
                    SetTL(textBox2, s1);
                }
                else
                {
                    textBox2.BackColor = Color.White;
                    SetTL(textBox2, s2);
                }

                if (NQ(textBox3.Text, trackBar3.Value))
                {
                    textBox3.BackColor = Color.Red;
                    SetTL(textBox3, s1);
                }
                else
                {
                    textBox3.BackColor = Color.White;
                    SetTL(textBox3, s2);
                }
            }

            if (gr.MeMode == JustGrafic.Mode.Time)
                timer1.Tick += new EventHandler((object o, EventArgs e) => act(NonEqu));
            else
            timer1.Tick += new EventHandler((object o, EventArgs e) =>act(NonEqu2));
            timer1.Start();
        }

        private void GetParamsFromFile()
        {
            if (System.IO.File.Exists("TrapeziParams.txt"))
            {
                var ss = Expendator.GetWordFromFile("TrapeziParams.txt").Split(' ');
                beg = ss[0].ToInt32() / 100.0;
                s = ss[1].ToInt32() / 100.0;
                end = ss[2].ToInt32() / 100.0;
            }
        }
        private void SetParamsInFile() => Expendator.WriteStringInFile("TrapeziParams.txt", $"{trackBar1.Value} {trackBar2.Value} {trackBar3.Value}");


        void ReDraw()
        {
            double ToMode(double t)
            {
                if (gr.MeMode == JustGrafic.Mode.Tick)
                    return t;
                return gr.xmas[0] + (gr.xmas.Last() - gr.xmas[0]) / (h - 1) * t;
            }

            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.AddXY(ToMode(0), 0);
            chart1.Series[0].Points.AddXY(ToMode(beg * h), 0);
            chart1.Series[0].Points.AddXY(ToMode((beg + s) * h), 1);
            chart1.Series[0].Points.AddXY(ToMode((1 - end - s) * h), 1);
            chart1.Series[0].Points.AddXY(ToMode((1 - end) * h), 0);
            chart1.Series[0].Points.AddXY(ToMode(h), 0);
        }
        void SetParams()
        {
            beg = trackBar1.Value / 100.0;
            end = trackBar3.Value / 100.0;
            s = trackBar2.Value / 100.0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetParamsInFile();
            this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            beg = textBox1.Text.ToDouble() / len;
            s = textBox2.Text.ToDouble() / len;
            end = textBox3.Text.ToDouble() / len;

            int t1 = (int)(beg * h);
            int t2 = (int)((beg + s) * h);
            int t3 = (int)((1.0 - end - s) * h);
            int t4 = (int)((1.0 - end) * h);


            int t12 = t2 - t1;
            double tg = (t12 == 0) ? 0 : 1.0 / t12;

            await Task.Run(() =>
            {
                Parallel.For(0, gr.arr2.GetLength(0), (int i) =>
                {
                    //for(int i = 0; i < gr.arr2.GetLength(0); i++)
                    //  {
                    for (int k = 0; k <= t1; k++)
                        gr.arr2[i][k] = 0.0;
                    for (int k = t4; k < h; k++)
                        gr.arr2[i][k] = 0.0;

                    for (int k = t1 + 1; k < t2; k++)
                        gr.arr2[i][k] *= (k - t1) * tg;
                    for (int k = t3 + 1; k < t4; k++)
                        gr.arr2[i][k] *= 1.0 - (k - t3) * tg;
                    //  }
                });
            });
            gr.ReSaveMas();
        }

        private double ValueForBoxInt(int val) => Math.Round(val * h / 100.0);
        private double ValueForBoxDouble(int val) => Math.Round(val * len / 100, mantis);
        private Func<int, double> ValueForBox;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar1.Value = 100 - trackBar2.Value * 2 - trackBar3.Value;
            label1.Text = $"До трапеции{Environment.NewLine}({trackBar1.Value}%)";
            textBox1.Text = $"{ValueForBox(trackBar1.Value)}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gr.Cancel();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar2.Value = (100 - trackBar1.Value - trackBar3.Value) / 2;
            label2.Text = "Под" + Environment.NewLine + "боковой" + Environment.NewLine + "стороной" + $"{Environment.NewLine}({trackBar2.Value}%)";
            textBox2.Text = $"{ValueForBox(trackBar2.Value)}";
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar3.Value = 100 - trackBar2.Value * 2 - trackBar1.Value;
            label3.Text = $"После трапеции{Environment.NewLine}({trackBar3.Value}%)";
            textBox3.Text = $"{ValueForBox(trackBar3.Value)}";
        }
    }
}
