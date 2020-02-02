using System;
using System.Text;
using System.IO;
using System.Drawing;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;

namespace МатКлассы
{
    #region Делегаты и перечисления
    /// <summary>
    /// Комплексная функция комплексного аргумента
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Complex ComplexFunc(Complex x);
    /// <summary>
    /// Действительные функции от точки
    /// </summary>
    /// <param name="x">Аргумент - пара действительных чисел (x,y), реализованная как точка Point</param>
    /// <returns></returns>
    public delegate double Functional(Point x);
    /// <summary>
    /// Действительная функция двух переменных
    /// </summary>
    /// <param name="u"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate double DRealFunc(double x, double u);
    /// <summary>
    /// Комплекснозначная функция двух действительных переменных
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public delegate Complex DComplexFunc(double x, double z);
    /// <summary>
    /// Комплексная функция двух переменных
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public delegate Complex DoubleComplexFunc(Complex a, Complex b);
    /// <summary>
    /// Вектор-функция от вектора и параметра
    /// </summary>
    /// <param name="x"></param>
    /// <param name="u"></param>
    /// <returns></returns>
    public delegate Vectors VRealFunc(double x, Vectors u);
    /// <summary>
    /// Вектор-функция
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Vectors VectorFunc(double x);
    /// <summary>
    /// Функция из Rn в Rn
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate Vectors VectorToVector(Vectors v);
    /// <summary>
    /// Функция двух векторов, выдающая вектор
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public delegate Vectors TwoVectorToVector(Vectors a, Vectors b);
    /// <summary>
    /// Действительная функция векторного аргумента
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate double AntiVectorFunc(Vectors v);
    /// <summary>
    /// Действительная функция комплексного переменного
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public delegate double RealFuncOfCompArg(Complex z);
    /// <summary>
    /// Действительная функция трёх аргументов, необходимая для вычисления площади сегментов с параметрами tx, ty при радиусе кривой r
    /// </summary>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public delegate double TripleFunc(double tx, double ty, double r);
    /// <summary>
    /// Функция многих аргументов
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate double MultiFunc(params double[] x);
    /// <summary>
    /// Действительная функция из какой-то системы функций
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="k">Номер функции в системе</param>
    /// <returns></returns>
    public delegate double SequenceFunc(double x, int k);
    /// <summary>
    /// Действительная функция от точки из системы функций
    /// </summary>
    /// <param name="z"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    public delegate double SeqPointFunc(Point z, int k);
    /// <summary>
    /// Полином из системы полиномов
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public delegate Polynom SequencePol(int k);
    /// <summary>
    /// Функция, возвращающая точку в зависимости от параметра
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate Point PointFunc(double t);
    /// <summary>
    /// Функция, возвращающая точку в зависимости от двух параметров
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate Point DPointFunc(double t, double r);

    /// <summary>
    /// Перечисление "род": для криволинейных интегралов, полиномов Чебышёва и т.д.
    /// </summary>
    public enum Kind : byte { FirstKind, SecondKind };
    /// <summary>
    /// Ортогональные функции, ортонормированные, неортогональные
    /// </summary>
    public enum SequenceFuncKind : byte { Orthogonal, Orthonormal, Other };

#endregion

    /// <summary>
    /// Вероятности
    /// </summary>
    public class Probability
    {
        /// <summary>
        /// Абстрактный класс случайной величины
        /// </summary>
        public abstract class RandVal
        {
            /// <summary>
            /// Математическое ожидание
            /// </summary>
            public abstract double M { get; }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public abstract double Dis { get; }
        }

        /// <summary>
        /// Дискретная случайная величина
        /// </summary>
        public class DisRandVal : RandVal
        {
            /// <summary>
            /// Значения случайной величины
            /// </summary>
            Vectors X;
            /// <summary>
            /// Значения вероятностей
            /// </summary>
            Vectors p;
            /// <summary>
            /// Функция распределения
            /// </summary>
            Func<double,double> F = null;

            //Конструкторы

            /// <summary>
            /// Конструктор по массиву вероятностей
            /// </summary>
            /// <param name="a"></param>
            public DisRandVal(double[] a)
            {
                if (!ProbOne(a)) throw new Exception("Сумма элементов в массиве не равна 1");
                X = new Vectors(a.Length);
                p = new Vectors(a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    X[i] = i + 1;
                    p[i] = a[i];
                }
            }
            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            /// <param name="n"></param>
            public DisRandVal(int n)
            {
                double[] a = new double[n];
                X = new Vectors(n); p = new Vectors(n);
                double val = 1.0 / n;
                for (int i = 0; i < n; i++) a[i] = val;
                DisRandVal r = new DisRandVal(a);
                this.X = r.X;
                this.p = r.p;
            }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="r"></param>
            public DisRandVal(DisRandVal r) { X = new Vectors(r.X); p = new Vectors(r.p); }
            /// <summary>
            /// Чтение из файла
            /// </summary>
            /// <param name="fs"></param>
            public DisRandVal(StreamReader fs)
            {
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                int n = st.Length;
                this.X = new Vectors(n); this.p = new Vectors(n);
                for (int i = 0; i < n; i++) X[i] = Convert.ToDouble(st[i]);
                s = fs.ReadLine();
                st = s.Split(' ');
                for (int i = 0; i < n; i++) p[i] = Convert.ToDouble(st[i]);
                if (!ProbOne(this.p.vector)) throw new Exception("Сумма элементов в массиве не равна 1");
                fs.Close();
            }

