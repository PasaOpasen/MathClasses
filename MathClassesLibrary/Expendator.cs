using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Random;

namespace МатКлассы
{
    /// <summary>
    /// Класс расширений для всяких методов
    /// </summary>
    public static class Expendator
    {
        static readonly SystemRandomSource randomgen = new SystemRandomSource();


        #region Функциональные переводчик
        /// <summary>
        /// Перевести действительную функцию комплексного переменного в функционал
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Functional ToFunctional(RealFuncOfCompArg f)
        {
            return (Point z) =>
            {
                return f(new Complex(z.x, z.y));
            };
        }
        /// <summary>
        /// Перевести функционал в действительную функцию комплексного переменного
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static RealFuncOfCompArg ToRealFuncOfCompArg(Functional f)
        {
            return (Complex z) =>
            {
                return f(new Point(z.Re, z.Im));
            };
        }
        /// <summary>
        /// Перевести функционал в функцию комплесного переменного
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexFunc ToCompFunc(Functional f)
        {
            return (Complex z) => f(new Point(z.Re, z.Im));
        }
        /// <summary>
        /// Преобразовать действительную функцию комплексного переменного в комплексную функцию
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexFunc ToCompFunc(RealFuncOfCompArg f) => (Complex z) => f(z);
        #endregion

        /// <summary>
        /// Минимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double Min(params double[] c)
        {
            double min = Math.Min(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) min = Math.Min(min, c[i]);
            return min;
        }

        public static int Min(params int[] c)
        {
            int min = Math.Min(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) min = Math.Min(min, c[i]);
            return min;
        }

        /// <summary>
        /// Максимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double Max(params double[] c)
        {
            double max = Math.Max(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) max = Math.Max(max, c[i]);
            return max;
        }

        /// <summary>
        /// Максимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int Max(params int[] c)
        {
            int max = Math.Max(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) max = Math.Max(max, c[i]);
            return max;
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static double Min(MultiFunc F)
        {
            return -1;
        }

        public static double Max(MultiFunc F)
        {
            MultiFunc e = (double[] x) => -F(x);
            return -Min(e);
        }

        public static double Max(this double[,] mas)
        {
            double d = Double.NegativeInfinity;
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                    if (mas[i, j] > d)
                        d = mas[i, j];
            return d;
        }

        /// <summary>
        /// Вывести строковое предаставление типа на консоль
        /// </summary>
        /// <param name="i"></param>
        public static void Show<T>(this T i) => Console.WriteLine(i.ToString());

        /// <summary>
        /// Строковое представление массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void Show<T>(this T[] t)
        {
            Console.WriteLine("---Begin");
            for (int i = 0; i < t.Length; i++)
                Console.WriteLine(Convert.ToString(t[i]));
            Console.WriteLine("---End");
        }

        /// <summary>
        /// Сумма первых N членов функционального ряда в точке x
        /// </summary>
        /// <param name="f"></param>
        /// <param name="x"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public static double Sum(SequenceFunc f, double x, int N)
        {
            double sum = 0;
            for (int i = 0; i < N; i++)
                sum += f(x, i);
            return sum;
        }

        public static double Abs(this double x) => Math.Abs(x);

        /// <summary>
        /// Более точное среднее арифметическое для двух чисел
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Average(Complex a, Complex b)
        {
            if (Math.Sign(a.Re) == Math.Sign(b.Re) && Math.Sign(a.Im) == Math.Sign(b.Im))
                return a + (b - a) / 2;
            return (a + b) / 2;
        }
        /// <summary>
        /// Переводит строку в действительное число через конвертер
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ToDouble(this string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception e)
            {
               s = s.Replace('.', ',');
                try
                {
                    return Convert.ToDouble(s);
                }
                catch
                {
                    if (s == "NA" || s == "nan" || s == "NaN" || s == "Nan")
                        return Double.NaN;
                    else
                        throw e;
                }
            }
        }

        public static string Swap(this string s, char a, char b)
        {
            var mas = s.ToCharArray();
            int ai = s.IndexOf(a), bi = s.IndexOf(b);
            char tmp = mas[ai];
            mas[ai] = mas[bi];
            mas[bi] = tmp;
            return new string(mas);
        }

