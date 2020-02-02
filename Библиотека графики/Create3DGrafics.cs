using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using МатКлассы;
using System.IO;
using nzy3d_winformsDemo;
using System.Windows.Forms;

namespace Библиотека_графики
{
    /// <summary>
    /// Класс с методами для создания 3D графиков
    /// </summary>
    public static class Create3DGrafics
    {

        private static async Task GetDataToFileAsync(string shortname, string savename, Func<double, double, double> F, double[] x, double[] y, IProgress<int> progress, System.Threading.CancellationToken token, StringsForGrafic forGrafic, bool parallel = true)
        {
            int lenx = x.Length;
            int leny = y.Length;
            int[] k = new int[lenx * leny];
            double[,] ur = new double[lenx, leny];

            void InnerLoop(int i)
            {
                for (int j = 0; j < leny; j++)
                {
                    if (token.IsCancellationRequested) return;
                    ur[i, j] = F(x[i], y[j]);
                    k[i * leny + j] = 1;
                }
                progress.Report(k.Sum());
            }

            await Task.Run(() =>
            {
                //нахождение массивов
                if (parallel)
                    Parallel.For(0, lenx, (int i) =>
                    {
                        InnerLoop(i);
                    });
                else
                    for (int i = 0; i < lenx; i++)
                    {
                        InnerLoop(i);
                    }
            });


            var filenames = new string[]
            {
                $"{shortname}(args).txt",
                $"{shortname}(vals).txt",
                $"{shortname}(info).txt"
            };
            Expendator.WriteInFile("3D Grafics Data Adress.txt", filenames);

            forGrafic.WriteInFile(filenames[2], shortname,savename);

            if (lenx == leny)
            {
                using (StreamWriter xs = new StreamWriter(filenames[0]))
                {
                    xs.WriteLine("x y");
                    for (int i = 0; i < lenx; i++)
                        xs.WriteLine($"{x[i]} {y[i]}");
                }

                using (StreamWriter ts = new StreamWriter(filenames[1]))
                {
                    ts.WriteLine("vals");
                    for (int i = 0; i < lenx; i++)
                        for (int j = 0; j < lenx; j++)
                            if (Double.IsNaN(ur[i, j]))
                                ts.WriteLine("NA");
                            else
                                ts.WriteLine($"{ur[i, j]}");
                }
            }
            else
            {
                double max = 0, a = 0, b = 0;
                for (int i = 0; i < lenx; i++)
                    for (int j = 0; j < leny; j++)
                        if (ur[i, j] > max)
                        {
                            max = ur[i, j];
                            a = x[i];
                            b = y[j];
                        }
                Expendator.WriteInFile(savename + "(MaxCoordinate).txt", new string[]
                {
                    "a b",
                    $"{a.ToRString()} {b.ToRString()}".Replace(',','.'),
                    $"maximum is {max}".Replace(',','.'),
                    $"omega = {1.0 / (a * 1000)}"
                });
            }
        }

        private static async Task GetDataToFileAsync(string shortname, Func<double, double, double> F, double xmin, NetOnDouble x, NetOnDouble y, IProgress<int> progress, System.Threading.CancellationToken token, StringsForGrafic forGrafic, bool parallel = true)
        {
            await GetDataToFileAsync(shortname,shortname, F, x.Array, y.Array, progress, token, forGrafic, parallel);
        }

        /// <summary>
        /// Тип графика
        /// </summary>
        public enum GraficType
        {
            /// <summary>
            /// Только Pdf график через persp3D
            /// </summary>
            Pdf,
            /// <summary>
            /// Тепловая карта через ggplot2
            /// </summary>
            Png,
            /// <summary>
            /// Вращающийся график в браузере
            /// </summary>
            Html,
            /// <summary>
            /// Все три варианта
            /// </summary>
            PdfPngHtml,
            /// <summary>
            /// График в окне с возможностью вращения и масштабирования через nzy3d
            /// </summary>
            Window
        }

