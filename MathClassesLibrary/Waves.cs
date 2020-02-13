using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using static МатКлассы.Number;
using МатКлассы;
using System.Collections.Generic;

namespace МатКлассы
{
    /// <summary>
    /// Класс, обеспечивающий исследование колебаний
    /// </summary>
    public static class Waves
    {
        /// <summary>
        /// Нормаль
        /// </summary>
        public struct Normal2D
        {
            /// <summary>
            /// Вектор нормали
            /// </summary>
            public Point n;
            /// <summary>
            /// Позиция приложения
            /// </summary>
            public Point Position;

            /// <summary>
            /// Создание вектора нормали к точке на окружности
            /// </summary>
            /// <param name="center">Центр окружности</param>
            /// <param name="position">Декартовы координаты точки на окружности</param>
            /// <param name="coefficent">Коэффициент умножения нормали</param>
            public Normal2D(Point center, Point position, double coefficent = 1)
            {
                double dx = position.x - center.x;
                double dy = position.y - center.y;
                double div = Math.Sqrt(dx * dx + dy * dy);
                this.n = new Point(dx / div * coefficent, dy / div * coefficent);
                this.Position = new Point(position);
            }
            /// <summary>
            /// Конструктор для копирования с дополнительным умножением
            /// </summary>
            /// <param name="coef"></param>
            /// <param name="normal"></param>
            /// <param name="position"></param>
            public Normal2D(double coef, Point normal, Point position)
            {
                this.n = new Point(normal.x * coef, normal.y * coef);
                this.Position = new Point(position);
            }

            /// <summary>
            /// Возвращает массив нормалей как массив точек
            /// </summary>
            /// <param name="mas"></param>
            /// <returns></returns>
            public static Point[] NormalsToPoins(Normal2D[] mas)
            {
                Point[] res = new Point[mas.Length];
                for (int i = 0; i < mas.Length; i++)
                    res[i] = new Point(mas[i].n);
                return res;
            }

            /// <summary>
            /// Умножить нормаль на число
            /// </summary>
            /// <param name="s"></param>
            /// <param name="d"></param>
            /// <returns></returns>
            public static Normal2D operator *(Normal2D s, double d) => new Normal2D( d, s.n, s.Position);
            public override string ToString()=> $"({Position.x}; {Position.y}) -> [{n.x}; {n.y}]";
            

            /// <summary>
            /// Угол относительно оси Х и точки, из которой исходит нормаль
            /// </summary>
            public double Corner => new Complex(n.x, n.y).Arg;

            public override bool Equals(object obj)
            {
                Normal2D v = (Normal2D)obj;
                return n.Equals(v.n) && Position.Equals(v.Position);
            }

            public override int GetHashCode()
            {
                var hashCode = 2114770179;
                hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(n);
                hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(Position);
                return hashCode;
            }
        }

        /// <summary>
        /// Окружность
        /// </summary>
        public struct Circle
        {
            /// <summary>
            /// Центр окружности
            /// </summary>
            public Point center;
            /// <summary>
            /// Радиус окружности
            /// </summary>
            public double radius;
            /// <summary>
            /// Создать окружность по центру и радиусу
            /// </summary>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            public Circle(Point center, double radius) { this.center = new Point(center); this.radius = radius; }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="c"></param>
            public Circle(Circle c) : this(c.center, c.radius) { }
            /// <summary>
            /// Окружностб по координатам центра и радиусу
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="radius"></param>
            public Circle(double x,double y,double radius) : this(new Point(x,y),radius) { }


            /// <summary>
            /// Возврат нормали в точке по аргументу
            /// </summary>
            /// <param name="arg"></param>
            /// <param name="len"></param>
            /// <returns></returns>
            public Normal2D GetNormal(double arg, double len = 1)
            {
                Point pos = new Point(center.x + radius * Math.Cos(arg), center.y + radius * Math.Sin(arg));
                return new Normal2D(center, pos, len);
            }

            /// <summary>
            /// Создержит ли круг точку (взято с запасом)
            /// </summary>
            /// <param name="p"></param>
            /// <param name="RadiusCoef">Коэффициент определяющий, сколько радиусов от центра круга считаются собственностью окружности</param>
            /// <returns></returns>
            public bool ContainPoint(Point p,double RadiusCoef=2.0) => Point.Eudistance(p, this.center) < this.radius * RadiusCoef;