        public static double Reverse(this double val)
        {
            return 1.0 / val;
        }
        public static Complex Reverse(this Complex val)
        {
            double abs = val.Abs;
            if (Double.IsNaN(abs) || Double.IsInfinity(abs)) return 0;
            return 1.0 / val;
        }
        public static double Sqr(this double val) => val * val;
        public static Complex Sqr(this Complex val) => val * val;
        public static double Pow(this double v, double deg) => Math.Pow(v, deg);
        public static Complex Pow(this Complex v, double deg) => Complex.Pow(v, deg);

        /// <summary>
        /// Выдаёт массив действительных частей элементов комплексного массива
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(this Complex[] c)
        {
            double[] res = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = c[i].Re;
            return res;
        }
        /// <summary>
        /// Переводит произвольный массив в массив действительных чисел через конвертер
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas<T>(this T[] c)
        {
            double[] res = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = Convert.ToDouble(c[i]);
            return res;
        }

        /// <summary>
        /// Переводит произвольный массив в массив целых чисел через конвертер
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int[] ToIntMas<T>(this T[] c)
        {
            int[] res = new int[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = Convert.ToInt32(c[i]);
            return res;
        }

        /// <summary>
        /// Перевести строку с числами в набор чисел
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int[] ToIntMas(this string s) => s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToIntMas();

        /// <summary>
        /// Переводит произвольный массив в массив строк через конвертер
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string[] ToStringMas<T>(this T[] c)
        {
            string[] res = new string[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = Convert.ToString(c[i]);
            return res;
        }
        /// <summary>
        /// Конкатенация двух массивов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T[] Union<T>(T[] a, T[] b)
        {
            T[] res = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                res[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                res[i + a.Length] = b[i];
            return res;
        }

        public static T[] Union<T>(params T[][] c)
        {
            //возможно, надо будет как-то преобразовать массив с
            int d2 = c.GetLength(1);
            int len = 0;
            for (int i = 0; i < d2; i++)
                len += c[i].Length;
            T[] res = new T[len];
            len = 0;
            for (int i = 0; i < d2; i++)
                for (int j = 0; j < c[i].Length; j++)
                    res[len++] = c[i][j];

            return res;
        }

        /// <summary>
        /// Возвращает исходный массив без указанного элемента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static T[] Without<T>(this T[] array,T elem) => array.Where(s => !s.Equals(elem)).ToArray();
       

        public class Compar : Comparer<double>
        {
            /// <summary>
            /// Компаратор по модулю
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public override int Compare(double x, double y)
            {
                return x.Abs().CompareTo(y.Abs());
            }

        }
        public class ComparPointTres<Tres> : Comparer<Tuple<Point, Tres>>
        {
            public override int Compare(Tuple<Point, Tres> x, Tuple<Point, Tres> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }

        /// <summary>
        /// Размерность дробной части (количество знаков после запятой)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int DimOfFractionalPath(this double d)
        {
            string s = d.ToString();
            if (s.Contains("E-"))
            {
                int i = s.IndexOf('E');
                int deg = (int)s.Substring(i + 2).ToDouble();
                if (s.Contains(',')) deg--;
                return i + deg - 1;
            }
            else
            {
                int i = s.IndexOf(',');
                if (i <= 0) return 0;
                int deg = s.Substring(i + 1).Length;
                return deg;
            }
        }

        public static decimal ToDecimal(this double i) => Convert.ToDecimal(i);
        public static float ToFloat(this double i) => Convert.ToSingle(i);
        public static double ToDouble(this int i) => (double)i;
        public static int ToInt(this double i) => (int)i;

        /// <summary>
        /// Среднее двух целых чисел (по правилам целочисленного деления)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Average(int a, int b) => (a + b) / 2;
        /// <summary>
        /// Сокращение числа на период
        /// </summary>
        /// <param name="d"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double ToPeriod(this double d, double period)
        {
            int f = (int)Math.Floor(d / period);
            return d - f * period;
        }

        /// <summary>
        /// Перевод матрицы в массив System.Numerics.Complex
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static System.Numerics.Complex[,] ToSystemNumComplex(this CSqMatrix mat)
        {
            System.Numerics.Complex[,] mas = new System.Numerics.Complex[mat.ColCount, mat.ColCount];
            for (int i = 0; i < mat.ColCount; i++)
                for (int j = 0; j < mat.ColCount; j++)
                    mas[i, j] = new System.Numerics.Complex(mat[i, j].Re, mat[i, j].Im);
            return mas;
        }
        /// <summary>
        /// Перевод в матрицу массива System.Numerics.Complex
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static CSqMatrix ToCSqMatrix(this System.Numerics.Complex[,] mas)
        {
            int k = mas.GetLength(0);
            Complex[,] res = new Complex[k, k];
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    res[i, j] = new Complex(mas[i, j].Real, mas[i, j].Imaginary);
            return new CSqMatrix(res);
        }

        /// <summary>
        /// Перевод числа в строку с обрезанием дробной части (оставить только n знаков после запятой)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ToString(this double d, int n)
        {
            n++;
            string s = d.ToString();
            int p = s.IndexOf(',');
            int e = s.IndexOf('E');
            if (e <= 0) e = s.Length;
            if (p > 0)
            {
                n = Math.Min(n, s.Length - p);
                return s.Substring(0, p + n) + s.Substring(e, s.Length - e);
            }
            else return s;

        }

        /// <summary>
        /// Кубический сплайн по сеточной функции с коэффициентами условий на границе
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is0outcut">Должна ли функция равняться 0 вне отрезка задания</param>
        /// <returns></returns>
        public static Func<double,double> ToSpline(this NetFunc f, double a = 0, double b = 0, bool is0outcut = true) => Polynom.CubeSpline(f.Points, a, b, is0outcut);

        /// <summary>
        /// Сумма комплексного массива
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex Sum(this Complex[] mas)
        {
            Complex sum = 0;
            for (int i = 0; i < mas.Length; i++)
                sum += mas[i];
            return sum;
        }
        /// <summary>
        /// Сумма комплексных массивов
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex[] Adding(this Complex[] mas, Complex[] mas2)
        {
            Complex[] sum = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                sum[i] = mas[i] + mas2[i];
            return sum;
        }
        /// <summary>
        /// Сумма комплексных массивов
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex[] Add(Complex[] mas, Complex[] mas2)
        {
            Complex[] sum = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                sum[i] = mas[i] + mas2[i];
            return sum;
        }
        /// <summary>
        /// Действительная часть комплексного массива
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] Re(this Complex[] m)
        {
            double[] res = new double[m.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = m[i].Re;
            return res;
        }
        /// <summary>
        /// Действительная часть комплексного массива
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] ReOf(Complex[] m)
        {
            double[] res = new double[m.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = m[i].Re;
            return res;
        }

        /// <summary>
        /// Вывести пустую строку
        /// </summary>
        public static void EmptyLine(int count =1)
        {
            for (int i = 0; i < count; i++)
                Console.WriteLine();
        }

        /// <summary>
        /// Примерный максимум модуля функции на отрезке
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double MaxApproxAbs(ComplexFunc f, double beg, double end, double step = 0.01)
        {
            double m(double c) => f(c).Abs;
            double max = m(beg), tmp;
            while (beg <= end)
            {
                beg += step;
                tmp = m(beg);
                if (tmp > max)
                    max = tmp;  //max.Show();           
            }
            return max;
        }

        /// <summary>
        /// Объединение двух массивов с удалением одного из двух близких (ближе eps) друг к другу элементов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static double[] Distinct(double[] a, double[] b, double eps = 1e-6)
        {
            double[] m = Expendator.Union(a, b);
            Array.Sort(m);
            List<double> l = new List<double>(a.Length);
            l.Add(m[0]);
            int k = 0;
            for (int i = 1; i < m.Length; i++)
                if (m[i] - m[k] >= eps)
                {
                    l.Add(m[i]);
                    k = i;
                }
            return l.ToArray();
        }

        /// <summary>
        /// Деление комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Div(this Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] / coef;
            return res;
        }
        /// <summary>
        /// Умножение комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Multiply(this Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] * coef;
            return res;
        }
        /// <summary>
        /// Умножение комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Mult(Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] * coef;
            return res;
        }
        /// <summary>
        /// Умножение пары комплексных чисел на комплексное числл
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static (Complex ur, Complex uz) Mult((Complex ur, Complex uz) mas, Complex coef) => (mas.ur * coef, mas.uz * coef);

        /// <summary>
        /// Записать массив векторов в файл
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mas"></param>
        public static void WriteInFile(string name = "1", params Vectors[] mas)
        {
            StreamWriter f = new StreamWriter(name + ".txt");
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = 0; j < mas[i].Deg - 1; j++)
                    f.Write(mas[i][j] + " ");
                f.WriteLine(mas[i][mas[i].Deg - 1]);
            }
            f.Close();
        }