        /// <summary>
        /// Создать 3D график в нужной форме
        /// </summary>
        /// <param name="graficType">Тип графика</param>
        /// <param name="shortname">Имя файла с графиками (без расширения)</param>
        /// <param name="path">Директория, куда будут сохраняться файлы</param>
        /// <param name="F">Функция, чей график надо построить</param>
        /// <param name="xmin">Начало отрезка по первой координате</param>
        /// <param name="xmax">Конец отрезка по первой координате</param>
        /// <param name="ymin">Начало отрезка по второй координате</param>
        /// <param name="ymax">Конец отрезка по второй координате</param>
        /// <param name="count">Число точек в разбиении отрезка. В сетке будет count*count этих точек</param>
        /// <param name="progress">Объект для отслеживания прогресса</param>
        /// <param name="token">Объект для отмены операции</param>
        /// <param name="title">Название поверхности</param>
        /// <param name="xlab">Название оси X</param>
        /// <param name="ylab">Название оси Y</param>
        /// <param name="zlab">Название оси Z</param>
        /// <param name="parallel">Выполнять ли вычисления параллельно</param>
        public static void MakeGrafic(GraficType graficType, string shortname, Func<double, double, double> F, NetOnDouble x, NetOnDouble y, IProgress<int> progress, System.Threading.CancellationToken token, StringsForGrafic forGrafic, bool parallel = true)
        {
            if (graficType == GraficType.Window)
            {
                new nzy3d_winformsDemo.Form1(forGrafic.Title, x.Begin, x.End, x.Count, y.Begin, y.End, y.Count, F).ShowDialog();
            }
            else
            {
                JustGetGraficInFilesAsync(shortname, shortname, F, x, y, progress, token,forGrafic, graficType,  parallel).GetAwaiter().GetResult();
                GetForm(shortname);
            }
        }
        /// <summary>
        /// Только создать 3D графики с сохранением в файлы
        /// </summary>
        /// <param name="shortname"></param>
        /// <param name="F"></param>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymin"></param>
        /// <param name="ymax"></param>
        /// <param name="count"></param>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <param name="graficType"></param>
        /// <param name="title"></param>
        /// <param name="xlab"></param>
        /// <param name="ylab"></param>
        /// <param name="zlab"></param>
        /// <param name="parallel"></param>
        public static async Task JustGetGraficInFilesAsync(string shortname,string savename, Func<double, double, double> F, NetOnDouble x, NetOnDouble y, IProgress<int> progress, System.Threading.CancellationToken token, StringsForGrafic forGrafic, GraficType graficType = GraficType.PdfPngHtml, bool parallel = true)
        {
            await GetDataToFileAsync(shortname, savename, F, x.Array, y.Array, progress, token, forGrafic, parallel);
            GraficTypeToFile(graficType);
            RemoveOlds(shortname);
            if (x.Count == y.Count)
                await Task.Run(() => Expendator.StartProcessOnly("Magic3Dscript.R"));
        }

        private static void GraficTypeToFile(GraficType type)
        {
            string s = "";
            switch (type)
            {
                case GraficType.Html:
                    s = "html";
                    break;
                case GraficType.Pdf:
                    s = "pdf";
                    break;
                case GraficType.Png:
                    s = "png";
                    break;
                default:
                    s = "all";
                    break;
            }
            Expendator.WriteStringInFile("GraficType.txt", s);
        }
        private static List<string> GetPaths(string shortname)
        {
            return new List<string>(new string[]
            {
                shortname+".pdf",
                shortname+".png",
                shortname+".html"
            });
        }
        private static void RemoveOlds(string name)
        {
            var p = GetPaths(name);
            foreach (var s in p)
                if (File.Exists(s))
                    File.Delete(s);
        }
        public static void GetForm(string shortname)
        {
            List<string> names = new List<string>(new string[] { "pdf", "png", "html" });
            List<string> paths = GetPaths(shortname);

            for (int i = 0; i < paths.Count; i++)
                if (!File.Exists(paths[i]) || new FileInfo(paths[i]).Length < 6000)
                {
                    names.RemoveAt(i);
                    paths.RemoveAt(i);
                    i--;
                }

            if (paths.Count > 0)
                new ManyDocumentsShower("3D grafics", names.ToArray(), paths.ToArray()).ShowDialog();
            else
                MessageBox.Show("Ни одного файла не получилось", "Ошибочка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    /// <summary>
    /// Набор строк, нужных при построении графиков
    /// </summary>
    public sealed class StringsForGrafic
    {
        public readonly string Title;
        public readonly string XLabel;
        public readonly string Ylabel;
        public readonly string Zlabel;

        public StringsForGrafic(string title = "", string xlab = "x", string ylab = "y", string zlab = "z")
        {
            Title = title;
            XLabel = xlab;
            Ylabel = ylab;
            Zlabel = zlab;
        }

        public void WriteInFile(string filename) => Expendator.WriteInFile(filename, new string[]
            {
                Title,
                XLabel,Ylabel,Zlabel
            });

        public void WriteInFile(string filename, string shortname,string savename) => Expendator.WriteInFile(filename, new string[]
        {
                shortname,
                savename,
                Title,
                XLabel,Ylabel,Zlabel
        });

    }
}
