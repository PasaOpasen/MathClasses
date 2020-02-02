using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using static МатКлассы.Number;

namespace МатКлассы
{
    public static partial class FuncMethods
    {

        /// <summary>
        /// Определённые интегралы
        /// </summary>
        public static class DefInteg
        {
            /// <summary>
            /// Шаг при интегрировании
            /// </summary>
            public static readonly double STEP = 0.0001;
            /// <summary>
            /// Оценка точности при интегрировании
            /// </summary>
            public static double EPS = STEP / 200/*Double.Epsilon*1E+58*/;
            /// <summary>
            /// Количество узлов
            /// </summary>
            public static int n = 20;
            /// <summary>
            /// Количество итераций при поиске корней многочлена
            /// </summary>
            public static int iter_count = 100;
            /// <summary>
            /// Количество шагов при интегрировании
            /// </summary>
            public static int h_Count = 5000;
            /// <summary>
            /// Шаги интегрирования в кратном интеграле по умолчанию
            /// </summary>
            public static double stx = 0.001, sty = 0.005;
            /// <summary>
            /// Число колец по умолчанию
            /// </summary>
            public static int countY = 100;

            /// <summary>
            /// Методы подсчёта интеграда
            /// </summary>
            public enum Method : byte
            {
                /// <summary>
                /// Метод средних прямоугольников
                /// </summary>
                MiddleRect,
                /// <summary>
                /// Метод трапеций
                /// </summary>
                Trapez,
                /// <summary>
                /// Метод Симпсона
                /// </summary>
                Simpson,
                /// <summary>
                /// Метод Гаусса
                /// </summary>
                Gauss,
                /// <summary>
                /// Метод Мелера (Эрмита)
                /// </summary>
                Meler,
                /// <summary>
                /// Метода Гаусса-Кронрода(по 15 точкам)
                /// </summary>
                GaussKronrod15,
                /// <summary>
                /// Метод Гаусса-Кронрода по 61 точке
                /// </summary>
                GaussKronrod61,
                /// <summary>
                /// Метод Гаусса-Кронрода по 61 точкам на основе процедуры с фортрана
                /// </summary>
                GaussKronrod61fromFortran,
                /// <summary>
                /// Метод Гаусса-Кронрода по 61 точке с эмпирическим делением отрезка
                /// </summary>
                GaussKronrod61Empire,
                /// <summary>
                /// Метод Гаусса-Кронрода, рассчитанный на использование в сумме интегралов по малым отрезкам
                /// </summary>
                GaussKronrod61Sq

            };
            /// <summary>
            /// Критерии подсчёта интеграла
            /// </summary>
            public enum Criterion : byte
            {
                /// <summary>
                /// Число шагов (узлов)
                /// </summary>
                StepCount,
                /// <summary>
                /// Точность
                /// </summary>
                Accuracy,
                /// <summary>
                /// Разбиение на несколько отрезков
                /// </summary>
                SegmentCount
            };

            /// <summary>
            /// Нахождение определённого интеграла методом средних прямоугольников
            /// </summary>
            /// <param name="F">Действительная функция действительного аргумента</param>
            /// <param name="a">Первая точка промежутка интегрирования</param>
            /// <param name="b">Последняя точка промежутка интегрирования</param>
            /// <returns></returns>
            public static double MiddleRect(Func<double, double> F, double a, double b)//метод средних прямоугольников
            {
                double result = 0, h = STEP;
                //h = (b - a) / n; //Шаг сетки

                for (int i = 0; i < (int)((b - a) / h); i++)
                {
                    result += F(a + h * (i + 0.5)); //вычисляем в средней точке и добавляем в сумму
                }
                result *= h;
                return result;
            }
            /// <summary>
            /// Нахождение определённого интеграла методом трапеций
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double Trapez(Func<double, double> f, double a, double b)//метод трапеций
            {
                double h = STEP, result = 0;
                int k = ((int)((b - a) / h) /*-1*/);

                result += (f(a) + f(b)) / 2;

                for (int i = 1; i < k; i++) result += f(a + i * h);

                return h * result;
            }
            /// <summary>
            /// Нахождение определённого интеграла методом Симпсона
            /// </summary>
            /// <param name="F"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double Simpson(Func<double, double> F, double a, double b)//метод Симпсона
            {
                double step = (b - a) / (double)h_Count;
                double S = 0, x, h = step;
                int i = 1;
                //отрезок [a, b] разобьем на N частей
                //h = (b - a) / N;
                x = a + h;
                while (i < h_Count && x < b)
                {
                    S += 4 * F(x); //Console.WriteLine($"x={x}  4F(x)={4*F(x)}");
                    x += h; i++;
                    //проверяем не вышло ли значение x за пределы полуинтервала [a, b)
                    if (x >= b || i == h_Count)
                    {
                        S = (h / 3) * (S + F(a) + F(b));

                        return S;
                    }

                    S += 2 * F(x); //Console.WriteLine($"x={x}  2F(x)={2 *F(x)}");
                    x += h; i++;
                }
                //Console.WriteLine(S +" "+b);
                S = (h / 3) * (S + F(a) + F(b));

                return S;
            }
            /// <summary>
            /// Нахождение определённого интеграла через квадратурные формулы Гаусса
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="a">Начало отрезка итегрирования</param>
            /// <param name="b">Конец отрезка интегрирования</param>
            /// <returns></returns>
            public static double Gauss(Func<double, double> f, double a, double b)
            {
                Func<double, double> fi = (double t) => { return f((a + b) / 2 + (b - a) / 2 * t); };//замена координат
                double sum = 0;
                Vectors root;
                if (RootGauss == null || RootGauss.Deg != n)
                {
                    Polynom p = Polynom.Lezh(DefInteg.n);//полином Лежандра
                    root = new Vectors(n);

                    for (int i = 1; i <= DefInteg.n; i++)//поиск корней многочлена Лежандра
                    {
                        root[i - 1] = Math.Cos(Math.PI * (4 * i - 1) / (4 * n + 2));
                        for (int j = 0; j < iter_count; j++)
                        {
                            Polynom tmp = p | 1;
                            root[i - 1] -= p.Value(root[i - 1]) / tmp.Value(root[i - 1]);
                        }
                    }
                    RootGauss = root;
                }
                else root = new Vectors(RootGauss);


                //root.Show();

                for (int i = 1; i <= DefInteg.n; i++)//суммирование
                {
                    //поиск приведённого многочлена
                    Polynom Ap = (new Polynom(1, root)) / (new Polynom(root[i - 1]));

                    sum += Ap.S(-1, 1) * fi(root[i - 1]) / Ap.Value(root[i - 1]);
                }

                return (b - a) / 2 * sum;
            }

            private static Vectors RootMeler = null;
            private static Vectors RootGauss = null;
            /// <summary>
            /// Нахождение определённого интеграла через квадратурные формулы Мелера
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="a">Начало отрезка итегрирования</param>
            /// <param name="b">Конец отрезка интегрирования</param>
            /// <returns></returns>
            public static double Meler(Func<double, double> f, double a, double b)
            {
                Func<double, double> fi = (double t) => { return f((a + b) / 2 + (b - a) / 2 * t); };//замена координат
                double sum = 0;
                Vectors root;
                int n = DefInteg.n;
                //if (RootMeler == null || RootMeler.Deg!=n)
                //{
                root = new Vectors(n);

                for (int i = 1; i <= n; i++)//поиск корней многочлена Чебышева
                {
                    root[i - 1] = Math.Cos(Math.PI * (2 * i - 1) / (2 * n));
                }
                // RootMeler = root;
                //}
                //else root = new Vectors(RootMeler);


                //root.Show();

                for (int i = 1; i <= n; i++)//суммирование
                {
                    sum += fi(root[i - 1]) * Math.Sqrt(1 - root[i - 1] * root[i - 1]);
                }

                return (b - a) / (2 * Math.PI) / n * sum;
            }
            /// <summary>
            /// Класс методов, связанных с вычислением интеграла квадратурами Гаусса-Кронрода
            /// </summary>
            public static class GaussKronrod
            {
                static GaussKronrod()
                {
                    MyGKInit();
                    GK_7_15_init();
                    List<string> r = new List<string>(4); r.Add("");
                    MasListDinnInfo[0] = new List<string>(r);
                    MasListDinnInfo[1] = new List<string>(r);
                    MasListDinnInfo[2] = new List<string>(r);
                    MasListDinnInfo[3] = new List<string>(r);
                }
                /// <summary>
                /// Процедурная реализация вектор-функции комплексного аргумента
                /// </summary>
                /// <param name="x">Аргумент</param>
                /// <param name="y">Вектор значений</param>
                /// <param name="N">Размерность вектора значений</param>
                public delegate void Myfunc(Complex x, ref Complex[] y, int N);
                /// <summary>
                /// Комплексная вектор-функция
                /// </summary>
                /// <param name="x"></param>
                /// <param name="N"></param>
                /// <returns></returns>
                public delegate Complex[] ComplexVectorFunc(Complex x, int N);

                /// <summary>
                /// Количество узлов при интегрировании
                /// </summary>
                public enum NodesCount : byte
                {
                    GK15, GK21, GK31, GK41, GK51, GK61
                }

                private static double[] GetNew(double[] x)
                {
                    double[] res = new double[x.Length + 1];
                    for (int i = 1; i <= x.Length; i++)
                        res[i] = x[i - 1];
                    return res;
                }
                /// <summary>
                /// Выбрать метод в зависимости от узлов
                /// </summary>
                /// <param name="c"></param>
                internal static void ChooseGK(NodesCount c)
                {
                    switch (c)
                    {
                        //case NodesCount.GK15:
                        //    GK_nodes = GetNew(x15);
                        //    K_weights = GetNew(wkronrod15);
                        //    G_weights = GetNew(wgauss15);
                        //    Nodes = 15;
                        //    break;
                        //case NodesCount.GK21:
                        //    GK_nodes = GetNew(x21);
                        //    K_weights = GetNew(wkronrod21);
                        //    G_weights = GetNew(wgauss21);
                        //    Nodes = 21;
                        //    break;
                        //case NodesCount.GK31:
                        //    GK_nodes = GetNew(x31);
                        //    K_weights = GetNew(wkronrod31);
                        //    G_weights = GetNew(wgauss31);
                        //    Nodes = 31;
                        //    break;
                        //case NodesCount.GK41:
                        //    GK_nodes = GetNew(x41);
                        //    K_weights = GetNew(wkronrod41);
                        //    G_weights = GetNew(wgauss41);
                        //    Nodes = 41;
                        //    break;
                        //case NodesCount.GK51:
                        //    GK_nodes = GetNew(x51);
                        //    K_weights = GetNew(wkronrod51);
                        //    G_weights = GetNew(wgauss51);
                        //    Nodes = 51;
                        //    break;
                        //case NodesCount.GK61:
                        //    GK_nodes = GetNew(x61);
                        //    K_weights = GetNew(wkronrod61);
                        //    G_weights = GetNew(wgauss61);
                        //    Nodes = 61;
                        //    break;
                        case NodesCount.GK15:
                            GK_nodes = _x15;
                            K_weights = _wkronrod15;
                            G_weights = _wgauss15;
                            Nodes = 15;
                            break;
                        case NodesCount.GK21:
                            GK_nodes = _x21;
                            K_weights = _wkronrod21;
                            G_weights = _wgauss21;
                            Nodes = 21;
                            break;
                        case NodesCount.GK31:
                            GK_nodes = _x31;
                            K_weights = _wkronrod31;
                            G_weights = _wgauss31;
                            Nodes = 31;
                            break;
                        case NodesCount.GK41:
                            GK_nodes = _x41;
                            K_weights = _wkronrod41;
                            G_weights = _wgauss41;
                            Nodes = 41;
                            break;
                        case NodesCount.GK51:
                            GK_nodes = _x51;
                            K_weights = _wkronrod51;
                            G_weights = _wgauss51;
                            Nodes = 51;
                            break;
                        case NodesCount.GK61:
                            GK_nodes = _x61;
                            K_weights = _wkronrod61;
                            G_weights = _wgauss61;
                            Nodes = 61;
                            break;
                    }
                }

                /// <summary>
                /// Размерность
                /// </summary>
                static int Nodes = 15, Nodes61 = 61;
                static bool Key = false;
                static double RV_eps_step_increment, norm_param = 1;
                static double[] GK_nodes, K_weights, G_weights, GK_nodes61, K_weights61, G_weights61;
                static double h = 0.1;
                static double eps = 0.001;