        /// <summary>
        /// Запустить процесс и выполнить какие-то действия по его окончанию
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="act"></param>
        public static void StartProcess(string fileName, Action act)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) => act();
            process.Start();
        }

        /// <summary>
        /// Дубликат массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static T[] Dup<T>(this T[] mas) where T : struct
        {
            T[] res = new T[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = mas[i];
            return res;
        }
        /// <summary>
        /// Дубликат массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static T[] dup<T>(this T[] mas) where T : Idup<T>
        {
            T[] res = new T[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = mas[i].dup;
            return res;
        }
        /// <summary>
        /// Срез массива
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] mas, int i, int j) where T : Idup<T>
        {
            T[] res = new T[j - i + 1];
            for (int s = 0; s < res.Length; s++)
                res[s] = mas[i + s].dup;
            return res;
        }
        /// <summary>
        /// Срез массива
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static double[] Slice(this double[] mas, int i, int j)
        {
            double[] res = new double[j - i + 1];
            for (int s = 0; s < res.Length; s++)
                res[s] = mas[i + s];
            return res;
        }

        public static int ToInt32(this string s) => Convert.ToInt32(s);
        public static int ToInt32(this object s) => Convert.ToInt32(s);

        public static Complex[] ToComplex(this double[] m) => new CVectors(m).ComplexMas;

        /// <summary>
        /// Преобразовать строку в массив действительных чисел
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(this string s)=>s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(st=>Convert.ToDouble(st)).ToArray();
        /// <summary>
        /// Преобразовать число в строку, из которой его можно воспроизвести
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToRString(this double s) => s.ToString("r");

        /// <summary>
        /// Создать и заполнить массив алгебраической прогрессией
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double[] Seq(double from, double to, int count, bool withto = true)
        {
            double[] res = new double[count];
            double h;
            if (withto)
                h = (to - from) / (count - 1);
            else
                h = (to - from) / (count);

            for (int i = 0; i < count; i++)
                res[i] = from + i * h;
            return res;
        }

        /// <summary>
        /// Применяет функцию к массиву и возвращает массив результатов
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="mas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T1[] Map<T1, T2>(this T2[] mas, Func<T2, T1> func)
        {
            T1[] res = new T1[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = func(mas[i]);
            return res;
        }

        /// <summary>
        /// Возвращает пару индексов массива, между элементами которого находится искомый элемент либо пару одинаковых индексов, когда элемент совпадает с каким-то элементом массива
        /// </summary>
        /// <param name="w"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        public static Tuple<int, int> BinarySearch(double[] w, double el)
        {
            int i = 0, j = w.Length - 1, c;
            if (el < w[0]) return new Tuple<int, int>(i, i);
            if (el > w[j]) return new Tuple<int, int>(j, j);

            while (j - i > 1)
            {
                c = (i + j) / 2;
                if (w[c] == el) return new Tuple<int, int>(c, c);
                if (el > w[c]) i = c;
                else j = c;
            }
            return new Tuple<int, int>(i, j);
        }

        /// <summary>
        /// Найти период в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static int GetPeriod<T>(T[] mas)
        {
            bool f = true;
            for (int k = 1; k <= mas.Length / 2; k++)
            {
                f = true;
                for (int i = 0; i < k; i++)
                {
                    int s = 0;
                    while (i + (k) * (s + 1) < mas.Length)
                    {
                        if (!mas[i + k * s].Equals(mas[i + (k) * (s + 1)]))
                        {
                            f = false;
                            goto z1;
                        }
                        s++;
                    }

                }
            z1:
                if (f) return k;
            }
            return 0;
        }

        /// <summary>
        /// Получить отношение двух чисел в процентах
        /// </summary>
        /// <param name="exist"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static double GetProcent(double exist, double all) => exist / all * 100.0;
        /// <summary>
        /// Вернуть округлённый процент
        /// </summary>
        /// <param name="exist"></param>
        /// <param name="all"></param>
        /// <param name="mantis"></param>
        /// <returns></returns>
        public static double GetProcent(double exist, double all, int mantis) => Math.Round(GetProcent(exist, all), mantis);

        public static CVectors ToCVector(this Complex[] m) => new CVectors(m);

        /// <summary>
        /// Записать одно слово в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="word"></param>
        public static void WriteStringInFile(string filename, string word)
        {
            using (StreamWriter f = new StreamWriter(filename))
                f.WriteLine(word);
        }
        /// <summary>
        /// Записать массив слов в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="word"></param>
        /// <param name="withoutfromend">Определяет, сколько строк с конца добавлять не нужно</param>
        public static void WriteInFile(string filename, string[] word, int withoutfromend = 0)
        {
            using (StreamWriter f = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                for (int i = 0; i < word.Length - withoutfromend; i++)
                    f.WriteLine(word[i]);
        }

        /// <summary>
        /// Прочесть все строки файла
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="withoutEmpty">Определяет, нужно ли считывать пустые строки</param>
        /// <returns></returns>
        public static string[] GetStringArrayFromFile(string filename, bool withoutEmpty = false)
        {
            string[] st;
            using (StreamReader f = new StreamReader(filename))
                st = f.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (withoutEmpty)
                st = st.Where(n => n.Length > 0).ToArray();
            return st;
        }

        /// <summary>
        /// Получить первую строку из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetWordFromFile(string filename)
        {
            string s;
            try
            {
                s = GetStringArrayFromFile(filename, true)[0];
            }
            catch
            {
                s = "";
            }
            return s;
        }

        /// <summary>
        /// Определяет директорию, считанную из файла и возвращает ответ о её существовании
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool IfDirectoryExists(string filename, out string directory)
        {
            directory = GetWordFromFile(filename);
            return Directory.Exists(directory);
        }

        /// <summary>
        /// Скопировать набор файлов из одной директории в другую, сохраняя имена
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="filenames"></param>
        public static void CopyFiles(string from, string to, params string[] filenames)
        {
            for (int i = 0; i < filenames.Length; i++)
                File.Copy(Path.Combine(from, filenames[i]), Path.Combine(to, filenames[i]), true);
        }

        /// <summary>
        /// Возвращает полный адрес ресурса по его краткому имени и проекту
        /// </summary>
        /// <param name="name">Имя файла с расширением</param>
        /// <param name="projectname">Имя проекта</param>
        /// <returns></returns>
        public static string GetResource(string name, string projectname)
        {
            string s = Environment.CurrentDirectory;
            s = s.Substring(0, s.IndexOf(projectname) + projectname.Length);
            return Path.Combine(s, "Resources", name);
        }
        /// <summary>
        /// Возвращает полный адрес ресурса по его краткому имени и проекту
        /// </summary>
        /// <param name="name">Имя файла с расширением</param>
        public static string GetResource(string name) => Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory)), "Resources", name);

        /// <summary>
        /// Переводит массив строк в одну строку
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string StringArrayToString(string[] array)
        {
            string res = "";
            for (int i = 0; i < array.Length - 1; i++)
                res += $"{array[i]}{Environment.NewLine}";
            return res + array[array.Length - 1];
        }

        /// <summary>
        /// Записать массив чисел в строку
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToStringFromExp(this int[] array)
        {
            string s = array[0].ToString();
            for (int i = 1; i < array.Length; i++)
                s += $" {array[i]}";
            return s;
        }

        /// <summary>
        /// Покомпонентный массив максимумов
        /// </summary>
        /// <param name="ar1"></param>
        /// <param name="ar2"></param>
        /// <returns></returns>
        public static int[] Max(int[] ar1, int[] ar2)
        {
            int[] res = new int[Math.Max(ar1.Length, ar2.Length)];

            if (ar1.Length == ar2.Length)
                for (int i = 0; i < res.Length; i++)
                    res[i] = Math.Max(ar1[i], ar2[i]);
            else if (ar1.Length > ar2.Length)
            {
                for (int i = 0; i < ar2.Length; i++)
                    res[i] = Math.Max(ar1[i], ar2[i]);
                for (int i = ar2.Length; i < ar1.Length; i++)
                    res[i] = ar1[i];
            }
            else
            {
                for (int i = 0; i < ar1.Length; i++)
                    res[i] = Math.Max(ar1[i], ar2[i]);
                for (int i = ar1.Length; i < ar2.Length; i++)
                    res[i] = ar2[i];
            }

            return res;
        }

        /// <summary>
        /// Повторить число count раз
        /// </summary>
        /// <param name="number"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] Repeat(int number, int count)
        {
            int[] m = new int[count];
            for (int i = 0; i < count; i++)
                m[i] = number;
            return m;
        }



        public static void StartProcessOnly(string fileName, bool global = false, string path = null)
        {
            path = path ?? Environment.CurrentDirectory;

            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(path, fileName);
            process.StartInfo.WorkingDirectory = path;

            if (!global)
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            process.Start();

            process.WaitForExit();

        }


        /// <summary>
        /// Осуществить копирование папки
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static async Task DirectoryCopyAsync(string from,string to)
        {
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.FileName = @"C:\WINDOWS\system32\xcopy.exe";
            proc.StartInfo.Arguments = $"{from} {to} /E /I /C /Y"; //https://ab57.ru/cmdlist/xcopy.html
            // /E = скопировать подпапки(пустые в том числе!).
            // /I = если дестинейшн не существует, создать папку с нужным именем. 
            await Task.Run(() =>
            {
                proc.Start();
                proc.WaitForExit();
            });
        }

        /// <summary>
        /// Возвращает индексы элементов массива, сумма которых равна указанному значению
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
        public static int[] IndexesWhichGetSum(int[] arr,int sum)
        {
            var len = arr.Length;
            string s;
            int sm;
            for(int i = 1; i < Math.Pow(2, len); i++)
            {
                s = Convert.ToString(i, 2);
                sm = 0;
                
                for (int j = 0; j < s.Length; j++)
                    if (s[j] == '1')
                        sm += arr[j];

                if (sm == sum)
                {
                  List<int> t=new List<int>(len);
                    for (int j = 0; j < s.Length; j++)
                        if (s[j] == '1')
                            t.Add(j);
                    return t.ToArray();
                }
            }

            return new int[0];
        }



        /// <summary>
        /// Возвращает случайное число из массива vals с вероятностью из массива probs
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="probs"></param>
        /// <returns></returns>
        public static int GetRandomNumberFromArrayWithProbabilities(int[] vals,double[] probs)
        {
            var vers = new double[probs.Length];
            double sum = probs.Sum();
            
            vers[0] =probs[0]/ sum;
            for (int i = 1; i < vers.Length-1; i++)
            {
                vers[i] = probs[i] / sum + vers[i - 1];
            }
            vers[vers.Length - 1] = 1.0;

            double rndval = randomgen.NextDouble();
            for (int i = 0; i < vers.Length; i++)
                if (vers[i] >= rndval)
                    return vals[i];
            return vals.Last();
        }

        /// <summary>
        /// Возвращает случайный элемент из массива vals с вероятностью из массива probs
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="probs"></param>
        /// <returns></returns>
        public static T GetRandomElementFromArrayWithProbabilities<T>(T[] vals, double[] probs)
        {
            var vers = new double[probs.Length];
            double sum = probs.Sum();

            vers[0] = probs[0] / sum;
            for (int i = 1; i < vers.Length - 1; i++)
            {
                vers[i] = probs[i] / sum + vers[i - 1];
            }
            vers[vers.Length - 1] = 1.0;

            double rndval = randomgen.NextDouble();
            for (int i = 0; i < vers.Length; i++)
                if (vers[i] >= rndval)
                    return vals[i];
            return vals.Last();
        }
        /// <summary>
        /// Возвращает случайный элемент из массива vals с вероятностью из массива probs
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="probs"></param>
        /// <returns></returns>
        public static T GetRandomElementFromArrayWithProbabilities<T>(T[] vals, int[] probs)
        {
            var vers = new double[probs.Length];
            double sum = probs.Sum();

            vers[0] = probs[0] / sum;
            for (int i = 1; i < vers.Length - 1; i++)
            {
                vers[i] = probs[i] / sum + vers[i - 1];
            }
            vers[vers.Length - 1] = 1.0;

            double rndval = randomgen.NextDouble();
            for (int i = 0; i < vers.Length; i++)
                if (vers[i] >= rndval)
                    return vals[i];
            return vals.Last();
        }
    }

}