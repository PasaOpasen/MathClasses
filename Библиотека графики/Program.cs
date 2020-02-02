using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.IO;

namespace Библиотека_графики
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(
                //new PdfOpen("e33", "formula")
                //new JustGrafic(new string[] { "ArrayA","ArrayB","ArrayC"},"Жопа",0.021,11420)
                new ManyDocumentsShower("maint", new string[] { "T1", "T2", "T3" }, new string[3] { "22.pdf", "22.png", "23.pdf" })
                );
        }
    }

    public static class ForChart
    {
        static double t = 0.05;

        /// <summary>
        /// Добавить ко всем графикам tooltips, сообщающие координаты точек
        /// </summary>
        /// <param name="chart"></param>
        public static void SetToolTips(ref Chart chart)
        {
            for (int i = 0; i < chart.Series.Count; i++)
                chart.Series[i].ToolTip = "X = #VALX, Y = #VALY";
        }
        /// <summary>
        /// Задать границы по оси Y так, чтобы осталось немного лишнего, но равномерно
        /// </summary>
        /// <param name="chart"></param>
        public static void SetAxisesY(ref Chart chart, double coef = 0.05)
        {
            t = coef;
            List<double> list = new List<double>();
            for (int i = 0; i < chart.Series.Count; i++)
                for (int j = 0; j < chart.Series[i].Points.Count; j++)
                    list.AddRange(chart.Series[i].Points[j].YValues);
            if (list.Count > 0)
            {
                double min = list.Min(), max = list.Max();

                if (min == max)
                {
                    min -= t;
                    max += t;
                }

                chart.ChartAreas[0].AxisY.Minimum = (min > 0) ? min * (1 - t) : min * (1 + t);
                chart.ChartAreas[0].AxisY.Maximum = (max > 0) ? max * (1 + t) : max * (1 - t);
            }

        }
        /// <summary>
        /// Очистить массивы точек и скрыть легенды
        /// </summary>
        /// <param name="chart"></param>
        public static void ClearPointsAndHideLegends(ref Chart chart)
        {
            for (int i = 0; i < chart.Series.Count; i++)
            {
                chart.Series[i].Points.Clear();
                chart.Series[i].IsVisibleInLegend = false;
            }
        }
        /// <summary>
        /// Сохранить рисунок с чарта
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="name">Имя файла</param>
        /// <param name="format">Формат изображения</param>
        public static void SaveImageFromChart(Chart chart, string name = "Без имени", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat format = System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить рисунок как...";
            savedialog.FileName = System.IO.Path.Combine(Environment.CurrentDirectory, name + ".png");
            savedialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";

            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.ShowHelp = true;
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    chart.SaveImage(savedialog.FileName, format);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Рисунок не сохранён", ee.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static Color[] DefaltColors
        {
            get
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Color[] m = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Yellow, Color.Black, Color.Chocolate, Color.HotPink };
                return m;
            }
        }
        /// <summary>
        /// Создание нескольких Series, подходящих под условия
        /// </summary>
        /// <param name="chart">Чарт, в котором ведётся создание</param>
        /// <param name="names">Массив имён</param>
        /// <param name="width">Ширина кривых</param>
        /// <param name="type">Тип графиков</param>
        /// <param name="style">Стиль рисования</param>
        /// <param name="mas">Массив цветов</param>
        public static void CreatSeries(ref Chart chart, string[] names, int width = 3, SeriesChartType type = SeriesChartType.Line, MarkerStyle style = MarkerStyle.Circle, Color[] mas = null)
        {
            if (mas == null)
                mas = DefaltColors;

            for (int i = 0; i < names.Length; i++)
            {
                chart.Series.Add(names[i]);
                chart.Series.Last().BorderWidth = width;
                chart.Series.Last().ChartType = type;
                chart.Series.Last().MarkerStyle = style;
                chart.Series.Last().BorderColor = mas[i];
            }
        }
        /// <summary>
        /// Рисование простого графика по массиву аргументов и массиву массивов значений на этих аргументах
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="args"></param>
        /// <param name="values"></param>
        public static void AddMasOfPoints(ref Chart chart, double[] args, params double[][] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                chart.Series[i].Points.Clear();
                chart.Series[i].Points.DataBindXY(args, values[i]);
            }
        }
    }

    public static class ImageActions
    {
        /// <summary>
        /// Слияние двух изображений в одно (рисует одно над другим снизу вверх)
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public static Image MergerOfImages(Image img1, Image img2)
        {
            Bitmap res = new Bitmap(Math.Max(img1.Width, img2.Width), img1.Height + img2.Height);

            Graphics g = Graphics.FromImage(res);
            g.DrawImage(img1, 0, 0);
            g.DrawImage(img2, 0, img1.Height);

            return res;
        }

        /// <summary>
        /// Слияние двух изображений и их сохранение в растровом и векторном форматах
        /// </summary>
        /// <param name="f1">Имя файла с нижним изображением</param>
        /// <param name="f2">Имя файла с верхним изображением</param>
        /// <param name="f3">Имя результирующего файла</param>
        public static void SaveRastAndVec(string f1, string f2, string f3)
        {
            var im = ImageActions.MergerOfImages(Image.FromFile(f2), Image.FromFile(f1));//
            im.Save(f3, System.Drawing.Imaging.ImageFormat.Bmp);
            im.Save(f3.Substring(0, f3.IndexOf(".bmp")) + ".emf", System.Drawing.Imaging.ImageFormat.Emf);
        }
    }

    public static class Other
    {
        private static void CopyControl(Control sourceControl, ref Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (PropertyInfo sourceProperty in sourceControl.GetType().GetProperties())
            {
                object newValue = sourceProperty.GetValue(sourceControl, null);

                MethodInfo mi = sourceProperty.GetSetMethod(true);
                if (mi != null)
                {
                    sourceProperty.SetValue(targetControl, newValue, null);
                }
            }
        }
        /// <summary>
        /// Массив разных цветов
        /// </summary>
        public static Color[] colors = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Violet, Color.OrangeRed, Color.Chocolate, Color.HotPink, Color.Orange, Color.Gold };

        private static Bitmap getControlScreenshot(Control c)
        {
            Bitmap res = new Bitmap(c.Width, c.Height);
            c.DrawToBitmap(res, new Rectangle(Point.Empty, c.Size));
            return res;
        }
        public static void MakeScreenShot(Control control, string name, System.Drawing.Imaging.ImageFormat format = null)
        {
            format = format ?? System.Drawing.Imaging.ImageFormat.Png;
            Bitmap scr = getControlScreenshot(control);
            scr.Save(name, format);
        }
    }
}