                static void GK_7_15_init()
                {
                    GK_nodes = new double[Nodes + 1];
                    K_weights = new double[Nodes + 1];
                    G_weights = new double[Nodes + 1];

                    GK_nodes[1] = -0.991455371120813; GK_nodes[2] = -0.949107912342759;
                    GK_nodes[3] = -0.864864423359769; GK_nodes[4] = -0.741531185599394;
                    GK_nodes[5] = -0.586087235467691; GK_nodes[6] = -0.405845151377397;
                    GK_nodes[7] = -0.207784955007898;
                    GK_nodes[8] = 0;
                    GK_nodes[9] = 0.207784955007898; GK_nodes[10] = 0.405845151377397;
                    GK_nodes[11] = 0.586087235467691; GK_nodes[12] = 0.741531185599394;
                    GK_nodes[13] = 0.864864423359769; GK_nodes[14] = 0.949107912342759;
                    GK_nodes[15] = 0.991455371120813;
                    K_weights[1] = 0.022935322010529; K_weights[2] = 0.063092092629979;
                    K_weights[3] = 0.104790010322250; K_weights[4] = 0.140653259715525;
                    K_weights[5] = 0.169004726639267; K_weights[6] = 0.190350578064785;
                    K_weights[7] = 0.204432940075298;
                    K_weights[8] = 0.209482141084728;
                    K_weights[9] = 0.204432940075298; K_weights[10] = 0.190350578064785;
                    K_weights[11] = 0.169004726639267; K_weights[12] = 0.140653259715525;
                    K_weights[13] = 0.104790010322250; K_weights[14] = 0.063092092629979;
                    K_weights[15] = 0.022935322010529;

                    G_weights[1] = 0; G_weights[2] = 0.129484966168870;
                    G_weights[3] = 0; G_weights[4] = 0.279705391489277;
                    G_weights[5] = 0; G_weights[6] = 0.381830050505119;
                    G_weights[7] = 0;
                    G_weights[8] = 0.417959183673469;
                    G_weights[9] = 0; G_weights[10] = 0.381830050505119;
                    G_weights[11] = 0; G_weights[12] = 0.279705391489277;
                    G_weights[13] = 0; G_weights[14] = 0.129484966168870;
                    G_weights[15] = 0;
                    RV_eps_step_increment = 1e-8;

                    //my 61
                    //GK_nodes61 = new double[Nodes61 + 1];
                    //K_weights61 = new double[Nodes61 + 1];
                    //G_weights61 = new double[Nodes61 + 1];
                    //for(int i=1;i<=61;i++)
                    //{
                    //    GK_nodes61[i] = x61[i-1];
                    //    K_weights61[i] = wkronrod61[i-1];
                    //    G_weights61[i] = wgauss61[i-1];
                    //}
                }
                /// <summary>
                /// Интеграл по коченому отрезку
                /// </summary>
                /// <param name="int_func">Интегрируемая функция</param>
                /// <param name="a">Начало отрезна интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="int_h">Начальный шаг</param>
                /// <param name="eps">Используемая погрешность</param>
                /// <param name="ret_arr">Вектор значений интеграла (результат)</param>
                /// <param name="N">Размерность вектора значений</param>
                public static void GK_adaptive_int(Myfunc int_func, Complex a, Complex b, double int_h, double eps, ref Complex[] ret_arr, int N)
                {
                    double eps_out = 0;
                    Complex t_i_h, t_x;
                    Complex[] ret_arr_0 = new Complex[N];
                    ret_arr = new Complex[N];
                    t_x = new Complex(a);
                    t_i_h = new Complex(int_h, 0);

                    if (Math.Abs(Complex.Imag(b) - Complex.Imag(a)) > eps)
                    {
                        if (Complex.Imag(b) < Complex.Imag(a))
                            t_i_h = new Complex(0, -int_h);
                        else
                            t_i_h = new Complex(0, int_h);
                    }

                    while (Math.Abs((double)(b - t_x)) > eps)
                    {
                        if (Math.Abs((double)(t_x + t_i_h)) > Math.Abs((double)b)) t_i_h = b - t_x;
                        GK_int(int_func, t_x, t_x + t_i_h, ref ret_arr_0, ref eps_out, N);//if(!Double.IsNaN(ret_arr_0[0].Abs))ret_arr_0.Show();

                        if (eps_out > eps)
                        {
                            t_i_h = t_i_h * 0.5;
                        }
                        else
                        {
                            try
                            {
                                ret_arr = Complex.Sum(ret_arr, ret_arr_0);
                                t_x = t_x + t_i_h;
                                //Console.WriteLine(t_x);
                                if (eps_out < 1e-3 * eps) t_i_h = t_i_h * 1.75;

                            }
                            catch (Exception e) { throw new Exception(e.Message); }
                            Vectors v = new Vectors(N);
                            for (int i = 0; i < v.Deg; i++) v[i] = Math.Abs((double)ret_arr_0[i]);
                            if ((v.Max) / norm_param < 2.5e-5) return;
                        }
                    }

                    int_h = Math.Abs((double)t_i_h);
                }
                /// <summary>
                /// Интеграл по коченому отрезку
                /// </summary>
                /// <param name="int_func">Интегрируемая функция</param>
                /// <param name="a">Начало отрезна интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="int_h">Начальный шаг</param>
                /// <param name="eps">Используемая погрешность</param>
                /// <param name="N">Размерность вектора значений</param>
                public static Complex[] Integral(Myfunc int_func, Complex a, Complex b, double int_h, double eps, int N)
                {
                    Complex[] y = Array.Empty<Complex>();
                    try { GK_adaptive_int(int_func, a, b, int_h, eps, ref y, N); } catch { throw new Exception("тут"); }
                    return y;
                }
                /// <summary>
                /// Интеграл по прямой
                /// </summary>
                /// <param name="int_func">Интегрируемая функция</param>
                /// <param name="a">Начало отрезна интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="int_h">Начальный шаг</param>
                /// <param name="eps">Используемая погрешность</param>
                /// <param name="N">Размерность вектора значений</param>
                public static Complex IntegralInf(ComplexFunc f, Complex a, Complex b, int tt = 100, int n = 15, ComplexFunc delta = null, int maxmult = 12, double vareps = 1e-6, double maxarg = 140)
                {
                    double epss = vareps;
                    Complex sum = MySuperGaussKronrod(f, a, b, delta, tt, n);//a.Show();b.Show();int_h.Show();eps.Show();
                    Complex p = new Complex(sum); if (Double.IsNaN(sum.Re)) $"Такая байда {sum} \t f({a})={f(a)} \t f({b})={f(b)} \t d({a})={delta(a)} \t d({b})={delta(b)}".Show();

                    Complex ab = b - a;//ab.Show();
                    int t = 1;
                    while (t <= maxmult && b.Abs < maxarg)//идти по всем множителям шага
                    {
                        //Console.WriteLine($"p={p} t={t} norm={p.Abs}");
                        Complex s1 = MySuperGaussKronrod(f, a - ab, a, delta, tt, n), s2 = MySuperGaussKronrod(f, b, b + ab, delta, tt, n);
                        p = s1 + s2;
                        a -= ab;
                        b += ab;
                        ab *= 2;
                        sum += p; if (Double.IsNaN(sum.Re)) $"Такая байда {sum} \t f({a})={f(a)} \t f({b})={f(b)} \t d({a})={delta(a)} \t d({b})={delta(b)}".Show();
                        while (p.Abs > epss && b.Abs < maxarg)//пока добавление больше погрешности
                        {
                            //Console.WriteLine($"p={p} t={t} norm={p.Abs}");
                            s1 = MySuperGaussKronrod(f, a - ab, a, delta, tt, n); s2 = MySuperGaussKronrod(f, b, b + ab, delta, tt, n);
                            p = s1 + s2;
                            a -= ab;
                            b += ab;
                            sum += p; if (Double.IsNaN(sum.Re)) $"Такая байда {sum} \t f({a})={f(a)} \t f({b})={f(b)} \t d({a})={delta(a)} \t d({b})={delta(b)}".Show();
                        }
                        //p = sum;//t.Show();
                        t++;
                    }

                    return sum;
                }
                /// <summary>
                /// Интеграл по отрезку от a до +inf
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="n"></param>
                /// <param name="maxmult"></param>
                /// <param name="vareps"></param>
                /// <returns></returns>
                public static Complex IntegralHalfInf(ComplexFunc f, double a, double begH, int tt = 100, int n = 15, ComplexFunc delta = null, int maxmult = 12, double vareps = 1e-6, double h = 0.2)
                {
                    double epss = vareps;
                    Complex sum = MySuperGaussKronrod(f, a, a + begH, delta, tt, n, h);//a.Show();b.Show();int_h.Show();eps.Show();
                    Complex p = new Complex(sum); if (Double.IsNaN(sum.Re)) { $"1 Такая байда {sum} \t f({a})={f(a)} \t f({a + begH})={f(a + begH)} \t d({a})={delta(a)} \t d({a + begH})={delta(a + begH)}".Show(); Console.ReadKey(); }

                    int t = 1; a += begH;
                    while (t <= maxmult && a < tt)//идти по всем множителям шага
                    {
                        //Console.WriteLine($"p={p} t={t} norm={p.Abs}");
                        p = MySuperGaussKronrod(f, a, a + begH, delta, tt, n, h); if (Double.IsNaN(p.Re)) { $"2 Такая байда {p} \t f({a})={f(a)} \t f({a + begH})={f(a + begH)} \t d({a})={delta(a)} \t d({a + begH})={delta(a + begH)}".Show(); Console.ReadKey(); }
                        a += begH;
                        begH *= 2;
                        sum += p;
                        while (p.Abs > epss && a < tt)//пока добавление больше погрешности
                        {
                            //Console.WriteLine($"p={p} t={t} norm={p.Abs}");
                            p = MySuperGaussKronrod(f, a, a + begH, delta, tt, n, h);
                            if (Double.IsNaN(p.Re) || Double.IsInfinity(p.Re))
                            {
                                $"3 При суммировании интегралов промужуточный интеграл равен = {MySuperGaussKronrod(f, a, a + begH, delta, tt, n, h)} \t f({a})={f(a)} \t f({a + begH})={f(a + begH)} \t d({a})={delta(a)} \t d({a + begH})={delta(a + begH)}".Show();
                                Console.ReadKey();
                            }
                            a += begH;
                            begH *= 2;
                            sum += p;
                        }
                        //p = sum;//t.Show();
                        t++;
                    }

                    return sum;
                }
                /// <summary>
                /// Определённый интеграл
                /// </summary>
                /// <param name="f">Интегрируемая функция</param>
                /// <param name="a">Начало отрезка интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <returns></returns>
                public static double Integral(Func<double, double> f, double a, double b)
                {
                    Myfunc z = (Complex x, ref Complex[] t, int N) => { t = new Complex[1]; t[0] = f(x.Re); };
                    Complex[] y = Integral(z, a, b, h, eps, 1);
                    return y[0].Re;
                }
                /// <summary>
                /// Несобственный интеграл по вещественной оси
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <returns></returns>
                public static double IntegralInf(ComplexFunc f, double a, double b, ComplexFunc delta = null, int t = 100, int n = 15, double vareps = 1e-6, double maxarg = 150)
                {
                    Complex y = IntegralInf(f, new Complex(a), new Complex(b), t, n, delta, 3, vareps, maxarg);
                    return y.Re;
                }

                static void GK_adaptive_int_inf(Myfunc int_func, Complex a, Complex b, ref double int_h, ref double eps, ref Complex[] ret_arr, int N)
                {
                    int i, ipr;
                    bool it;
                    double[] temp_arr = new double[N /*+ 1*/];
                    double t1, eps10, pm, pt, t, int_h_1, t_h;
                    Complex t_x_a, t_x_b;
                    Complex[] ret_arr_0 = new Complex[N /*+ 1*/];
                    ret_arr = new Complex[N /*+ 1*/];
                    t_x_b = new Complex(a);
                    t = Math.Abs((double)(b - a));
                    eps10 = eps * 10;
                    ipr = 0;
                    it = true;
                    while (it)
                    {
                        t_x_a = new Complex(t_x_b); t_x_b = t_x_b + int_h; t1 = Math.Abs((double)(t_x_b - a));
                        if (t1 > t)
                        {
                            t_x_b = b;
                            it = false;
                        }
                        int_h_1 = int_h;
                        GK_adaptive_int(int_func, t_x_a, t_x_b, int_h_1, eps, ref ret_arr_0, N);
                        Vectors v = new Vectors(N);

                        for (int j = 0; j < v.Deg; j++) v[j] = Math.Abs((double)ret_arr_0[j /*+ 1*/]);
                        if ((v.Min < 1e-10) && (Math.Abs((double)t_x_a) > 50.0) && (v.Max < 1e-7)) return;
                        try
                        {
                            if (v.Max / norm_param < 1e-8) return;

                            t_h = Math.Abs((double)(t_x_b - t_x_a));
                            temp_arr = (double[])(v / t_h);

                            ret_arr = Complex.Sum(ret_arr, ret_arr_0);

                            if (Math.Abs(int_h) < 10 * Math.Abs(int_h_1))
                            {
                                int_h = 4 * int_h;
                            }
                            else int_h = 4 * int_h_1;
                            // at infinity
                            pm = 0;
                            for (i = 0/*1*/; i </*=*/ N; i++)
                            {
                                if (Math.Abs((double)ret_arr[i]) > 1e-15)
                                {
                                    pt = Math.Abs((double)(temp_arr[i] * t1 / ret_arr[i]));
                                    if (pt > pm) pm = pt;
                                }
                            }

                            if (pm < eps10)
                            {
                                ipr = ipr + 1;
                                if (ipr > 4) return;
                            }
                            else
                            {
                                ipr = 0;
                            }
                        }
                        catch { throw new Exception(""); }
                    }
                }

                static void GK_int(Myfunc int_func, Complex a, Complex b, ref Complex[] ret_arr, ref double eps_out, int N)
                {
                    //ChooseGK(nodesCount);

                    // implicit none;
                    int i;
                    Complex[] GK_nodes_arb = new Complex[Nodes + 1], K_weights_arb = new Complex[Nodes + 1], G_weights_arb = new Complex[Nodes + 1];
                    Complex[,] temp_arr = new Complex[N + 1, 3];

                    GK_nodes_arb = Complex.Sum(Complex.Mult(0.5 * (b - a), GK_nodes), 0.5 * (b + a));
                    K_weights_arb = Complex.Mult((b - a), (double[])((Vectors)K_weights * 0.5));
                    G_weights_arb = Complex.Mult((b - a), (double[])((Vectors)G_weights * 0.5));

                    for (i = 1; i <= Nodes; i++)
                    {
                        int_func(GK_nodes_arb[i], ref ret_arr, N);
                        //ret_arr.Show();
                        //K_weights_arb.Show();
                        //temp_arr[0, 0].Show();

                        for (int j = 1; j <= N; j++)
                        {
                            try
                            {
                                //Console.WriteLine(j + " <= " + N);
                                temp_arr[j, 1] += K_weights_arb[i] * ret_arr[j - 1];
                                temp_arr[j, 2] += G_weights_arb[i] * ret_arr[j - 1];
                            }
                            catch (Exception e) { throw new Exception(e.Message); }
                        }

                    }

                    Vectors v = new Vectors(N);
                    for (int j = 1; j <= N; j++)
                        v[j - 1] = Math.Abs((double)(temp_arr[j, 1] - temp_arr[j, 2]));
                    eps_out = v.Max;
                    for (int j = 1; j <= N; j++)
                        ret_arr[j - 1] = new Complex(temp_arr[j, 1]);
                }

