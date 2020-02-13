using System;
using System.Collections.Generic;
using System.Linq;
using static МатКлассы.Number;

namespace МатКлассы
{
    /// <summary>
    /// Методы, связанные с функциями
    /// </summary>
    public static partial class FuncMethods
    {
        #region Внутренние функции
        /// <summary>
        /// Система мономов
        /// </summary>
        public static SequenceFunc Monoms = (double x, int i) => { return Math.Pow(x, i); };
        /// <summary>
        /// Система мономов как полиномы
        /// </summary>
        public static SequencePol Monom = (int i) => Graphs.Degree(i);

        /// <summary>
        /// Система многочленов Лежандра, ортогональных на отрезке
        /// </summary>
        public static SequenceFunc Lezhandrs(double a, double b)
        {
            return (double x, int k) =>
            {
                double s = (2 * x - 2 * a) / (b - a) - 1;
                Polynom p = Polynom.Lezh(k);
                //double s = ((x + 1) / 2 - a) / (b - a);
                //double s = (x * (b - a) + a) * 2 - 1;

                return p.Value(s);
            };
            //return (double x, int k) => Polynom.Lezh(k).Value(x*(b-a)+a);
        }
        /// <summary>
        /// Система многочленов Лежандра как многочленов, ортогональных на отрезке
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SequencePol Lezhandr(double a, double b)
        {
            //Polynom s = new Polynom(new double[] { (1.0/2-a)/(b-a),1.0/2/(b-a)});
            Polynom s = new Polynom(new double[] { -2 * a / (b - a) - 1, 2.0 / (b - a) });
            return (int k) =>
            {
                Polynom p = Polynom.Lezh(k);
                return p.Value(s);
            };
            //return (double x, int k) => Polynom.Lezh(k).Value(x*(b-a)+a);
        }

        /// <summary>
        /// Система многочленов Чебышева, ортогональных на отрезке
        /// </summary>
        public static SequenceFunc Chebs(double a, double b)
        {
            return (double x, int k) =>
            {
                double s = (2 * x - 2 * a) / (b - a) - 1;
                Polynom p = Polynom.Cheb(k);
                return p.Value(s);
            };
        }
        /// <summary>
        /// Система многочленов Чебышева как многочленов, ортогональных на отрезке
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SequencePol Cheb(double a, double b)
        {
            Polynom s = new Polynom(new double[] { -2 * a / (b - a) - 1, 2 / (b - a) });
            return (int k) =>
            {
                Polynom p = Polynom.Cheb(k);
                return p.Value(s);
            };
        }

        /// <summary>
        /// Система многочленов Лагерра, ортогональных на отрезке
        /// </summary>
        public static SequenceFunc Lagerrs(double a, double b) { return null; }
        /// <summary>
        /// Система многочленов Эрмита, ортогональных на отрезке
        /// </summary>
        public static SequenceFunc Hermits(double a, double b) { return null; }
        /// <summary>
        /// Тригонометрическая система, ортогональная на отрезке
        /// </summary>
        public static SequenceFunc TrigSystem(double a, double b)
        {
            double l = (b - a) / 2, s2l = Math.Sqrt(2 * l), sl = Math.Sqrt(l);
            return (double x, int k) =>
            {
                k++;
                if (k == 1) return 1.0 / s2l;
                if (k % 2 == 0) return Math.Cos(k / 2 * x * Math.PI / l) / sl;
                return Math.Sin((k - 1) / 2 * x * Math.PI / l) / sl;
            };
        }

        private static double Haar(double x, int k, int m)
        {
            double val = 0, sq = Math.Pow(2, m);
            if (x == 1) x -= 0.0000001;

            if (x >= (double)k / sq && x < ((double)k + 0.5) / sq) val = Math.Sqrt(sq);
            else if (x >= ((double)k + 0.5) / sq && x </*=*/ ((double)k + 1) / sq) val = -Math.Sqrt(sq);

            return val;
        }
        /// <summary>
        /// Система функций Хаара, ортогональных на отрезке
        /// </summary>
        public static SequenceFunc HaarSystem(double a, double b)
        {
            return (double t, int n) =>
            {
                double x = (t - a) / (b - a);
                n++;
                double val = 1;
                if (n >= 2)
                {
                    int m = (int)Math.Log(n - 1, 2);
                    int k = n - 1 - (int)Math.Pow(2, m);
                    val = Haar(x, k, m);
                }
                return val;
            };
        }

        /// <summary>
        /// Колокол Гаусса
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static Func<double,double> GaussBell(double mean=0,double sd=1)
        {
            double a = 1.0 / (sd * Math.Sqrt(2 * Math.PI));
            double s2 = -2 * sd;
            double sqr(double x) => x * x;
            return (double t) => a * Math.Exp(sqr(t-mean) / s2);

        }
        /// <summary>
        /// Нормированный колокол Гаусса
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public static Func<double, double> GaussBell2(double mean = 0, double sd = 1)
        {
            double s2 = -2 * sd;
            double sqr(double x) => x * x;
            return (double t) => Math.Exp(sqr(t - mean) / s2);

        }

        #endregion

        /// <summary>
        /// Аппроксимация функций системой функций на отрезке
        /// </summary>
        /// <param name="f">Функция, которую надо аппроксимировать</param>
        /// <param name="p">Функция, зависящая от действительного аргумента и номера</param>
        /// <param name="kind">Класс функций в системе (ортогональные/ортонормальные/другие)</param>
        /// <param name="n">Число функций из системы</param>
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        /// <returns></returns>
        public static Func<double,double> Approx(Func<double,double> f, SequenceFunc p, SequenceFuncKind kind, int n, double a, double b, bool ultra = false)
        {
            double[] c = new double[n];
            Func<double,double>[] fi = new Func<double,double>[n];
            //for (int i = 0; i < n; i++)
            //{
            //    fi[i] = new Func<double,double>((double x) => { return p(x, i); });
            //    //Console.WriteLine(fi[i](3));
            //}
            if (ultra)
            {
                SLAU T = new SLAU(p, f, n, a, b, kind);//T.Show();
                T.begin = a; T.end = b; T.f = f; T.p = p;
                T.UltraHybrid(T.Size, kind);
                return (double x) =>
                {
                    double sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                        sum += T.x[i] * p(x, i);
                    }
                    return sum;
                };
            }
            else
            {
                switch (kind)
                {
                    case SequenceFuncKind.Orthogonal:
                        for (int i = 0; i < n; i++)
                        {
                            fi[i] = new Func<double,double>((double x) => { return p(x, i); });
                            c[i] = RealFuncMethods.ScalarPower(f, fi[i], a, b) / RealFuncMethods.ScalarPower(fi[i], fi[i], a, b);//Console.WriteLine(c[3]);
                        }
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                            {
                                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                                sum += c[i] * p(x, i);
                            }
                            return sum;
                        };
                    case SequenceFuncKind.Orthonormal:
                        for (int i = 0; i < n; i++)
                        {
                            fi[i] = new Func<double,double>((double x) => { return p(x, i); });
                            c[i] = RealFuncMethods.ScalarPower(f, fi[i], a, b) * (b - a);
                        }
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                            {
                                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                                sum += c[i] * p(x, i);
                            }
                            return sum;
                        };
                    case SequenceFuncKind.Other:
                        SLAU T = new SLAU(p, f, n, a, b);//T.Show();
                        T.begin = a; T.end = b; T.f = f; T.p = p;
                        //T.GaussSpeedyMinimize(T.Size);
                        T.GaussSelection();
                        //T.Show();
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                                sum += T.x[i] * p(x, i);

