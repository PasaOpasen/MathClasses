using System;
using System.Collections.Generic;
using System.Linq;
using PointV = System.Tuple<double, МатКлассы.Vectors>;
using VectorNetFunc = System.Collections.Generic.List<System.Tuple<double, МатКлассы.Vectors>>;

namespace МатКлассы
{
    public static partial class FuncMethods
    {
       
        /// <summary>
        /// Класс решения ОДУ
        /// </summary>
        public static class ODU
        {
            /// <summary>
            /// Метод решения ОДУ
            /// </summary>
            public enum Method:byte
            {
                /// <summary>
                /// Метод Эйлера
                /// </summary>
                E1,
                /// <summary>
                /// Метод Эйлера с пересчётом
                /// </summary>
                E2,
                /// <summary>
                /// Метод Хойна
                /// </summary>
                H,
                /// <summary>
                /// Метод Рунге-Кутты 3 порядка
                /// </summary>
                RK3,
                /// <summary>
                /// Метод Рунге-Кутты 4 порядка
                /// </summary>
                RK4,
                /// <summary>
                /// Правило трёх восьмых
                /// </summary>
                P38,
                /// <summary>
                /// Метод Фельдберга
                /// </summary>
                F,
                /// <summary>
                /// Метод Ческино
                /// </summary>
                C
            }
            private static SqMatrix E1, E2, H, RK3, Rk4, P38;
            private static Matrix F, C;
            private static int MaxStepChange = 2000;
            static ODU()
            {
                E1 = new SqMatrix(2);
                E1[1, 1] = 1;
                E2 = new SqMatrix(3);
                E2[1, 0] = 0.5; E2[1, 1] = 0.5; E2[2, 2] = 1;
                H = new SqMatrix(4);
                H[1, 0] = 1.0 / 3; H[1, 1] = 1.0 / 3; H[2, 0] = 2.0 / 3; H[2, 2] = 2.0 / 3; H[3, 1] = 1.0 / 4; H[3, 3] = 3.0 / 4;
                RK3 = new SqMatrix(4);
                RK3[1, 0] = 0.5; RK3[1, 1] = 0.5; RK3[2, 0] = 1; RK3[2, 2] = 1; RK3[3, 1] = 1.0 / 6; RK3[3, 2] = 4.0 / 6; RK3[3, 3] = 1.0 / 6;
                Rk4 = new SqMatrix(5);
                Rk4[1, 0] = 0.5; Rk4[1, 1] = 0.5; Rk4[2, 0] = 0.5; Rk4[2, 2] = 0.5; Rk4[3, 0] = 1; Rk4[3, 3] = 1; Rk4[4, 1] = 1.0 / 6; Rk4[4, 2] = 2.0 / 6; Rk4[4, 3] = 2.0 / 6; Rk4[4, 4] = 1.0 / 6;
                P38 = new SqMatrix(5);
                P38[1, 0] = 1.0 / 3; P38[1, 1] = 1.0 / 3; P38[2, 0] = 2.0 / 3; P38[2, 1] = -1.0 / 3; P38[2, 2] = 1; P38[3, 0] = 1; P38[3, 1] = 1; P38[3, 2] = -1; P38[3, 3] = 1; P38[4, 1] = 1.0 / 8; P38[4, 2] = 3.0 / 8; P38[4, 3] = 3.0 / 8; P38[4, 4] = 1.0 / 8;
                F = new Matrix(5, 4);
                F[1, 0] = 1; F[1, 1] = 1; F[2, 0] = 0.5; F[2, 1] = 0.25; F[2, 2] = 0.25; F[3, 1] = 0.5; F[3, 2] = 0.5; F[4, 1] = 1.0 / 6; F[4, 2] = 1.0 / 6; F[4, 3] = 4.0 / 6;
                C = new Matrix(6, 5);
                C[1, 0] = 0.25; C[1, 1] = 0.25; C[2, 0] = 0.5; C[2, 2] = 0.5; C[3, 0] = 1; C[3, 1] = 1; C[3, 3] = -2; C[3, 4] = 2; C[4, 1] = 1; C[4, 2] = -2; C[4, 3] = 2; C[5, 1] = 1.0 / 6; C[5, 3] = 4.0 / 6; C[5, 4] = 1.0 / 6;
            }