                public static List<string>[] MasListDinnInfo = new List<string>[4];
                /// <summary>
                /// Информация о последнем найденном с помощью DINN5_GK интеграле 
                /// </summary>
                private static List<string> LastListOfDINN5GK;
                /// <summary>
                /// Базовый DINN с фортрана
                /// </summary>
                /// <param name="CF"></param>
                /// <param name="t1"></param>
                /// <param name="t2"></param>
                /// <param name="t3"></param>
                /// <param name="t4"></param>
                /// <param name="tm"></param>
                /// <param name="tp"></param>
                /// <param name="eps"></param>
                /// <param name="pr"></param>
                /// <param name="gr"></param>
                /// <param name="N"></param>
                /// <param name="Rd"></param>
                /// <remarks>
                /// ! Программа вычисления N интегралов по полубесконечному контуру 
                ///!           в случае обратной волны 
                ///!
                ///!  subroutine СF(u, s, n) - подынтегральные функции; u - аргумент(complex16),
                ///!                   s(n) - массив значений функций в точке u(complex16),
                ///!                   n - число интегралов(integer)
                ///! [t1, t2],[t3, t4] - участки отклонения контура вниз(real8)
                ///!         [t2, t3] - участок отклонения контура вверх(real8)
                ///! tm,tp > 0 - величины отклонения контура вниз и вверх(real8)
                ///! (если нет обратной волны, то следует положить t2 = t3 = t1, tp = 0 
                ///!  обход полюсов при этом будет только снизу на участке[t1, t4]
                ///!  с отклонением на tm)
                ///! eps -  отн.погрешность,  pr - начальный шаг,
                ///! N- число интегралов(integer)
                ///! Rd(N) - выход: массив значений интегралов
                /// </remarks>
                static void DINN5_GK(Myfunc CF, double t1, double t2, double t3, double t4, double tm, double tp, double eps, double pr, double gr, int N, out Complex[] Rd)
                {
                    // implicit none
                    int i;
                    double int_h;
                    Complex a, b;
                    Complex[] sb = new Complex[N];

                    //GK_7_15_init();
                    Rd = new Complex[N];
                    if (t1 < t4)
                    {
                        // [0, t1]
                        a = 0; b = t1; int_h = 0.25 * Math.Abs((double)(b - a));
                        GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                        Rd = Complex.Sum(Rd, sb);//Rd.Show();
                        //LastListOfDINN5GK.Add($"\tНа участке [0,t1] GK_adaptive_int = {sb[0]}");
                        if (t3 - t2 < eps)
                        { // no inverse poles case
                            a = b; b = new Complex(t1, -tm); int_h = 0.25 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            // LastListOfDINN5GK.Add($"\tНа участке [t1,t1-tm*i] GK_adaptive_int = {sb[0]}");

                            a = b; b = new Complex(t4, -tm); int_h = 0.05 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t1-tm*i,t4-tm*i] GK_adaptive_int = {sb[0]}");

                            a = b; b = t4; int_h = 0.25 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t4-tm*i,t4] GK_adaptive_int = {sb[0]}");
                        }
                        else
                        {
                            if (t2 - t1 > eps)
                            {
                                // first deviation from below

                                a = b; b = new Complex(t1, -tm); int_h = 0.5 * Math.Abs((double)(b - a));
                                GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                                Rd = Complex.Sum(Rd, sb);/*Rd.Show();*/
                                //LastListOfDINN5GK.Add($"\tНа участке [t1,-tm*i] GK_adaptive_int = {sb[0]}");

                                a = b; b = new Complex(t2, -tm); int_h = 0.5 * Math.Abs((double)(b - a));
                                GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                                Rd = Complex.Sum(Rd, sb);
                                //LastListOfDINN5GK.Add($"\tНа участке [-tm,t2-tm*i] GK_adaptive_int = {sb[0]}");
                            }

                            // diviation from above

                            a = b; b = new Complex(t2, tp); int_h = 0.5 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t1 либо t2-tm*i,t2+tp] GK_adaptive_int = {sb[0]}");

                            a = b; b = new Complex(t3, tp); int_h = 0.5 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            // LastListOfDINN5GK.Add($"\tНа участке [t2+tp,t3+tp*i] GK_adaptive_int = {sb[0]}");

                            a = b; b = new Complex(t3, -tm); int_h = 0.5 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t3+tp*i,t3-tm*i] GK_adaptive_int = {sb[0]}");

                            // second diviation from below
                            a = b; b = new Complex(t4, -tm); int_h = 0.25 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t3-tm*i,t4-tm*i] GK_adaptive_int = {sb[0]}");

