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
using Point = МатКлассы.Point;

namespace Библиотека_графики
{
    public partial class MostSimpleGrafic : Form
    {
        public MostSimpleGrafic(Func<double, double> f, NetOnDouble argumentNet)
        {
            InitializeComponent();
            chart1.Series[0].IsVisibleInLegend = false;
            var points = МатКлассы.Point.Points(new Func<double,double>(t => f(t)), argumentNet.Count - 1, argumentNet.Begin, argumentNet.End,true);
            for (int i = 0; i < points.Length; i++)
                chart1.Series[0].Points.AddXY(points[i].x, points[i].y);
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        public MostSimpleGrafic(Func<double, double>[] f, NetOnDouble argumentNet, string[] names,bool parallel=true)
        {
            InitializeComponent();
            chart1.Series.Clear();
            for (int k = 0; k < f.Length; k++)
            {
                chart1.Series.Add(names[k]);
                chart1.Series[k].BorderWidth = 3;
                chart1.Series[k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

                var points = МатКлассы.Point.Points(new Func<double,double>(t => f[k](t)), argumentNet.Count - 1, argumentNet.Begin, argumentNet.End, parallel);
                
                for (int i = 0; i < points.Length; i++)
                    chart1.Series[k].Points.AddXY(points[i].x, points[i].y);
            }

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        public MostSimpleGrafic(Func<double, double> f,Point[] mas, string name, bool parallel = true)
        {
            InitializeComponent();
            chart1.Series[0].IsVisibleInLegend = false;
            chart1.Series.Add(name);
            chart1.Series[1].BorderWidth = 3;
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            Point[] p = new Point[mas.Length];
                Parallel.For(0, p.Length, (int i) => p[i] = new Point(mas[i].x, f(mas[i].x)));


            for (int i = 0; i < p.Length; i++)
            {
                chart1.Series[0].Points.AddXY(mas[i].x, mas[i].y);
                chart1.Series[1].Points.AddXY(p[i].x, p[i].y);
            }
                
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForChart.SaveImageFromChart(chart1);
        }
    }
}