            private static void Get2Tmp(ref double tmp, ref double tmpp, double step, Matrix A, double[] k, DRealFunc f, double u, double t, int r)
            {
                tmp = 0; tmpp = 0;
                for (int i = 0; i < k.Length; i++)
                {
                    double tmp2 = 0;
                    for (int j = 0; j < i; j++)
                        tmp2 += A[i, j + 1] * k[j];
                    k[i] = f(t + A[i, 0] * step, u + tmp2 * step);
                    tmp += A[r, i + 1] * k[i];
                    if (A.RowCount != A.ColCount) tmpp += A[r + 1, i + 1] * k[i];
                }
            }

            /// <summary>
            /// Решение приведённого ОДУ первого порядка
            /// </summary>
            /// <param name="f">Свободная функция переменных u и x, где u - искомая функция</param>
            /// <param name="begin">Начальный аргумент по задаче Коши</param>
            /// <param name="end">Конечный аргумент</param>
            /// <param name="step">Шаг интегрирования</param>
            /// <param name="M">Метод поиска решения</param>
            /// <param name="begval">Значение функции при начальном аргументе</param>
            /// <param name="eps">Допустимый уровень расчётных погрешностей</param>
            /// <returns></returns>
            public static NetFunc ODUsearch(DRealFunc f, double begin = 0, double end = 10, double step = 0.01, Method M = Method.E1, double begval = 1, double eps = 0.0001, bool controllingstep = false)
            {
                double thisstep = step;
                NetFunc res = new NetFunc();
                res.Add(new Point(begin, begval));
                step *= Math.Sign(end - begin);

                Matrix A;
                switch (M)
                {
                    case Method.E1:
                        A = E1;
                        break;
                    case Method.E2:
                        A = E2;
                        break;
                    case Method.H:
                        A = H;
                        break;
                    case Method.RK3:
                        A = RK3;
                        break;
                    case Method.RK4:
                        A = Rk4;
                        break;
                    case Method.P38:
                        A = P38;
                        break;
                    case Method.F:
                        A = F;
                        break;
                    default:
                        A = C;
                        break;
                }

                int r = A.RowCount - 1;
                if (A.RowCount != A.ColCount) r--;

                while (begin </*=*/end)
                {
                    double u = res.LastVal();
                    double t = res.LastArg();

                    double[] k = new double[r];
                    double tmp = 0, tmpp = 0;
                    int stepchange = 0;

                    //double[] h2 = new double[2];

                    //Get2Tmp(ref tmp, ref tmpp, step/2, A, k, f, u, t, r);
                    //double uh1 = u + tmp * step/2;
                    //Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, uh1, t+step/2, r);
                    //double uh2 = uh1 + tmp * step/2;


                    Get2Tmp(ref tmp, ref tmpp, step, A, k, f, u, t, r);
                    double val1 = u + step * tmp, val2 = u + step * tmpp;
                    double R = 0.2 * Math.Abs(val1 - val2);



                    if (controllingstep && stepchange <= MaxStepChange)
                    {
                        Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, u, t, r);
                        double uh1 = u + tmp * step / 2;
                        Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, uh1, t + step / 2, r);
                        double uh2 = uh1 + tmp * step / 2;

                        int p;
                        switch (M)
                        {
                            case Method.E1:
                                p = 1;
                                break;
                            case Method.E2:
                                p = 2;
                                break;
                            case Method.H:
                                p = 3;
                                break;
                            case Method.RK3:
                                p = 3;
                                break;
                            case Method.RK4:
                                p = 4;
                                break;
                            case Method.P38:
                                p = 4;
                                break;
                            case Method.F:
                                p = 3;
                                break;
                            default:
                                p = 4;
                                break;
                        }
                        double RR;
                        if (A.RowCount != A.ColCount)
                            RR = Math.Abs((uh2 - val2) / (1 - 1.0 / Math.Pow(2, p)));
                        else
                            RR = Math.Abs((uh2 - val1) / (1 - 1.0 / Math.Pow(2, p)));

                        /*if (RR < eps / 64) { step *= 2; stepchange++; }
                        else */
                        if (RR > eps) { step /= 2; stepchange++; }
                        else
                        {
                            begin += step;
                            step = thisstep;//возврат к исходному шагу
                            if (A.RowCount != A.ColCount) res.Add(new Point(begin, val2));
                            else res.Add(new Point(begin, val1));
                        }
                    }
                    else if (A.RowCount != A.ColCount && stepchange <= MaxStepChange)
                        if (R > eps)
                        {
                            step /= 2;
                            stepchange++;
                        }
                        else if (R <= eps / 64)
                        {
                            begin += step;
                            res.Add(new Point(begin, val2));
                            step *= 2;
                            stepchange++;
                        }
                        else
                        {
                            begin += step;
                            res.Add(new Point(begin, val2));
                        }
                    else
                    {
                        begin += step;
                        res.Add(new Point(begin, val1));
                    }

