using System;
using System.Collections.Generic;
using System.IO;

namespace МатКлассы
{
    public static partial class FuncMethods
    {
           /// <summary>
        /// Сеточная функция
        /// </summary>
        public class NetFunc
        {
            /// <summary>
            /// Список узлов
            /// </summary>
            private List<Point> Knots;
            /// <summary>
            /// Интерполяционный многочлен Лагранжа для этой сеточной функции
            /// </summary>
            private Func<double,double> Lag = null;
            /// <summary>
            /// Интерполяционная рациональная функция для этой сеточной функции
            /// </summary>
            private Func<double,double> R = null;
            /// <summary>
            /// Интерполяционный кубический сплайн для этой сеточной функции
            /// </summary>
            private Func<double,double> CubeSpline = null;
            /// <summary>
            /// Значение сеточной функции в конце области определения
            /// </summary>
            /// <returns></returns>
            public double LastVal() => this[this.CountKnots - 1];
            /// <summary>
            /// Последний аргумент сеточной функции
            /// </summary>
            /// <returns></returns>
            public double LastArg() => Arg(this.CountKnots - 1);
            /// <summary>
            /// Массив значений сеточной функции
            /// </summary>
            /// <returns></returns>
            public double[] Values
            {
                get
                {
                    Point[] p = this.Points;
                    double[] res = new double[p.Length];
                    for (int i = 0; i < p.Length; i++)
                        res[i] = p[i].y;
                    return res;
                }

            }

            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            public NetFunc() { this.Knots = new List<Point>(); }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="f"></param>
            public NetFunc(NetFunc f) { this.Knots = new List<Point>(f.Knots); }
            /// <summary>
            /// Генерация сеточной функции по списку точек
            /// </summary>
            /// <param name="L"></param>
            public NetFunc(List<Point> L) { this.Knots = new List<Point>(L); this.Refresh(); }
            /// <summary>
            /// Генерация сеточной функции по массиву точек
            /// </summary>
            /// <param name="P"></param>
            public NetFunc(Point[] P)
            {
                this.Knots = new List<Point>();
                for (int i = 0; i < P.Length; i++) this.Knots.Add(P[i]);
                this.Refresh();
            }
            /// <summary>
            /// Генерация сеточной функции по массиву точек, расположенному в файле
            /// </summary>
            /// <param name="fs"></param>
            public NetFunc(StreamReader fs)
            {
                Point[] P = Point.Points(fs);
                this.Knots = new List<Point>();
                for (int i = 0; i < P.Length; i++) this.Knots.Add(P[i]);
                this.Refresh();
            }
            /// <summary>
            /// Генерация сеточной функции по точкам от действительной функции
            /// </summary>
            /// <param name="f">Действительная функция</param>
            /// <param name="n">Число точек</param>
            /// <param name="a">Начало отрезка интерполяции</param>
            /// <param name="b">Конец отрезка интерполяции</param>
            public NetFunc(Func<double,double> f, int n, double a, double b)
            {
                Point[] P = Point.Points(f, n, a, b);
                this.Knots = new List<Point>();
                for (int i = 0; i < P.Length; i++) this.Knots.Add(P[i]);
            }
            /// <summary>
            /// Генерация сеточной функции по действительной фунции и набору абцисс
            /// </summary>
            /// <param name="f"></param>
            /// <param name="c"></param>
            public NetFunc(Func<double,double> f, double[] c)
            {
                Point[] P = Point.Points(f, c);
                this.Knots = new List<Point>();
                for (int i = 0; i < P.Length; i++) this.Knots.Add(P[i]);
                this.Refresh();
            }
            /// <summary>
            /// Задание сеточной функции по массиву узлов и массиву значений в узлах
            /// </summary>
            /// <param name="arg"></param>
            /// <param name="val"></param>
            public NetFunc(double[] arg, double[] val)
            {
                List<Point> p = new List<Point>();
                for (int i = 0; i < arg.Length; i++)
                    p.Add(new Point(arg[i], val[i]));
                this.Knots = new List<Point>(p); this.Refresh();
            }
            /// <summary>
            /// Усреднённая сеточная функция с условием, что все сеточные функции определены на одной и той же сетке
            /// </summary>
            /// <param name="mas"></param>
            public NetFunc(NetFunc[] mas)
            {
                this.Knots = new List<Point>(mas.Length);
                for (int i = 0; i < this.Knots.Count; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < mas.Length; j++)
                        sum += mas[j].Knots[i].y;
                    this.Knots[i] = new Point(mas[0].Knots[i].x, sum / mas.Length);
                }
            }