                            return sum;
                        };
                    default:
                        return (double x) => { return x; };
                }
            }



        }
        /// <summary>
        /// Аппроксимация функций системой полиномов на отрезке
        /// </summary>
        /// <param name="f">Функция, которую надо аппроксимировать</param>
        /// <param name="p">Полином из некоторой системы</param>
        /// <param name="kind">Класс функций в системе (ортогональные/ортонормальные/другие)</param>
        /// <param name="n">Число функций из системы</param>
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        /// <returns></returns>
        public static Func<double,double> Approx(Func<double,double> f, SequencePol p, SequenceFuncKind kind, int n, double a, double b, bool ulrta = false)
        {
            double[] c = new double[n];
            if (ulrta)
            {
                SLAU T = new SLAU(p, f, n, a, b, kind);//T.Show();
                T.begin = a; T.end = b; T.f = f; T.p = (double x, int j) => p(j).Value(x);
                T.UltraHybrid(T.Size, kind);
                return (double x) =>
                {
                    double sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                        sum += T.x[i] * p(i).Value(x);
                    }
                    return sum;
                };
            }
            else
            {
                switch (kind)
                {
                    case SequenceFuncKind.Orthogonal:
                        for (int i = 0; i < n; i++)
                        {
                            SLAU Y = new SLAU(p, f, n, a, b);//Y.Show();
                            c[i] = RealFuncMethods.ScalarPower(f, p(i).Value, a, b) / Polynom.ScalarP(p(i), p(i), a, b);//Console.WriteLine(c[3]);
                        }
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                            {
                                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                                sum += c[i] * p(i).Value(x);
                            }
                            return sum;
                        };
                    case SequenceFuncKind.Orthonormal:
                        for (int i = 0; i < n; i++)
                        {
                            c[i] = RealFuncMethods.ScalarPower(f, p(i).Value, a, b) * (b - a);
                        }
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                            {
                                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                                sum += c[i] * p(i).Value(x);
                            }
                            return sum;
                        };
                    case SequenceFuncKind.Other:
                        SLAU T = new SLAU(p, f, n, a, b);//T.Show();
                        T.begin = a; T.end = b; T.f = f; T.p = (double x, int j) => p(j).Value(x);
                        //T.Holets(T.Size);
                        //var st = new Vectors(T.Size-1);
                        //for (int k = 1; k < T.Size; k++) st[k-1]=(new SqMatrix(T.A, k)).Det;"".Show();st.Show();
                        T.GaussSelection();
                        //T.GaussSpeedyMinimize(T.Size);
                        //T.Show();
                        return (double x) =>
                        {
                            double sum = 0;
                            for (int i = 0; i < n; i++)
                            {
                                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
                                sum += T.x[i] * p(i).Value(x);
                            }
                            return sum;
                        };
                    default:
                        return (double x) => { return x; };
                }
            }
        }
        /// <summary>
        /// Аппроксимация сеточной функции системой функций
        /// </summary>
        /// <param name="f">Сеточная функция, которую требуется аппроксимировать</param>
        /// <param name="p">Аппроксимирующие функции из некоторой системы</param>
        /// <param name="kind">Характер аппроксимирующих функций (ортогональные/ортонормальные/неортогональные)</param>
        /// <param name="n">Размерность системы (по умолчанию совпадает с размерностью сеточной функции)</param>
        /// <returns></returns>
        public static Func<double,double> Approx(NetFunc f, SequenceFunc p, SequenceFuncKind kind, int n = 0)
        {
            if (n == 0) n = f.CountKnots;
            double[] c = new double[n];
            Func<double,double>[] fi = new Func<double,double>[n];
            //switch (kind)
            //{
            //    case SequenceFuncKind.Orthogonal:
            //        for (int i = 0; i < n; i++)
            //        {
            //            fi[i] = new Func<double,double>((double x) => { return p(x, i); });
            //            c[i] = NetFunc.ScalarP(f, fi[i]) / NetFunc.ScalarP(fi[i], fi[i],f.Arguments);//Console.WriteLine(c[3]);
            //        }
            //        return (double x) =>
            //        {
            //            double sum = 0;
            //            for (int i = 0; i < n; i++)
            //            {
            //                sum += c[i] * p(x, i);
            //            }
            //            return sum;
            //        };
            //    case SequenceFuncKind.Orthonormal:
            //        for (int i = 0; i < n; i++)
            //        {
            //            fi[i] = new Func<double,double>((double x) => { return p(x, i); });
            //            c[i] = NetFunc.ScalarP(f, fi[i])*f.CountKnots;
            //        }
            //        return (double x) =>
            //        {
            //            double sum = 0;
            //            for (int i = 0; i < n; i++)
            //            {
            //                //fi[i] = new Func<double,double>((double t) => { return p(t, i); });
            //                sum += c[i] * p(x, i);
            //            }
            //            return sum;
            //        };
            //    case SequenceFuncKind.Other:
            SLAU T = new SLAU(p, f, n);
            T.GaussSelection();//T.Show();
            return (double x) =>
            {
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    sum += T.x[i] * p(x, i);
                }
                return sum;
            };
            //    default:
            //        return (double x) => { return x; };
            //}
        }

        public static Func<double,double> ApproxForLezhandr(Func<double,double> f, Func<double,double>[] masL, double a, double b)
        {
            int n = masL.Length;
            double[] c = new double[n];
            for (int i = 0; i < n; i++)
                c[i] = RealFuncMethods.ScalarPower(f, masL[i], a, b) / RealFuncMethods.ScalarPower(masL[i], masL[i], a, b);//Console.WriteLine(c[3]);

            return (double x) =>
            {
                double sum = 0;
                for (int i = 0; i < n; i++)
                    sum += c[i] * masL[i](x);
                return sum;
            };
        }

        /// <summary>
        /// Продемонстрировать аппроксимацию сеточной функции (созданной по действительной) системой функций
        /// </summary>
        /// <param name="f">Действительная функция (по которой строится сеточная)</param>
        /// <param name="c">Узлы (абциссы) для сеточной функции</param>
        /// <param name="p">Аппроксимирующие функции из некоторой системы</param>
        /// <param name="kind">Характер аппроксимирующих функций (ортогональные/ортонормальные/неортогональные)</param>
        /// <param name="n">Размерность системы (по умолчанию совпадает с размерностью сеточной функции)</param>
        /// <returns></returns>
        public static void ShowApprox(Func<double,double> f, double[] c, SequenceFunc p, SequenceFuncKind kind, int n = 0)
        {
            NetFunc g = new NetFunc(f, c);
            Console.WriteLine("Точки сеточной функции:"); g.Show();

            if (n == 0) n = c.Length;
            Func<double,double> ap = FuncMethods.Approx(g, p, kind, n);


            Console.WriteLine("Размерность системы равна {0}, размерность сеточной функции равна {1}", n, c.Length);
            Console.WriteLine("Аппроксимация исходной функции полученной функцией");
            Console.WriteLine("\t(в интегральной среднеквадратичной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistance(f, ap, g.MinArg, g.MaxArg));
            Console.WriteLine("\t(в равномерной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, ap, g.MinArg, g.MaxArg));
            Console.WriteLine("Аппроксимация сеточной функции полученной функцией");
            Console.WriteLine("\t(в дискретной среднеквадратичной норме) равна {0}", FuncMethods.NetFunc.Distance(g, ap));
        }
        /// <summary>
        /// Продемонстрировать аппроксимацию только сеточной функции системой функций
        /// </summary>
        /// <param name="g">Сеточная функция</param>
        /// <param name="p">Аппроксимирующие функции из некоторой системы</param>
        /// <param name="kind">Характер аппроксимирующих функций (ортогональные/ортонормальные/неортогональные)</param>
        /// <param name="n">Размерность системы (по умолчанию совпадает с размерностью сеточной функции)</param>
        /// <returns></returns>
        public static void ShowApprox(NetFunc g, SequenceFunc p, SequenceFuncKind kind, int n = 0)
        {
            Console.WriteLine("Точки сеточной функции:"); g.Show();

            if (n == 0) n = g.CountKnots;
            Func<double,double> ap = FuncMethods.Approx(g, p, kind, n);

            Console.WriteLine("Размерность системы равна {0}, размерность сеточной функции равна {1}", n, g.CountKnots);
            Console.WriteLine("Аппроксимация сеточной функции полученной функцией");
            Console.WriteLine("\t(в дискретной среднеквадратичной норме) равна {0}", FuncMethods.NetFunc.Distance(g, ap));
        }

        /// <summary>
        /// Продемонстрировать аппроксимацию действительной функции системой функций
        /// </summary>
        /// <param name="f"></param>
        /// <param name="p"></param>
        /// <param name="kind"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void ShowApprox(Func<double,double> f, SequenceFunc p, SequenceFuncKind kind, int n, double a, double b)
        {
            Func<double,double> ap = FuncMethods.Approx(f, p, kind, n, a, b);

            Console.WriteLine("Размерность системы равна {0}.", n);
            Console.WriteLine("Аппроксимация исходной функции полученной функцией");
            Console.WriteLine("\t(в интегральной среднеквадратичной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistance(f, ap, a, b));
            Console.WriteLine("\t(в равномерной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, ap, a, b));
        }
        /// <summary>
        /// Продемонстрировать аппроксимацию действительной функции системой функций
        /// </summary>
        /// <param name="f"></param>
        /// <param name="p"></param>
        /// <param name="kind"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void ShowApprox(Func<double,double> f, SequencePol p, SequenceFuncKind kind, int n, double a, double b)
        {
            Func<double,double> ap = FuncMethods.Approx(f, p, kind, n, a, b);

            Console.WriteLine("Размерность системы равна {0}.", n);
            Console.WriteLine("Аппроксимация исходной функции полученной функцией");
            Console.WriteLine("\t(в интегральной среднеквадратичной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistance(f, ap, a, b));
            Console.WriteLine("\t(в равномерной норме) равна {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, ap, a, b));
        }

        public static Tuple<Vectors,Vectors> UltraVsNormal(Func<double,double> f, SequenceFunc s,SequenceFuncKind kind,int nn=80,double a=-1, double b = 1)
        {
            SLAU slau = new SLAU(s, f, nn, a, b, kind);
            //slau.UltraHybrid(slau.Size);

            Vectors ult = new Vectors(nn); //ult.Show();
            Vectors norm = new Vectors(nn);

            for (int i = 1; i <= slau.Size; i++)
            {
                Func<double,double> g = FuncMethods.Approx(f, s, SequenceFuncKind.Other, i, a, b);
                norm[i-1] = Math.Log10(FuncMethods.RealFuncMethods.NormDistance(f, g, a, b));
                g = FuncMethods.Approx(f, s, SequenceFuncKind.Other, i, a, b,true);
                ult[i-1] = Math.Log10(FuncMethods.RealFuncMethods.NormDistance(f, g, a, b));
            }
            //norm.Show();

            return new Tuple<Vectors, Vectors>(norm, ult);

        }

        /// <summary>
        /// Методы для действительных функций
        /// </summary>
        public static class RealFuncMethods
        {
            /// <summary>
            /// Стандартное скалярное произведение функций
            /// </summary>
            /// <param name="f"></param>
            /// <param name="g"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double ScalarPower(Func<double,double> f, Func<double,double> g, double a, double b)
            {
                Func<double,double> F = (double e) => { return f(e) * g(e); };
                //double tmp = DefInteg.Simpson(F, a, b);
                double tmp = DefInteg.GaussKronrod.GaussKronrodSum(F, a, b,61,800);
                if (b != a) { tmp /= Math.Abs(b - a); }
                return tmp;
            }
            /// <summary>
            /// Норма L(2)[a,b] функции через скалярное произведение
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double NormScalar(Func<double,double> f, double a, double b) { return Math.Sqrt(ScalarPower(f, f, a, b)); }
            /// <summary>
            /// Расстояние между функциями по норме L(2)[a,b]
            /// </summary>
            /// <param name="f"></param>
            /// <param name="g"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double NormDistance(Func<double,double> f, Func<double,double> g, double a, double b)
            {
                Func<double,double> t = (double x) => { return f(x) - g(x); };
                return NormScalar(t, a, b);
            }
            /// <summary>
            /// Норма функции в пространстве С[a,b]
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double NormC(Func<double,double> f, double a, double b)
            {
                double h = DefInteg.STEP;
                double max = Math.Abs(f(a));
                for (double t = a + h; t <= b; t += h)
                    if (Math.Abs(f(t)) > max) max = Math.Abs(f(t));
                return max;
            }
            /// <summary>
            /// Расстояние между функциями по норме С[a,b]
            /// </summary>
            /// <param name="f"></param>
            /// <param name="g"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double NormDistanceС(Func<double,double> f, Func<double,double> g, double a, double b)
            {
                Func<double,double> t = (double x) => { return f(x) - g(x); };
                return NormC(t, a, b);
            }
        }

        /// <summary>
        /// Матричные и векторные функции
        /// </summary>
        public class MatrixFunc<T1, T2>
        {
            public delegate Matrix Delegate(T1 t1, T2 t2);
            /// <summary>
            /// Главная функция экземпляра класса
            /// </summary>
            public Delegate Value;

            /// <summary>
            /// Нулевая матрица указанной размерности
            /// </summary>
            /// <param name="i"></param>
            /// <param name="j"></param>
            public MatrixFunc(int i, int j)
            {
                this.Value = (T1 t, T2 r) => new Matrix(i, j);
            }

            /// <summary>
            /// Создание матричной функции по делегату
            /// </summary>
            /// <param name="s"></param>
            public MatrixFunc(Delegate s) { this.Value = new Delegate(s); }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="w"></param>
            public MatrixFunc(MatrixFunc<T1, T2> w) : this(w.Value) { }

            /// <summary>
            /// Вектор-функция по массиву функций
            /// </summary>
            /// <param name="F"></param>
            public MatrixFunc(Func<T1, T2, double>[] F)
            {
                this.Value = (T1 t1, T2 t2) =>
                  {
                      Matrix M = new Matrix(F.Length, 1);
                      for (int i = 0; i < F.Length; i++)
                          M[i, 0] = F[i](t1, t2);
                      return M;
                  };
            }
            /// <summary>
            /// Матричная функция по массиву функций
            /// </summary>
            /// <param name="F"></param>
            public MatrixFunc(Func<T1, T2, double>[,] F)
            {
                this.Value = (T1 t1, T2 t2) =>
                {
                    Matrix M = new Matrix(F.GetLength(0), F.GetLength(1));
                    for (int i = 0; i < F.GetLength(0); i++)
                        for (int j = 0; j < F.GetLength(1); j++)
                            M[i, j] = F[i, j](t1, t2);
                    return M;
                };
            }

            /// <summary>
            /// Значение экземпляра класса на аргументах
            /// </summary>
            /// <param name="t1"></param>
            /// <param name="t2"></param>
            /// <returns></returns>
            public Matrix this[T1 t1, T2 t2]
            {
                get { return this.Value(t1, t2); }
            }

            public static MatrixFunc<T1, T2> operator +(MatrixFunc<T1, T2> A, MatrixFunc<T1, T2> B)
            {
                Delegate t = (T1, T2) =>
                  {
                      Matrix a = A[T1, T2];
                      Matrix b = B[T1, T2];
                      return a + b;
                  };
                return new MatrixFunc<T1, T2>(t);
            }
            public static MatrixFunc<T1, T2> operator -(MatrixFunc<T1, T2> A)
            {
                Delegate t = (T1, T2) =>
                {
                    return -A[T1, T2];
                };
                return new MatrixFunc<T1, T2>(t);
            }
            public static MatrixFunc<T1, T2> operator -(MatrixFunc<T1, T2> A, MatrixFunc<T1, T2> B) { return A + (-B); }
            public static MatrixFunc<T1, T2> operator *(MatrixFunc<T1, T2> A, MatrixFunc<T1, T2> B)
            {
                Delegate t = (T1, T2) =>
                {
                    Matrix a = A[T1, T2];
                    Matrix b = B[T1, T2];
                    return a * b;
                };
                return new MatrixFunc<T1, T2>(t);
            }

            //здесь ещё нужен метод интегрировния (в результате будет матрица)
        }

        /// <summary>
        /// Методы оптимизации
        /// </summary>
        public static class Optimization
        {
            /// <summary>
            /// Точность методов
            /// </summary>
            public static /*readonly*/ double EPS = 1e-12;
            /// <summary>
            /// Шаг
            /// </summary>
            public static /*readonly*/ double STEP = 0.0001;
            /// <summary>
            /// Максимальный шаг поиска корня
            /// </summary>
            public static double s = 2;
            //static double h=0;
            /// <summary>
            /// Контроль над тем, модифицируется функция в методе или нет
            /// </summary>
            public enum ModifyFunction : byte
            {
                /// <summary>
                /// Да, модифицировать функцию
                /// </summary>
                Yes,
                /// <summary>
                /// Нет, не модифицировать функцию
                /// </summary>
                No
            };

            //private delegate Complex? MiniSearch(ComplexFunc f, double h, double q);
            private delegate Complex? MiniSearch(ComplexFunc f, Complex h, Complex q);
            private static Complex[] ForRootsSearching(ComplexFunc ff, Complex a, Complex b, MiniSearch M, double s = 0, ModifyFunction Mod = ModifyFunction.No)
            {
                var roots = new List<Complex?>();
                //double h = a.Re;
                Complex h = a;
                if (s <= 0) s = Optimization.s;
                //double step = s;
                Complex step = (b - a) / (b - a).Abs * s;
                ComplexFunc f = new ComplexFunc(ff);

                while (/*h <= b.Re - STEP*/(h - b + (b - a) / (b - a).Abs * STEP).Abs >= EPS && (b - h).Abs < (b - h + step).Abs)//пройти по всему отрезку
                {
                    step = s;
                    while (/*step >= STEP*/(step.Abs - STEP) >= EPS /*&& h <= b.Re - STEP*/)//пока шаг не достаточно маленький
                    {
                        while (!ExistRootConidtion(f, h, h + step) && /*h <= b.Re - STEP*/(h - b + (b - a) / (b - a).Abs * STEP).Abs >= EPS && (b - h).Abs < (b - h + step).Abs)
                        {
                            //Console.WriteLine($"!ExistRootConidtion(f, h, h + step){!ExistRootConidtion(f, h, h + step)} {h} <= {b.Re - STEP}");
                            h += step;
                            //Console.WriteLine($"h={h} \t|f(h)|={ff(h).Abs} \tstep={step}");  /*f(h).Show();h.Show(); */
                        }//доходить до корня
                        step /= 2;
                        //ExistRootConidtion(f, h, h + step).Show();
                    }
                    // try
                    //{
                    if (ExistRootConidtion(f, h, h + 2 * step))//во избежание повторов корней
                    {
                        //"+++++++++Добралось до поиска корня++++++++++".Show();
                        try { roots.Add(M(f, h, h + 2 * step)); } catch { $"Исключение на поиске корня в отрезке [{h} , {h + 2 * step}]".Show(); }
                        //if (roots.Last() != null) Console.WriteLine($"корень={roots.Last()} значение в корне по модулю = {ff((Complex)roots.Last()).Abs}");
                        if (Mod == ModifyFunction.Yes)
                        {
                            f = (Complex z) =>
                              {
                                  Complex p = new Complex(1, 0);
                                  for (int i = 0; i < roots.Count; i++)
                                      if (roots[i] != null)
                                          p *= (Complex)(z - roots[i]);
                                  return ff(z) / p;
                              };
                            h -= 2 * step;
                        }
                    }

                    h += 2 * step;//если ищутся кратные корни, надо проходить участок заново, а иначе идти дальше
                                  //}
                                  //catch(Exception e) { throw new Exception(e.Message); }
                }

                //преобразовать массив корней
                roots.RemoveAll(t => t == null);
                var c = new Complex[roots.Count];
                for (int i = 0; i < c.Length; i++)
                    c[i] = (Complex)roots[i];
                Array.Sort(c);
                if (Mod == ModifyFunction.No) c.Distinct();//если кратные корни не могут искаться, отсеять повторы
                return c;
            }

            private static bool ExistRootConidtion(ComplexFunc f, Complex a, Complex b)
            {
                Complex x1 = f(a), x2 = f(b);
                return (Math.Sign((x1.Re) * (x2.Re)) <= 0 && (Math.Sign((x1.Im) * (x2.Im)) <= 0));
                //return (Math.Sign((x1.Re/Math.Abs(x1.Re)) * (x2.Re / Math.Abs(x2.Re))) <= 0 && (Math.Sign((x1.Im / Math.Abs(x1.Im)) * (x2.Im / Math.Abs(x2.Im)))<= 0))/*|| x1.Im * x2.Im==0|| x1.Re * x2.Re==0*/;
            }
            /// <summary>
            /// Метод бисекции(дихотомии)
            /// </summary>
            /// <param name="f">Комплексная функция</param>
            /// <param name="a">Начало отрезка поиска корня</param>
            /// <param name="b">Конец отрезка поиска корня</param>
            /// <returns></returns>
            public static Complex[] Bisec(ComplexFunc f, Complex a, Complex b, double s = 0, ModifyFunction Mod = ModifyFunction.No)
            {
                return ForRootsSearching(f, a, b, MiniBisec, s, Mod);
            }
            private static Complex? MiniBisec(ComplexFunc f, double a, double b) { return MiniBisec(f, (Complex)a, (Complex)b); }
            private static Complex? MiniBisec(ComplexFunc f, Complex a, Complex b)
            {
                Complex v = new Complex();
                //if (!ExistRootConidtion(f, a, b)) throw new Exception("Между этими точками не существует корня!");
                if (!ExistRootConidtion(f, a, b)) return null;
                v = Expendator.Average(a, b);
                while (f(v).Abs > EPS && (b - a).Abs > EPS)
                {
                    //Console.WriteLine($"\t a={a} \tb={b} \tf(a)={f(a)} \tf(b)={f(b)}");
                    //Console.WriteLine($"\t a-b={a-b} \tv={v} f(v)={f(v)}");
                    //Console.WriteLine();

                    if (ExistRootConidtion(f, v, b))
                        a = v;
                    else
                        b = v;
                    v = Expendator.Average(a, b);
                }
                return v;
            }

            internal class PointC
            {
                internal Complex x, y;
                internal PointC(Complex x, Complex y) { this.x = x; this.y = y; }
            }

            private static Complex W(PointC[] p)
            {
                Complex sum = new Complex(0, 0), pow = new Complex(1, 0);
                for (int j = 0; j < p.Length; j++)
                {
                    for (int l = 0; l < p.Length; l++)
                        if (j != l) pow *= p[j].x - p[l].x;
                    sum += p[j].y / pow;
                    pow = 1;
                }
                return sum;
            }
            /// <summary>
            /// Разделённая разность по массиву точек (с рекурсией)
            /// </summary>
            /// <param name="p">Массив точек</param>
            /// <param name="i">Номер начального элемента в разности</param>
            /// <param name="j">Номер конечного элемента в разности</param>
            /// <returns></returns>
            private static Complex W(PointC[] p, int i, int j)
            {
                if (j - i == 1) return (p[j].y - p[i].y) / (p[j].x - p[i].x);
                return (W(p, i + 1, j) - W(p, i, j - 1)) / (p[j].x - p[i].x);
            }

            /// <summary>
            /// Поиск всех корней комплексной функции с действительной частью на указанном действительном отрезке
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Complex[] FullMuller(ComplexFunc f, Complex a, Complex b, double s = 0, ModifyFunction Mod = ModifyFunction.No)
            {
                return ForRootsSearching(f, a, b, ForFullMuller, s, Mod);
                //var roots = new List<Complex?>();
                //double h = a.Re;
                //double step = s;

                //while (h <= b.Re - STEP)//пройти по всему отрезку
                //{
                //    //Console.WriteLine($"{h} <= {b.Re - STEP}");
                //    step = s;
                //    while (step > 10*STEP /*&& h <= b.Re - STEP*/)//пока шаг не достаточно маленький
                //    {
                //        while (!ExistRootConidtion(f, h, h + step) && h <= b.Re - STEP)
                //        {
                //            h += step;
                //            //Console.WriteLine($"h={h} f(h)={f(h)}");  /*f(h).Show();h.Show(); Console.WriteLine($"{h} <= {b.Re - STEP}");*/
                //        }//доходить до корня
                //        step /= 2;

                //    } 
                //    roots.Add(ForFullMuller(f, h, h + STEP));
                //    if (roots.Last() != null) Console.WriteLine($"корень={roots.Last()} значение в корне = {f((Complex)roots.Last())}");
                //    h += STEP;
                //}

                //roots.RemoveAll(t => t == null);
                //var c = new Complex[roots.Count];
                //for (int i = 0; i < c.Length; i++)
                //    c[i] = (Complex)roots[i];
                //return c;
            }

            private static Complex? ForFullMuller(ComplexFunc f, /*double*/ Complex h1, /*double*/ Complex h2)
            {
                double step = (h2 - h1).Abs;
                Complex z1 = new Complex(h1.Re, 0), z2 = new Complex(h1.Re, -step / 10), z3 = new Complex(h1.Re, step / 10);
                // try
                //{
                Complex res = Muller(f, z1, z2, z3);
                /*if (res.Re >= h1 && res.Re <= h2)*/
                return res;

                //}
                //catch(Exception e) { throw new Exception(e.Message); }

                return null;
            }

            /// <summary>
            /// Метод Мюллера (парабол) поиска нуля функции
            /// </summary>
            /// <param name="f">Функция комплексного переменного</param>
            /// <param name="x1">Первая точка</param>
            /// <param name="x2">Вторая точка</param>
            /// <param name="x3">Третья точка</param>
            /// <returns></returns>
            public static Complex Muller(ComplexFunc f, Complex x1, Complex x2, Complex x3)
            {
                PointC[] mas = new PointC[]
                {
                    new PointC(x1,f(x1)),
                    new PointC(x2,f(x2)),
                    new PointC(x3,f(x3))
                };
                Complex w = W(new PointC[] { mas[2], mas[1] }) + W(new PointC[] { mas[2], mas[0] }) - W(new PointC[] { mas[1], mas[0] });
                Complex f123 = W(mas);
                Complex fk = f(x3);
                Complex xk1;
                Complex[] roots = new Complex[2];

                do
                {
                    try
                    {
                        roots[0] = x3 - 2 * fk / (w + Complex.Sqrt(w * w - 4 * fk * f123));
                    }
                    catch (Exception e) { throw new Exception(e.Message); }
                    roots[1] = x3 - 2 * fk / (w - Complex.Sqrt(w * w - 4 * fk * f123));
                    xk1 = MinUnder(roots, x3);

                    x1 = x2;
                    x2 = x3;
                    x3 = xk1;
                    mas = new PointC[]
                      {
                    new PointC(x1,f(x1)),
                    new PointC(x2,f(x2)),
                    new PointC(x3,f(x3))
                    };
                    fk = f(x3);
                    f123 = W(mas);
                    w = W(new PointC[] { mas[2], mas[1] }) + W(new PointC[] { mas[2], mas[0] }) - W(new PointC[] { mas[1], mas[0] });
                } while (fk.Abs > EPS && (x3 - x2).Abs > EPS /*&&(fk.Abs<f(x2).Abs)*/);

                return xk1;

            }
            /// <summary>
            /// Наименее отклоняющаяся от комплексного числа точка из массива 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="z"></param>
            /// <returns></returns>
            private static Complex MinUnder(Complex[] x, Complex z)
            {
                double v = (x[0] - z).Abs;
                Complex val = x[0];
                for (int i = 0; i < x.Length; i++)
                    if ((x[i] - z).Abs < v)
                    {
                        v = (x[i] - z).Abs;
                        val = x[i];
                    }
                return val;
            }

            /// <summary>
            /// Метод хорд поиска корня
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Complex[] Chord(ComplexFunc f, Complex a, Complex b, double s = 0, ModifyFunction Mod = ModifyFunction.No) { return ForRootsSearching(f, a, b, ForChord, s, Mod); }
            private static Complex? ForChord(ComplexFunc f, Complex x0, Complex x1)
            {
                Complex xn = new Complex();
                Complex fx0 = f(x0);
                do
                {
                    Complex fx1 = f(x1);
                    xn = x1 - fx1 / (fx1 - fx0) * (x1 - x0);
                    x0 = x1;
                    x1 = xn;
                } while (/*f(xn).Abs > EPS &&*/ (x0 - x1).Abs > EPS);
                return xn;
            }
            private static Complex? ForChord(ComplexFunc f, double x0, double x1) => ForChord(f, (Complex)x0, (Complex)x1);

            private static Complex Derivative(ComplexFunc f, Complex x) => (f(x + EPS) - f(x)) / EPS;
            /// <summary>
            /// Вариация метода
            /// </summary>
            public enum Variety : byte
            {
                /// <summary>
                /// Простейшая вариация метода
                /// </summary>
                Simple,
                /// <summary>
                /// Традиционная вариация метода
                /// </summary>
                Usual
            }
            /// <summary>
            /// Метод Ньютона поиска корня
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Complex[] Neu(ComplexFunc f, Complex a, Complex b, Variety va = Variety.Usual, double s = 0, ModifyFunction Mod = ModifyFunction.No)
            {
                Complex? F(ComplexFunc q, /*double*/ Complex w, /*double*/ Complex e) => ForNeu(f, w, e, va);
                return ForRootsSearching(f, a, b, F, s, Mod);
            }
            private static Complex? ForNeu(ComplexFunc f, Complex a, Complex b, Variety va = Variety.Usual)
            {
                Complex x0 = b, xn;
                if (va == Variety.Usual) do
                    {
                        xn = x0 - f(x0) / Derivative(f, x0);
                        x0 = xn;
                    } while (f(xn).Abs > EPS);
                else
                {
                    Complex der = Derivative(f, x0);
                    do
                    {
                        xn = x0 - f(x0) / der;
                        x0 = xn;
                    } while (f(xn).Abs > EPS);
                }
                return xn;
            }
            private static Complex? ForNeu(ComplexFunc f, double x0, double x1, Variety va = Variety.Usual) => ForNeu(f, (Complex)x0, (Complex)x1, va);

            /// <summary>
            /// Комбинированный метод поиска корня
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Complex[] ChordNeu(ComplexFunc f, Complex a, Complex b, double s = 0, ModifyFunction Mod = ModifyFunction.No) { return ForRootsSearching(f, a, b, ForChordNeu, s, Mod); }
            private static Complex? ForChordNeu(ComplexFunc f, Complex a, Complex b)
            {
                Complex xl0 = a, xr0 = b, xl, xr;
                do
                {
                    xr = xr0 - f(xr0) / Derivative(f, xl0);
                    xl = xl0 - f(xl0) / (f(xr0) - f(xl0)) * (xr0 - xl0);
                    xr0 = xr; xl0 = xl;
                } while ((xl - xr).Abs > EPS);
                return Expendator.Average(xr, xl);
            }
            private static Complex? ForChordNeu(ComplexFunc f, double x0, double x1) => ForChordNeu(f, (Complex)x0, (Complex)x1);

            private static Complex[] GlobalMin(ComplexFunc f, Complex[] p)
            {
                Complex c = p[0];
                for (int i = 1; i < p.Length; i++)
                    if (f(p[i]).Abs < f(c).Abs)
                        c = p[i];

                return p.Where(n => f(n).Abs == f(c).Abs).ToArray();
            }
            private static Complex[] GlobalMax(ComplexFunc f, Complex[] p)
            {
                var r = new List<Complex>();

                Complex c = p[0];
                for (int i = 1; i < p.Length; i++)
                    if (f(p[i]).Abs > f(c).Abs)
                        c = p[i];

                return r.Where(n => f(n).Abs == f(c).Abs).ToArray();
            }
            private static bool ExistMin(ComplexFunc f, Complex a, Complex b)
            {
                //Complex x1 = Derivative(f, a), x2 = Derivative(f, b);
                //    return (Math.Sign((x1.Re) * (x2.Re)) <= 0 && (Math.Sign((x1.Im) * (x2.Im)) <= 0));
                Complex ff(Complex z) => Derivative(f, z);
                return ff(a).Re <= 0 && ff(b).Re > 0;
            }

            private delegate Complex? FuncMinSearch(ComplexFunc f, Complex h, Complex q);
            /// <summary>
            /// Поиск экстремумов функции указанным методом
            /// </summary>
            /// <param name="ff">Функция комплексного переменного</param>
            /// <param name="a">Начало отрезка поиска</param>
            /// <param name="b">Конец отрезка поиска</param>
            /// <param name="M">Метод локального поиска</param>
            /// <param name="s">Максимальный шаг поиска</param>
            /// <returns></returns>
            public static Complex[] MinSearch(RealFuncOfCompArg ff, Complex a, Complex b, MinimumVar M, double s = 0)
            {
                var roots = new List<Complex?>();
                double h = a.Re;
                if (s <= 0) s = Optimization.s;
                double step = s;
                ComplexFunc f = Expendator.ToCompFunc(ff);

                while (h <= b.Re - STEP)//пройти по всему отрезку
                {
                    step = s;
                    while (step >= STEP /*&& h <= b.Re - STEP*/)//пока шаг не достаточно маленький
                    {
                        while (!ExistMin(f, h, h + step) && h <= b.Re - STEP && Derivative(f, h).Abs > EPS)
                        {
                            h += step;
                            //Console.WriteLine($"h={h} \t|f`(h)|={Derivative(f,h).Abs} \tstep={step}");  /*f(h).Show();h.Show(); */
                        }//доходить до корня
                        step /= 2;
                        //ExistRootConidtion(f, h, h + step).Show();
                    }
                    //"Тут0".Show();
                    //ExistMin(f, -Math.PI/2-STEP, -Math.PI / 2 + STEP).Show();
                    // try
                    //{
                    if (ExistMin(f, h, h + 2 * step))//во избежание повторов корней
                    {
                        //"+++++++++Добралось до поиска корня++++++++++".Show();
                        roots.Add(MinimumSearch(ff, h, h + 2 * step, M));
                        //if (roots.Last() != null) Console.WriteLine($"корень={roots.Last()} значение в корне по модулю = {ff((Complex)roots.Last()).Abs}");
                    }

                    h += 2 * step;//если ищутся кратные корни, надо проходить участок заново, а иначе идти дальше
                                  //}
                                  //catch(Exception e) { throw new Exception(e.Message); }
                }

                //преобразовать массив минимумов
                roots.RemoveAll(t => t == null);
                var c = new Complex[roots.Count];
                for (int i = 0; i < c.Length; i++)
                    c[i] = (Complex)roots[i];
                Array.Sort(c);
                c.Distinct();
                return c;
            }
            /// <summary>
            /// Методы поиска минимума
            /// </summary>
            public enum MinimumVar : byte
            {
                /// <summary>
                /// Метод золотого сечения
                /// </summary>
                GoldSection
            }
            /// <summary>
            /// Поиск минимума функции указанным методом
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="M"></param>
            /// <returns></returns>
            public static Complex? MinimumSearch(RealFuncOfCompArg f, Complex a, Complex b, MinimumVar M)
            {
                Complex? k;
                switch (M)
                {
                    default:
                        k = GoldSect(f, a, b);
                        break;
                }
                return k;
            }
            /// <summary>
            /// Метод золотого сечения поиска минимума функции на указанном отрезке
            /// </summary>
            /// <param name="f"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Complex? GoldSect(RealFuncOfCompArg f, Complex a, Complex b)
            {
                ComplexFunc ff = Expendator.ToCompFunc(f);
                //"Тут".Show();
                if (Derivative(ff, a).Abs < EPS) return a;
                if (Derivative(ff, b).Abs < EPS) return b;

                double e = 2.0 / (3 + Math.Sqrt(5));
                Complex x2 = a + e * (b - a), x3 = b - e * (b - a);
                //if (f(x2).Abs > f(a).Abs || f(x3).Abs > f(b).Abs) return null;

                if (!(Derivative(ff, a).Re < 0 && Derivative(ff, b).Re > 0)) return null;

                while ((b - a).Abs > EPS && NotExter(f, a, b, x2, x3))
                {
                    //if (f(x2).Abs < EPS) return x2;
                    //if (f(x3).Abs < EPS) return x3;

                    Complex[] mas = new Complex[] { a, x2, x3, b }; //mas.Show();Console.WriteLine();
                    Complex t1 = GlobalMin(Expendator.ToCompFunc(f), mas)[0];
                    Complex[] newmas = mas.Where(n => n != t1).ToArray();
                    Complex t2 = GlobalMin(Expendator.ToCompFunc(f), newmas)[0];
                    //newmas = mas.Where(n => n != t2).ToArray();
                    //a = newmas[0];b = newmas[1];
                    if (t1 == a && t2 == b || t2 == a && t1 == b) return Expendator.Average(a, b);
                    a = t1; b = t2;
                    x2 = a + e * (b - a); x3 = b - e * (b - a);
                }

                return Exter(f, a, b, x3, x2);
                //return Expendator.Average(x2, x3);
            }

            private static bool NotExter(RealFuncOfCompArg f, params Complex[] z)
            {
                ComplexFunc ff = Expendator.ToCompFunc(f);
                for (int i = 0; i < z.Length; i++)
                    if (Derivative(ff, z[i]).Abs < EPS)
                        return false;
                return true;
            }
            /// <summary>
            /// Вернуть из нескольких чисел наиболее близкую к экстремуму точку
            /// </summary>
            /// <param name="f"></param>
            /// <returns></returns>
            private static Complex Exter(RealFuncOfCompArg f, params Complex[] z)
            {
                ComplexFunc ff = Expendator.ToCompFunc(f);
                Complex tmp = z[0];
                for (int i = 1; i < z.Length; i++)
                    if (Derivative(ff, z[i]).Abs < Derivative(ff, tmp).Abs)
                        tmp = z[i];
                return tmp;
            }

            /// <summary>
            /// Презентация работы группы методов поиска корня
            /// </summary>
            /// <param name="f">Функция комплексного переменного</param>
            /// <param name="a">Начало отрезка поиска</param>
            /// <param name="b">Конец отрезка поиска</param>
            /// <param name="s">Максимальный шаг поиска</param>
            /// <param name="Mod">Искать/не искать кратные корни</param>
            public static void Presentaion(ComplexFunc f, Complex a, Complex b, double s = 0, ModifyFunction Mod = ModifyFunction.No)
            {
                Console.WriteLine("----------------Презентация работы группы методов поиска корня------------");
                Console.WriteLine($"\tОтрезок поиска корня: [{a};{b}]");
                if (s != 0) Console.WriteLine($"\tМаксимальный шаг при поиске: {s}");
                else Console.WriteLine($"\tМаксимальный шаг при поиске: {Optimization.s}");
                if (Mod == ModifyFunction.No) Console.WriteLine("\t----Ищутся разные корни");
                else Console.WriteLine("\t----Ищутся все корни, включая кратные");

                Complex[] k = Bisec(f, a, b, s, Mod); GetText(f, k, "бисекции");
                k = FullMuller(f, a, b, s, Mod); GetText(f, k, "Мюллера");
                k = Chord(f, a, b, s, Mod); GetText(f, k, "хорд");
                k = Neu(f, a, b, Variety.Simple, s, Mod); GetText(f, k, "Ньютона (урощённый)");
                k = Neu(f, a, b, Variety.Usual, s, Mod); GetText(f, k, "Ньютона (стандартный)");
                k = ChordNeu(f, a, b, s, Mod); GetText(f, k, "комбинированным хорд-Ньютона");

            }
            private static void GetText(ComplexFunc f, Complex[] k, string s)
            {
                Console.WriteLine("------Корни, найденные методом {0}:", s);
                for (int i = 0; i < k.Length; i++)
                    try { Console.WriteLine($"Корень: {k[i]} ; значение в корне (по модулю): {f(k[i]).Abs}"); } catch { "вось".Show(); }
                //k.Show();
            }

            /// <summary>
            /// Ключевая точка
            /// </summary>
            public enum CriticalPoint : byte
            {
                /// <summary>
                /// Корень
                /// </summary>
                Root,
                /// <summary>
                /// Минимум
                /// </summary>
                Minimum
            }

            /// <summary>
            /// Методы поиска корня
            /// </summary>
            public enum RootSearchMethod : byte
            {
                /// <summary>
                /// Метод бисекции
                /// </summary>
                Bisec,
                /// <summary>
                /// Метод парабол
                /// </summary>
                Muller,
                /// <summary>
                /// Метод хорд
                /// </summary>
                Chord,
                /// <summary>
                /// Метод Ньютона
                /// </summary>
                Neu,
                /// <summary>
                /// Комбинированный метод хорд-Ньютона
                /// </summary>
                ChordNeu
            }
            /// <summary>
            /// Поиск корня комплексной функции в прямоугольнике
            /// </summary>
            /// <param name="ff">Комплексная функция</param>
            /// <param name="a">Правый верхний угол прямоугольника</param>
            /// <param name="b">Левый верхний угол прямоугольника</param>
            /// <param name="c">Левый нижний угол прямоугольника</param>
            /// <param name="d">Правый нижний угол прямоугольника</param>
            /// <param name="M">Метод поиска корня на отрезке</param>
            /// <param name="s">Шаг в методе поиска корня на отрезке</param>
            /// <param name="sh">Шаг параллельных прямых по прямоугольнику</param>
            /// <returns></returns>
            public static Complex[] SearchRoots(ComplexFunc ff, Complex a, Complex b, Complex c, Complex d, RootSearchMethod M = RootSearchMethod.ChordNeu, double s = 0, double sh = 0.1)
            {
                if (s == 0) s = Optimization.s;
                if (sh == 0) sh = Optimization.s;
                var res = new List<Complex>();
                Complex st = (d - a) / (d - a).Abs * sh;
                Complex sr = (a - b) / (a - b).Abs * sh / (d - a).Abs;
                MiniSearch S;
                switch (M)
                {
                    case RootSearchMethod.Bisec:
                        S = MiniBisec;
                        break;
                    case RootSearchMethod.Chord:
                        S = ForChord;
                        break;
                    case RootSearchMethod.ChordNeu:
                        S = ForChordNeu;
                        break;
                    case RootSearchMethod.Neu:
                        S = /*ForNeu*/(ComplexFunc f, Complex at, Complex bt) => ForNeu(f, at, bt, Variety.Usual);
                        break;
                    default:
                        S = ForFullMuller;
                        break;

                }
                res.AddRange(ForRootsSearching(ff, a, b, S, s));//res.Show();
                Complex x = a, y = b;
                int i = 0;
                while ((x - d).Abs > EPS && (d - x + st).Abs > (x - d).Abs)
                {
                    x += st;//x.Show();
                    y += sr;
                    i++; if (i == 1000) { x.Show(); i = 0; }
                    res.AddRange(ForRootsSearching(ff, b + x - a, x, S, s));
                    //res.AddRange(ForRootsSearching(ff, y, c+y-b, S, s));
                }
                res = new List<Complex>(res.Distinct());
                return res.ToArray();
            }


            private static void Halfc(ComplexFunc F, double tmin, double tmax, double step, double eps, int Nmax, out double[] dz, int Nx = 0)
            {
                int it, ir, ii;
                double t1, t2, rf1, rf2, if1, if2, epsf, signr, signi, u1, u2, u, sgr, sgi, rfu1, rfu2, ifu1, ifu2, rfu, ifu;
                dz = new double[Nmax + 1];
                Complex ft;
                Nx = 0; it = 1; epsf = eps / 1e7;

                t1 = tmin; ft = F(t1);
                rf1 = ft.Re; if1 = ft.Im;

                g1: t2 = t1 + step;
                if (t2 > tmax)
                {
                    t2 = tmax; it = -1;
                }

                ft = F(t2); rf2 = ft.Re; if2 = ft.Im;

                if (Math.Abs(rf2) < epsf)
                { signr = -1; ir = -1; }
                else
                { signr = rf1 * rf2; ir = 1; }

                if (Math.Abs(if2) < epsf)
                { signi = -1; ii = -1; }
                else
                { signi = if1 * if2; ii = 1; }

                if ((signr < 0) && (signi < 0))
                {
                    if ((ir < 0) && (ii < 0))
                    {
                        Nx += 1;
                        dz[Nx] = t2;//F(t2).Show(); этот не косячит
                        goto g2;
                    }

                    u1 = t1; rfu1 = rf1; ifu1 = if1;

                    u2 = t2; rfu2 = rf2; ifu2 = if2;

                    g3: u = (u1 + u2) / 2; ft = F(u);

                    rfu = ft.Re; ifu = ft.Im;

                    sgr = rfu1 * rfu; sgi = ifu1 * ifu;

                    if ((ir > 0) && (ii > 0) && (sgr * sgi < 0)) goto g2;
                    //else
                    if (ir > 0)
                    {
                        if (sgr > 0)
                        {
                            u1 = u; rfu1 = rfu; ifu1 = ifu;
                        }
                        else
                        {
                            u2 = u; rfu2 = rfu; ifu2 = ifu;
                        }
                    }
                    else if (sgi > 0)
                    {
                        u1 = u; rfu1 = rfu; ifu1 = ifu;
                    }
                    else
                    {
                        u2 = u; rfu2 = rfu; ifu2 = ifu;
                    }

                    if (Math.Abs(u1 - u2) > eps) goto g3;
                    //if (F(u).Abs < 1) { Nx += 1; dz[Nx] = u; }
                    Nx += 1; dz[Nx] = u; //F(u).Show();//этот выбор иногда косячит
                }
                g2: t1 = t2; rf1 = rf2; if1 = if2;
                if ((Nx < Nmax) && (it > 0)) goto g1;

            }
            /// <summary>
            /// Массив действительных корней комплексной функции на отрезке
            /// </summary>
            /// <param name="f">Функция</param>
            /// <param name="tmin">Начало отрезка</param>
            /// <param name="tmax">Конец отрезка</param>
            /// <param name="step">Шаг</param>
            /// <param name="eps">Точность</param>
            /// <param name="Nmax">Число итераций</param>
            /// <returns></returns>
            public static Vectors Halfc(ComplexFunc f, double tmin, double tmax, double step, double eps, int Nmax = 10)
            {
                double[] res;
                Halfc(f, tmin, tmax, step, eps, Nmax, out res);
                res = res.Distinct().ToArray();
                Array.Sort(res);
                return new Vectors(res);
            }

            
        }

        /// <summary>
        /// Класс интегральных преобразований
        /// </summary>
        public static class IntegralTransformations
        {
            public static ComplexFunc Furier(Func<double,double> f)
            {
                return (Complex w) =>
                {
                    ComplexFunc fe = (Complex x) => f(x.Re) * Complex.Exp(Complex.I * x * w);
                    return DefInteg.GaussKronrod.IntegralInf(fe, -20, 20);
                };
            }
            public static Func<double,double> FurierRevers(ComplexFunc f, ComplexFunc delta = null, int t = 100)
            {
                return (double x) =>
                {
                    ComplexFunc fe = (Complex w) => f(w) * Complex.Exp(-Complex.I * x * w);
                    return 1.0 / (2 * Math.PI) * DefInteg.GaussKronrod.IntegralInf(fe, -1, 1, delta, t);
                };
            }

            public static void Test(Func<double,double> f, double arg)
            {
                ComplexFunc f1 = Furier(f); //f1(arg).Abs.Show();
                Func<double,double> f2 = FurierRevers(f1); f2(arg).Show();
                (f(arg) - f2(arg)).Show();
            }
        }
    }
}