                    if (Math.Abs(end - begin) < step) step = Math.Abs(end - begin);
                }

                return res;
            }
            /// <summary>
            /// Решение приведённого ОДУ первого порядка
            /// </summary>
            /// <param name="f">Свободная функция переменных u и x, где u - искомая функция</param>
            /// <param name="begin">Начальный аргумент по задаче Коши</param>
            /// <param name="end">Конечный аргумент</param>
            /// <param name="stepcount">Количество шагов интегрирования</param>
            /// <param name="M">Метод поиска решения</param>
            /// <param name="begval">Значение функции при начальном аргументе</param>
            public static NetFunc ODUsearch(DRealFunc f, double begin = 0, double end = 10, int stepcount = 50, Method M = Method.E1, double begval = 1, double eps = 0.00001)
            {
                double step = (end - begin) / (stepcount);
                return ODUsearch(f, begin, end, step, M, begval);
            }
            private static void Get2Tmp(ref Vectors tmp, ref Vectors tmpp, double step, Matrix A, Vectors[] k, VRealFunc f, Vectors u, double t, int r)
            {
                tmp = new Vectors(tmp.Deg);
                tmpp = new Vectors(tmp.Deg);
                for (int i = 0; i < k.Length; i++)
                {
                    Vectors tmp2 = new Vectors(tmp.Deg);
                    for (int j = 0; j < i; j++)
                        tmp2 += A[i, j + 1] * k[j];
                    k[i] = f(t + A[i, 0] * step, u + tmp2 * step);
                    tmp += A[r, i + 1] * k[i];
                    if (A.RowCount != A.ColCount) tmpp += A[r + 1, i + 1] * k[i];
                }
            }
            /// <summary>
            /// Решение системы ОДУ первого порядка
            /// </summary>
            /// <param name="f">Свободная функция переменных u и x, где u - искомая функция</param>
            /// <param name="begin">Начальный аргумент по задаче Коши</param>
            /// <param name="end">Конечный аргумент</param>
            /// <param name="step">Шаг интегрирования</param>
            /// <param name="M">Метод поиска решения</param>
            /// <param name="begval">Значение функции при начальном аргументе</param>
            /// <param name="eps">Допустимый уровень расчётных погрешностей</param>
            /// <returns></returns>
            public static VectorNetFunc ODUsearch(VRealFunc f, Vectors begval, double begin = 0, double end = 10, double step = 0.01, Method M = Method.E1, double eps = 0.0001, bool controllingstep = false)
            {
                //$"Вызов функции от вектора {begval}".Show();
                double thisstep = step;
                VectorNetFunc res = new VectorNetFunc();

                res.Add(new PointV(begin, begval));
                step *= Math.Sign(end - begin);

                Matrix A;
                switch (M)
                {
                    case Method.E1:
                        A = E1;
                        break;
                    case Method.E2:
                        A = E2;
                        break;
                    case Method.H:
                        A = H;
                        break;
                    case Method.RK3:
                        A = RK3;
                        break;
                    case Method.RK4:
                        A = Rk4;
                        break;
                    case Method.P38:
                        A = P38;
                        break;
                    case Method.F:
                        A = F;
                        break;
                    default:
                        A = C;
                        break;
                }

                int r = A.RowCount - 1;
                if (A.RowCount != A.ColCount) r--;

                while (begin </*=*/end)
                {
                    Vectors u = res.Last().Item2;
                    double t = res.Last().Item1;

                    Vectors[] k = new Vectors[r];
                    Vectors tmp = new Vectors(u.Deg), tmpp = new Vectors(u.Deg);
                    int stepchange = 0;

                    //double[] h2 = new double[2];

                    //Get2Tmp(ref tmp, ref tmpp, step/2, A, k, f, u, t, r);
                    //double uh1 = u + tmp * step/2;
                    //Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, uh1, t+step/2, r);
                    //double uh2 = uh1 + tmp * step/2;


                    Get2Tmp(ref tmp, ref tmpp, step, A, k, f, u, t, r);
                    Vectors val1 = u + step * tmp, val2 = u + step * tmpp;
                    double R = 0.2 * (val1 - val2).EuqlidNorm;


                    if (controllingstep && stepchange <= MaxStepChange)
                    {
                        Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, u, t, r);
                        Vectors uh1 = u + tmp * step / 2;
                        Get2Tmp(ref tmp, ref tmpp, step / 2, A, k, f, uh1, t + step / 2, r);
                        Vectors uh2 = uh1 + tmp * step / 2;

                        int p;
                        switch (M)
                        {
                            case Method.E1:
                                p = 1;
                                break;
                            case Method.E2:
                                p = 2;
                                break;
                            case Method.H:
                                p = 3;
                                break;
                            case Method.RK3:
                                p = 3;
                                break;
                            case Method.RK4:
                                p = 4;
                                break;
                            case Method.P38:
                                p = 4;
                                break;
                            case Method.F:
                                p = 3;
                                break;
                            default:
                                p = 4;
                                break;
                        }
                        double RR;
                        if (A.RowCount != A.ColCount)
                            RR = ((uh2 - val2) / (1 - 1.0 / Math.Pow(2, p))).EuqlidNorm;
                        else
                            RR = ((uh2 - val1) / (1 - 1.0 / Math.Pow(2, p))).EuqlidNorm;

                        /*if (RR < eps / 64) { step *= 2; stepchange++; }
                        else */
                        if (RR > eps) { step /= 2; stepchange++; }
                        else
                        {
                            begin += step;
                            step = thisstep;//возврат к исходному шагу
                            if (A.RowCount != A.ColCount) res.Add(new PointV(begin, val2));
                            else res.Add(new PointV(begin, val1));
                        }
                    }
                    else if (A.RowCount != A.ColCount && stepchange <= MaxStepChange)
                        if (R > eps)
                        {
                            step /= 2;
                            stepchange++;
                        }
                        else if (R <= eps / 64)
                        {
                            begin += step;
                            res.Add(new PointV(begin, val2));
                            step *= 2;
                            stepchange++;
                        }
                        else
                        {
                            begin += step;
                            res.Add(new PointV(begin, val2));
                        }
                    else
                    {
                        begin += step;
                        res.Add(new PointV(begin, val1));
                    }

                    if (Math.Abs(end - begin) < step) step = Math.Abs(end - begin);
                }