            private void Refresh()
            {
                Knots.Sort();
                for (int i = 0; i < this.CountKnots - 1; i++)
                    if (this.Knots[i].x == this.Knots[i + 1].x)
                    {
                        this.Delete(i);
                        i--;
                    }
            }
            /// <summary>
            /// Значение сеточной функции в такой-то точке её сетки
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public double this[int i]
            {
                get
                {
                    return this.Knots[i].y;
                }
            }
            /// <summary>
            /// Аргумент сеточной функции в таком-то узле её сетки
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public double Arg(int i)
            { return this.Knots[i].x; }
            /// <summary>
            /// Массив аргументов сеточной функции
            /// </summary>
            public double[] Arguments
            {
                get
                {
                    double[] c = new double[this.Knots.Count];
                    for (int i = 0; i < c.Length; i++)
                        c[i] = this.Knots[i].x;
                    return c;
                }
            }

            /// <summary>
            /// Количество узлов
            /// </summary>
            public int CountKnots
            {
                get { return Knots.Count; }
            }
            /// <summary>
            /// Минимальный аргумент
            /// </summary>
            public double MinArg
            {
                get { return this.Knots[0].x; }
            }
            /// <summary>
            /// Максимальный аргумент
            /// </summary>
            public double MaxArg
            {
                get { return this.Knots[CountKnots - 1].x; }
            }
            /// <summary>
            /// Интерполяционный полином Лагранжа этой сеточной функции
            /// </summary>
            public Func<double,double> Lagrange
            {
                get
                {
                    if (this.Lag != null) return this.Lag;

                    Point[] P = Point.Points(this.Knots);
                    Polynom Pol = Polynom.Lag(P);
                    this.Lag = Pol.Value;
                    return Pol.Value;
                }
            }
            /// <summary>
            /// Интерполяционный кубический сплайн этой сеточной функции
            /// </summary>
            public Func<double,double> Spline
            {
                get
                {
                    if (this.CubeSpline != null) return this.CubeSpline;

                    Point[] P = Point.Points(this.Knots);
                    Func<double,double> Pol = Polynom.CubeSpline(P);
                    this.CubeSpline = Pol;
                    return Pol;
                }
            }
            /// <summary>
            /// Интерполяционная рациональная функция этой сеточной функции
            /// </summary>
            public Func<double,double> RatFunc(int p, int q)
            {
                if (this.R != null) return this.R;

                Point[] P = Point.Points(this.Knots);
                Func<double,double> Pol = Polynom.R(P, p, q);
                this.R = Pol;
                return Pol;
            }
            /// <summary>
            /// Массив узлов сеточной функции
            /// </summary>
            public Point[] Points
            {
                get
                {
                    Point[] p = new Point[this.CountKnots];
                    for (int i = 0; i < p.Length; i++) p[i] = new Point(this.Knots[i]);
                    return p;
                }
            }

            /// <summary>
            /// Добавить узел в функцию
            /// </summary>
            /// <param name="p"></param>
            public void Add(Point p)
            {
                Knots.Add(p);
                /*this.CountKnots++;*/
                Knots.Sort();
                for (int i = 0; i < this.CountKnots - 1; i++)
                    if (this.Knots[i].x == this.Knots[i + 1].x)
                    {
                        this.Delete(i);
                        i--;
                    }
            }
            /// <summary>
            /// Удалить элемент из списка
            /// </summary>
            /// <param name="k"></param>
            public void Delete(int k) { this.Knots.RemoveAt(k); /*this.CountKnots--;*/ }
            /// <summary>
            /// Удалить элемент с указанной абциссой
            /// </summary>
            /// <param name="x"></param>
            public void Delete(double x)
            {
                if (this.Knots.Exists((Point p) => p.x == x))
                {
                    Point a = this.Knots.Find((Point p) => { return p.x == x; });
                    int k = this.Knots.IndexOf(a);
                    this.Delete(k);
                }
            }