                            a = b; b = t4; int_h = 0.5 * Math.Abs((double)(b - a));
                            GK_adaptive_int(CF, a, b, int_h, eps, ref sb, N);
                            Rd = Complex.Sum(Rd, sb);
                            //LastListOfDINN5GK.Add($"\tНа участке [t4-tm*i,t4] GK_adaptive_int = {sb[0]}");
                        }
                        Vectors v = new Vectors(N);
                        for (i = 0; i < v.Deg; i++)
                            v[i] = Math.Abs((double)Rd[i]);
                        norm_param = v.Max;
                        // Console.WriteLine(norm_param);
                    }

                    if (gr > t4)
                    {
                        a = t4;
                        b = gr;
                        int_h = 0.33; //0.25*Math.Abs((double)(b-a));
                        if (Key)
                        {
                            GK_adaptive_int_inf(CF, a, b, ref int_h, ref eps, ref sb, N);
                            //LastListOfDINN5GK.Add($"\tНа участке gr > t4 GK_adaptive_int_inf = {sb[0]}");
                        }
                        else
                        {
                            GK_adaptive_int(CF, a, b, int_h, eps * 10, ref sb, N);
                            //LastListOfDINN5GK.Add($"\tНа участке gr > t4 GK_adaptive_int = {sb[0]}");

                        }
                        Rd = Complex.Sum(Rd, sb);

                    }
                    //LastListOfDINN5GK.Add("");
                    //for(int o=0;o<3;o++)
                    //MasListDinnInfo[o] = new List<string>(MasListDinnInfo[o+1]);
                    //MasListDinnInfo[3] = LastListOfDINN5GK;
                }
                public static Complex[] DINN5_GK(ComplexVectorFunc f, double t1, double t2, double t3, double t4, double tm, double tp, double eps, double pr, double gr, int N, NodesCount nodesCount = NodesCount.GK15)
                {
                    ChooseGK(nodesCount);
                    Complex[] Result;
                    Myfunc ff = (Complex x, ref Complex[] y, int n) => y = f(x, n);
                    DINN5_GK(ff, t1, t2, t3, t4, tm, tp, eps, pr, gr, N, out Result);
                    return Result;
                }
                /// <summary>
                /// Подсчёт несобственного комплексного интеграла от 0 до inf с учётом полюсов
                /// </summary>
                /// <param name="f">Функция комплексного переменного</param>
                /// <param name="t1"></param>
                /// <param name="t2"></param>
                /// <param name="t3"></param>
                /// <param name="t4"></param>
                /// <param name="tm">Величина отклонения контура вниз</param>
                /// <param name="tp">Величина отклонения контура вверх</param>
                /// <param name="eps">Погрешность</param>
                /// <param name="pr">Начальный шаг</param>
                /// <param name="gr">Верхний предел</param>
                /// <remarks>
                /// ВЗЯТО С ДОКУМЕНТАЦИИ ОТ ФОРТРАНА
                /// ! [t1,t2],[t3,t4] - участки отклонения контура вниз (real8)
                ///!         [t2, t3] - участок отклонения контура вверх(real8)
                ///! tm,tp > 0 - величины отклонения контура вниз и вверх(real8)
                ///! (если нет обратной волны, то следует положить t2 = t3 = t1, tp = 0 
                ///!  обход полюсов при этом будет только снизу на участке[t1, t4]
                ///!  с отклонением на tm)
                /// </remarks>
                /// <returns></returns>
                public static Complex DINN_GK(ComplexFunc f, double t1, double t2, double t3, double t4, double tm, double tp = 0, double eps = 1e-4, double pr = 1e-2, double gr = 1e4, NodesCount nodesCount = NodesCount.GK15)
                {
                    ComplexVectorFunc ff = (Complex x, int n) => new Complex[] { f(x) };
                    return DINN5_GK(ff, t1, t2, t3, t4, tm, tp, eps, pr, gr, 1, nodesCount)[0];
                }
                /// <summary>
                /// Подсчёт несобственного комплексного интеграла от 0 до inf с учётом полюсов
                /// </summary>
                /// <param name="f">Функция комплексного переменного</param>
                /// <param name="t1"></param>
                /// <param name="t2"></param>
                /// <param name="t3"></param>
                /// <param name="t4"></param>
                /// <param name="tm">Величина отклонения контура вниз</param>
                /// <param name="tp">Величина отклонения контура вверх</param>
                /// <param name="eps">Погрешность</param>
                /// <param name="pr">Начальный шаг</param>
                /// <param name="gr">Верхний предел</param>
                /// <param name="nodesCount">Число узлов в квадратурах</param>
                /// <remarks>
                /// ВЗЯТО С ДОКУМЕНТАЦИИ ОТ ФОРТРАНА
                /// ! [t1,t2],[t3,t4] - участки отклонения контура вниз (real8)
                ///!         [t2, t3] - участок отклонения контура вверх(real8)
                ///! tm,tp > 0 - величины отклонения контура вниз и вверх(real8)
                ///! (если нет обратной волны, то следует положить t2 = t3 = t1, tp = 0 
                ///!  обход полюсов при этом будет только снизу на участке[t1, t4]
                ///!  с отклонением на tm)
                /// </remarks>
                /// <returns></returns>
                public static Complex DINN_GK(Func<Complex, Complex> f, double t1, double t2, double t3, double t4, double tm, double tp = 0, double eps = 1e-4, double pr = 1e-2, double gr = 1e4, NodesCount nodesCount = NodesCount.GK15)
                {
                    ComplexVectorFunc ff = (Complex x, int n) => new Complex[] { f(x) };
                    return DINN5_GK(ff, t1, t2, t3, t4, tm, tp, eps, pr, gr, 1, nodesCount)[0];
                }
                /// <summary>
                /// Несобственный интеграл с нулевыми параметрами (если не надо делать обход контура)
                /// </summary>
                /// <param name="f"></param>
                /// <returns></returns>
                public static Complex DINN_GKwith0(ComplexFunc f, double eps = 1e-4, NodesCount nodesCount = NodesCount.GK15) => DINN_GK(f, 0, 0, 0, 0, 0, 0, eps, nodesCount: nodesCount);
                /// <summary>
                /// Несобственный интеграл по всей оси от конкретно этой функции
                /// </summary>
                /// <param name="f"></param>
                /// <returns></returns>
                public static Complex DINN_GKwith0Full(ComplexFunc f, double eps = 1e-4, NodesCount nodesCount = NodesCount.GK15)
                {
                    ComplexFunc f3 = t => f(t) + f(-t);
                    return DINN_GKwith0(f3, eps, nodesCount);
                }

                static void GK_adaptive_int_RealV(Myfunc int_func, double a, double b, double int_h, double eps, Complex[] ret_arr, int N)
                {
                    // implicit none;
                    double eps_out = 0, t_i_h, t_x;
                    Complex[] ret_arr_0 = new Complex[N + 1];

                    t_x = a;
                    t_i_h = int_h;
                    while (b - t_x > eps)
                    {
                        if (t_x + t_i_h > b) t_i_h = b - t_x;

                        GK_int_RealV(int_func, t_x, t_x + t_i_h, ret_arr_0, eps_out, N);
                        if (eps_out > eps)
                        {
                            t_i_h = t_i_h * 0.5;
                        }
                        else
                        {
                            ret_arr = Complex.Sum(ret_arr, ret_arr_0);
                            t_x = t_x + t_i_h;
                            //   Console.WriteLine(t_x);
                            if (eps_out < RV_eps_step_increment) t_i_h = t_i_h * 1.5;
                        }
                    }

                    int_h = t_i_h;
                }

                static void GK_int_RealV(Myfunc int_func, double a, double b, Complex[] ret_arr, double eps_out, int N)
                {
                    // implicit none;
                    int i;
                    double[] GK_nodes_arb = new double[Nodes + 1], K_weights_arb = new double[Nodes + 1], G_weights_arb = new double[Nodes + 1];

                    Complex[,] temp_arr = new Complex[N + 1, 3];


                    GK_nodes_arb = (double[])((Vectors)(GK_nodes) * 0.5 * (b - a) + 0.5 * (b + a));
                    K_weights_arb = (double[])((Vectors)(K_weights) * 0.5 * (b - a));
                    G_weights_arb = (double[])((Vectors)(G_weights) * 0.5 * (b - a));

                    for (i = 1; i <= Nodes; i++)
                    {
                        int_func(GK_nodes_arb[i], ref ret_arr, N);
                        for (int j = 1; j <= N; j++)
                        {
                            temp_arr[j, 1] = temp_arr[j, 1] + K_weights_arb[i] * ret_arr[j - 1];
                            temp_arr[j, 2] = temp_arr[j, 2] + G_weights_arb[i] * ret_arr[j - 1];
                        }
                    }

                    for (i = 0; i < N; i++)
                        ret_arr[i] = temp_arr[i, 1] - temp_arr[i, 2];

                    Vectors v = new Vectors(N);
                    for (i = 0; i < N; i++)
                        v[i] = ret_arr[i].Abs;
                    eps_out = (200 * v.Max) * 1.5;
                    for (i = 0; i < N; i++)
                        ret_arr[i] = temp_arr[i, 1];
                }

                private static double[] x15, x21, x31, x41, x51, x61, wgauss15, wgauss21, wgauss31, wgauss41, wgauss51, wgauss61, wkronrod15, wkronrod21, wkronrod31, wkronrod41, wkronrod51, wkronrod61, _x15, _x21, _x31, _x41, _x51, _x61, _wgauss15, _wgauss21, _wgauss31, _wgauss41, _wgauss51, _wgauss61, _wkronrod15, _wkronrod21, _wkronrod31, _wkronrod41, _wkronrod51, _wkronrod61;
                private static int ng15 = 4, ng21 = 5, ng31 = 8, ng41 = 10, ng51 = 13, ng61 = 15;
                private static void MyGKInit()
                {
                    x15 = new double[15];
                    x21 = new double[21];
                    x31 = new double[31];
                    x41 = new double[41];
                    x51 = new double[51];
                    x61 = new double[61];
                    wgauss15 = new double[15];
                    wgauss21 = new double[21];
                    wgauss31 = new double[31];
                    wgauss41 = new double[41];
                    wgauss51 = new double[51];
                    wgauss61 = new double[61];
                    wkronrod15 = new double[15];
                    wkronrod21 = new double[21];
                    wkronrod31 = new double[31];
                    wkronrod41 = new double[41];
                    wkronrod51 = new double[51];
                    wkronrod61 = new double[61];

                    wgauss15[0] = 0.129484966168869693270611432679082;
                    wgauss15[1] = 0.279705391489276667901467771423780;
                    wgauss15[2] = 0.381830050505118944950369775488975;
                    wgauss15[3] = 0.417959183673469387755102040816327;
                    x15[0] = 0.991455371120812639206854697526329;
                    x15[1] = 0.949107912342758524526189684047851;
                    x15[2] = 0.864864423359769072789712788640926;
                    x15[3] = 0.741531185599394439863864773280788;
                    x15[4] = 0.586087235467691130294144838258730;
                    x15[5] = 0.405845151377397166906606412076961;
                    x15[6] = 0.207784955007898467600689403773245;
                    x15[7] = 0.000000000000000000000000000000000;
                    wkronrod15[0] = 0.022935322010529224963732008058970;
                    wkronrod15[1] = 0.063092092629978553290700663189204;
                    wkronrod15[2] = 0.104790010322250183839876322541518;
                    wkronrod15[3] = 0.140653259715525918745189590510238;
                    wkronrod15[4] = 0.169004726639267902826583426598550;
                    wkronrod15[5] = 0.190350578064785409913256402421014;
                    wkronrod15[6] = 0.204432940075298892414161999234649;
                    wkronrod15[7] = 0.209482141084727828012999174891714;

                    wgauss21[0] = 0.066671344308688137593568809893332;
                    wgauss21[1] = 0.149451349150580593145776339657697;
                    wgauss21[2] = 0.219086362515982043995534934228163;
                    wgauss21[3] = 0.269266719309996355091226921569469;
                    wgauss21[4] = 0.295524224714752870173892994651338;
                    x21[0] = 0.995657163025808080735527280689003;
                    x21[1] = 0.973906528517171720077964012084452;
                    x21[2] = 0.930157491355708226001207180059508;
                    x21[3] = 0.865063366688984510732096688423493;
                    x21[4] = 0.780817726586416897063717578345042;
                    x21[5] = 0.679409568299024406234327365114874;
                    x21[6] = 0.562757134668604683339000099272694;
                    x21[7] = 0.433395394129247190799265943165784;
                    x21[8] = 0.294392862701460198131126603103866;
                    x21[9] = 0.148874338981631210884826001129720;
                    x21[10] = 0.000000000000000000000000000000000;
                    wkronrod21[0] = 0.011694638867371874278064396062192;
                    wkronrod21[1] = 0.032558162307964727478818972459390;
                    wkronrod21[2] = 0.054755896574351996031381300244580;
                    wkronrod21[3] = 0.075039674810919952767043140916190;
                    wkronrod21[4] = 0.093125454583697605535065465083366;
                    wkronrod21[5] = 0.109387158802297641899210590325805;
                    wkronrod21[6] = 0.123491976262065851077958109831074;
                    wkronrod21[7] = 0.134709217311473325928054001771707;
                    wkronrod21[8] = 0.142775938577060080797094273138717;
                    wkronrod21[9] = 0.147739104901338491374841515972068;
                    wkronrod21[10] = 0.149445554002916905664936468389821;

                    wgauss31[0] = 0.030753241996117268354628393577204;
                    wgauss31[1] = 0.070366047488108124709267416450667;
                    wgauss31[2] = 0.107159220467171935011869546685869;
                    wgauss31[3] = 0.139570677926154314447804794511028;
                    wgauss31[4] = 0.166269205816993933553200860481209;
                    wgauss31[5] = 0.186161000015562211026800561866423;
                    wgauss31[6] = 0.198431485327111576456118326443839;
                    wgauss31[7] = 0.202578241925561272880620199967519;
                    x31[0] = 0.998002298693397060285172840152271;
                    x31[1] = 0.987992518020485428489565718586613;
                    x31[2] = 0.967739075679139134257347978784337;
                    x31[3] = 0.937273392400705904307758947710209;
                    x31[4] = 0.897264532344081900882509656454496;
                    x31[5] = 0.848206583410427216200648320774217;
                    x31[6] = 0.790418501442465932967649294817947;
                    x31[7] = 0.724417731360170047416186054613938;
                    x31[8] = 0.650996741297416970533735895313275;
                    x31[9] = 0.570972172608538847537226737253911;
                    x31[10] = 0.485081863640239680693655740232351;
                    x31[11] = 0.394151347077563369897207370981045;
                    x31[12] = 0.299180007153168812166780024266389;
                    x31[13] = 0.201194093997434522300628303394596;
                    x31[14] = 0.101142066918717499027074231447392;
                    x31[15] = 0.000000000000000000000000000000000;
                    wkronrod31[0] = 0.005377479872923348987792051430128;
                    wkronrod31[1] = 0.015007947329316122538374763075807;
                    wkronrod31[2] = 0.025460847326715320186874001019653;
                    wkronrod31[3] = 0.035346360791375846222037948478360;
                    wkronrod31[4] = 0.044589751324764876608227299373280;
                    wkronrod31[5] = 0.053481524690928087265343147239430;
                    wkronrod31[6] = 0.062009567800670640285139230960803;
                    wkronrod31[7] = 0.069854121318728258709520077099147;
                    wkronrod31[8] = 0.076849680757720378894432777482659;
                    wkronrod31[9] = 0.083080502823133021038289247286104;
                    wkronrod31[10] = 0.088564443056211770647275443693774;
                    wkronrod31[11] = 0.093126598170825321225486872747346;
                    wkronrod31[12] = 0.096642726983623678505179907627589;
                    wkronrod31[13] = 0.099173598721791959332393173484603;
                    wkronrod31[14] = 0.100769845523875595044946662617570;
                    wkronrod31[15] = 0.101330007014791549017374792767493;

                    wgauss41[0] = 0.017614007139152118311861962351853;
                    wgauss41[1] = 0.040601429800386941331039952274932;
                    wgauss41[2] = 0.062672048334109063569506535187042;
                    wgauss41[3] = 0.083276741576704748724758143222046;
                    wgauss41[4] = 0.101930119817240435036750135480350;
                    wgauss41[5] = 0.118194531961518417312377377711382;
                    wgauss41[6] = 0.131688638449176626898494499748163;
                    wgauss41[7] = 0.142096109318382051329298325067165;
                    wgauss41[8] = 0.149172986472603746787828737001969;
                    wgauss41[9] = 0.152753387130725850698084331955098;
                    x41[0] = 0.998859031588277663838315576545863;
                    x41[1] = 0.993128599185094924786122388471320;
                    x41[2] = 0.981507877450250259193342994720217;
                    x41[3] = 0.963971927277913791267666131197277;
                    x41[4] = 0.940822633831754753519982722212443;
                    x41[5] = 0.912234428251325905867752441203298;
                    x41[6] = 0.878276811252281976077442995113078;
                    x41[7] = 0.839116971822218823394529061701521;
                    x41[8] = 0.795041428837551198350638833272788;
                    x41[9] = 0.746331906460150792614305070355642;
                    x41[10] = 0.693237656334751384805490711845932;
                    x41[11] = 0.636053680726515025452836696226286;
                    x41[12] = 0.575140446819710315342946036586425;
                    x41[13] = 0.510867001950827098004364050955251;
                    x41[14] = 0.443593175238725103199992213492640;
                    x41[15] = 0.373706088715419560672548177024927;
                    x41[16] = 0.301627868114913004320555356858592;
                    x41[17] = 0.227785851141645078080496195368575;
                    x41[18] = 0.152605465240922675505220241022678;
                    x41[19] = 0.076526521133497333754640409398838;
                    x41[20] = 0.000000000000000000000000000000000;
                    wkronrod41[0] = 0.003073583718520531501218293246031;
                    wkronrod41[1] = 0.008600269855642942198661787950102;
                    wkronrod41[2] = 0.014626169256971252983787960308868;
                    wkronrod41[3] = 0.020388373461266523598010231432755;
                    wkronrod41[4] = 0.025882133604951158834505067096153;
                    wkronrod41[5] = 0.031287306777032798958543119323801;
                    wkronrod41[6] = 0.036600169758200798030557240707211;
                    wkronrod41[7] = 0.041668873327973686263788305936895;
                    wkronrod41[8] = 0.046434821867497674720231880926108;
                    wkronrod41[9] = 0.050944573923728691932707670050345;
                    wkronrod41[10] = 0.055195105348285994744832372419777;
                    wkronrod41[11] = 0.059111400880639572374967220648594;
                    wkronrod41[12] = 0.062653237554781168025870122174255;
                    wkronrod41[13] = 0.065834597133618422111563556969398;
                    wkronrod41[14] = 0.068648672928521619345623411885368;
                    wkronrod41[15] = 0.071054423553444068305790361723210;
                    wkronrod41[16] = 0.073030690332786667495189417658913;
                    wkronrod41[17] = 0.074582875400499188986581418362488;
                    wkronrod41[18] = 0.075704497684556674659542775376617;
                    wkronrod41[19] = 0.076377867672080736705502835038061;
                    wkronrod41[20] = 0.076600711917999656445049901530102;

                    wgauss51[0] = 0.011393798501026287947902964113235;
                    wgauss51[1] = 0.026354986615032137261901815295299;
                    wgauss51[2] = 0.040939156701306312655623487711646;
                    wgauss51[3] = 0.054904695975835191925936891540473;
                    wgauss51[4] = 0.068038333812356917207187185656708;
                    wgauss51[5] = 0.080140700335001018013234959669111;
                    wgauss51[6] = 0.091028261982963649811497220702892;
                    wgauss51[7] = 0.100535949067050644202206890392686;
                    wgauss51[8] = 0.108519624474263653116093957050117;
                    wgauss51[9] = 0.114858259145711648339325545869556;
                    wgauss51[10] = 0.119455763535784772228178126512901;
                    wgauss51[11] = 0.122242442990310041688959518945852;
                    wgauss51[12] = 0.123176053726715451203902873079050;
                    x51[0] = 0.999262104992609834193457486540341;
                    x51[1] = 0.995556969790498097908784946893902;
                    x51[2] = 0.988035794534077247637331014577406;
                    x51[3] = 0.976663921459517511498315386479594;
                    x51[4] = 0.961614986425842512418130033660167;
                    x51[5] = 0.942974571228974339414011169658471;
                    x51[6] = 0.920747115281701561746346084546331;
                    x51[7] = 0.894991997878275368851042006782805;
                    x51[8] = 0.865847065293275595448996969588340;
                    x51[9] = 0.833442628760834001421021108693570;
                    x51[10] = 0.797873797998500059410410904994307;
                    x51[11] = 0.759259263037357630577282865204361;
                    x51[12] = 0.717766406813084388186654079773298;
                    x51[13] = 0.673566368473468364485120633247622;
                    x51[14] = 0.626810099010317412788122681624518;
                    x51[15] = 0.577662930241222967723689841612654;
                    x51[16] = 0.526325284334719182599623778158010;
                    x51[17] = 0.473002731445714960522182115009192;
                    x51[18] = 0.417885382193037748851814394594572;
                    x51[19] = 0.361172305809387837735821730127641;
                    x51[20] = 0.303089538931107830167478909980339;
                    x51[21] = 0.243866883720988432045190362797452;
                    x51[22] = 0.183718939421048892015969888759528;
                    x51[23] = 0.122864692610710396387359818808037;
                    x51[24] = 0.061544483005685078886546392366797;
                    x51[25] = 0.000000000000000000000000000000000;
                    wkronrod51[0] = 0.001987383892330315926507851882843;
                    wkronrod51[1] = 0.005561932135356713758040236901066;
                    wkronrod51[2] = 0.009473973386174151607207710523655;
                    wkronrod51[3] = 0.013236229195571674813656405846976;
                    wkronrod51[4] = 0.016847817709128298231516667536336;
                    wkronrod51[5] = 0.020435371145882835456568292235939;
                    wkronrod51[6] = 0.024009945606953216220092489164881;
                    wkronrod51[7] = 0.027475317587851737802948455517811;
                    wkronrod51[8] = 0.030792300167387488891109020215229;
                    wkronrod51[9] = 0.034002130274329337836748795229551;
                    wkronrod51[10] = 0.037116271483415543560330625367620;
                    wkronrod51[11] = 0.040083825504032382074839284467076;
                    wkronrod51[12] = 0.042872845020170049476895792439495;
                    wkronrod51[13] = 0.045502913049921788909870584752660;
                    wkronrod51[14] = 0.047982537138836713906392255756915;
                    wkronrod51[15] = 0.050277679080715671963325259433440;
                    wkronrod51[16] = 0.052362885806407475864366712137873;
                    wkronrod51[17] = 0.054251129888545490144543370459876;
                    wkronrod51[18] = 0.055950811220412317308240686382747;
                    wkronrod51[19] = 0.057437116361567832853582693939506;
                    wkronrod51[20] = 0.058689680022394207961974175856788;
                    wkronrod51[21] = 0.059720340324174059979099291932562;
                    wkronrod51[22] = 0.060539455376045862945360267517565;
                    wkronrod51[23] = 0.061128509717053048305859030416293;
                    wkronrod51[24] = 0.061471189871425316661544131965264;
                    wkronrod51[25] = 0.061580818067832935078759824240055;

                    wgauss61[0] = 0.007968192496166605615465883474674;
                    wgauss61[1] = 0.018466468311090959142302131912047;
                    wgauss61[2] = 0.028784707883323369349719179611292;
                    wgauss61[3] = 0.038799192569627049596801936446348;
                    wgauss61[4] = 0.048402672830594052902938140422808;
                    wgauss61[5] = 0.057493156217619066481721689402056;
                    wgauss61[6] = 0.065974229882180495128128515115962;
                    wgauss61[7] = 0.073755974737705206268243850022191;
                    wgauss61[8] = 0.080755895229420215354694938460530;
                    wgauss61[9] = 0.086899787201082979802387530715126;
                    wgauss61[10] = 0.092122522237786128717632707087619;
                    wgauss61[11] = 0.096368737174644259639468626351810;
                    wgauss61[12] = 0.099593420586795267062780282103569;
                    wgauss61[13] = 0.101762389748405504596428952168554;
                    wgauss61[14] = 0.102852652893558840341285636705415;
                    x61[0] = 0.999484410050490637571325895705811;
                    x61[1] = 0.996893484074649540271630050918695;
                    x61[2] = 0.991630996870404594858628366109486;
                    x61[3] = 0.983668123279747209970032581605663;
                    x61[4] = 0.973116322501126268374693868423707;
                    x61[5] = 0.960021864968307512216871025581798;
                    x61[6] = 0.944374444748559979415831324037439;
                    x61[7] = 0.926200047429274325879324277080474;
                    x61[8] = 0.905573307699907798546522558925958;
                    x61[9] = 0.882560535792052681543116462530226;
                    x61[10] = 0.857205233546061098958658510658944;
                    x61[11] = 0.829565762382768397442898119732502;
                    x61[12] = 0.799727835821839083013668942322683;
                    x61[13] = 0.767777432104826194917977340974503;
                    x61[14] = 0.733790062453226804726171131369528;
                    x61[15] = 0.697850494793315796932292388026640;
                    x61[16] = 0.660061064126626961370053668149271;
                    x61[17] = 0.620526182989242861140477556431189;
                    x61[18] = 0.579345235826361691756024932172540;
                    x61[19] = 0.536624148142019899264169793311073;
                    x61[20] = 0.492480467861778574993693061207709;
                    x61[21] = 0.447033769538089176780609900322854;
                    x61[22] = 0.400401254830394392535476211542661;
                    x61[23] = 0.352704725530878113471037207089374;
                    x61[24] = 0.304073202273625077372677107199257;
                    x61[25] = 0.254636926167889846439805129817805;
                    x61[26] = 0.204525116682309891438957671002025;
                    x61[27] = 0.153869913608583546963794672743256;
                    x61[28] = 0.102806937966737030147096751318001;
                    x61[29] = 0.051471842555317695833025213166723;
                    x61[30] = 0.000000000000000000000000000000000;
                    wkronrod61[0] = 0.001389013698677007624551591226760;
                    wkronrod61[1] = 0.003890461127099884051267201844516;
                    wkronrod61[2] = 0.006630703915931292173319826369750;
                    wkronrod61[3] = 0.009273279659517763428441146892024;
                    wkronrod61[4] = 0.011823015253496341742232898853251;
                    wkronrod61[5] = 0.014369729507045804812451432443580;
                    wkronrod61[6] = 0.016920889189053272627572289420322;
                    wkronrod61[7] = 0.019414141193942381173408951050128;
                    wkronrod61[8] = 0.021828035821609192297167485738339;
                    wkronrod61[9] = 0.024191162078080601365686370725232;
                    wkronrod61[10] = 0.026509954882333101610601709335075;
                    wkronrod61[11] = 0.028754048765041292843978785354334;
                    wkronrod61[12] = 0.030907257562387762472884252943092;
                    wkronrod61[13] = 0.032981447057483726031814191016854;
                    wkronrod61[14] = 0.034979338028060024137499670731468;
                    wkronrod61[15] = 0.036882364651821229223911065617136;
                    wkronrod61[16] = 0.038678945624727592950348651532281;
                    wkronrod61[17] = 0.040374538951535959111995279752468;
                    wkronrod61[18] = 0.041969810215164246147147541285970;
                    wkronrod61[19] = 0.043452539701356069316831728117073;
                    wkronrod61[20] = 0.044814800133162663192355551616723;
                    wkronrod61[21] = 0.046059238271006988116271735559374;
                    wkronrod61[22] = 0.047185546569299153945261478181099;
                    wkronrod61[23] = 0.048185861757087129140779492298305;
                    wkronrod61[24] = 0.049055434555029778887528165367238;
                    wkronrod61[25] = 0.049795683427074206357811569379942;
                    wkronrod61[26] = 0.050405921402782346840893085653585;
                    wkronrod61[27] = 0.050881795898749606492297473049805;
                    wkronrod61[28] = 0.051221547849258772170656282604944;
                    wkronrod61[29] = 0.051426128537459025933862879215781;
                    wkronrod61[30] = 0.051494729429451567558340433647099;

                    int n = 15, ng = ng15;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x15[i] = -x15[n - 1 - i];
                        wkronrod15[i] = wkronrod15[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss15[n - 2 - 2 * i] = wgauss15[i];
                        wgauss15[1 + 2 * i] = wgauss15[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss15[2 * i] = 0;
                    }

                    n = 21; ng = ng21;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x21[i] = -x21[n - 1 - i];
                        wkronrod21[i] = wkronrod21[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss21[n - 2 - 2 * i] = wgauss21[i];
                        wgauss21[1 + 2 * i] = wgauss21[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss21[2 * i] = 0;
                    }

                    n = 31; ng = ng31;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x31[i] = -x31[n - 1 - i];
                        wkronrod31[i] = wkronrod31[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss31[n - 2 - 2 * i] = wgauss31[i];
                        wgauss31[1 + 2 * i] = wgauss31[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss31[2 * i] = 0;
                    }

                    n = 41; ng = ng41;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x41[i] = -x41[n - 1 - i];
                        wkronrod41[i] = wkronrod41[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss41[n - 2 - 2 * i] = wgauss41[i];
                        wgauss41[1 + 2 * i] = wgauss41[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss41[2 * i] = 0;
                    }

                    n = 51; ng = ng51;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x51[i] = -x51[n - 1 - i];
                        wkronrod51[i] = wkronrod51[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss51[n - 2 - 2 * i] = wgauss51[i];
                        wgauss51[1 + 2 * i] = wgauss51[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss51[2 * i] = 0;
                    }

                    n = 61; ng = ng61;
                    // copy nodes
                    for (int i = n - 1; i >= n / 2; i--)
                    {
                        x61[i] = -x61[n - 1 - i];
                        wkronrod61[i] = wkronrod61[n - 1 - i];
                    }
                    // copy Gauss weights
                    for (int i = ng - 1; i >= 0; i--)
                    {
                        wgauss61[n - 2 - 2 * i] = wgauss61[i];
                        wgauss61[1 + 2 * i] = wgauss61[i];
                    }
                    for (int i = 0; i <= n / 2; i++)
                    {
                        wgauss61[2 * i] = 0;
                    }


                    _x15 = GetNew(x15);
                    _x21 = GetNew(x21);
                    _x31 = GetNew(x31);
                    _x41 = GetNew(x41);
                    _x51 = GetNew(x51);
                    _x61 = GetNew(x61);
                    _wgauss15 = GetNew(wgauss15);
                    _wgauss21 = GetNew(wgauss21);
                    _wgauss31 = GetNew(wgauss31);
                    _wgauss41 = GetNew(wgauss41);
                    _wgauss51 = GetNew(wgauss51);
                    _wgauss61 = GetNew(wgauss61);
                    _wkronrod15 = GetNew(wkronrod15);
                    _wkronrod21 = GetNew(wkronrod21);
                    _wkronrod31 = GetNew(wkronrod31);
                    _wkronrod41 = GetNew(wkronrod41);
                    _wkronrod51 = GetNew(wkronrod51);
                    _wkronrod61 = GetNew(wkronrod61);
                }
                /// <summary>
                /// Взятый с alglib метод Гаусса-Кронрода с выбором числа точек
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="n"></param>
                /// <returns></returns>
                public static Complex MySimpleGaussKronrod(ComplexFunc f, Complex a, Complex b, int n = 61, bool ChooseStepByCompareRes = false, int MaxDivCount = 3, bool parallel = false)
                {
                    double[] x;
                    double[] wkronrod;
                    double[] wgauss;

                    //if (!(n == 15 | n == 21 | n == 31 | n == 41 | n == 51 | n == 61)) throw new Exception("GKQNodesTbl: incorrect N!");

                    switch (n)
                    {
                        case 61:
                            x = x61;
                            wkronrod = wkronrod61;
                            wgauss = wgauss61;
                            break;
                        case 15:
                            x = x15;
                            wkronrod = wkronrod15;
                            wgauss = wgauss15;
                            break;

                        case 31:
                            x = x31;
                            wkronrod = wkronrod31;
                            wgauss = wgauss31;
                            break;
                        case 41:
                            x = x41;
                            wkronrod = wkronrod41;
                            wgauss = wgauss41;
                            break;
                        case 51:
                            x = x15;
                            wkronrod = wkronrod51;
                            wgauss = wgauss51;
                            break;
                        case 21:
                            x = x21;
                            wkronrod = wkronrod21;
                            wgauss = wgauss21;
                            break;
                        default:
                            throw new Exception("GKQNodesTbl: incorrect N! (n должно быть 15/21/31/41/51/61)");
                    }

                    ComplexFunc t = (Complex r) => (a + (r + 1) / 2 * (b - a));
                    Complex sumKR = new Complex(0), sumGS = new Complex(0);
                    if (!parallel)
                        for (int i = 0; i < n; i++)
                        {
                            Complex tmp = f(t(x[i]));
                            sumKR += wkronrod[i] * tmp;
                            sumGS += wgauss[i] * tmp;
                        }
                    else
                    {
                        Complex[] sumsK = new Complex[n], sumsG = new Complex[n];
                        Parallel.For(0, n, (int i) =>
                        {
                            Complex tmp = f(t(x[i]));
                            sumsK[i] = wkronrod[i] * tmp;
                            sumsG[i] += wgauss[i] * tmp;
                        });
                        sumKR = sumsK.Sum();
                        sumGS = sumsG.Sum();
                    }


                    if (!ChooseStepByCompareRes) return sumKR / 2 * (b - a);

                    if (MaxDivCount > 0 && (sumGS - sumKR).Abs > /*sumKR.Abs / 1000*/1e-8)
                        return MySimpleGaussKronrod(f, a, a + (b - a) / 2, n, true, MaxDivCount - 1, parallel) + MySimpleGaussKronrod(f, a + (b - a) / 2, b, n, true, MaxDivCount - 1, parallel);
                    else
                        return sumKR / 2 * (b - a);
                }
                /// <summary>
                /// Взятый с alglib метод Гаусса-Кронрода с выбором числа точек
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="n"></param>
                /// <returns></returns>
                public static double MySimpleGaussKronrod(Func<double, double> f, double a, double b, int n = 61, bool ChooseStepByCompareRes = false, int MaxDivCount = 3, bool parallel = false)
                {
                    return MySimpleGaussKronrod((Complex t) => f(t.Re), new Complex(a), new Complex(b), n, ChooseStepByCompareRes, MaxDivCount, parallel).Re;
                }
                /// <summary>
                /// Метод Гаусса-Кронрода с выбором числа точек
                /// </summary>
                /// <param name="f">Интегрируемая функция</param>
                /// <param name="a">Начало отрезка интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="n">Число точек в методе Гаусса-Кронрода</param>
                /// <param name="ChooseStepByCompareRes">Требуется ли пересчитывать с меньшими отрезками, если разница между методами Гаусса и Гаусса-Кронрода существенная</param>
                /// <param name="MaxDivCount">Сколько ещё раз можно поделить отрезок (максимальное число делений)</param>
                /// <returns></returns>
                public static CVectors MySimpleGaussKronrod(Func<Complex, CVectors> f, Complex a, Complex b, int n = 61, bool ChooseStepByCompareRes = true, int MaxDivCount = 3)
                {
                    double[] x;
                    double[] wkronrod;
                    double[] wgauss;

                    //if (!(n == 15 | n == 21 | n == 31 | n == 41 | n == 51 | n == 61)) throw new Exception("GKQNodesTbl: incorrect N!");

                    switch (n)
                    {
                        case 61:
                            x = x61;
                            wkronrod = wkronrod61;
                            wgauss = wgauss61;
                            break;
                        case 15:
                            x = x15;
                            wkronrod = wkronrod15;
                            wgauss = wgauss15;
                            break;

                        case 31:
                            x = x31;
                            wkronrod = wkronrod31;
                            wgauss = wgauss31;
                            break;
                        case 41:
                            x = x41;
                            wkronrod = wkronrod41;
                            wgauss = wgauss41;
                            break;
                        case 51:
                            x = x15;
                            wkronrod = wkronrod51;
                            wgauss = wgauss51;
                            break;
                        case 21:
                            x = x21;
                            wkronrod = wkronrod21;
                            wgauss = wgauss21;
                            break;
                        default:
                            throw new Exception("GKQNodesTbl: incorrect N! (n должно быть 15/21/31/41/51/61)");
                    }


                    ComplexFunc t = (Complex r) => (a + (r + 1) / 2 * (b - a));
                    CVectors sumKR = new CVectors(f((a + b) / 2).Degree), sumGS = new CVectors(sumKR.Degree);
                    for (int i = 0; i < n; i++)
                    {
                        CVectors tmp = f(t(x[i])); if (tmp == null) tmp.Show();
                        sumKR += wkronrod[i] * tmp;
                        sumGS += wgauss[i] * tmp;
                    }
                    if (!ChooseStepByCompareRes)
                        return sumKR / 2 * (b - a);

                    if (MaxDivCount > 0 && (sumGS - sumKR).Abs > sumKR.Abs / 1000)
                        return MySimpleGaussKronrod(f, a, a + (b - a) / 2, n, true, MaxDivCount - 1) + MySimpleGaussKronrod(f, a + (b - a) / 2, b, n, true, MaxDivCount - 1);
                    else
                        return sumKR / 2 * (b - a);
                }

                /// <summary>
                /// Метод Гаусса-Кронрода, который вместо отрезка делает обход контура, если на отрезке есть полюса
                /// </summary>
                /// <param name="f">Интегрируемая функция</param>
                /// <param name="a">Начало отрезка интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="delta">Знаменатель интегрируемой функции</param>
                /// <param name="t">Предполагаемый радиус отрезка, вне которого полюсов нет</param>
                /// <param name="n">Число узлов интегрировани</param>
                /// <param name="h">Отклонение контура</param>
                /// <returns></returns>
                public static Complex MySuperGaussKronrod(ComplexFunc f, Complex a, Complex b, ComplexFunc delta = null, int t = 100, int n = 61, double h = 0.2)
                {
                    if (delta == null || Math.Max(b.Re, -a.Re) > t) return MySimpleGaussKronrod(f, a, b, n);//если знаменатель неизвестен либо отрезок вне зоны нахождения предполагаемых полюсов, интегрировать по-обычному
                    if (a.Im != 0 && a.Im != b.Im) return MySimpleGaussKronrod(f, a, b, n);//если отрезок не на вещественной оси, интегрировать
                    if (a.Re * b.Re < 0 || delta(0) != 0) return MySuperGaussKronrod(f, a, 0, delta, t, n, h) + MySuperGaussKronrod(f, 0, b, delta, t, n, h);//если концы отрезка по обе стороны от 0, разбить на 2
                    var tmp = Optimization.Neu(delta, a, b);//tmp.Show();//найти корни на отрезке (надо знать, существуют или нет)
                    if (a.Re < 0) h *= -1;//если отрезок слева от 0, брать обход снизу
                    if (tmp == null) return MySimpleGaussKronrod(f, a, b, n);//если нет полюсов, решать по-обычному
                    else
                        return MySimpleGaussKronrod(f, a, a + h, n) + MySimpleGaussKronrod(f, a + h, b + h, n) + MySimpleGaussKronrod(f, b + h, b, n);//иначе обойти контур
                }

                /// <summary>
                /// Метод Гаусса-Кронрода, использующий параллельные вычисления за счёт разбиения отрезка интегрирования на несколько частей
                /// </summary>
                /// <param name="f">Интегрируемая функция</param>
                /// <param name="a">Начало отрезка интегрирования</param>
                /// <param name="b">Конец отрезка интегрирования</param>
                /// <param name="n">Число точек в методе</param>
                /// <param name="count">На сколько отрезков разбиватся исходный отрезок</param>
                /// <returns></returns>
                public static Complex ParallelGaussKronrod(ComplexFunc f, Complex a, Complex b, int n = 61, int count = 5)
                {
                    Complex[] mas = new Complex[count];
                    Complex step = (b - a) / (count - 1);
                    Parallel.For(1, count, i => mas[i] = MySimpleGaussKronrod(f, a + (i - 1) * step, a + i * step, n));
                    Array.Sort(mas);
                    Complex sum = 0;
                    for (int i = 0; i < count; i++)
                        sum += mas[i];
                    return sum;
                }

                /// <summary>
                /// Взятый с alglib метод Гаусса-Кронрода с выбором числа точек
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="n"></param>
                /// <returns></returns>
                public static Complex GaussKronrodSum(ComplexFunc f, Complex a, Complex b, int n = 61, int count = 50)
                {
                    Complex h = (b - a) / (count - 1);
                    Complex sum = 0;
                    for (int i = 1; i < count; i++)
                    {
                        sum += MySimpleGaussKronrod(f, a + h * (i - 1), a + i * h, n);
                    }
                    return sum;
                }
                /// <summary>
                /// Взятый с alglib метод Гаусса-Кронрода с выбором числа точек
                /// </summary>
                /// <param name="f"></param>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="n"></param>
                /// <returns></returns>
                public static double GaussKronrodSum(Func<double, double> f, double a, double b, int n = 61, int count = 50)
                {
                    return GaussKronrodSum((Complex t) => f(t.Re), new Complex(a), new Complex(b), n, count).Re;
                }
            }

            private delegate double FUNC(Func<double, double> f, double a, double b);
            /// <summary>
            /// Подсчёт определённого интеграла выбранными методом и относительно выбранного критерия
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="a">Начало отрезка интегрирования</param>
            /// <param name="b">Конец отрезка интегрирования</param>
            /// <param name="M">Метод подсчёта интеграла</param>
            /// <param name="C">Критерий подсчёта интеграла (указанное число шагов/точность/разбиение интеграла на сумму интегралов по нескольким частям отрезка интегрирования)</param>
            /// <param name="count">Число шагов при интегрировании</param>
            /// <param name="eps">Точность интеграла</param>
            /// <param name="seqcount">На сколько частей разбивается отрезок</param>
            /// <returns></returns>
            public static double DefIntegral(Func<double, double> f, double a, double b, DefInteg.Method M, DefInteg.Criterion C, int count = 15, double eps = 0.0001, int seqcount = 2, bool GetEPS = false, int n = 15)
            {
                //GaussKronrod.ChooseGK(GaussKronrod.NodesCount.GK15);

                DefInteg.h_Count = 5000;
                DefInteg.n = 20;
                DefInteg.EPS = STEP / 200;

                FUNC Met, TMP = (Func<double, double> q, double w, double e) => GaussKronrod.MySimpleGaussKronrod(q, w, e, n), TMP2 = (Func<double, double> q, double w, double e) => GaussKronrod.MySimpleGaussKronrod(q, w, e, 61);
                FUNC Gmet = (Func<double, double> q, double w, double e) => { return GaussKronrod.Integral(q, w, e); };
                FUNC GEmp = (Func<double, double> q, double w, double e) => { return GaussKronrod.MySimpleGaussKronrod(q, w, e, 61, true); };
                FUNC GS = (Func<double, double> q, double w, double e) => { return GaussKronrod.GaussKronrodSum(q, w, e, 61, 2); };

                switch (M)
                {
                    case Method.MiddleRect:
                        Met = MiddleRect;
                        break;
                    case Method.Trapez:
                        Met = Trapez;
                        break;
                    case Method.Simpson:
                        Met = Simpson;
                        break;
                    case Method.Meler:
                        Met = Meler;
                        break;
                    case Method.GaussKronrod15:
                        //Met = GaussKronrod.Integral;
                        Met = TMP;
                        break;
                    case Method.GaussKronrod61:
                        //Met = GaussKronrod.Integral;
                        Met = TMP2;
                        break;
                    case Method.GaussKronrod61fromFortran:
                        Met = Gmet;
                        break;
                    case Method.GaussKronrod61Empire:
                        Met = GEmp;
                        break;
                    case Method.GaussKronrod61Sq:
                        Met = GS;
                        break;
                    default: //Method.Gauss:
                        Met = Gauss;
                        break;
                }
                if (C == Criterion.StepCount)
                {
                    if (M == Method.Gauss || M == Method.Meler)
                    {
                        DefInteg.n = count;
                        DefInteg.h_Count = DefInteg.n;
                        DefInteg.EPS = Double.NaN;
                        return Met(f, a, b);
                    }
                    DefInteg.h_Count = count;
                    //DefInteg.h_Count *= 2;
                    double _I1 = Met(f, a, b);
                    if (GetEPS)
                    {
                        DefInteg.h_Count *= 2;
                        double _I2 = Met(f, a, b);
                        DefInteg.EPS = Math.Abs(_I1 - _I2);
                        DefInteg.h_Count /= 2/*4*/;
                    }
                    return _I1;
                }
                if (C == Criterion.Accuracy)
                {
                    if (M == Method.Gauss || M == Method.Meler)
                    {
                        DefInteg.EPS = eps;
                        DefInteg.n = 20;
                        //DefInteg.n += 5;
                        double I11 = Met(f, a, b);
                        DefInteg.n += 5;
                        double I22 = Met(f, a, b);

                        DefInteg.n = 20;
                        while (Math.Abs(I11 - I22) >= DefInteg.EPS)
                        {
                            DefInteg.n += 5;
                            //DefInteg.n += 5;
                            I11 = Met(f, a, b);
                            DefInteg.n += 5;
                            I22 = Met(f, a, b);
                            DefInteg.n -= 5/*10*/;
                        }

                        //DefInteg.n += 5/*10*/;
                        DefInteg.h_Count = DefInteg.n;
                        DefInteg.EPS = Math.Abs(I11 - I22);
                        return I11;
                    }

                    DefInteg.EPS = eps;
                    DefInteg.h_Count = count;
                    //DefInteg.h_Count *= 2;
                    double I1 = Met(f, a, b);
                    DefInteg.h_Count *= 2;
                    double I2 = Met(f, a, b);

                    DefInteg.h_Count = count;
                    while (Math.Abs(I1 - I2) >= DefInteg.EPS)
                    {
                        DefInteg.h_Count *= 2;
                        //DefInteg.h_Count *= 2;
                        I1 = Met(f, a, b);
                        DefInteg.h_Count *= 2;
                        I2 = Met(f, a, b);
                        DefInteg.h_Count /= 2/*4*/;
                    }

                    //DefInteg.h_Count *= 4;
                    return I1;
                }
                if (C == Criterion.SegmentCount)
                {
                    double h = (b - a) / seqcount, s = 0;
                    DefInteg.h_Count = count;
                    DefInteg.n = count; DefInteg.EPS = Double.NaN;
                    for (int i = 1; i <= seqcount; i++) s += Met(f, a + (i - 1) * h, a + i * h);
                    return s;
                }
                return 0;
            }
            /// <summary>
            /// Подсчёт кратного интеграла
            /// </summary>
            /// <param name="f">Функционал под интегралом</param>
            /// <param name="c">Кривая, являющаяся границей области интегрирования</param>
            /// <param name="S">Функция трёх переменных, задающая зависимость площади куска от параметров tx, ty и радиуса</param>
            /// <param name="M">Метод обычного интегрирования</param>
            /// <param name="C">Критерий интегрирования</param>
            /// <param name="count">Число шагов (тоже не требуется)</param>
            /// <param name="eps">Точность (требуется при определённом критерии интегрирования)</param>
            /// <param name="parallel">Требуется ли считать интегралы по кольцам параллельно</param>
            /// <param name="tx">Шаг "по кольцу"</param>
            /// <param name="ty">Шаг "по радиусу фигуры"</param>
            /// <returns></returns>
            public static double DoubleIntegral(Functional f, Curve c, TripleFunc S, DefInteg.Method M = DefInteg.Method.Simpson, double tx = 0.01, double ty = 0.01, DefInteg.Criterion C = DefInteg.Criterion.StepCount, int count = 20, double eps = 0.001, bool parallel = true, bool makesort = true, double rmin = 0)
            {
                double sum = 0;//окончательный результат
                int countk = (int)Math.Floor((c.BaseRadius - rmin) / ty);//число колец
                //if (countk < 1) return 0;
                if (!parallel)
                    for (int k = 0; k < countk; k++)//проход по кольцам
                    {
                        double rk = c.BaseRadius - ty * (k + 0.5) + rmin;//радиус кольца (контура посередине)
                        double lk = c.End(rk)/*2 * Math.PI * rk*/;//"длина" кольца (требуется для подсчёта числа шагов при поиске определённого интеграла)
                        Curve cc = new Curve(c.a, c.a + c.End(rk), c.U, c.V, rk);//кривая, по которой будет интегрирование
                        Func<double, double> tmp = (double x) => f(cc.Transfer(x));//отображение кольца в отрезок, который отображается в действительную функцию
                        double s = DefIntegral(tmp, cc.a, cc.b, M, C, (int)Math.Floor(lk / tx));//интеграл по отрезку(кругу) с нужным числом шагов                
                        if (M != DefInteg.Method.GaussKronrod15 && M != DefInteg.Method.GaussKronrod61 && M != DefInteg.Method.GaussKronrod61fromFortran)//если метод требует разбиения отрезка интегрирования на части
                            sum += s * (S(tx, ty, rk) - S(tx, ty, rmin)) / tx;//умножение интеграла на отношение площади к шагу
                        else sum += s * (S(cc.b - cc.a, ty, rk) - S(cc.b - cc.a, ty, rmin)) / (cc.b - cc.a);//если интегрирования проводится сразу по всему кольцу
                    }
                else//такие же вычисления, но параллельно
                {
                    double[] mas = new double[countk];
                    Parallel.For(0, countk, (int k) =>
                      {
                          double rk = c.BaseRadius - ty * (k + 0.5) + rmin;//радиус кольца (окружности посередине)
                          double lk = c.End(rk)/*2 * Math.PI * rk*/;//длина кольца
                          Curve cc = new Curve(c.a, c.a + c.End(rk), c.U, c.V, rk);//кривая, по которой будет интегрирование
                          Func<double, double> tmp = (double x) => f(cc.Transfer(x));//отображение кольца в отрезок, который отображается в действительную функцию
                          double s = DefIntegral(tmp, cc.a, cc.b, M, C, (int)Math.Floor(lk / tx));//интеграл по отрезку(кругу) с нужным числом шагов
                          if (M != DefInteg.Method.GaussKronrod15 && M != DefInteg.Method.GaussKronrod61 && M != DefInteg.Method.GaussKronrod61fromFortran)
                              mas[k] = s * (S(tx, ty, rk) - S(tx, ty, rmin)) / tx;//умножение интеграла на отношение площади к шагу                        
                          else mas[k] = s * (S(cc.b - cc.a, ty, rk) - S(cc.b - cc.a, ty, rmin)) / (cc.b - cc.a);
                          //mas[k].Show();
                          //(S(tx, ty, rk) - S(tx, ty, rmin)).Show();
                      });

                    if (makesort) Array.Sort(mas, new Expendator.Compar());
                    sum = mas.Sum();
                }
                return sum;
            }
            /// <summary>
            /// Подсчёт кратного интеграла
            /// </summary>
            /// <param name="f">Функционал под интегралом</param>
            /// <param name="c">Граница области интегрирования</param>
            /// <param name="S">Функция, определяющая площадь сегмента</param>
            /// <param name="M">Метод интегрирования</param>
            /// <param name="tx">Шаг по кольцу</param>
            /// <param name="cy">Число колец</param>
            /// <param name="C">Критерий интегрирования</param>
            /// <param name="count"></param>
            /// <param name="eps"></param>
            /// <returns></returns>
            public static double DoubleIntegral(Functional f, Curve c, TripleFunc S, DefInteg.Method M = DefInteg.Method.Simpson, double tx = 0.01, int cy = 15, DefInteg.Criterion C = DefInteg.Criterion.StepCount, int count = 20, double eps = 0.001, bool parallel = true, bool makesort = true, double rmin = 0)
            {
                double ty = c.BaseRadius / cy;//ty.Show();
                                              //Math.Floor(1.03).Show();
                return DoubleIntegral(f, c, S, M, tx, ty, C, count, eps, parallel, makesort, rmin);
            }
            /// <summary>
            /// Подсчёт двойного интеграла на области, описываемой кривой при бесконечном радиусе
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="c">Кривая</param>
            /// <param name="S">Функция площади</param>
            /// <param name="M">Метод обычного интегрирования</param>
            /// <param name="tx">Шаг внутри кольца</param>
            /// <param name="cy">Число колец</param>
            /// <param name="C">Критерий интегрирования</param>
            /// <param name="count">Число шагов</param>
            /// <param name="eps">Погрешность</param>
            /// <param name="parallel">Нужно ли распараллеливание</param>
            /// <param name="makesort">Нужна ли сортировка данных</param>
            /// <param name="Rstep">Шаг по радиусу</param>
            /// <returns></returns>
            /// <remarks>Тестирование на вейвлетах показало, что увеличение радиуса колец увеличивает точность вычислений на доли процентов, зато время работы увеличивается на 20-30%, то есть вариант неоптимален</remarks>
            public static double DoubleIntegralInf(Functional f, Curve c, TripleFunc S, DefInteg.Method M = DefInteg.Method.GaussKronrod61, double tx = 0.01, int cy = 100, DefInteg.Criterion C = DefInteg.Criterion.StepCount, int count = 20, double eps = 1e-8, bool parallel = true, bool makesort = true, double Rstep = 3, double Rmax = Double.PositiveInfinity, int min_iter = 1, int changestepcount = 0)
            {
                List<double> tmp = new List<double>(30);
                double i = 1;
                int mk = 0;
                do
                {
                    while (true)
                    {
                        c.BaseRadius = i * Rstep;
                        tmp.Add(DoubleIntegral(f, c, c.S, M, tx, cy, C, count, eps, parallel, makesort, (i - 1) * Rstep));
                        if (i >= min_iter)
                            if (tmp.Last().Abs() < eps || i * Rstep >= Rmax) break;
                        i++;
                    }
                    mk++;
                    Rstep *= i;
                    //cy *= 2;
                    i = 2;
                }
                while (mk <= changestepcount + 1);

                if (makesort) tmp.Sort(new Expendator.Compar());
                return tmp.Sum();
            }
            /// <summary>
            /// Подсчёт двойного интеграла в правой полуплоскости
            /// </summary>
            /// <param name="f"></param>
            /// <param name="M"></param>
            /// <param name="tx"></param>
            /// <param name="cy"></param>
            /// <param name="C"></param>
            /// <param name="count"></param>
            /// <param name="eps"></param>
            /// <param name="parallel"></param>
            /// <param name="makesort"></param>
            /// <param name="Rstep"></param>
            /// <returns></returns>
            public static double DoubleIntegralIn_IandIV(Functional f, DefInteg.Method M = DefInteg.Method.GaussKronrod61, double tx = 0.01, int cy = 100, DefInteg.Criterion C = DefInteg.Criterion.StepCount, int count = 20, double eps = 1e-8, bool parallel = true, bool makesort = true, double Rstep = 0.1, double Rmax = Double.PositiveInfinity, int min_iter = 1, int changestepcount = 0)
            {
                Curve c = new Curve(-Math.PI / 2, Math.PI / 2, (double t, double r) => r * Math.Cos(t), (double t, double r) => r * Math.Sin(t), Rstep);
                c.S = (double x, double y, double r) => x * y * r;
                c.End = (double r) => Math.PI / 2;

                return DoubleIntegralInf(f, c, c.S, M, tx, cy, C, count, eps, parallel, makesort, Rstep, Rmax, min_iter, changestepcount);
            }
            /// <summary>
            /// Подсчёт двойного интеграла для всей плоскости
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="M">Метод интегрирования</param>
            /// <param name="tx">Шаг по кольцу</param>
            /// <param name="cy">Число колец</param>
            /// <param name="C">Критерий (не следует менять)</param>
            /// <param name="count">Число отрезков разбиения (не надо трогать)</param>
            /// <param name="eps">Точность</param>
            /// <param name="parallel">Вычислять ли параллельно</param>
            /// <param name="makesort">Делать ли сортировку перед суммированием</param>
            /// <param name="Rstep">Шаг по радиусу колец</param>
            /// <param name="Rmax">Максимальный радиус</param>
            /// <param name="min_iter">Минимальное число итераций, которые требуется выполнить</param>
            /// <param name="changestepcount">Сколько раз изменять шаг (изменять шаг непроизводительно)</param>
            /// <param name="a">Первая полуось эллипса</param>
            /// <param name="b">Вторая полуось эллипса</param>
            /// <returns></returns>
            public static double DoubleIntegralIn_FULL(Functional f, DefInteg.Method M = DefInteg.Method.GaussKronrod61, double tx = 0.01, int cy = 100, DefInteg.Criterion C = DefInteg.Criterion.StepCount, int count = 20, double eps = 1e-10, bool parallel = true, bool makesort = true, double Rstep = 0.1, double Rmax = Double.PositiveInfinity, int min_iter = 5, int changestepcount = 0, double a = 1, double b = 1)
            {
                b /= a;
                a = 1;

                Curve c = new Curve(0, 2 * Math.PI, (double t, double r) => a * r * Math.Cos(t), (double t, double r) => b * r * Math.Sin(t), Rstep);
                //c.u = (double t) => c.BaseRadius * Math.Cos(t);
                //c.v = (double t) => c.BaseRadius * Math.Sin(t);
                c.S = (double x, double y, double r) => x * y * r * a * b;
                c.End = (double r) => 2 * Math.PI;
                return DoubleIntegralInf(f, c, c.S, M, tx, cy, C, count, eps, parallel, makesort, Rstep, Rmax, min_iter, changestepcount);
            }

            /// <summary>
            /// Кратный интеграл по более точным квадратурам
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="c">Граница области интегрирования</param>
            /// <param name="S">Функция площади сегмента</param>
            /// <param name="M">Метод обычного интегрирования</param>
            /// <param name="cy">Число колец</param>
            /// <param name="rmin">Минимальный радиус</param>
            /// <param name="n">Число узлов в методе Гаусса-Кронрода для поиска криволинейного интеграла</param>
            /// <returns></returns>
            public static double DoubleIntegralSuper(Functional f, Curve c, TripleFunc S, DefInteg.Method M = Method.GaussKronrod61, int cy = 200, double rmin = 0, int n = 61)
            {
                double sum = 0;//окончательный результат
                int countk = cy;//число колец
                double ty = c.BaseRadius / cy;

                double[] mas = new double[countk];
                Func<double, double> curveint = (double r) =>
                 {
                     double rk = r;//радиус кольца (окружности посередине)
                            double lk = c.End(rk);//длина кольца
                            Curve cc = new Curve(c.a, c.a + c.End(rk), c.U, c.V, rk);//кривая, по которой будет интегрирование
                            Func<double, double> tmp = (double x) => f(cc.Transfer(x));//отображение кольца в отрезок, который отображается в действительную функцию
                            double s = GaussKronrod.MySimpleGaussKronrod(tmp, cc.a, cc.b, n); //DefIntegral(tmp, cc.a, cc.b, M, C, (int)Math.Floor(lk / tx));//интеграл по отрезку(кругу) с нужным числом шагов
                            return s / (cc.b - cc.a);
                 };

                Parallel.For(0, countk, (int k) =>
                {
                    double rk1 = c.BaseRadius - ty * (k) + rmin;
                    double rk2 = c.BaseRadius - ty * (k + 1) + rmin;
                    Curve cc1 = new Curve(c.a, c.a + c.End(rk1), c.U, c.V, rk1);
                    Curve cc2 = new Curve(c.a, c.a + c.End(rk2), c.U, c.V, rk2);

                    mas[k] = DefIntegral(curveint, rk1, rk2, M, Criterion.StepCount) / (rk1 - rk2) * (S(cc1.b - cc1.a, ty, rk1) - S(cc2.b - cc2.a, ty, rmin));
                });

                return mas.Sum();
            }

            /// <summary>
            /// Вариация метода Монте-Карло
            /// </summary>
            public enum MonteKarloEnum : byte
            {
                /// <summary>
                /// Обычный
                /// </summary>
                Usual,
                /// <summary>
                /// Геометрический
                /// </summary>
                Geo
            }
            /// <summary>
            /// Подсчёт определённого интеграла методом Монте-Карло
            /// </summary>
            /// <returns></returns>
            public static double MonteKarlo(MultiFunc F, MonteKarloEnum e = MonteKarloEnum.Usual, params Point[] p)
            {
                int c = DefInteg.n;//число точек
                double[][] tmp = new double[c][];//массив точек на области задания функции
                double[] mas = new double[c];//массив случайных значений функции
                double[][] masG = new double[c][];//массив точек
                double sum = 0, s = 1.0;
                Random rand = new Random(Environment.TickCount);

                for (int i = 0; i < c; i++)
                {
                    tmp[i] = GetRandomPoint(rand, p);
                    //Vectors v = new Vectors(tmp[i]);v.Show();
                    mas[i] = F(tmp[i]);
                    //Console.WriteLine(mas[i]);
                    //Random r = new Random(Environment.TickCount);
                    double t = Expendator.Min(F) + (Expendator.Max(F) - Expendator.Min(F)) * rand.NextDouble();

                    //Point[] t = new Point[p.Length + 1];
                    //t[p.Length] = new Point(Expendator.Min(F), Expendator.Max(F));
                    //for (int j = 0; j < p.Length; j++) t[j] = new Point(p[j]);
                    //masG[i] = GetRandomPoint(t);

                    masG[i] = new double[p.Length + 1];
                    masG[i][p.Length] = t;
                    for (int j = 0; j < p.Length; j++) masG[i][j] = tmp[i][j];
                    //Vectors v = new Vectors(masG[i]); v.Show(); Console.WriteLine("\tзначение функции---->"+mas[i]);
                }
                for (int i = 0; i < p.Length; i++)
                    s *= Math.Abs(p[i].x - p[i].y);

                switch (e)
                {
                    case MonteKarloEnum.Usual:
                        for (int i = 0; i < c; i++)
                            sum += mas[i];
                        sum /= c;
                        sum *= s;
                        return sum;
                    case MonteKarloEnum.Geo:
                        int k = 0;
                        for (int i = 0; i < c; i++)
                            if (masG[i][p.Length] <= mas[i]) k++;
                        sum = (double)(s * k) / c * Math.Abs(Expendator.Min(F) - Expendator.Max(F));
                        return sum;
                    default:
                        return 0;
                }
            }
            private static double[] GetRandomPoint(Random rand, params Point[] p)
            {
                double[] d = new double[p.Length];

                for (int i = 0; i < d.Length; i++)
                {
                    //double ra = (double)rand.Next(0, 1000) / 1000;
                    d[i] = p[i].x + (p[i].y - p[i].x) * /*ra*/rand.NextDouble();
                }
                return d;
            }

            /// <summary>
            /// Несобственный интеграл от минус бесконечности до бесконечности
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            public static double ImproperFirstKind(Func<double, double> f)
            {
                double t_max = 10, t_step = 1;
                double t = 1;//длина шага в сумме интегралов; когда t большое или маленькое, интеграл считается слишком неточно - единица более-менее подходит для начального шага
                double beg = 0, end = t, kp = Simpson(f, beg, end), km = Simpson(f, -end, -beg), sum = 0;

                for (; t < t_max; t += t_step)
                    do
                    {
                        beg += t; end += t;
                        sum += kp /*+ km*/;
                        kp = Simpson(f, beg, end);
                        //km = Simpson(f, -end, -beg);
                    } while ((kp > EPS) /*|| (km > EPS)*/);


                t = 1; beg = 0; end = t;
                for (; t < t_max; t += t_step)
                    do
                    {
                        beg += t; end += t;
                        sum += /*kp + */km;
                        //kp = Simpson(f, beg, end);
                        km = Simpson(f, -end, -beg);
                    } while (/*(kp > EPS) ||*/ (km > EPS));

                return sum;

            }
            /// <summary>
            /// Несобственный интеграл на отрезке от a до бесконечности
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <returns></returns>
            public static double ImproperFirstKindInf(Func<double, double> f, double a)
            {
                double t_max = 100, t_step = 1;
                double t = 1;//длина шага в сумме интегралов; когда t большое или маленькое, интеграл считается слишком неточно - единица более-менее подходит для начального шага
                double beg = a, end = a + t, kp = Simpson(f, beg, end), sum = 0;

                for (; t < t_max; t += t_step)
                    do
                    {
                        beg += t; end += t;
                        sum += kp;
                        kp = Simpson(f, beg, end);
                    } while (kp > EPS);

                return sum;
            }

            /// <summary>
            /// Демонстрация посчёта интегралов разными методами
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            public static void Demonstration(Func<double, double> f, double a, double b)
            {
                Console.WriteLine("Интеграл методом средних прямоугольников = \t" + MiddleRect(f, a, b));
                Console.WriteLine("Интеграл методом трапеций = \t" + Trapez(f, a, b));
                Console.WriteLine("Интеграл методом Симпсона = \t" + Simpson(f, a, b));
                Console.WriteLine("Интеграл методом Гаусса = \t" + Gauss(f, a, b));
                Console.WriteLine("Интеграл методом Мелера = \t" + Meler(f, a, b));
            }
            /// <summary>
            /// Демонстрация подсчёта кратных интегралов
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="c">Граница области интегрирования</param>
            /// <param name="S">Функция площади</param>
            /// <param name="pog">Массив погрешностей от разных методов</param>
            /// <param name="time">Массив временных затрат от разных методов</param>
            /// <param name="tx">Шаг интегрирования</param>
            /// <param name="cy">Шаг по кольцу</param>
            /// <param name="existsolve">Выводить на консоль интегралы (либо погрешности)</param>
            /// <param name="integ">Истинное значение интеграла</param>
            /// <param name="s">Строка, в которой записана интегрируемая функция</param>
            public static void Demonstration(Functional f, Curve c, TripleFunc S, out double[] pog, out TimeSpan[] time, double tx = 0.01, int cy = 15, bool existsolve = true, double integ = 0, string s = "")
            {
                $"\tРезультат подсчёта двойного интеграла при числе колец {cy} и шаге по кольцу {tx} для функции {s}: ".Show();
                double[] res = new double[6];
                time = new TimeSpan[6];
                DateTime t = DateTime.Now;
                res[0] = DoubleIntegral(f, c, S, Method.MiddleRect, tx, cy); time[0] = (DateTime.Now - t); t = DateTime.Now;
                res[1] = DoubleIntegral(f, c, S, Method.Trapez, tx, cy); time[1] = (DateTime.Now - t); t = DateTime.Now;
                res[2] = DoubleIntegral(f, c, S, Method.Simpson, tx, cy); time[2] = (DateTime.Now - t); t = DateTime.Now;
                res[3] = DoubleIntegral(f, c, S, Method.Meler, tx, cy); time[3] = (DateTime.Now - t); t = DateTime.Now;
                res[4] = DoubleIntegral(f, c, S, Method.GaussKronrod15, tx, cy); time[4] = (DateTime.Now - t); t = DateTime.Now;
                res[5] = DoubleIntegral(f, c, S, Method.GaussKronrod61, tx, cy); time[5] = (DateTime.Now - t); t = DateTime.Now;
                pog = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    pog[i] = Math.Abs(integ - res[i]);
                    //time[i] = time[i].Seconds;
                }


                if (existsolve)
                {
                    "-----------Отличие от истинного решения при подсчёте".Show();
                    Console.WriteLine($"Методом средних прямоугольников (время {time[0].TotalSeconds}) =\t" + pog[0]);
                    Console.WriteLine($"Методом трапеций (время {time[1].TotalSeconds}) =\t" + pog[1]);
                    Console.WriteLine($"Методом Симпсона (время {time[2].TotalSeconds}) =\t" + pog[2]);
                    Console.WriteLine($"Методом Мелера (время {time[3].TotalSeconds}) =\t" + pog[3]);
                    Console.WriteLine($"Методом Гаусса-Кронрода (время {time[4].TotalSeconds}) (15 точек) =\t" + pog[4]);
                    Console.WriteLine($"Методом Гаусса-Кронрода (время {time[5].TotalSeconds}) (61 точка) =\t" + pog[5]);
                }
                else
                {
                    Console.WriteLine($"Методом средних прямоугольников =\t" + res[0]);
                    Console.WriteLine($"Методом трапеций (время {time[1].TotalSeconds}) =\t" + res[1]);
                    Console.WriteLine($"Методом Симпсона (время {time[2].TotalSeconds}) =\t" + res[2]);
                    Console.WriteLine($"Методом Мелера (время {time[3].TotalSeconds}) =\t" + res[3]);
                    Console.WriteLine($"Методом Гаусса-Кронрода (время {time[4].TotalSeconds}) (15 точек) =\t" + res[4]);
                    Console.WriteLine($"Методом Гаусса-Кронрода (время {time[5].TotalSeconds}) (61 точка) =\t" + res[5]);
                }
                "".Show();
            }
            /// <summary>
            /// Оформление результатов кратного интегрирования разными методами в Excel
            /// </summary>
            /// <param name="f">Интегрируемая функция</param>
            /// <param name="c">Граница области</param>
            /// <param name="S">Функция площади</param>
            /// <param name="cy">Массив значений количества колец</param>
            /// <param name="tx">Шаг по кольцу</param>
            /// <param name="integ">Истинное значение интеграла</param>
            public static void DemostrationToExcel(Functional f, Curve c, TripleFunc S, int[] cy, double tx = 0.01, double integ = 0)
            {
                double[] pog, zeit;
                TimeSpan[] time;
                Vectors[] tmp = new Vectors[cy.Length];
                for (int i = 0; i < cy.Length; i++)
                {
                    Demonstration(f, c, S, out pog, out time, tx, cy[i], integ: integ);
                    zeit = new double[time.Length];
                    for (int j = 0; j < zeit.Length; j++)
                        zeit[j] = time[j].TotalSeconds;

                    double[] t = Expendator.Union(new double[] { cy[i] }, Vectors.Mix(new Vectors(zeit), new Vectors(pog)).DoubleMas);
                    tmp[i] = new Vectors(t);
                }
                string[] st = new string[] { "", "Средние прямоугольники", "", "Трапеции", "", "Симпсон", "", "Мелер", "", "Гаусс-Кронрод 15", "", "Гаусс-Кронрод 61", "" };
                ИнтеграцияСДругимиПрограммами.CreatTableInExcel(st, tmp);
            }

            /// <summary>
            /// Записать в файл результаты вычисления интегралов от разных функций на одной и той же кривой
            /// </summary>
            /// <param name="eps">Файл, куда будут записываться данные число колец-точность</param>
            /// <param name="epstime">Файл, куда будут записываться данные число колец-точность*время вычислений</param>
            /// <param name="f">Массив интегрируемых функций</param>
            /// <param name="c">Граница области интегрирования</param>
            /// <param name="S">Функция площади</param>
            /// <param name="cy">Массив числа колец</param>
            /// <param name="integ">Массив интегралов</param>
            public static void DemonstrationToFile(string eps, string epstime, Functional[] f, Curve c, TripleFunc S, int[] cy, double[] integ, DefInteg.Method M = DefInteg.Method.GaussKronrod61)
            {
                StreamWriter ep = new StreamWriter(eps);
                StreamWriter ept = new StreamWriter(epstime);

                double[] epss = new double[f.Length];
                double[] times = new double[f.Length];

                //проход по всем числам колец
                for (int i = 0; i < cy.Length; i++)
                {
                    ep.Write(cy[i]);
                    ept.Write(cy[i]);

                    //проход по всем функциям
                    for (int k = 0; k < f.Length; k++)
                    {
                        DateTime t = DateTime.Now;
                        //epss[k] = (DoubleIntegral(f[k], c, S, M,tx:0.01, cy: cy[i], parallel: true) - integ[k]).Abs();
                        epss[k] = (DoubleIntegralSuper(f[k], c, S, M, cy: cy[i], n: 61) - integ[k]).Abs();
                        times[k] = (DateTime.Now - t).Ticks * epss[k];

                        ep.Write(" " + ((Double.IsInfinity(Math.Log10(epss[k]))) ? "NA" : Math.Log10(epss[k]).ToString()));
                        ept.Write(" " + ((Double.IsInfinity(Math.Log10(times[k]))) ? "NA" : Math.Log10(times[k]).ToString()));
                    }
                    ep.WriteLine();
                    ept.WriteLine();
                }

                ep.Close();
                ept.Close();
            }

            /// <summary>
            /// Класс методов с вычетами
            /// </summary>
            public static class Residue
            {
                public static double eps = 1e-6;

                /// <summary>
                /// Производная функции со вторым порядком точности
                /// </summary>
                /// <param name="f"></param>
                /// <param name="z"></param>
                /// <returns></returns>
                public static Complex Derivative(ComplexFunc f, Complex z) => (f(z + eps) - f(z - eps)) / (2 * eps);

                /// <summary>
                /// Вычет в точке (простом полюсе) у функции, представимой в виде частного, где полюс есть нуль знаменателя
                /// </summary>
                /// <param name="g">Числитель функции</param>
                /// <param name="d">Знаменатель функции</param>
                /// <param name="z">Полюс (простой)</param>
                /// <returns></returns>
                private static Complex Res(ComplexFunc g, ComplexFunc d, Complex z) => g(z) / Derivative(d, z);
                /// <summary>
                /// Сумма вычетов функции по набору полюсов
                /// </summary>
                /// <param name="g">Числитель функции</param>
                /// <param name="d">Знаменатель функции</param>
                /// <param name="qe">Дополнительный множитель, не содержащий особенностей в заданных полюсах</param>
                /// <param name="mas">Массив полюсов</param>
                /// <returns></returns>
                public static Complex ResSum(ComplexFunc g, ComplexFunc d, ComplexFunc qe, Complex[] mas)
                {
                    //mas.Show();
                    Complex sum = 0;
                    for (int i = 0; i < mas.Length; i++)
                    {
                        sum += Res(g, d, mas[i]) * qe(mas[i]); //(qe(mas[i])).Show();
                    }
                    return sum;
                }
            }

            /// <summary>
            /// Кратный интеграл
            /// </summary>
            public class AreaForDoubleInteg
            {
                /// <summary>
                /// Верхний внешний предел
                /// </summary>
                private double ExternalLimitUp { get; set; }
                /// <summary>
                /// Нижний внешний предел
                /// </summary>
                private double ExternalLimitDown { get; set; }
                /// <summary>
                /// Верхний внутренний предел
                /// </summary>
                private Func<double, double> InternalLimitUp;
                /// <summary>
                /// Нижний внутренний предел
                /// </summary>
                private Func<double, double> InternalLimitDown;

                /// <summary>
                /// Конструктор по пределам интегрирования
                /// </summary>
                /// <param name="a"></param>
                /// <param name="b"></param>
                /// <param name="af"></param>
                /// <param name="bf"></param>
                public AreaForDoubleInteg(double a, double b, Func<double, double> af, Func<double, double> bf)
                {
                    ExternalLimitUp = b;
                    ExternalLimitDown = a;
                    InternalLimitDown = new Func<double, double>(af);
                    InternalLimitUp = new Func<double, double>(bf);
                }

                /// <summary>
                /// Вычислить кратный интеграл
                /// </summary>
                /// <param name="f"></param>
                /// <param name="method"></param>
                /// <param name="stepcount"></param>
                /// <returns></returns>
                public double DInteg(Functional f, Method method, int stepcount = 100)
                {
                    // Func<double,double> integ = (double x) => DefInteg.DefIntegral((double y) => f(new Point(x, y)), InternalLimitDown(x), InternalLimitUp(x), method, Criterion.StepCount, stepcount);
                    //ShowIn(0.1);
                    Func<double, double> integ = (double x) => GaussKronrod.MySimpleGaussKronrod(new Func<double, double>((double y) => f(new Point(x, y))), InternalLimitDown(x), InternalLimitUp(x), 61, true, stepcount);
                    return DefIntegral(integ, ExternalLimitDown, ExternalLimitUp, method, Criterion.StepCount, stepcount);
                }
                /// <summary>
                /// Вычислить кратный интеграл
                /// </summary>
                /// <param name="f"></param>
                /// <param name="method"></param>
                /// <param name="stepcount"></param>
                /// <returns></returns>
                public double DInteg(Functional f, int stepcount = 100, bool parallel = true)
                {
                    Func<double, double> integ = (double x) => GaussKronrod.MySimpleGaussKronrod((double y) => f(new Point(x, y)), InternalLimitDown(x), InternalLimitUp(x), 61, true, stepcount / 3);

                    //Func<double,double> integ = (double x) =>
                    //{
                    //    double s = 0;
                    //    double lim1 = InternalLimitDown(x),lim2=InternalLimitUp(x);
                    //    double step = (lim2 - lim1) / (stepcount - 1);
                    //    Func<double,double> ff=(double y) => f(new Point(x, y));

                    //    for (int i = 1; i < stepcount; i++)
                    //        s+=GaussKronrod.MySimpleGaussKronrod(ff,lim1+(i-1)*step ,lim1+i*step, 61, true, 2);
                    //    return s;
                    //};
                    //var func = new Memoize<double, double>((double t) => integ(t));
                    //Func<double,double> integ_ = (double t) => func.Value(t);

                    double res = 0;
                    double h = (ExternalLimitUp - ExternalLimitDown) / (stepcount - 1);
                    for (int i = 1; i < stepcount; i++)
                        res += GaussKronrod.MySimpleGaussKronrod(integ, ExternalLimitDown + h * (i - 1), ExternalLimitDown + h * (i), 61, false, 1, parallel);
                    return res;

                }

                private void ShowIn(double x) => $"a = {ExternalLimitDown} b = {ExternalLimitUp} y({x}) =[{InternalLimitDown(x)}; {InternalLimitUp(x)}]".Show();
            }
        }
    }
}