                return res;
            }

            /// <summary>
            /// Решение задачи о стрельбе
            /// </summary>
            /// <param name="f">Свободная функция в системе ОДУ</param>
            /// <param name="F">Функция из граничных условий</param>
            /// <param name="alp">Вектор альфа начального приближения при поиске корня</param>
            /// <param name="list">Промежуточный список вектор-функций</param>
            /// <param name="vlist">Промежуточный список векторов</param>
            /// <param name="netlist">Промежуточный список вектор-функций (так как делегаты передаются плохо)</param>
            /// <param name="begin">Начало отрезка задания аргумента</param>
            /// <param name="end">Конец отрезка задания аргумента</param>
            /// <param name="stepcount">Число шагов при решении ОДУ</param>
            /// <param name="M">Метод решения ОДУ</param>
            /// <param name="eps">Погрешность</param>
            /// <param name="l">Коэффициент для метода итераций</param>
            /// <param name="controlstep">Нужно ли следить за шагом при решении ОДУ</param>
            /// <returns></returns>
            public static VectorFunc ShootQu(VRealFunc f, TwoVectorToVector F, Vectors alp, out List<VectorFunc> list, out List<Vectors> vlist, out List<VectorNetFunc> netlist, double begin = 0, double end = 10, int stepcount = 50, Method M = Method.RK4, double eps = 1e-5, double l = 0.1, bool controlstep = false)
            {
                list = new List<VectorFunc>(); vlist = new List<Vectors>(); netlist = new List<VectorNetFunc>();
                double step = (end - begin) / (stepcount - 1);
                VRealFunc u = (double t, Vectors v) =>
                {
                    var k = ODUsearch(f, v, begin, end, step, M, eps, controlstep);
                    if (t == end) return k.Last().Item2;
                    double arg = begin; int i = 1;
                    while (arg < t) arg = k[i++].Item1;
                    return k[--i].Item2;
                };

                VectorToVector FF = (Vectors v) => F(v, u(end, v));
                Vectors xn;
                if (FF(alp).EuqlidNorm > eps)
                {
                    Vectors fx0 = FF(alp);//fx0.Show();
                    vlist.Add(fx0);
                    list.Add((double t) => u(t, new Vectors(fx0)));
                    VectorNetFunc tmp = new VectorNetFunc();
                    for (int i = 0; i < stepcount; i++)
                    {
                        double arg = begin + i * step;
                        tmp.Add(new Tuple<double, Vectors>(arg, list.Last()(arg)));
                    }
                    netlist.Add(tmp);
                    VectorToVector ef = (Vectors v) => v - l * FF(v);

                    //double stnorm, stvector, neunorm, neuvector;
                    bool delay = true;
                    while ((ef(fx0) - fx0).EuqlidNorm > eps && delay)
                    {
                        xn = ef(fx0) - fx0;
                        Console.WriteLine($"Norm f(v)-v ={xn.EuqlidNorm}");//fx0.Show();
                        fx0 = ef(fx0);
                        vlist.Add(fx0);//fx0.Show();
                        list.Add(new VectorFunc((double t) => u(t, fx0)));
                        //$"list({3}) = {list.Last()(3)}".Show();
                        tmp = new VectorNetFunc();
                        for (int i = 0; i < stepcount; i++)
                        {
                            double arg = begin + i * step;
                            tmp.Add(new Tuple<double, Vectors>(arg, list.Last()(arg)));
                        }
                        netlist.Add(tmp);
                        if ((ef(fx0) - fx0 - xn).EuqlidNorm < eps / 1e4) delay = false;
                    }
                }
                else
                {
                    vlist.Add(alp);
                    list.Add((double t) => u(t, new Vectors(alp)));

                    VectorNetFunc tmp = new VectorNetFunc();
                    for (int i = 0; i < stepcount; i++)
                    {
                        double arg = begin + i * step;
                        tmp.Add(new Tuple<double, Vectors>(arg, list.Last()(arg)));
                    }
                    netlist.Add(tmp);
                }

                return list.Last();

            }

            /// <summary>
            /// Решение задачи Штурма-Лиувилля
            /// </summary>
            /// <param name="g">Функция внутри второй производной</param>
            /// <param name="h">Функция при первой производной</param>
            /// <param name="s">Функция при искомой функции</param>
            /// <param name="f">Свободная функция</param>
            /// <param name="a">Начало отрезка</param>
            /// <param name="b">Конец отрезка</param>
            /// <param name="N">Число шагов</param>
            /// <param name="A"></param>
            /// <param name="B"></param>
            /// <param name="C"></param>
            /// <param name="D"></param>
            /// <param name="A1"></param>
            /// <param name="B1"></param>
            /// <param name="C1"></param>
            /// <param name="D1"></param>
            /// <returns></returns>
            public static NetFunc SchLiuQu(Func<double,double> g, Func<double,double> h, Func<double,double> s, Func<double,double> f, out double nevaska, double a = 0, double b = 10, int N = 50, double A = 1, double B = 1, double D = 1, double A1 = 1, double B1 = 1, double D1 = 1, bool firstkind = true)
            {
                double[] hn = new double[N + 1], fn = new double[N + 1], sn = new double[N + 1], tn = new double[N + 1], an = new double[N + 1], bn = new double[N + 1], cn = new double[N + 1], dn = new double[N + 1];
                double t = (b - a) / N;

                for (int i = 0; i < N + 1; i++)
                {
                    double arg = a + i * t;
                    tn[i] = arg;
                    hn[i] = h(arg);
                    fn[i] = f(arg);
                    sn[i] = s(arg);
                    an[i] = (g(arg - t / 2) / t - hn[i] / 2) / t;
                    cn[i] = (g(arg + t / 2) / t + hn[i] / 2) / t;
                    bn[i] = an[i] + cn[i] - sn[i];//поставил sn вместо hn
                                                  //bn[i] = (g(arg + t / 2) - g(arg - t / 2)) / t / t - sn[i];
                    dn[i] = fn[i];
                }

                double k1 = 0, k2 = 0;

                if (firstkind)
                {
                    bn[0] = A / t - B; cn[0] = A / t; dn[0] = D;
                    an[N] = -A1 / t; bn[N] = -A1 / t - B1; dn[N] = D1;
                }
                else
                {
                    bn[0] = 3 * A / 2 / t - B; cn[0] = 2 * A / t; k1 = -A / 2 / t;
                    bn[N] = -3 * A1 / 2 / t - B1; an[N] = -2 * A1 / t; k2 = A1 / 2 / t;
                }

                dn[0] = D;
                dn[N] = D1;

                SLAU S = new SLAU(N + 1);
                S.A[0, 0] = -bn[0];
                S.A[0, 1] = cn[0]; S.A[0, 2] = k1;
                S.A[N, N - 1] = an[N]; S.A[N, N - 2] = k2;
                S.A[N, N] = -bn[N];
                S.b[0] = dn[0]; S.b[N] = dn[N];
                for (int i = 1; i < N; i++)
                {
                    S.A[i, 0 + i - 1] = an[i];
                    S.A[i, 1 + i - 1] = -bn[i];
                    S.A[i, 2 + i - 1] = cn[i];
                    S.b[i] = dn[i];
                }

                S.Show(); "".Show();

                double c1 = S.A[0, 2] / S.A[1, 2], c2 = S.A[N, N - 2] / S.A[N - 1, N - 2];
                for (int i = 0; i < 3; i++)
                {
                    S.A[0, i] -= S.A[1, i] * c1;
                    S.A[N, N - i] -= S.A[N - 1, N - i] * c2;
                }
                S.b[0] -= S.b[1] * c1; S.b[N] -= S.b[N - 1] * c2;

                //S.Show(); "".Show();

                S.ProRace(); S.Show(); nevaska = S.Nevaska;

                NetFunc res = new NetFunc();
                for (int i = 0; i < N + 1; i++)
                    res.Add(new Point(tn[i], S.x[i]));
                return res;
            }

            /// <summary>
            /// Решение уравнения теплопроводности явной либо неявной схемой
            /// </summary>
            /// <param name="f">Свободная фунция из уравнения</param>
            /// <param name="f1">Функция из первого краевого условия</param>
            /// <param name="f2">Функция из второго краевого условия</param>
            /// <param name="u0">Функция из начальных условий</param>
            /// <param name="u">Искомая функция (нужна для вычисления точности самого решения)</param>
            /// <param name="a">Коэффициент при второй производной</param>
            /// <param name="A1"></param>
            /// <param name="B1"></param>
            /// <param name="A2"></param>
            /// <param name="B2"></param>
            /// <param name="x0">Начало отрезка по пространству</param>
            /// <param name="X">Конец отрезка по пространству</param>
            /// <param name="t0">Начало отрезка по времени</param>
            /// <param name="T">Конец отрезка по времени</param>
            /// <param name="xcount">Число шагов по пространству</param>
            /// <param name="tcount">Число шагов по времени</param>
            /// <param name="accuracy">Выводимая точность</param>
            /// <param name="explict">Использовать явную схему либо нет</param>
            /// <returns></returns>
            public static List<NetFunc> TU(DRealFunc f, Func<double,double> f1, Func<double,double> f2, Func<double,double> u0, DRealFunc u, double a, double A1, double B1, double A2, double B2, double x0, double X, double t0, double T, int xcount, int tcount, out double accuracy, bool explict = true, bool thirdkind = true)
            {
                List<NetFunc> res = new List<NetFunc>();
                double h = (X - x0) / (xcount - 1);
                double tau = (T - t0) / (tcount - 1);
                double[] x = new double[xcount], t = new double[tcount];
                double[] value = new double[xcount];

                for (int i = 0; i < xcount; i++)
                    x[i] = x0 + i * h;
                for (int i = 0; i < tcount; i++)
                    t[i] = t0 + i * tau;

                for (int i = 0; i < xcount; i++)
                    value[i] = u0(x[i]);
                res.Add(new NetFunc(x, value));

                double th = tau / h / h;
                double h1 = A1 + h * B1, h2 = A2 + h * B2; //(h1*h1.Reverse()).Show();

                if (explict)
                    for (int i = 1; i < tcount; i++)
                    {
                        for (int j = 1; j < xcount - 1; j++)
                            value[j] = res[i - 1].Values[j] + a * th * (res[i - 1].Values[j - 1] - 2 * res[i - 1].Values[j] + res[i - 1].Values[j + 1]) + tau * f(t[i/*-1*/], x[j]);
                        if (thirdkind)
                        {
                            value[0] = (A1 * value[1] + h * f1(t[i])) / h1;
                            value[xcount - 1] = (A2 * value[xcount - 2] + h * f2(t[i])) / h2;
                        }
                        else
                        {
                            value[0] = f1(t[i]) / B1;
                            value[xcount - 1] = f2(t[i]) / B2;
                        }

                        res.Add(new NetFunc(x, value));
                        //new Vectors (res.Last().Values).Show();
                    }
                else
                {
                    SLAU s = new SLAU(xcount);

                    for (int i = 1; i < tcount; i++)
                    {
                        if (thirdkind)
                        {
                            s.A[0, 0] = h1; s.A[0, 1] = -A1; s.b[0] = h * f1(t[i]);
                            s.A[xcount - 1, xcount - 1] = h2; s.A[xcount - 1, xcount - 2] = -A2; s.b[xcount - 1] = h * f2(t[i]);
                        }
                        else
                        {
                            s.A[0, 0] = B1; s.b[0] = f1(t[i]);
                            s.A[xcount - 1, xcount - 1] = B2; s.b[xcount - 1] = f2(t[i]);
                        }


                        for (int j = 1; j < xcount - 1; j++)
                        {
                            s.A[j, j - 2 + 1] = -a * th;
                            s.A[j, j - 2 + 2] = a * 2 * th + 1;
                            s.A[j, j - 2 + 3] = -a * th;
                            s.b[j] = res[i - 1].Values[j - 2 + 2] + tau * f(t[i/*-1*/], x[j]);
                        }

                        s.ProRace();
                        // "".Show();
                        //s.Show();
                        value = s.x;
                        res.Add(new NetFunc(x, value));
                    }
                }

                accuracy = 0;
                double[,] ac = new double[tcount, xcount];
                for (int i = 0; i < t.Length; i++)
                    for (int j = 0; j < x.Length; j++)
                        ac[i, j] = Math.Abs(u(t[i], x[j]) - res[i].Values[j]);
                accuracy = new Matrix(ac).Max;

                return res;
            }
        }
    }
}