            /// <summary>
            /// Возвращает массив точек на окружности
            /// </summary>
            /// <param name="args">Углы точек относительно центра окружности и оси X</param>
            /// <param name="weights">Веса точек (по умолчанию единичные)</param>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnCircle(double[] args, double[] weights = null)
            {
                weights = weights ?? Vectors.Create(1.0, args.Length).DoubleMas;

                Normal2D[] res = new Normal2D[args.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Normal2D(this.center, new Point(center.x + radius * Math.Cos(args[i]), center.y + radius * Math.Sin(args[i])), weights[i]);
                return res;
            }
            /// <summary>
            /// Возвращает массив равномерно рассположенных по окружности нормалей
            /// </summary>
            /// <param name="count"></param>
            /// <param name="weights"></param>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnCircle(int count, double[] weights = null)
            {
                double h = 2 * Math.PI / count;
                weights = weights ?? Vectors.Create(h* radius , count).DoubleMas;
                
                double[] args = new double[count];
                for (int i = 0; i < count; i++)
                    args[i] = i * h;

                return GetNormalsOnCircle(args, weights);
            }


            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значений Re ur, Im ur, Abs ur, Re uz, Im uz, Abs uz в файл (чтобы потом нарисовать графики). Реализация параллельная
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="x0">Начало отрезка по х</param>
            /// <param name="X">Конец отрезка по х</param>
            /// <param name="xcount">Число точек по х</param>
            /// <param name="y0">Начало отрезка по у</param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="ycount">Число точек по у</param>
            public static void FieldToFileParallel(string filename, Func<double, double, (Complex ur , Complex uz)> F, double x0, double X, int xcount, double y0, double Y, int ycount, IProgress<int> progress, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "")
            {
                int[] k = new int[xcount * ycount];
                double[] x = Expendator.Seq(x0, X, xcount);
                double[] y = Expendator.Seq(y0, Y, ycount);
                Complex[,] ur = new Complex[xcount, ycount], uz = new Complex[xcount, ycount];

                //нахождение массивов
                Parallel.For(0, xcount, (int i) =>
                {
                    (Complex ur , Complex uz) tmp;
                    for (int j = 0; j < ycount; j++)
                    {
                        if (token.IsCancellationRequested) return;
                        if (!Filter(new Point(x[i], y[j])))
                        {
                            tmp = F(x[i], y[j]);
                            ur[i, j] = new Complex(tmp.Item1);
                            uz[i, j] = new Complex(tmp.Item2);
                        }
                        else//иначе типа NA
                        {
                            ur[i, j] = new Complex(Double.NaN);
                            uz[i, j] = new Complex(Double.NaN);
                        }
                        k[(i) * ycount + j] = 1;                       
                    }
                   progress.Report(k.Sum());
                });

                //-------------------------------------------------------------------------------------
                //запись в файлы
                StreamWriter fs = new StreamWriter(filename);
                string se = filename.Substring(0, filename.Length - 4);//-.txt
                StreamWriter ts = new StreamWriter(se + "(title).txt");
                StreamWriter xs = new StreamWriter(se + "(x).txt");
                StreamWriter ys = new StreamWriter(se + "(y).txt");

                ts.WriteLine($"{title}");

                xs.WriteLine("x");
                for (int i = 0; i < xcount; i++)
                    xs.WriteLine(x[i]);

                ys.WriteLine("y");
                for (int i = 0; i < ycount; i++)
                    ys.WriteLine(y[i]);

                fs.WriteLine("urRe urIm urAbs uzRe uzIm uzAbs");
                for (int i = 0; i < xcount; i++)
                    for (int j = 0; j < ycount; j++)
                        if (Double.IsNaN(ur[i, j].Abs))
                            fs.WriteLine("NA NA NA NA NA NA");
                        else
                            fs.WriteLine($"{ur[i, j].Re} {ur[i, j].Im} {ur[i, j].Abs} {uz[i, j].Re} {uz[i, j].Im} {uz[i, j].Abs}");

                fs.Close();
                ts.Close();
                xs.Close();
                ys.Close();
            }

            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значенийur, ur, uz в файл (чтобы потом нарисовать графики)
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="x0">Начало отрезка по х</param>
            /// <param name="X">Конец отрезка по х</param>
            /// <param name="xcount">Число точек по х</param>
            /// <param name="y0">Начало отрезка по у</param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="ycount">Число точек по у</param>
            /// <param name="k">Массив для отслеживания прогресса</param>
            public static void FieldToFileOLD(string filename, string path, Func<double, double, (double ur , double uz)> F, double x0, double X, int xcount, double y0, double Y, int ycount, IProgress<int> progress, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "", bool parallel = true)
            {
                int[] k = new int[xcount * ycount];
                double[] x = Expendator.Seq(x0, X, xcount);
                double[] y = Expendator.Seq(y0, Y, ycount);
                double[,] ur = new double[xcount, ycount], uz = new double[xcount, ycount];

                //нахождение массивов
                if (parallel)
                    Parallel.For(0, xcount, (int i) =>
                    {
                        (double ur , double uz) tmp;
                        for (int j = 0; j < ycount; j++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))
                            {
                                 tmp = F(x[i], y[j]);
                                ur[i, j] = tmp.Item1;
                                uz[i, j] = tmp.Item2;
                            }
                            else//иначе типа NA
                            {
                                ur[i, j] = Double.NaN;
                                uz[i, j] = Double.NaN;
                            }
                            k[(i) * ycount + j] = 1;
                        }
                        progress.Report(k.Sum());
                    });
                else
                    for (int i = 0; i < xcount; i++)
                    {
                        (double ur , double uz) tmp;
                        for (int j = 0; j < ycount; j++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))
                            {
                                tmp = F(x[i], y[j]);
                                ur[i, j] = tmp.Item1;
                                uz[i, j] = tmp.Item2;
                            }
                            else//иначе типа NA
                            {
                                ur[i, j] = Double.NaN;
                                uz[i, j] = Double.NaN;
                            }
                            k[(i) * ycount + j] = 1;   
                        }
                        progress.Report(k.Sum());
                    }