            //Свойства
            /// <summary>
            /// Функция распределения дискретной случайной величины
            /// </summary>
            public Func<double,double> FDist
            {
                get
                {
                    return (double x) =>
                {
                    if (x < this.X[0]) return 0;
                    if (x > this.X[p.Deg - 1]) return 1;
                    double k = this.p[0];
                    for (int i = 1; i < this.p.Deg; i++)
                    {
                        if (x <= this.X[i]) return k;
                        k += this.p[i];
                    }
                    return k;
                };
                }
            }

            //методы
            /// <summary>
            /// Подходит ли массив под массив вероятностей
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            private bool ProbOne(double[] k)
            {
                double sum = 0;
                for (int i = 0; i < k.Length; i++) sum += k[i];
                if (sum == 1) return true;
                return false;
            }
            /// <summary>
            /// Проиллюстрировать
            /// </summary>
            public void Show()
            {
                this.X.PrintMatrix();
                this.p.PrintMatrix();
            }
            /// <summary>
            /// Мат. ожидание этой СВ
            /// </summary>
            /// <returns></returns>
            public override double M
            {
                get
                {
                    DisRandVal R = new DisRandVal(this);
                    return DisRandVal.MatExp(R);
                }
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public override double Dis
            {
                get { return DisRandVal.Dispersion(this); }
            }
            /// <summary>
            /// Мат. ожидание СВ
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double MatExp(DisRandVal R)
            {
                double sum = 0;
                for (int i = 0; i < R.X.Deg; i++) sum += R.X[i] * R.p[i];
                return sum;
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double Dispersion(DisRandVal R) { return CenM(R, 2); }
            /// <summary>
            /// Начальный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double BegM(DisRandVal R, int n) { return MatExp((R) ^ n); }
            /// <summary>
            /// Центральный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double CenM(DisRandVal R, int n) { return MatExp((R - R.M) ^ n); }
            /// <summary>
            /// По неравенству Чебышева вероятность того, что случайная величина отклонится от мат. ожидания не менее чем на eps
            /// </summary>
            /// <param name="R"></param>
            /// <param name="eps"></param>
            public static void NerCheb(DisRandVal R, double eps) { Console.WriteLine("<= {0}", R.Dis / eps / eps); }

            //операторы
            /// <summary>
            /// Смещение СВ
            /// </summary>
            /// <param name="A"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public static DisRandVal operator -(DisRandVal A, double m)
            {
                DisRandVal M = new DisRandVal(A);
                for (int i = 0; i < M.X.Deg; i++) M.X[i] -= m;
                return M;
            }
            /// <summary>
            /// Случайная величина в степени
            /// </summary>
            /// <param name="A"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static DisRandVal operator ^(DisRandVal A, int n)
            {
                DisRandVal M = new DisRandVal(A);
                for (int i = 0; i < M.X.Deg; i++) M.X[i] = Math.Pow(M.X[i], n);
                return M;
            }
        }

        /// <summary>
        /// Непрерывная случайная величина
        /// </summary>
        public class ConRandVal : RandVal
        {
            /// <summary>
            /// Тип распределения (нормальное, равномерное, пуассоновское, экспоненциальное и т. д.)
            /// </summary>
            public enum BasisDistribution : byte { Normal, Uniform, Puasson, Exp, Other };

            /// <summary>
            /// Вспомогательная функция
            /// </summary>
            private Func<double,double> x = (double t) => { return t; };

            /// <summary>
            /// Функция распределения
            /// </summary>
            private Func<double,double> F = null;
            /// <summary>
            /// Плотность распределения
            /// </summary>
            private Func<double,double> f = null;
            private BasisDistribution TypeValue = BasisDistribution.Other;
            /// <summary>
            /// Пока не известные мат. ожидание и дисперсия
            /// </summary>
            private double? m = null, d = null;

            //Конструкторы
            /// <summary>
            /// Конструктор по функции распределения и плотности распределения
            /// </summary>
            /// <param name="A"></param>
            /// <param name="a"></param>
            public ConRandVal(Func<double,double> A, Func<double,double> a) { F = A; f = a; }//по обеим функциям
                                                                       /// <summary>
                                                                       /// Конструктор только по плотности распределению
                                                                       /// </summary>
                                                                       /// <param name="a"></param>
            public ConRandVal(Func<double,double> a) { f = a;/* F = (double t) => { return DefInteg.};*/ }//по плотности распределения
                                                                                               /// <summary>
                                                                                               /// Конструктор копирования
                                                                                               /// </summary>
                                                                                               /// <param name="S"></param>
            public ConRandVal(ConRandVal S) { this.f = S.f; this.F = S.F; this.x = S.x; this.TypeValue = S.TypeValue; }
            /// <summary>
            /// Конструктор по одному из основных распределений с двумя аргументами
            /// </summary>
            /// <param name="Type"></param>
            /// <param name="m"></param>
            /// <param name="D"></param>
            public ConRandVal(BasisDistribution Type, double m, double D)
            {
                switch (Type)
                {
                    //Нормальное распределение
                    case BasisDistribution.Normal:
                        this.f = (double s) => { return 1.0 / Math.Sqrt(1 * Math.PI * D) * Math.Exp(-1.0 / 2 / D * (s - m) * (s - m)); };
                        this.m = m;
                        this.d = D;
                        this.F = (double x) => { return FuncMethods.DefInteg.Simpson((double t) => { return Math.Exp(-t * t / 2); }, 0, x); };
                        return;
                    //Равномерное распределение
                    case BasisDistribution.Uniform:
                        this.f = (double s) => { return 1.0 / (D - m); };
                        this.m = (D + m) / 2;
                        this.d = (D - m) * (D - m) / 12;
                        this.F = (double s) =>
                        {
                            if (s < m) return 0;
                            if (m < s && s <= D) return (s - m) / (D - m);
                            return 1;
                        };
                        return;
                    //Распределение Пуассона
                    case BasisDistribution.Puasson:
                        int m_new = (int)m;
                        double tmp = Math.Exp(-D);
                        this.f = (double s) => { return Math.Pow(D, m_new) / Combinatorik.P(m_new) * tmp; };
                        this.m = D;
                        this.d = D;
                        return;
                    default:
                        throw new Exception("Такого конструктора не существует");

                }
            }
            /// <summary>
            /// Конструктор по параметру экспоненциального распределния
            /// </summary>
            /// <param name="l"></param>
            public ConRandVal(double l)
            {
                this.f = (double s) =>
                {
                    if (s < 0) return 0;
                    return l * Math.Exp(-l * s);
                };
                this.m = 1 / l;
                this.d = 1 / l / l;

                this.F = (double s) =>
                {
                    if (s < 0) return 0;
                    return 1 - Math.Exp(-l * s);
                };
            }
            /// <summary>
            /// Конструктор нормального распределения по умолчанию
            /// </summary>
            public ConRandVal()
            {
                ConRandVal T = new ConRandVal(BasisDistribution.Normal, 0, 1);
                this.f = T.f; this.m = T.m; this.d = T.d;
            }

            //Операторы
            /// <summary>
            /// Случайная величина в степени
            /// </summary>
            /// <param name="a"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static ConRandVal operator ^(ConRandVal a, int n)
            {
                ConRandVal S = new ConRandVal(a);
                S.x = (double t) => { return Math.Pow(t, n); };
                return S;
            }
            /// <summary>
            /// Сдвиг случайной величины
            /// </summary>
            /// <param name="A"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public static ConRandVal operator -(ConRandVal A, double m)
            {
                ConRandVal S = new ConRandVal(A);
                S.x = (double t) => { return t - m; };
                return S;
            }

            //Методы
            /// <summary>
            /// Мат. ожидание
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double MatExp(ConRandVal R)
            {
                Func<double,double> xf = (double t) => { return R.x(t) * R.f(t); };
                return FuncMethods.DefInteg.ImproperFirstKind(xf);
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double Dispersion(ConRandVal R) { return /*CenM(R, 2);*/ MatExp(R ^ 2) - R.M * R.M; }
            /// <summary>
            /// Начальный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double BegM(ConRandVal R, int n) { return MatExp((R) ^ n); }
            /// <summary>
            /// Центральный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double CenM(ConRandVal R, int n) { return MatExp((R - R.M) ^ n); }
            /// <summary>
            /// По неравенству Чебышева вероятность того, что случайная величина отклонится от мат. ожидания не менее чем на eps
            /// </summary>
            /// <param name="R"></param>
            /// <param name="eps"></param>
            public static void NerCheb(ConRandVal R, double eps) { Console.WriteLine("<= {0}", R.Dis / eps / eps); }
            /// <summary>
            /// Вывести на консоль информацию о случайной величине
            /// </summary>
            public void Show()
            {
                Console.WriteLine("Мат. ожидание: {0} ; дисперсия: {1} ; тип распределения: {2}", this.M, this.Dis, this.TypeValue);
            }
            /// <summary>
            /// Вероятность попадания случайной величины в интервал
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public double P(double a, double b)
            {
                if (this.F != null) return F(b) - F(a);
                return FuncMethods.DefInteg.Simpson(this.f, a, b);
            }

            //Свойства
            /// <summary>
            /// Мат. ожидание
            /// </summary>
            public override double M
            {
                get
                {
                    if (this.m != null) return (double)this.m;
                    ConRandVal R = new ConRandVal(this);
                    this.m = ConRandVal.MatExp(R);
                    return (double)this.m;
                }
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public override double Dis
            {
                get
                {
                    if (this.d != null) return (double)this.d;
                    this.d = ConRandVal.Dispersion(this);
                    return (double)this.d;
                }
            }

            //Константы класса
        }
    }
}

