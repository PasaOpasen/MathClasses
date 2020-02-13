using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Реализация алгоритма роя частиц и подобных
    /// </summary>
    public static class BeeHiveAlgorithm
    {
        #region Примеры тестовых функций
        /// <summary>
        /// Многомерная парабола
        /// </summary>
        public static readonly Func<Vectors, double> Parabol = (Vectors v) => v.DistNorm;
        /// <summary>
        /// Функция Растригина
        /// </summary>
        public static readonly Func<Vectors, double> Rastr = (Vectors v) =>
        {
            double s = 10 * v.Deg;
            for (int i = 0; i < v.Deg; i++)
                s += v[i] * v[i] - 10 * Math.Cos(2 * Math.PI * v[i]);

            return s;
        };
        /// <summary>
        /// Функция Швеля
        /// </summary>
        public static readonly Func<Vectors, double> Shvel = (Vectors v) =>
        {
            double s = 0;
            for (int i = 0; i < v.Deg; i++)
                s += -v[i] * Math.Sin(Math.Sqrt(v[i].Abs()));

            return s;
        };
        #endregion

        #region Метод роя частиц

        /// <summary>
        /// Параметры шага для роя
        /// </summary>
        //tex:Каждая частица в рое делает примерно следующий шаг: $v_{i+1} = w v_i + \varphi_p \cdot random_1 (p_i - x_i) + \varphi_g \cdot random_2 (g_i - x_i)$
        public static double w = 0.3, fp = 2, fg = 5;

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода (подряд)</param>
        /// <param name="center">Центр распредления точек</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Func<Vectors, double> f, int n = 1, double min = -1e12, double max = 1e12, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, Vectors center = null, int maxiter = 150)
        {
            Vectors minimum = new Vectors(n, min);
            Vectors maximum = new Vectors(n, max);

            Hive hive;
            if (center == null) hive = new Hive(minimum, maximum, f, countpoints);
            else hive = new Hive(minimum + center, maximum + center, f, countpoints, center);

            return Gets(hive, eps, maxcountstep, maxiter);

        }

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода (подряд)</param>
        /// <param name="center">Центр распредления точек</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Vectors[] vals,Func<Vectors, double> f, int n = 1, double min = -1e12, double max = 1e12, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            Vectors minimum = new Vectors(n, min);
            Vectors maximum = new Vectors(n, max);

            Hive hive= new Hive(minimum, maximum, f, countpoints-vals.Length,vals);
            return Gets(hive, eps, maxcountstep, maxiter);

        }

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="minimum">Вектор минимальных значений</param>
        /// <param name="maximum">Вектор максимальных значений</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Func<Vectors, double> f, Vectors minimum, Vectors maximum, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            return Gets(new Hive(minimum, maximum, f, countpoints), eps, maxcountstep, maxiter);
        }

        /// <summary>
        /// Найти минимум функции уже по готовому рою
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="eps"></param>
        /// <param name="maxcountstep"></param>
        /// <param name="maxiter"></param>
        /// <returns></returns>
        private static Tuple<Vectors, double> Gets(Hive hive, double eps = 1e-10, int maxcountstep = 100, int maxiter = 150)
        {
            if (maxiter <= 0) maxiter = Int32.MaxValue;
            double e = hive.val;
            int c = maxcountstep, k = 0;

            Console.WriteLine($"Погрешность после инициализации пчёл:  {e}");
            while (e > eps && maxcountstep > 0 && hive.Radius > eps)
            {
                hive.MakeStep(w, fp, fg);
                k++;
                if (hive.val < e)
                {
                    Console.WriteLine($"Hive method (iter {k}):  {e} ---> {hive.val}");
                    e = hive.val;
                    maxcountstep = c;
                    hive.g.Save($"val = {e}.txt");
                }
                else
                    maxcountstep--;
                //Debug.WriteLine( $"c = {maxcountstep}  val = {hive.val}");
                if (k == maxiter) break;
            }
            return new Tuple<Vectors, double>(hive.g, hive.val);
        }

        /// <summary>
        /// Рой пчёл
        /// </summary>
        public sealed class Hive
        {
            /// <summary>
            /// Массив пчёл
            /// </summary>
           readonly Bee[] bees;
            /// <summary>
            /// Наилучшее положение в рое
            /// </summary>
            public Vectors g { get; private set; }
            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double val { get; private set; }

            /// <summary>
            /// Радиус роя как наибольшее расстояние между наилучшим положением в рое и наилучшими положениями отдельных частиц
            /// </summary>
            public double Radius
            {
                get
                {
                    double d = 0, di;
                    for (int i = 0; i < bees.Length; i++)
                    {
                        di = (g - bees[i].p).EuqlidNorm;
                        if (di > d)
                            d = di;
                    }
                    return d;
                }
            }
            private readonly Func<Vectors, double> func;

            /// <summary>
            /// Попытаться обновить наилучшее положение
            /// </summary>
            /// <param name="gnew"></param>
            public void UpdateG(Vectors gnew)
            {
                double v = func(gnew);
                if (v < val)
                {
                    val = v;
                    g.MoveTo(gnew);
                }
            }

            /// <summary>
            /// Сгенерировать рой частиц
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            /// <param name="count"></param>
            /// <param name="v"></param>
            public Hive(Vectors min, Vectors max, Func<Vectors, double> f, int count = 1000, params Vectors[] v)
            {
                this.func = new Func<Vectors, double>(f);

                bees = new Bee[count + v.Length];
                //for (int i = 0; i < count; i++)
                Parallel.For(0, count, (int i) =>
                {
                    bees[i] = new Bee(min, max, f);
                });

                for (int i = count; i < count + v.Length; i++)
                    bees[i] = new Bee(v[i - count], min, max, f);

                g = bees[0].p.dup;
                val = bees[0].bestval;
                ReCount();
            }

            private void ReCount()
            {
                for (int i = 0; i < bees.Length; i++)
                    if (bees[i].bestval < val)
                    {
                        val = bees[i].bestval;
                        g = bees[i].p.dup;
                    }
            }

            /// <summary>
            /// Сделать шаг по дискретному времени
            /// </summary>
            /// <param name="w"></param>
            /// <param name="fp"></param>
            /// <param name="fg"></param>
            /// <param name="parallel"></param>
            public void MakeStep(double w = 0.3, double fp = 2, double fg = 5, bool parallel = true)
            {
                void Iter(int i)
                {
                    bees[i].RecalcV(w, fp, fg, this.g);
                    bees[i].Move();
                    bees[i].ReCount();
                }

                if (parallel)
                    Parallel.For(0, bees.Length, (int i) => Iter(i));
                else
                    for (int i = 0; i < bees.Length; i++)
                        Iter(i);

                ReCount();
            }
        }

        /// <summary>
        /// Классы пчелы
        /// </summary>
        private sealed class Bee
        {
            /// <summary>
            /// Текущее положение частицы
            /// </summary>
           readonly Vectors x;
            /// <summary>
            /// Наилучшее положение частицы
            /// </summary>
            public Vectors p { get; private set; }
            /// <summary>
            /// Текущая скорость частицы
            /// </summary>
            Vectors v;
            /// <summary>
            /// Генератор случайных чисел
            /// </summary>
           readonly MathNet.Numerics.Random.CryptoRandomSource random = new MathNet.Numerics.Random.CryptoRandomSource();


            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double bestval { get; private set; }
            /// <summary>
            /// Целевая функция
            /// </summary>
            Func<Vectors, double> f;

            /// <summary>
            /// Создать частицу в окне решений
            /// </summary>
            /// <param name="min">Минимальные возможные значения положения</param>
            /// <param name="max">Максимальные возможные значения положения</param>
            /// <param name="f">Целевая функция</param>
            public Bee(Vectors min, Vectors max, Func<Vectors, double> f)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();

                x = new Vectors(min);
                for (int i = 0; i < x.Deg; i++)
                {
                    x[i] += r.NextDouble() * (max[i] - min[i]);
                }
                // v = null;f = null;bestval = double.MaxValue;p = null;  

                WhenX(min, max, f);
            }

            /// <summary>
            /// Задать пчелу по известному начальному положению
            /// </summary>
            /// <param name="x"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public Bee(Vectors x, Vectors min, Vectors max, Func<Vectors, double> f)
            {
                this.x = x.dup;
                WhenX(min, max, f);
            }
            /// <summary>
            /// Задать наилучшее положение и случайные скорости, когда x уже известно
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public void WhenX(Vectors min, Vectors max, Func<Vectors, double> f)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();
                p = new Vectors(x);
                this.f = new Func<Vectors, double>(f);
                bestval = f(p);

                Vectors vmax = (max - min), vmin = -vmax;
                v = new Vectors(vmin);
                for (int i = 0; i < x.Deg; i++)
                    v[i] += r.NextDouble() * (vmax[i] - vmin[i]);
            }

            /// <summary>
            /// Переопределить скорость
            /// </summary>
            /// <param name="w">Коэффициент инерции</param>
            /// <param name="fp">Весовой коэффициент для p</param>
            /// <param name="fg">Весовой коэффициент для g</param>
            /// <param name="g">Наилучшее положение по рою</param>
            public void RecalcVOld(double w, double fp, double fg, Vectors g)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();
                double fi = fg + fp;

                Vectors rp = new Vectors(v.Deg), rg = new Vectors(v.Deg);
                for (int i = 0; i < v.Deg; i++)
                {
                    rp[i] = r.NextDouble();
                    rg[i] = r.NextDouble();
                }

                v = 2 * w / Math.Abs(2 - fi - Math.Sqrt(fi * (fi - 4))) * (v + fp * Vectors.CompMult(rp, p - x) + fg * Vectors.CompMult(rg, g - x));
            }
            /// <summary>
            /// Переопределить скорость
            /// </summary>
            /// <param name="w">Коэффициент инерции</param>
            /// <param name="fp">Весовой коэффициент для p</param>
            /// <param name="fg">Весовой коэффициент для g</param>
            /// <param name="g">Наилучшее положение по рою</param>
            public void RecalcV(double w, double fp, double fg, Vectors g)
            {
                double fi = fg + fp;
                double coef = 2 * w / Math.Abs(2 - fi - Math.Sqrt(fi * (fi - 4)));

                for (int i = 0; i < v.Deg; i++)
                    v[i] = coef * (v[i] + fp * random.NextDouble() * (p[i] - x[i]) + fg * random.NextDouble() * (g[i] - x[i]));
            }

            /// <summary>
            /// Сделать шаг по скорости
            /// </summary>
            public void Move() => x.FastAdd(v);

            /// <summary>
            /// Переопределить наилучшее положение частицы, если можно
            /// </summary>
            public void ReCount()
            {
                double t = f(x);
                if (t < bestval)
                {
                    Console.WriteLine($"{bestval} --> {t}");
                    bestval = t;
                    p.MoveTo(x);
                }
            }
        }
        #endregion

        #region Метод роя частиц 2D

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода</param>
        /// <param name="center">Центр распредления точек</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Point, double> GetGlobalMin(Func<Point, double> f, Point center, double min = -1e12, double max = 1e12, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            Point minimum = new Point(min);
            Point maximum = new Point(max);

            Hive2D hive;
            if (center == Point.Zero) hive = new Hive2D(minimum, maximum, f, countpoints);
            else hive = new Hive2D(minimum + center, maximum + center, f, countpoints, center);

            return Gets2D(hive, eps, maxcountstep, maxiter);

        }
        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="minimum">Вектор минимальных значений</param>
        /// <param name="maximum">Вектор максимальных значений</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Point, double> GetGlobalMin(Func<Point, double> f, Point minimum, Point maximum, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            return Gets2D(new Hive2D(minimum, maximum, f, countpoints), eps, maxcountstep, maxiter);
        }

        /// <summary>
        /// Найти минимум функции уже по готовому рою
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="eps"></param>
        /// <param name="maxcountstep"></param>
        /// <param name="maxiter"></param>
        /// <returns></returns>
        private static Tuple<Point, double> Gets2D(Hive2D hive, double eps = 1e-10, int maxcountstep = 100, int maxiter = 150)
        {
            if (maxiter <= 0) maxiter = Int32.MaxValue;
            double e = hive.val;
            int c = maxcountstep, k = 0;

            Debug.WriteLine($"Погрешность после инициализации пчёл:  {e}");
            while (e > eps && maxcountstep > 0 && hive.Radius > eps)
            {
                hive.MakeStep(w, fp, fg);
                k++;
                if (hive.val < e)
                {
                    Debug.WriteLine($"Hive method (iter {k}):  {e} ---> {hive.val}");
                    e = hive.val;
                    maxcountstep = c;
                }
                else
                    maxcountstep--;
                //Debug.WriteLine( $"c = {maxcountstep}  val = {hive.val}");
                if (k == maxiter) break;
            }
            return new Tuple<Point, double>(hive.g, hive.val);
        }

        /// <summary>
        /// Рой пчёл
        /// </summary>
        private sealed class Hive2D
        {
            /// <summary>
            /// Массив пчёл
            /// </summary>
            readonly Bee2D[] bees;
            /// <summary>
            /// Наилучшее положение в рое
            /// </summary>
            public Point g { get; private set; }
            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double val { get; private set; }

            /// <summary>
            /// Радиус роя как наибольшее расстояние между наилучшим положением в рое и наилучшими положениями отдельных частиц
            /// </summary>
            public double Radius
            {
                get
                {
                    double d = 0, di;
                    for (int i = 0; i < bees.Length; i++)
                    {
                        di = Point.Eudistance(g, bees[i].p);
                        if (di > d)
                            d = di;
                    }
                    return d;
                }
            }
            private readonly Func<Point, double> func;

            /// <summary>
            /// Попытаться обновить наилучшее положение
            /// </summary>
            /// <param name="gnew"></param>
            public void UpdateG(Point gnew)
            {
                double v = func(gnew);
                if (v < val)
                {
                    val = v;
                    g = gnew;
                }
            }

            /// <summary>
            /// Сгенерировать рой частиц
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            /// <param name="count"></param>
            /// <param name="v"></param>
            public Hive2D(Point min, Point max, Func<Point, double> f, int count = 1000, params Point[] v)
            {
                this.func = new Func<Point, double>(f);

                bees = new Bee2D[count + v.Length];
                //for (int i = 0; i < count; i++)
                Parallel.For(0, count, (int i) =>
                {
                    bees[i] = new Bee2D(min, max, f);
                });

                for (int i = count; i < count + v.Length; i++)
                    bees[i] = new Bee2D(v[i - count], min, max, f);

                g = bees[0].p.dup;
                val = bees[0].bestval;
                ReCount();
            }

            private void ReCount()
            {
                for (int i = 0; i < bees.Length; i++)
                    if (bees[i].bestval < val)
                    {
                        val = bees[i].bestval;
                        g = bees[i].p;
                    }
            }

            /// <summary>
            /// Сделать шаг по дискретному времени
            /// </summary>
            /// <param name="w"></param>
            /// <param name="fp"></param>
            /// <param name="fg"></param>
            /// <param name="parallel"></param>
            public void MakeStep(double w = 0.3, double fp = 2, double fg = 5, bool parallel = true)
            {
                void Iter(int i)
                {
                    bees[i].RecalcV(w, fp, fg, this.g);
                    bees[i].Move();
                    bees[i].ReCount();
                }

                if (parallel)
                    Parallel.For(0, bees.Length, (int i) => Iter(i));
                else
                    for (int i = 0; i < bees.Length; i++)
                        Iter(i);

                ReCount();
            }
        }

        /// <summary>
        /// Классы пчелы
        /// </summary>
        private struct Bee2D
        {
            /// <summary>
            /// Текущее положение частицы
            /// </summary>
            Point x;
            /// <summary>
            /// Наилучшее положение частицы
            /// </summary>
            public Point p { get; private set; }
            /// <summary>
            /// Текущая скорость частицы
            /// </summary>
            Point v;
            /// <summary>
            /// Генератор случайных чисел
            /// </summary>
            readonly MathNet.Numerics.Random.CryptoRandomSource random;


            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double bestval { get; private set; }
            /// <summary>
            /// Целевая функция
            /// </summary>
            readonly Func<Point, double> f;

            /// <summary>
            /// Создать частицу в окне решений
            /// </summary>
            /// <param name="min">Минимальные возможные значения положения</param>
            /// <param name="max">Максимальные возможные значения положения</param>
            /// <param name="f">Целевая функция</param>
            public Bee2D(Point min, Point max, Func<Point, double> f) : this(GetRandom(min, max), min, max, f) { }

            private static Point GetRandom(Point min,Point max)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();
                Point p = new Point(min);
                p.x += r.NextDouble() * (max.x - min.x);
                p.y += r.NextDouble() * (max.y - min.y);
                return p;
            }

            /// <summary>
            /// Задать пчелу по известному начальному положению
            /// </summary>
            /// <param name="x"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public Bee2D(Point x, Point min, Point max, Func<Point, double> f)
            {
                random = new MathNet.Numerics.Random.CryptoRandomSource();
                this.x = x;

                p = x;
                this.f = new Func<Point, double>(f);
                bestval = f(p);

                Point vmax = (max - min), vmin = -vmax;
                v = vmin;
                v.x = random.NextDouble() * (vmax.x - vmin.x);
                v.y = random.NextDouble() * (vmax.y - vmin.y);
            }

            /// <summary>
            /// Переопределить скорость
            /// </summary>
            /// <param name="w">Коэффициент инерции</param>
            /// <param name="fp">Весовой коэффициент для p</param>
            /// <param name="fg">Весовой коэффициент для g</param>
            /// <param name="g">Наилучшее положение по рою</param>
            public void RecalcV(double w, double fp, double fg, Point g)
            {
                double fi = fg + fp;
                double coef = 2 * w / Math.Abs(2 - fi - Math.Sqrt(fi * (fi - 4)));

                v.x = coef * (v.x + fp * random.NextDouble() * (p.x - x.x) + fg * random.NextDouble() * (g.x - x.x));
                v.y = coef * (v.y + fp * random.NextDouble() * (p.y - x.y) + fg * random.NextDouble() * (g.y - x.y));
            }

            /// <summary>
            /// Сделать шаг по скорости
            /// </summary>
            public void Move() => x.FastAdd(v);

            /// <summary>
            /// Переопределить наилучшее положение частицы, если можно
            /// </summary>
            public void ReCount()
            {
                double t = f(x);
                if (t < bestval)
                {
                    bestval = t;
                    p = x;
                }
            }
        }
        #endregion

        #region Метод роя частиц 1D

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода</param>
        /// <param name="center">Центр распредления точек</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static (double ur , double uz) GetGlobalMin(Func<double, double> f, double center = 0, double min = -1e12, double max = 1e12, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            double minimum = min;
            double maximum = max;

            Hive1D hive;
            if (center == 0) hive = new Hive1D(minimum, maximum, f, countpoints);
            else hive = new Hive1D(minimum + center, maximum + center, f, countpoints, center);

            return Gets1D(hive, eps, maxcountstep, maxiter);

        }
        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="minimum">Вектор минимальных значений</param>
        /// <param name="maximum">Вектор максимальных значений</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число неудачных итераций метода</param>
        /// <param name="maxiter">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static (double ur , double uz) GetGlobalMin(Func<double, double> f, double minimum, double maximum, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            return Gets1D(new Hive1D(minimum, maximum, f, countpoints), eps, maxcountstep, maxiter);
        }

        /// <summary>
        /// Найти минимум функции уже по готовому рою
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="eps"></param>
        /// <param name="maxcountstep"></param>
        /// <param name="maxiter"></param>
        /// <returns></returns>
        private static (double ur , double uz) Gets1D(Hive1D hive, double eps = 1e-10, int maxcountstep = 100, int maxiter = 150)
        {
            if (maxiter <= 0) maxiter = Int32.MaxValue;
            double e = hive.val;
            int c = maxcountstep, k = 0;

            Debug.WriteLine($"Погрешность после инициализации пчёл:  {e}");
            while (e > eps && maxcountstep > 0 && hive.Radius > eps)
            {
                hive.MakeStep(w, fp, fg);
                k++;
                if (hive.val < e)
                {
                    Debug.WriteLine($"Hive method (iter {k}):  {e} ---> {hive.val}");
                    e = hive.val;
                    maxcountstep = c;
                }
                else
                    maxcountstep--;
                //Debug.WriteLine( $"c = {maxcountstep}  val = {hive.val}");
                if (k == maxiter) break;
            }
            return (hive.g, hive.val);
        }

        /// <summary>
        /// Рой пчёл
        /// </summary>
        private sealed class Hive1D
        {
            /// <summary>
            /// Массив пчёл
            /// </summary>
            readonly Bee1D[] bees;
            /// <summary>
            /// Наилучшее положение в рое
            /// </summary>
            public double g { get; private set; }
            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double val { get; private set; }

            /// <summary>
            /// Радиус роя как наибольшее расстояние между наилучшим положением в рое и наилучшими положениями отдельных частиц
            /// </summary>
            public double Radius
            {
                get
                {
                    double d = 0, di;
                    for (int i = 0; i < bees.Length; i++)
                    {
                        di = Math.Abs(g - bees[i].p);
                        if (di > d)
                            d = di;
                    }
                    return d;
                }
            }
            private readonly Func<double, double> func;

            /// <summary>
            /// Попытаться обновить наилучшее положение
            /// </summary>
            /// <param name="gnew"></param>
            public void UpdateG(double gnew)
            {
                double v = func(gnew);
                if (v < val)
                {
                    val = v;
                    g = gnew;
                }
            }

            /// <summary>
            /// Сгенерировать рой частиц
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            /// <param name="count"></param>
            /// <param name="v"></param>
            public Hive1D(double min, double max, Func<double, double> f, int count = 1000, params double[] v)
            {
                this.func = new Func<double, double>(f);

                bees = new Bee1D[count + v.Length];
                //for (int i = 0; i < count; i++)
                Parallel.For(0, count, (int i) =>
                {
                    bees[i] = new Bee1D(min, max, f);
                });

                for (int i = count; i < count + v.Length; i++)
                    bees[i] = new Bee1D(v[i - count], min, max, f);

                g = bees[0].p;
                val = bees[0].bestval;
                ReCount();
            }

            private void ReCount()
            {
                for (int i = 0; i < bees.Length; i++)
                    if (bees[i].bestval < val)
                    {
                        val = bees[i].bestval;
                        g = bees[i].p;
                    }
            }

            /// <summary>
            /// Сделать шаг по дискретному времени
            /// </summary>
            /// <param name="w"></param>
            /// <param name="fp"></param>
            /// <param name="fg"></param>
            /// <param name="parallel"></param>
            public void MakeStep(double w = 0.3, double fp = 2, double fg = 5, bool parallel = true)
            {
                void Iter(int i)
                {
                    bees[i].RecalcV(w, fp, fg, this.g);
                    bees[i].Move();
                    bees[i].ReCount();
                }

                if (parallel)
                    Parallel.For(0, bees.Length, (int i) => Iter(i));
                else
                    for (int i = 0; i < bees.Length; i++)
                        Iter(i);

                ReCount();
            }
        }

        /// <summary>
        /// Классы пчелы
        /// </summary>
        private struct Bee1D
        {
            /// <summary>
            /// Текущее положение частицы
            /// </summary>
            double x;
            /// <summary>
            /// Наилучшее положение частицы
            /// </summary>
            public double p { get; private set; }
            /// <summary>
            /// Текущая скорость частицы
            /// </summary>
            double v;
            /// <summary>
            /// Генератор случайных чисел
            /// </summary>
            readonly MathNet.Numerics.Random.CryptoRandomSource random;


            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double bestval { get; private set; }
            /// <summary>
            /// Целевая функция
            /// </summary>
            readonly Func<double, double> f;

            /// <summary>
            /// Создать частицу в окне решений
            /// </summary>
            /// <param name="min">Минимальные возможные значения положения</param>
            /// <param name="max">Максимальные возможные значения положения</param>
            /// <param name="f">Целевая функция</param>
            public Bee1D(double min, double max, Func<double, double> f) : this(min + new MathNet.Numerics.Random.CryptoRandomSource().NextDouble() * (max - min), min, max, f) { }

            /// <summary>
            /// Задать пчелу по известному начальному положению
            /// </summary>
            /// <param name="x"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public Bee1D(double x, double min, double max, Func<double, double> f)
            {
                random = new MathNet.Numerics.Random.CryptoRandomSource();
                this.x = x;

                p = x;
                this.f = new Func<double, double>(f);
                bestval = f(p);

                double vmax = (max - min), vmin = -vmax;
                v = vmin;
                v = random.NextDouble() * (vmax - vmin);
            }

            /// <summary>
            /// Переопределить скорость
            /// </summary>
            /// <param name="w">Коэффициент инерции</param>
            /// <param name="fp">Весовой коэффициент для p</param>
            /// <param name="fg">Весовой коэффициент для g</param>
            /// <param name="g">Наилучшее положение по рою</param>
            public void RecalcV(double w, double fp, double fg, double g)
            {
                double fi = fg + fp;
                double coef = 2 * w / Math.Abs(2 - fi - Math.Sqrt(fi * (fi - 4)));

                v = coef * (v + fp * random.NextDouble() * (p - x) + fg * random.NextDouble() * (g - x));
            }

            /// <summary>
            /// Сделать шаг по скорости
            /// </summary>
            public void Move() => x += v;

            /// <summary>
            /// Переопределить наилучшее положение частицы, если можно
            /// </summary>
            public void ReCount()
            {
                double t = f(x);
                if (t < bestval)
                {
                    bestval = t;
                    p = x;
                }
            }
        }
        #endregion

        #region Метод пчелиной колонии

        /// <summary>
        /// Оптимизация методом пчелиной колонии
        /// </summary>
        /// <param name="f">Оптимизируемая функция</param>
        /// <param name="min">Нижняя граница области решений</param>
        /// <param name="max">Верхняя граница области решений</param>
        /// <param name="n">Размерность области решений</param>
        /// <param name="s">Общее число пчёл</param>
        /// <param name="p">Число пчёл, выбранных для последующего исследования (p меньше s)</param>
        /// <param name="e">Число особо исследуемых пчёл (e меньше p)</param>
        /// <param name="sp">Число вспомогательных пчёл для пчёл из p</param>
        /// <param name="se">Число вспомогательных пчёл для пчёл из e</param>
        /// <param name="delta">Радиус окрестности</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="maxcount">Максимальное число итераций</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Func<Vectors, double> f, Vectors min, Vectors max, int n = 1, int s = 1000, int p = 300, int e = 100, int sp = 50, int se = 100, double delta = 1.0, double eps = 1e-10, int maxcount = 10)
        {
            SBee[] mas = SBee.Create(f, min, max, s);
            int k = 0;

            while (SBee.GetBest(mas).v > eps && maxcount > 0 && k < 3)
            {
                double old = SBee.GetBest(mas).v;
                SBee.MakeStep(ref mas, f, min, max, n, p, e, sp, se, delta);
                double now = SBee.GetBest(mas).v;
                if (now < old)
                {
                    Debug.WriteLine($"\tColony: {old} ---> {now}");
                    k = 0;
                }
                else k++;


                maxcount--;
            }

            SBee res = SBee.GetBest(mas);
            return new Tuple<Vectors, double>(res.x, res.v);
        }

        /// <summary>
        /// Класс упрощённой пчелы
        /// </summary>
        private sealed class SBee : IComparable
        {
            public Vectors x { get; private set; }
            public double v { get; private set; }

            public SBee(Vectors vec, double f)
            {
                v = f;
                x = vec.dup;
            }

            public SBee(Vectors vec, Func<Vectors, double> f) : this(vec, f(vec)) { }

            public SBee(SBee be) : this(be.x, be.v) { }

            public int CompareTo(object obj) => this.CompareTo((SBee)obj);
            public int CompareTo(SBee obj)
            {
                return v.CompareTo(obj.v);
            }

            /// <summary>
            /// Наилучшая пчела в массиве (у которой наименьшее значение)
            /// </summary>
            /// <param name="mas"></param>
            /// <returns></returns>
            public static SBee GetBest(SBee[] mas)
            {
                int i = 0;
                double d = mas[0].v;
                for (int k = 1; k < mas.Length; k++)
                    if (mas[k].v < d)
                    {
                        d = mas[k].v;
                        i = k;
                    }
                return mas[i];
            }

            /// <summary>
            /// Получить случайный массив упрощённых пчёл
            /// </summary>
            /// <param name="f"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="count">Число пчёл</param>
            /// <returns></returns>
            public static SBee[] Create(Func<Vectors, double> f, Vectors min, Vectors max, int count = 100, bool withaverage = true)
            {

                SBee[] res = new SBee[count];
                Vectors[] tmp = new Vectors[count];

                Parallel.For(0, count, (int i) =>
                {
                    tmp[i] = Vectors.Create(min, max);
                });

                for (int i = 0; i < count; i++)
                {
                    res[i] = new SBee(tmp[i], f(tmp[i]));
                }

                if (withaverage)
                    res[res.Length - 1] = new SBee((max + min) / 2, f);

                return res;
            }

            /// <summary>
            /// Сделать шаг по алгоритму пчелиной колонии
            /// </summary>
            /// <param name="mas"></param>
            /// <param name="f"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="n"></param>
            /// <param name="p"></param>
            /// <param name="e"></param>
            /// <param name="sp"></param>
            /// <param name="se"></param>
            /// <param name="delta"></param>
            public static void MakeStep(ref SBee[] mas, Func<Vectors, double> f, Vectors min, Vectors max, int n = 1, int p = 300, int e = 100, int sp = 50, int se = 100, double delta = 1.0)
            {
                Array.Sort(mas);

                Vectors[] t= new Vectors[se];

                for (int i = 0; i < e; i++)
                {
                    SBee it = new SBee(mas[i]), tmp;
                    Vectors cent = it.x.dup;


                    Parallel.For(0, t.Length, (int u) =>
                    {
                        t[u] = Vectors.Create(cent, delta);
                    });

                    for (int j = 0; j < se; j++)
                    {
                        tmp = new SBee(t[j], f);
                        if (tmp.v < it.v)
                            it = new SBee(tmp);
                    }
                    mas[i] = new SBee(it);
                }

                t = new Vectors[sp];
                for (int i = e; i < p; i++)
                {
                    SBee it = new SBee(mas[i]), tmp;
                    Vectors cent = it.x.dup;


                    Parallel.For(0, t.Length, (int u) =>
                    {
                        t[u] = Vectors.Create(cent, delta);
                    });

                    for (int j = 0; j < sp; j++)
                    {
                        tmp = new SBee(t[j], f);
                        if (tmp.v < it.v)
                            it = new SBee(tmp);
                    }
                    mas[i] = new SBee(it);
                }



                t = new Vectors[mas.Length - p];
                Parallel.For(0, t.Length, (int i) =>
                {
                    t[i] = Vectors.Create(min, max);
                });

                for (int i = p; i < mas.Length; i++)
                    mas[i] = new SBee(t[i - p], f);

            }
        }
        #endregion
    }
}