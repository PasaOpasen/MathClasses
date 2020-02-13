using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Точки на плоскости
    /// </summary>
    public struct Point : IComparable, ICloneable, Idup<Point>
    {
        /// <summary>
        /// Начало координат в нуле
        /// </summary>
        public static readonly Point Zero;

        /// <summary>
        /// Абцисса
        /// </summary>
        public double x;
        /// <summary>
        /// Ордината
        /// </summary>
        public double y;

        static Point()
        {
            Zero = new Point(0, 0);
        }

        #region Конструкторы и свойства
        /// <summary>
        /// Точка с одинаковыми координатами
        /// </summary>
        /// <param name="a"></param>
        public Point(double a) { x = a; y = a; }

        /// <summary>
        /// Точка по своим координатам
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Point(double a, double b) { x = a; y = b; }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="p"></param>
        public Point(Point p) : this(p.x, p.y) { }

        /// <summary>
        /// Расстояние от точки до (0,0)
        /// </summary>
        public double Abs => Eudistance(this, Zero);

        /// <summary>
        /// Дубликат точки
        /// </summary>
        public Point dup => new Point(this);

        /// <summary>
        /// Точка с переставленными координатами
        /// </summary>
        public Point Swap => new Point(this.y, this.x);
        #endregion

        #region Простые методы
        /// <summary>
        /// Добавить число к обеим координатам точки
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point Add(Point p, double d) => new Point(p.x + d, p.y + d);

        /// <summary>
        /// Сложить точки как вектора
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point Add(Point p, Point d) => new Point(p.x + d.x, p.y + d.y);

        /// <summary>
        /// Сместить точку
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Point Add(Point p, double dx, double dy) => Add(p, new Point(dx, dy));

        /// <summary>
        /// Центр множества точек как их взвешенная сумма
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Point Center(Point[] mas)
        {
            double x = mas[0].x, y = mas[0].y;
            for (int i = 1; i < mas.Length; i++)
            {
                x += mas[i].x;
                y += mas[i].y;
            }
            return new Point(x / mas.Length, y / mas.Length);
        }
        /// <summary>
        /// Среднее расстояние от центра множества
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static double MeanDist(Point[] mas)
        {
            Point center = Center(mas);
            double dist = 0;
            for (int i = 0; i < mas.Length; i++)
                dist += Eudistance(mas[i], center);
            return dist / mas.Length;
        }

        /// <summary>
        /// Евклидово расстояние между точками
        /// </summary>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double Eudistance(Point z, Point w) => Math.Sqrt((z.x - w.x) * (z.x - w.x) + (z.y - w.y) * (z.y - w.y));
        /// <summary>
        /// Значение функции базисного потенциала, связанного с этой точкой
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double PotentialF(Point z) => Math.Log(1.0 / Point.Eudistance(this, z));

        /// <summary>
        /// Функция второго базисного потенциала, сцепленного с точкой z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double BPotentialF(Point z)
        {
            double r = Point.Eudistance(this, z);
            return Math.Log(1.0 / r) * r * r;
        }


        /// <summary>
        /// Возвращает координаты нижнего левого и верхнего правого угла прямоугольника, содержащего все точки массива
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Tuple<Point, Point> GetBigRect(Point[] mas)
        {
            Point min = mas[0].dup;
            Point max = min.dup;
            for (int i = 1; i < mas.Length; i++)
            {
                if (min.x > mas[i].x)
                {
                    min.x = mas[i].x;
                }

                if (min.y > mas[i].y)
                {
                    min.y = mas[i].y;
                }

                if (max.x < mas[i].x)
                {
                    max.x = mas[i].x;
                }

                if (max.y < mas[i].y)
                {
                    max.y = mas[i].y;
                }
            }

            return new Tuple<Point, Point>(min, max);
        }

        /// <summary>
        /// Перевести массив чисел в последовательность точек на плоскости
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point[] GetSequence(double[] c)
        {
            Point[] p = new Point[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                p[i] = new Point(i, c[i]);
            }

            return p;
        }
        #endregion

        #region Операторы
        public static Point operator -(Point p) => new Point(-p.x, -p.y);

        public static bool operator !=(Point a, Point b) => !(a == b);

        /// <summary>
        /// Скалярное произведение точек как векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator *(Point a, Point b) => a.x * b.x + a.y * b.y;

        public static Point operator *(double s, Point p) => new Point(s * p.x, s * p.y);
        public static Point operator *(Point p,double s ) => new Point(s * p.x, s * p.y);

        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
        public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);

        public static bool operator <(Point a, Point b)
        {
            if (a.y < b.y)
            {
                return true; //cравнение по второй координате
            }
            else if (a.y > b.y)
            {
                return false;
            }
            else
            {
                return a.x < b.x; //если вторые координаты равны, сравнение по первой координате
            }
        }

        public static bool operator ==(Point a, Point b) => (a.x == b.x) && (a.y == b.y);

        /// <summary>
        /// Сравнение точек по установленной по умолчанию упорядоченности
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Point a, Point b)
        {
            return (b < a);
        }
        #endregion

        #region Методы, связанные с рисованием графиков функций
        /// <summary>
        /// Набор n+1 точек на графике функции f, разбитых равномерно на отрезке от a до b
        /// </summary>
        /// <param name="f"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point[] Points(Func<double,double> f, int n, double a, double b, bool parallel = false)
        {
            double h = (b - a) / n;
            Point[] points = new Point[n + 1];
            if (parallel)
                Parallel.For(0, n + 1, (int i) => points[i] = new Point(a + h * i, f(a + h * i)));
            else
                for (int i = 0; i <= n; i++)
                    points[i] = new Point(a + h * i, f(a + h * i));

            return points;
        }

        /// <summary>
        /// Вывести массив точек, через которые проходит функция
        /// </summary>
        /// <param name="f">Функция, заданная на отрезке</param>
        /// <param name="h">Шаг обхода отрезка</param>
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        /// <returns></returns>
        public static Point[] Points(Func<double,double> f, double h, double a, double b)
        {
            int n = (int)((b - a) / h);
            Point[] points = new Point[n];
            for (int i = 0; i < n; i++)
            {
                points[i] = new Point(a + h * i, f(a + h * i));
            }

            return points;
        }

        /// <summary>
        /// Считать массив точек из файла
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static Point[] Points(StreamReader fs)
        {
            string s = fs.ReadLine();
            string[] st = s.Split(' ');
            int n = Convert.ToInt32(st[0]);
            Point[] p = new Point[n];

            for (int k = 0; k < n; k++)
            {
                s = fs.ReadLine();
                st = s.Split(' ');//в аргументах указывается массив символов, которым разделяются числа
                p[k] = new Point(Convert.ToDouble(st[0]), Convert.ToDouble(st[1]));
            }

            fs.Close();
            return p;
        }

        /// <summary>
        /// Массив точек, через которые проходит функция, по массиву абцисс эти точек
        /// </summary>
        /// <param name="f"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point[] Points(Func<double,double> f, double[] c,bool parallel=false)
        {
            Point[] p = new Point[c.Length];
            if (parallel)
                Parallel.For(0, c.Length, (int i) => p[i] = new Point(c[i], f(c[i])));
            else
            for (int i = 0; i < c.Length; i++)          
                p[i] = new Point(c[i], f(c[i]));
            
            return p;
        }

        /// <summary>
        /// Генерация массива точек по списку точек
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Point[] Points(List<Point> L) => L.ToArray();

        /// <summary>
        /// Создать массив точек со значениями из файла и с аргументами с определённой закономерностью
        /// </summary>
        /// <param name="begin">Начало отрезка по аргументам</param>
        /// <param name="step">Шаг по отрезкку</param>
        /// <param name="count">Число точек</param>
        /// <param name="filename">Имя файла со значениями</param>
        /// <param name="path">Пусть к файлу</param>
        /// <param name="byevery">Брать каждый какой-то элемент</param>
        /// <returns></returns>
        public static Point[] CreatePointArray(double begin, double step, int count, string filename, string path = null,int byevery=1)
        {
            path = path ?? Environment.CurrentDirectory;
            var res = new Point[count];
            using (StreamReader f = new StreamReader(Path.Combine(path, filename)))
                for (int i = 0; i < count; i++)
                    res[i] = new Point(begin + i * step, f.ReadLine().Replace('.',',').ToDouble());
            if(byevery==1)
            return res;

            var pc = new Point[count / byevery];
            for (int i = 0; i < pc.Length; i++)
                pc[i] = res[i * byevery];
            return pc;
        }


        //то же самое, только отдельными массивами выводятся первые и вторые координаты точек (сделано для рисования в Chart)
        public static double[] PointsX(Func<double,double> f, int n, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, n, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, n, a, b)[i]);
            }

            double[] x = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                x[i] = p[i].x;
            }

            return x;
        }

        public static double[] PointsX(Func<double,double> f, double h, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, h, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, h, a, b)[i]);
            }

            double[] x = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                x[i] = p[i].x;
            }

            return x;
        }

        public static double[] PointsY(Func<double,double> f, int n, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, n, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, n, a, b)[i]);
            }

            double[] y = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                y[i] = p[i].y;
            }

            return y;
        }
        public static double[] PointsY(Func<double,double> f, double h, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, h, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, h, a, b)[i]);
            }

            double[] y = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                y[i] = p[i].y;
            }

            return y;
        }
        #endregion

        /// <summary>
        /// Показать массив точек на консоли
        /// </summary>
        /// <param name="f"></param>
        public static void Show(Point[] f)
        {
            //for (int i = 0; i < f.Length; i++) Console.Write("{0} \t", f[i].x); Console.WriteLine();
            //for (int i = 0; i < f.Length; i++) Console.Write("{0} \t", f[i].y); Console.WriteLine();
            for (int i = 0; i < f.Length; i++)
            {
                Console.WriteLine(f[i].ToString());
            }
        }

        public object Clone()
        {
            return (object)new Point(this);
        }

        public int CompareTo(object obj) => CompareTo((Point)obj);
        public int CompareTo(Point p)
        {
            if (this.x < p.x)
            {
                return -1;
            }

            if (this.x == p.x)
            {
                if (this.y < p.y)
                {
                    return -1;
                }

                if (this.y == p.y)
                {
                    return 0;
                }
            }
            return 1;
        }

        public override bool Equals(object obj) => Equals((Point)obj);
        public bool Equals(Point point) => x == point.x && y == point.y;

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Показать координаты точки на консоли
        /// </summary>
        public void Show() { Console.WriteLine(this.ToString()); }

        /// <summary>
        /// Строковое изображение точки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} , {1})", this.x, this.y);
        }

        public void MoveTo(Point t)
        {
            x = t.x;
            y = t.y;
        }       
        public void FastAdd(Point p)
        {
            x += p.x;
            y += p.y;
        }
    }
}