                //-------------------------------------------------------------------------------------
                //запись в файлы
                StreamWriter fs = new StreamWriter(Path.Combine(path, filename));
                string se = filename.Substring(0, filename.Length - 4);//-.txt
                StreamWriter ts = new StreamWriter(Path.Combine(path, se + "(title).txt"));
                StreamWriter xs = new StreamWriter(Path.Combine(path, se + "(x).txt"));
                StreamWriter ys = new StreamWriter(Path.Combine(path, se + "(y).txt"));
                StreamWriter info = new StreamWriter(Path.Combine(path, se + "(info).txt"));

                Parallel.Invoke(
                    () =>
                    {
                        ts.WriteLine($"{title}");

                        xs.WriteLine("x");
                        for (int i = 0; i < xcount; i++)
                            xs.WriteLine(x[i]);

                        ys.WriteLine("y");
                        for (int i = 0; i < ycount; i++)
                            ys.WriteLine(y[i]);

                        fs.WriteLine("ur uz");
                        for (int i = 0; i < xcount; i++)
                            for (int j = 0; j < ycount; j++)
                                if (Double.IsNaN(ur[i, j].Abs()))
                                    fs.WriteLine("NA NA");
                                else
                                    fs.WriteLine($"{ur[i, j]} {uz[i, j]}");
                    },
                    () =>
                    {
                        using (StreamWriter fur = new StreamWriter(Path.Combine(path, title + " (ur).txt")))
                        using (StreamWriter fuz = new StreamWriter(Path.Combine(path, title + " (uz).txt")))
                        {
                            for (int j = 0; j < ycount; j++)
                                for (int i = 0; i < xcount; i++)
                                    if (Double.IsNaN(ur[i, j].Abs()))
                                    {
                                        fur.Write("NA ");
                                        fuz.Write("NA ");
                                    }
                                    else
                                    {
                                        fur.Write($"{ur[i, j]} ".Replace(',', '.'));
                                        fuz.Write($"{uz[i, j]} ".Replace(',', '.'));
                                    }
                        }
                    }
                    );


