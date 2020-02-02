using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static МатКлассы.Number;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace МатКлассы
{
    /// <summary>
    /// Класс всяких полезных штук для работы с другими программами
    /// </summary>
    public class ИнтеграцияСДругимиПрограммами
    {
        private static string CharForExcel(char begchar, int count)
        {
            int tmp = (count - Convert.ToInt32('A') + Convert.ToInt32(begchar));//tmp.Show();
            int tmp2 = tmp / 26;
            string ress = "";
            while (tmp2 > 0)
            {
                char s = (tmp2 > 0) ? Convert.ToChar(Convert.ToInt32('A') + tmp2 % 26 - 1) : default(char);
                ress = s.ToString() + ress;
                tmp2 /= 26;
            }
            tmp %= 26;
            string res = (Convert.ToChar(Convert.ToInt32('A') + tmp)).ToString();
            //if (tmp2 > 0)
            res = ress + res;
            //res.Show();
            return res;
        }

        /// <summary>
        /// Создать в Excel таблицу, по которой можно построить 3D поверхность
        /// </summary>
        /// <param name="f">Функция двух переменных</param>
        /// <param name="x0">Начало отрезка по первому аргументу</param>
        /// <param name="X">Конец отрезка по первому аргументу</param>
        /// <param name="xcount">Число шагов по первому аргументу</param>
        /// <param name="y0">Начало отрезка по второму аргументу</param>
        /// <param name="Y">Конец отрезка по второму аргументу</param>
        /// <param name="ycount">Число шагов по второму аргументу</param>
        public static void CreatTableInExcel(DRealFunc f, double x0, double X, int xcount, double y0, double Y, int ycount)
        {
            Application application = new Application();
            application.Workbooks.Add(Type.Missing);
            Worksheet sheet = (Worksheet)application.Sheets[1];
            var t = Type.Missing;

            double hx = (X - x0) / (xcount - 1), hy = (Y - y0) / (ycount - 1);

            for (int i = 0; i < ycount; i++)
            {
                sheet.Range[CharForExcel('B', i) + "2", t].Value2 = y0 + i * hy;
            }
            for (int i = 0; i < xcount; i++)
            {
                sheet.Range["A" + (3 + i).ToString(), t].Value2 = x0 + i * hx;
            }

            //for (int j = 0; j < xcount; j++)
            Parallel.For(0, xcount, (int j) =>
            {
                for (int i = 0; i < ycount; i++)
                    sheet.Range[CharForExcel('B', i) + (3 + j).ToString(), t].Value2 = f(sheet.Range["A" + (3 + j).ToString(), t].Value2, sheet.Range[CharForExcel('B', i) + "2", t].Value2);
            });

            sheet.Range["A2", CharForExcel('B', ycount) + (3 + xcount).ToString()].Activate();

            Excel.Chart chart = sheet.ChartObjects().Add(10, 10, 500, 500).Chart;
            chart.ChartType = Excel.XlChartType.xl3DArea;
            chart.SetSourceData(sheet.Range["A2", CharForExcel('B', ycount) + (3 + xcount).ToString()]);

            application.Visible = true;
        }
        /// <summary>
        /// Создать в Excel таблицу, по которой можно построить серию графиков функциё одной переменной
        /// </summary>
        /// <param name="args">Массив аргументов</param>
        /// <param name="values">Массив векторов значений</param>
        public static void CreatTableInExcel<T>(T[] args, params Vectors[] values)
        {
            Application application = new Application();
            application.Workbooks.Add(Type.Missing);
            Worksheet sheet = (Worksheet)application.Sheets[1];
            var t = Type.Missing;

            for (int i = 0; i < args.Length; i++)
            {
                sheet.Range["A" + (3 + i).ToString(), t].Value2 = args[i].ToString();
            }

            for (int i = 0; i < values.Length; i++)
                for (int j = 0; j < values[i].Deg; j++)
                    sheet.Range[CharForExcel('B', i) + (3 + j).ToString(), t].Value2 = (values[i][j]);

            sheet.Range["A2", CharForExcel('B', values.Length) + (3 + values[0].Deg).ToString()].Activate();
            application.Visible = true;
        }
        /// <summary>
        /// Создать в Excel таблицу, по таблице double
        /// </summary>
        public static void CreatTableInExcel(double[][] mas)
        {
            Application application = new Application();
            application.Workbooks.Add(Type.Missing);
            Worksheet sheet = (Worksheet)application.Sheets[1];
            var t = Type.Missing;

            for (int j = 0; j < mas.GetLength(0); j++)
                for (int i = 0; i < mas.GetLength(1); i++)
                    sheet.Range[CharForExcel('B', i) + (3 + j).ToString(), t].Value2 = mas[j][i];

            sheet.Range["A2", CharForExcel('B', mas.GetLength(1)) + (3 + mas.GetLength(0)).ToString()].Activate();
            application.Visible = true;
        }
        /// <summary>
        /// Создать таблицу по словарю Точка-Комплексное число
        /// </summary>
        /// <typeparam name="Targ"></typeparam>
        /// <param name="dic"></param>
        public static async void CreatTableInExcel(ConcurrentDictionary<Point, Lazy<Complex>> dic, string name = "Name", Complex.ComplMode mode = Complex.ComplMode.Abs)
        {
            Func<Complex, double> f;
            switch (mode)
            {
                case Complex.ComplMode.Abs:
                    f = c => c.Abs;
                    break;
                case Complex.ComplMode.Re:
                    f = c => c.Re;
                    break;
                case Complex.ComplMode.Im:
                    f = c => c.Im;
                    break;
                default:
                    f = c => c.Arg;
                    break;
            }

            var d1 = dic.Keys.ToArray();
            var d2 = dic.Values.ToArray();

            Application application = new Application();
            application.Workbooks.Add(Type.Missing);
            Worksheet sheet = (Worksheet)application.Sheets[1];
            sheet.Name = name;
            var t = Type.Missing;
            //sheet.Range[sheet.Cells[1, 1], sheet.Cells[d1.Count()+3, d1.Count() + 3]].NumberFormat = "Экспоненциальный";
            application.Visible = true;

            var point = new Point[d1.Length];
            for (int i = 0; i < d1.Length; i++)
                point[i] = new Point(d1[i]);

            Array.Sort(point);
            List<double> x = new List<double>(0);
            List<double> y = new List<double>(0);
            await Task.Run(() =>
            {

                for (int i = 0; i < d1.Length; i++)
                {
                    int a = Array.IndexOf(d1, point[i]);
                    int ix = x.IndexOf(point[i].x), iy = y.IndexOf(point[i].y);
                    if (ix < 0)
                    {
                        ix = x.Count;
                        x.Add(point[i].x);
                    }
                    if (iy < 0)
                    {
                        iy = y.Count;
                        y.Add(point[i].y);
                    }
                    sheet.Range[CharForExcel('B', ix) + "2", t].Value2 = point[i].x;
                    sheet.Range["A" + (3 + iy).ToString(), t].Value2 = point[i].y;

                    string tmp = f(d2[a].Value).ToFloat().ToString();
                    sheet.Cells[3 + iy, 1 + ix] = /*f(*/d2[a].Value/*)*/;
                }
            }
            );

            //Область сортировки             
            Microsoft.Office.Interop.Excel.Range range = sheet.get_Range("A3", CharForExcel('B', x.Count) + (3 + y.Count).ToString());
            //По какому столбцу сортировать
            Microsoft.Office.Interop.Excel.Range rangeKey = sheet.get_Range("A3");
            //Добавляем параметры сортировки
            sheet.Sort.SortFields.Add(rangeKey);
            sheet.Sort.SetRange(range);
            sheet.Sort.Orientation = Microsoft.Office.Interop.Excel.XlSortOrientation.xlSortColumns;
            sheet.Sort.SortMethod = Microsoft.Office.Interop.Excel.XlSortMethod.xlPinYin;
            sheet.Sort.Apply();
            //Очищаем фильтр
            sheet.Sort.SortFields.Clear();

            sheet.Range["A2", CharForExcel('B', x.Count) + (3 + y.Count).ToString()].Activate();

        }
        /// <summary>
        /// Создать таблицу и графики по комплексной функции двух действительных переменных
        /// </summary>
        /// <param name="f">Функция</param>
        /// <param name="x0"></param>
        /// <param name="X"></param>
        /// <param name="xcount">Число шагов по оси Х</param>
        /// <param name="y0"></param>
        /// <param name="Y"></param>
        /// <param name="ycount">Число шагов по оси Y</param>
        /// <param name="dic">Дополнительный словарь значений</param>
        /// <param name="withgraph">Нужно ли вдобавок рисовать графики</param>
        public static async void CreatTableInExcel(DComplexFunc f, double x0, double X, int xcount, double y0, double Y, int ycount, ConcurrentDictionary<Point, Lazy<Complex>> dic = null, bool withgraph = true)
        {
            FixProcesBefore();

            //задание приложения
            Application application = new Application();
            application.SheetsInNewWorkbook = 4;
            application.Workbooks.Add(Type.Missing);
            Worksheet sheetRe = (Worksheet)application.Sheets[1], sheetIm = (Worksheet)application.Sheets[2], sheetAbs = (Worksheet)application.Sheets[3], sheetArg = (Worksheet)application.Sheets[4];
            var t = Type.Missing;

            sheetRe.Name = "Re";
            sheetIm.Name = "Im";
            sheetAbs.Name = "Abs";
            sheetArg.Name = "Arg";

            //переменные
            double hx = (X - x0) / (xcount - 1), hy = (Y - y0) / (ycount - 1);
            string vs;
            double vd;
            List<double> x = new List<double>(), y = new List<double>();


            await Task.Run(() =>
            {
                //строка и столбец аргументов
                for (int i = 0; i < ycount; i++)
                {
                    vs = CharForExcel('B', i) + "2";
                    vd = y0 + i * hy;
                    y.Add(vd);
                    sheetRe.Range[vs, t].Value2 = vd;
                    sheetIm.Range[vs, t].Value2 = vd;
                    sheetAbs.Range[vs, t].Value2 = vd;
                    sheetArg.Range[vs, t].Value2 = vd;
                }
                for (int i = 0; i < xcount; i++)
                {
                    vs = "A" + (3 + i).ToString();
                    vd = x0 + i * hx;
                    x.Add(vd);
                    sheetRe.Range[vs, t].Value2 = vd;
                    sheetIm.Range[vs, t].Value2 = vd;
                    sheetAbs.Range[vs, t].Value2 = vd;
                    sheetArg.Range[vs, t].Value2 = vd;
                }

                //заполнение значений в таблице
                //for (int j = 0; j < xcount; j++)
                Parallel.For(0, xcount, (int j) =>
                {
                    for (int i = 0; i < ycount; i++)
                    {
                        var vss = CharForExcel('B', i) + (3 + j).ToString();
                        Complex tmp = f(x[j], y[i]);
                        sheetRe.Range[vss, t].Value2 = tmp.Re;
                        sheetIm.Range[vss, t].Value2 = tmp.Im;
                        sheetAbs.Range[vss, t].Value2 = tmp.Abs;
                        sheetArg.Range[vss, t].Value2 = tmp.Arg;
                    }
                });

                //если есть дополнительные значения из словаря
                if (dic != null)
                {
                    ConcurrentDictionary<Point, Lazy<Complex>> dicc = new ConcurrentDictionary<Point, Lazy<Complex>>(dic);
                    //считать ключи и значения
                    var d1 = dicc.Keys.ToArray();
                    var d2 = dicc.Values.ToArray();
                    var point = new Point[d1.Length];
                    for (int i = 0; i < d1.Length; i++)
                        point[i] = new Point(d1[i]);
                    Array.Sort(point);
                    //записать точку в таблицу, если там её не было
                    for (int i = 0; i < d1.Length; i++)
                    {
                        int a = Array.IndexOf(d1, point[i]);
                        int ix = x.IndexOf(point[i].x), iy = y.IndexOf(point[i].y);
                        int ixt = ix, iyt = iy;
                        if (ix < 0)
                        {
                            ix = x.Count;
                            x.Add(point[i].x);
                        }
                        if (iy < 0)
                        {
                            iy = y.Count;
                            y.Add(point[i].y);
                        }
                        if (ixt * iyt <= 0)
                        {

                            for (int j = 1; j < 4; j++)
                            {
                                Worksheet sheet = (Worksheet)application.Sheets[j];
                                sheet.Range[CharForExcel('B', ix) + "2", t].Value2 = point[i].x;
                                sheet.Range["A" + (3 + iy).ToString(), t].Value2 = point[i].y;
                            }
                            Complex tmp = d2[a].Value;
                            sheetRe.Cells[3 + iy, 1 + ix] = tmp.Re;
                            sheetIm.Cells[3 + iy, 1 + ix] = tmp.Im;
                            sheetAbs.Cells[3 + iy, 1 + ix] = tmp.Abs;
                            sheetArg.Cells[3 + iy, 1 + ix] = tmp.Arg;
                        }
                    }

                    //сортировать
                    for (int i = 1; i <= 4; i++)
                    {
                        Worksheet sheet = (Worksheet)application.Sheets[i];
                        //Область сортировки             
                        Microsoft.Office.Interop.Excel.Range range = sheet.get_Range("A3", CharForExcel('B', x.Count) + (3 + y.Count).ToString());
                        //По какому столбцу сортировать
                        Microsoft.Office.Interop.Excel.Range rangeKey = sheet.get_Range("A3");
                        //Добавляем параметры сортировки
                        sheet.Sort.SortFields.Add(rangeKey);
                        sheet.Sort.SetRange(range);
                        sheet.Sort.Orientation = Microsoft.Office.Interop.Excel.XlSortOrientation.xlSortColumns;
                        sheet.Sort.SortMethod = Microsoft.Office.Interop.Excel.XlSortMethod.xlPinYin;
                        sheet.Sort.Apply();
                        //Очищаем фильтр
                        sheet.Sort.SortFields.Clear();

                        //Добавляем параметры сортировки
                        range = sheet.get_Range("B2", CharForExcel('B', x.Count) + (3 + y.Count).ToString());
                        rangeKey = sheet.get_Range("B2");
                        sheet.Sort.SortFields.Add(rangeKey);
                        sheet.Sort.SetRange(range);
                        sheet.Sort.Orientation = Microsoft.Office.Interop.Excel.XlSortOrientation.xlSortRows;
                        sheet.Sort.SortMethod = Microsoft.Office.Interop.Excel.XlSortMethod.xlPinYin;
                        sheet.Sort.Apply();
                        //Очищаем фильтр
                        sheet.Sort.SortFields.Clear();
                    }
                }

                //создать графики
                if (withgraph)
                {
                    vs = CharForExcel('B', ycount) + (3 + xcount).ToString();
                    for (int i = 1; i <= 4; i++)
                    {
                        Worksheet sheet = (Worksheet)application.Sheets[i];
                        //sheet.Range["A2", CharForExcel('B', ycount) + (3 + xcount).ToString()].Activate();
                        Excel.Chart chart = sheet.ChartObjects().Add(10, 10, 800, 800).Chart;
                        chart.ChartType = Excel.XlChartType.xl3DArea;
                        chart.SetSourceData(sheet.Range["A2", vs]);
                    }
                }
            });

            application.Visible = true;
            //application.ThisWorkbook.Save();
            //application.ThisWorkbook.Saved = true;
            //application.ThisWorkbook.SaveCopyAs($"Документ от {DateTime.Now}");
            //application.Quit();
            //application.ActiveWorkbook.Close(0);
            //application.Workbooks.Close();
            //application.Quit();
        }

        //private void ClearMemory(Application excelApp)
        //{
        //    Process excelProcess = Process.GetProcessesByName("EXCEL")[0];
        //    if (!excelProcess.CloseMainWindow())
        //    {
        //        excelProcess.Kill();
        //    }
        //    excelApp.DisplayAlerts = false;
        //    excelApp.ActiveWorkbook.Close(0);
        //    excelApp.Quit();
        //    Marshal.ReleaseComObject(excelApp);
        //}
        static Process[] processesBefore;
        static void FixProcesBefore() => processesBefore = Process.GetProcessesByName("excel");
        public static void Kill()
        {

            // Get Excel processes after opening the file.
            Process[] processesAfter = Process.GetProcessesByName("excel");

            // Now find the process id that was created, and store it.
            int processID = 0;
            foreach (Process process in processesAfter)
            {
                if (!processesBefore.Select(p => p.Id).Contains(process.Id))
                {
                    processID = process.Id;
                }
            }

            // And now kill the process.
            if (processID != 0)
            {
                Process process = Process.GetProcessById(processID);
                //process.Close();
                process.Kill();
            }
        }
    }
}