            /// <summary>
            /// Очистить список узлов
            /// </summary>
            public void Clear() { this.Knots.Clear(); this.R = null; this.Lag = null; this.CubeSpline = null; }
            /// <summary>
            /// Значение сеточной функции в точке, наиболее близкой к точке x
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public double Value(double x)
            {
                Point a = new Point(Knots[0]), b = new Point(a);
                if (this.Knots.Exists((Point p) => p.x == x))
                {
                    a = this.Knots.Find((Point p) => { return p.x == x; });
                    return a.y;
                }

                double k = Math.Abs(x - Knots[0].x);
                while (a != null)
                {
                    a = this.Knots.Find((Point p) => { return Math.Abs(p.x - x) < k; });
                    if (a != null) b = new Point(a);
                    k = Math.Abs(a.x - x);
                }
                return b.y;
            }

            /// <summary>
            /// Вывести массив узлов на консоль
            /// </summary>
            public void Show()
            {
                Point[] P = new Point[this.CountKnots];
                for (int i = 0; i < this.CountKnots; i++) P[i] = new Point(Knots[i]);
                Point.Show(P);
            }

            /// <summary>
            /// Скалярное произведение сеточных функций
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double ScalarP(NetFunc a, NetFunc b)
            {
                if (a.Knots.Count != b.Knots.Count) throw new Exception("Сеточные функции имеют разную размерность!");
                double sum = 0;
                for (int i = 0; i < a.Knots.Count; i++) sum += a[i] * b[i];
                sum /= a.Knots.Count;
                return sum;
            }
            /// <summary>
            /// Скалярное произведение сеточной и действительной функции
            /// </summary>
            /// <param name="a"></param>
            /// <param name="f"></param>
            /// <returns></returns>
            public static double ScalarP(NetFunc a, Func<double,double> f)
            {
                double[] c = a.Arguments;
                NetFunc b = new NetFunc(f, c);
                return ScalarP(a, b);
            }
            /// <summary>
            /// Скалярное произведение сеточной и действительной функции
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <returns></returns>
            public static double ScalarP(Func<double,double> f, NetFunc a) { return NetFunc.ScalarP(a, f); }
            /// <summary>
            /// Скалярное произведение двух действителььных функций на сетке
            /// </summary>
            /// <param name="f"></param>
            /// <param name="g"></param>
            /// <param name="c"></param>
            /// <returns></returns>
            public static double ScalarP(Func<double,double> f, Func<double,double> g, double[] c)
            {
                NetFunc a = new NetFunc(f, c);
                NetFunc b = new NetFunc(g, c);
                return ScalarP(a, b);
            }
            /// <summary>
            /// Норма сеточной функции
            /// </summary>
            /// <param name="a"></param>
            /// <returns></returns>
            public double Norm(NetFunc a) { return Math.Sqrt(NetFunc.ScalarP(a, a)); }
            /// <summary>
            /// Расстояние между сеточными функциями
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double Distance(NetFunc a, NetFunc b)
            {
                if (a.Knots.Count != b.Knots.Count) throw new Exception("Сеточные функции имеют разную размерность!");
                double sum = 0;
                for (int i = 0; i < a.Knots.Count; i++) sum += (a[i] - b[i]) * (a[i] - b[i]);
                sum /= a.Knots.Count;
                return Math.Sqrt(sum);
            }
            /// <summary>
            /// Расстояние между сеточной функцией и действительной функцией
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double Distance(NetFunc a, Func<double,double> b)
            {
                NetFunc c = new NetFunc(b, a.Arguments);
                return Distance(a, c);
            }
        }
    }
}