                fs.Close();
                ts.Close();
                xs.Close();
                ys.Close();
                info.Close();
            }

            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значенийur, ur, uz в файл (чтобы потом нарисовать графики). Рабочая версия для последовательного вызова
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="path"></param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="parallel"></param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="urt"></param>
            /// <param name="uzt"></param>
            /// <param name="token"></param>
            /// <param name="Filter"></param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="k">Массив для отслеживания прогресса</param>
            public static void FieldToFile(string filename, string path, Func<double, double, (double ur , double uz)> F, double[] x, double[] y, ref double[,] urt, ref double[,] uzt, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "", bool parallel = true)
            {
                int xcount = x.Length;
                double[,] ur = urt, uz = uzt;

                if (parallel)
                    Parallel.For(0, xcount, (int i) =>
                    {
                        (double ur , double uz) tmp;

                        for (int j = 0; j < xcount; j++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))
                            {
                                tmp = F(x[i], y[j]);
                                ur[i, j] = tmp.Item1;
                                uz[i, j] = tmp.Item2;
                            }
                            else//иначе типа NA
                            {
                                ur[i, j] = Double.NaN;
                                uz[i, j] = Double.NaN;
                            }
                        }
                    });
                else
                    for (int i = 0; i < xcount; i++)
                    {
                        (double ur , double uz) tmp;
                        for (int j = 0; j < xcount; j++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))
                            {
                                tmp = F(x[i], y[j]);
                                ur[i, j] = tmp.Item1;
                                uz[i, j] = tmp.Item2;
                            }
                            else//иначе типа NA
                            {
                                ur[i, j] = Double.NaN;
                                uz[i, j] = Double.NaN;
                            }
                        }
                    }

                //-------------------------------------------------------------------------------------
                //запись в файлы
                StreamWriter fs = new StreamWriter(Path.Combine(path, filename));
                string se = filename.Substring(0, filename.Length - 4);//-.txt
                StreamWriter ts = new StreamWriter(Path.Combine(path, se + "(title).txt"));
                StreamWriter info = new StreamWriter(Path.Combine(path, se + "(info).txt"));
                ts.WriteLine($"{title}");
                fs.WriteLine("ur uz");

                using (StreamWriter fur = new StreamWriter(Path.Combine(path, title + " (ur).txt")))
                using (StreamWriter fuz = new StreamWriter(Path.Combine(path, title + " (uz).txt")))
                    for (int j = 0; j < xcount; j++)
                        for (int i = 0; i < xcount; i++)
                            if (Double.IsNaN(ur[i, j].Abs()) || Double.IsNaN(uz[i, j].Abs()))
                            {
                                fs.WriteLine("NA NA");
                                fur.Write("NA ");
                                fuz.Write("NA ");
                            }
                            else
                            {
                                fs.WriteLine($"{ur[i, j]} {uz[i, j]}");
                                fur.Write($"{ur[i, j]} ".Replace(',', '.'));
                                fuz.Write($"{uz[i, j]} ".Replace(',', '.'));
                            }


                fs.Close();
                ts.Close();
                info.Close();
            }
            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значений ur, uz в файл последовательно (не для рисования графиков). Рабочая версия для параллельного вызова
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="x0">Начало отрезка по х</param>
            /// <param name="X">Конец отрезка по х</param>
            /// <param name="xcount">Число точек по х</param>
            /// <param name="y0">Начало отрезка по у</param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="ycount">Число точек по у</param>
            /// <param name="k">Массив для отслеживания прогресса</param>
            public static void FieldToFile(string path, Func<double, double, (double ur , double uz)> F, double[] x, double[] y, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "")
            {
                int xcount = x.Length;
                int ycount = y.Length;
                (double ur , double uz) tmp;

                using (StreamWriter fur = new StreamWriter(Path.Combine(path, title + " (ur).txt")))
                using (StreamWriter fuz = new StreamWriter(Path.Combine(path, title + " (uz).txt")))               
                    for (int j = 0; j < ycount; j++)
                        for (int i = 0; i < xcount; i++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))//больше или равно, потому что в массивах изначально нули
                            {
                                tmp = F(x[i], y[j]);
                                fur.Write($"{tmp.Item1} ".Replace(',', '.'));
                                fuz.Write($"{tmp.Item2} ".Replace(',', '.'));
                            }
                            else//иначе типа NA
                            {
                                fur.Write("NA ");
                                fuz.Write("NA ");

                            }
                        }                
            }
        }

        /// <summary>
        /// Окружность с вырезом, представимым как круг с центром на большой окружности
        /// </summary>
        public class DCircle:Idup<DCircle>
        {
            public static DCircle Example = new DCircle(new Point(1, 1), 15, 5);

            /// <summary>
            /// Окружности
            /// </summary>
            private Circle circle1, circle2;
            /// <summary>
            /// Аргумент, определяющий положение центра меньшей окружности
            /// </summary>
            private double arg;
            /// <summary>
            /// Центральные половинные углы окружностей в радианах
            /// </summary>
            private double alp1, alp2;

            public int FirstNomnalsCount = 90, SecondNomnalsCount = 30;

            /// <summary>
            /// Радиус большего круга
            /// </summary>
            public double Radius => circle1.radius;
            /// <summary>
            /// Центр полумесяца как центр большего круга
            /// </summary>
            public Point Center => circle1.center;
            /// <summary>
            /// Копия большего круга
            /// </summary>
            public Circle BigCircle => new Circle(circle1);

            /// <summary>
            /// Возвращает пару диаметров и аргумент центра
            /// </summary>
            public Tuple<double, double, double> DiamsAndArg => new Tuple<double, double, double>(circle1.radius * 2, circle2.radius * 2, arg);

            public DCircle dup => new DCircle(this);

            /// <summary>
            /// Окружность с вырезом
            /// </summary>
            /// <param name="center">Центр большей окружности</param>
            /// <param name="diam1">Диамерт большей окружности</param>
            /// <param name="diam2">Диаметр меньшей окружности</param>
            /// <param name="arg">Угол в радианах, определяющий положение центра меньшей окружности</param>
            /// <param name="FirstNomnalsCount"></param>
            /// <param name="SecondNomnalsCount"></param>
            public DCircle(Point center, double diam1 = 1.6, double diam2 = 0.5, double arg = 4.8, int FirstNomnalsCount = 90, int SecondNomnalsCount = 30)
            {
                this.arg = arg;

                double r1 = diam1 / 2.0, r2 = diam2 / 2.0;
                circle1 = new Circle(center, r1);
                circle2 = new Circle(new Point(center.x + r1, center.y), r2);

                double tmp = r2 / r1;
                alp1 = Math.Acos(1.0 - 0.5 * tmp * tmp);
                alp2 = Math.Acos(0.5 * tmp);

                this.FirstNomnalsCount = FirstNomnalsCount;
                this.SecondNomnalsCount = SecondNomnalsCount;
            }

            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="dc"></param>
            public DCircle(DCircle dc)
            {
                this.alp1 = dc.alp1;
                this.arg = dc.arg;
                this.alp2 = dc.alp2;
                this.circle1 = new Circle(dc.circle1);
                this.circle2 = new Circle(dc.circle2);

                this.FirstNomnalsCount = dc.FirstNomnalsCount;
                this.SecondNomnalsCount = dc.SecondNomnalsCount;
            }

            /// <summary>
            /// Содержит ли окружность с вырезом точку в своей внутренности (взято с запасом)
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public bool ContainPoint(Point p,double radius=2.0)=> circle1.ContainPoint(p,radius);

            /// <summary>
            /// Содержит ли окружность с вырезом точку
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public bool ContainPointCurrent(Point p)
            {
                p = p.Turn(circle1.center, -arg);
                return Point.Eudistance(circle1.center, p) <= circle1.radius && Point.Eudistance(circle2.center, p) >= circle2.radius;
            }

            /// <summary>
            /// Возвращает нормаль для точки на плоскости
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public Point GetNormal(Point p, double len = 0.1, double eps = 0.01)
            {
                p = p.Turn(circle1.center, -arg);
                Point nil = Point.Zero;
                double corner = new Complex(p.x - circle1.center.x, p.y - circle1.center.y).Arg;
                if (corner < -alp1 - eps || corner > alp1 + eps)
                    return circle1.GetNormal(corner, len).n.Turn(nil, arg);

                double d1 = Point.Eudistance(circle1.center, p);
                double d2 = Point.Eudistance(circle2.center, p);

                if (d2 == 0) return new Point(Math.Cos(arg) * len, Math.Sin(arg) * len);
                Normal2D res = new Normal2D(circle2.center, p, len);
                if (d1 > circle1.radius + eps)
                    return res.n.Turn(nil, arg);
                return (res * (-1)).n.Turn(nil, arg);
            }

            /// <summary>
            /// Возвращает массив точек и массив нормалей в этих точках
            /// </summary>
            /// <param name="FirstNomnalsCount">Число точек на большей окружности</param>
            /// <param name="SecondNomnalsCount">Число точек на меньшей окружности</param>
            /// <returns></returns>
            public Tuple<Point[], Point[]> GetArraysForDraw(int FirstNomnalsCount = 100, int SecondNomnalsCount = 10, double len = 0.1, double eps = 0.00001)
            {
                double h1 = 2.0*(Math.PI  -  alp1 -  eps) / (FirstNomnalsCount - 1);
                double h2 = 2 * alp2 / (SecondNomnalsCount - 1), tmp;

                Point[] p = new Point[FirstNomnalsCount + SecondNomnalsCount];
                Point[] n = new Point[FirstNomnalsCount + SecondNomnalsCount];

                for (int i = 0; i < FirstNomnalsCount; i++)
                {
                    tmp = alp1 + eps + i * h1;
                    p[i] = new Point(circle1.radius * Math.Cos(tmp) + circle1.center.x, circle1.radius * Math.Sin(tmp) + circle1.center.y).Turn(circle1.center, arg);
                    n[i] = GetNormal(p[i], 0.05 * circle1.radius);
                }
                for (int i = 0; i < SecondNomnalsCount; i++)
                {
                    tmp = -Math.PI + alp2 - i * h2;
                    p[FirstNomnalsCount + i] = new Point(circle2.radius * Math.Cos(tmp) + circle2.center.x, circle2.radius * Math.Sin(tmp) + circle2.center.y).Turn(circle1.center, arg);
                    n[FirstNomnalsCount + i] = GetNormal(p[FirstNomnalsCount + i], 0.2 * circle2.radius);
                }
                return new Tuple<Point[], Point[]>(p, n);
            }

            /// <summary>
            /// Возвращает массив точек и массив нормалей в этих точках
            /// </summary>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnDCircle(double eps = 0.01)
            {
                double m1 = 2 * Math.PI * circle1.radius / FirstNomnalsCount;
                double m2 = 0.85 * 2 * circle2.radius * alp2 / SecondNomnalsCount;

                double h1 = 2*(Math.PI  -  alp1 - eps) / (FirstNomnalsCount - 1);
                double h2 = 2 * alp2 / (SecondNomnalsCount - 1), tmp;

                Point p, n;
                Normal2D[] res = new Normal2D[FirstNomnalsCount + SecondNomnalsCount];

                for (int i = 0; i < FirstNomnalsCount; i++)
                {
                    tmp = alp1 + eps + i * h1;
                    p = new Point(circle1.radius * Math.Cos(tmp) + circle1.center.x, circle1.radius * Math.Sin(tmp) + circle1.center.y).Turn(circle1.center, arg);
                    n = GetNormal(p, 1);
                    res[i] = new Normal2D(m1, n, p);
                }
                for (int i = 0; i < SecondNomnalsCount; i++)
                {
                    tmp = -Math.PI + alp2 - i * h2;
                    p = new Point(circle2.radius * Math.Cos(tmp) + circle2.center.x, circle2.radius * Math.Sin(tmp) + circle2.center.y).Turn(circle1.center, arg);
                    n = GetNormal(p, 1);
                    res[FirstNomnalsCount + i] = new Normal2D(m2, n, p);
                }
                return res;
            }

            public void MoveTo(DCircle t)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Поворот точки на угол относительно центра
        /// </summary>
        /// <param name="p"></param>
        /// <param name="center"></param>
        /// <param name="corner"></param>
        /// <returns></returns>
        public static Point Turn(this Point p, Point center, double corner)
        {
            double sin = Math.Sin(corner);
            double cos = Math.Cos(corner);
            Point t = new Point(p.x - center.x, p.y - center.y);
            return new Point(t.x * cos - t.y * sin + center.x, t.x * sin + t.y * cos + center.y);
        }
    }
}