using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using МатКлассы;
using System.Linq;


namespace МатКлассы
{
    /// <summary>
    /// Класс с вейвлетным преобразованием
    /// </summary>
    public sealed class Wavelet : IDisposable
    {
        private static readonly double sqrt2pi = Math.Sqrt(2 * Math.PI);
        private static readonly double frac1sqrt2pi = 1.0 / sqrt2pi;
        private static readonly Func<Complex, Complex> sigma =
            (Complex z) => SumsAndLimits.Sum(1, n =>
            {
                int nsqr = n * n;
                return Complex.Sin(Math.PI * z * nsqr) / nsqr;
            }, eps, ndo: 12, ndomax: 150);


        /// <summary>
        /// Число узлов для интегрирования
        /// </summary>
        public static DefInteg.GaussKronrod.NodesCount countNodes = DefInteg.GaussKronrod.NodesCount.GK61;
        /// <summary>
        /// Коллекция нормирующих множителей
        /// </summary>
        public static ConcurrentDictionary<Wavelets, double> Cpsi = new ConcurrentDictionary<Wavelets, double>(2, 6);

        /// <summary>
        /// Частота (нужна только для вейвлета Морле)
        /// </summary>
        public readonly double w;
        /// <summary>
        /// Материнский вейвлет/анализирующий вейвлет
        /// </summary>
        //tex:$\psi : R \rightarrow C$
        private readonly Func<double, Complex> Mother;
        /// <summary>
        /// Фурье-образ материнского вейвлета
        /// </summary>
        private readonly Func<Complex, Complex> FMother;
        /// <summary>
        /// Тип исходного вейвлета
        /// </summary>
        public Wavelets Type { get; }

        /// <summary>
        /// Допустимая погрешность
        /// </summary>
        public static double eps = 1e-15;

        /// <summary>
        /// Перечисление доступных вейвлетов
        /// </summary>
        public enum Wavelets : byte
        {
            /// <summary>
            /// Гауссов вейвлет первого порядка
            /// </summary>
            WAVE,
            /// <summary>
            /// Мексиканская шляпа
            /// </summary>
            MHAT,
            /// <summary>
            /// "difference of gaussians"
            /// </summary>
            DOG,
            /// <summary>
            /// "Littlewood & Paley"
            /// </summary>
            LP,
            /// <summary>
            /// Хаар-вейвлет
            /// </summary>
            HAAR,
            /// <summary>
            /// Французская шляпа
            /// </summary>
            FHAT,
            /// <summary>
            /// Вейвлет Морле
            /// </summary>
            Morlet,
            /// <summary>
            /// Вейвлет Габора
            /// </summary>
            Gabor
        }

        /// <summary>
        /// Создание вейвлета по масштабному множителю с указанием вейвлета из перечисления
        /// </summary>
        /// <param name="W"></param>
        /// <param name="ww"></param>
        /// <param name="k"></param>
        public Wavelet(Wavelets W = Wavelets.MHAT, double ww = 1)
        {
            this.w = ww;
            this.Type = W;
            switch (W)
            {
                case Wavelets.WAVE:
                    this.Mother = (double t) => -t * Math.Exp(-t * t / 2);
                    this.FMother = (Complex w) => Complex.I * w * sqrt2pi * Complex.Exp(-w * w / 2);
                    break;
                case Wavelets.MHAT:
                    this.Mother = (double t) => { double sqr = t * t; return (1 - sqr) * Math.Exp(-sqr / 2); };
                    this.FMother = (Complex w) =>
                    {
                        var sqr = -w * w;
                        return sqr * sqrt2pi * Complex.Exp(sqr / 2);
                    };
                    break;
                case Wavelets.DOG:
                    this.Mother = (double t) => { double sqr = -t * t / 2; return Math.Exp(sqr) - 0.5 * Math.Exp(sqr / 4); };
                    this.FMother = (Complex w) =>
                    {
                        var sqr = -w * w;
                        return sqrt2pi * (Complex.Exp(sqr / 2) - Complex.Exp(2 * sqr));
                    };
                    break;
                case Wavelets.LP:
                    this.Mother = t => 
                    {
                        double pt = t * Math.PI;
                        return (2.0*Math.Cos(pt) - 1.0)*Math.Sin(pt) / pt;
                    };
                    this.FMother = (Complex w) =>
                    {
                        double tmp = w.Abs;
                        if (tmp <= 2 * Math.PI && tmp >= Math.PI)
                            return frac1sqrt2pi;
                        return 0;
                    };
                    break;
                case Wavelets.HAAR:
                    this.Mother = t =>
                    {
                        if (t >= 0)
                        {
                            if (t <= 0.5) return 1;
                            if (t <= 1) return -1;
                            return 0;
                        }
                        return 0;
                    };
                    this.FMother = (Complex w) => 4 * Complex.I * Complex.Exp(Complex.fracI2 * w) / w * Complex.Sin(w / 4).Sqr();
                    break;
                case Wavelets.FHAT:
                    this.Mother = t =>
                    {
                        double q = t.Abs();
                        if (q <= 1.0 / 3) return 1;
                        if (q <= 1) return -0.5;
                        return 0;
                    };
                    this.FMother = (Complex w) => 4 * Complex.Sin(w / 3).Pow(3) / w;
                    break;
                case Wavelets.Morlet:
                    this.Mother = t => Math.Exp(-t.Sqr() / 2) * Complex.Exp(Complex.I * w * t);
                    this.FMother = (Complex w) => sigma(w) * sqrt2pi * Complex.Exp(-(w - this.w).Sqr() / 2);
                    break;
                case Wavelets.Gabor:
                    const double w_0 = 2 * Math.PI;
                    double gamma = Math.PI * Math.Sqrt(2.0 / Math.Log(2));
                    double fracw0gamma = Math.Sqrt(2 * Math.Log(2));
                    double fracgammaw0 = 1.0 / fracw0gamma;
                    double sqrfracw0gamma = fracw0gamma * fracw0gamma;
                    double sqrfracgammaw0 = fracgammaw0 * fracgammaw0;
                    double fracpis = Math.Pow(Math.PI, -0.25);
                    double sqrtfracw0gammapis = fracpis * Math.Sqrt(fracw0gamma);
                    double fracs2pipis = sqrt2pi / fracpis;
                    double Gabortmp = -sqrfracw0gamma / 2;
                    this.Mother = t => sqrtfracw0gammapis * Math.Exp(Gabortmp * t * t) * Complex.Expi(w_0 * t);
                    this.FMother = (Complex w) => fracs2pipis * Math.Sqrt(fracgammaw0) * Complex.Exp(-sqrfracgammaw0 / 2 * (w - w_0).Sqr());
                    break;
            }
        }
        /// <summary>
        /// Создать вейвлет
        /// </summary>
        /// <param name="W"></param>
        /// <param name="k"></param>
        /// <param name="ww"></param>
        /// <returns></returns>
        public static Wavelet Create(Wavelets W = Wavelets.MHAT, double ww = 1) => new Wavelet(W, ww);

        /// <summary>
        /// Функция, получившаяся при последнем анализе 
        /// </summary>
        public Func<double, double, Complex> ResultMemoized = null;
        private Memoize<Point, Complex> Resultmems = null;

        /// <summary>
        /// Вейвлет-образ указанной функции
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public Func<double, double, Complex> GetAnalys(Func<double, double> f)
        {
            //tex: $Wf(a,b) = \dfrac{1}{|a|^{0.5}} \int_{-\infty}^{\infty} f(t) {\bar \psi(\dfrac{t-b}{a}) dt}$, еще написано, что a>0, но тогда зачем модуль
            Func<double, double, Complex> s = (double a, double b) =>
               {
                   Func<Complex, Complex> F1 = (Complex t) => f(t.Re) * this.Mother((t.Re - b) / a).Conjugate;
                   Func<Complex, Complex> F2 = (Complex t) => f(-t.Re) * this.Mother((-t.Re - b) / a).Conjugate;
                   double con = 1.0 / Math.Sqrt(Math.Abs(a));
                   Complex
                   t1 = DefInteg.GaussKronrod.DINN_GK(F1, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: countNodes),
                   t2 = DefInteg.GaussKronrod.DINN_GK(F2, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: countNodes);

                   return con * (t1 + t2);
               };

            return MemoizeAndReturn(s);
        }
        /// <summary>
        /// Вейвлет-преобразование от массива точек по формулам Котеса
        /// </summary>
        /// <param name="farr">Временной ряд</param>
        /// <param name="tmin">Минимум по времени</param>
        /// <param name="tmax">Максимум по времени</param>
        /// <param name="epsForWaveletValues">Ограничение на интегрирование</param>
        /// <returns></returns>
        public Func<double, double, Complex> GetAnalys(Point[] farr, double tmin = double.NaN, double tmax = double.NaN, double epsForWaveletValues = 0)
        {
            var inds = GetMinMaxIndexies(farr, tmin, tmax);

            double h3 = Math.Abs(farr[1].x - farr[0].x) / 3;

            var ind = Array.IndexOf(farr, farr.First(point => point.y != 0));
            var f = farr.Slice(
                Math.Max(ind == 0 ? 0 : ind - 1, inds.Item1),
               Math.Min(farr.Length - 1, inds.Item2));
            //var f = farr;
            int n = (f.Length - 1) / 2;
            int up = Math.Min( 150,n-1);

            //tex: $Wf(a,b) = \dfrac{1}{|a|^{0.5}} \int_{-\infty}^{\infty} f(t) {\bar \psi(\dfrac{t-b}{a}) dt}$, еще написано, что a>0, но тогда зачем модуль
            Func<double, double, Complex> s;
            if (epsForWaveletValues <= 0)
                s = (double a, double b) =>
                {
                    if (a == 0) return 0;
                    double con = h3 / Math.Sqrt(Math.Abs(a));

                    Complex sum0 = f[0].y * this.Mother((f[0].x - b) / a) + f[f.Length - 1].y * this.Mother((f[f.Length - 1].x - b) / a);
                    Complex sum = 0;
                    int i2;
                    for (int i = 1; i <= n - 1; i++)
                    {
                        i2 = 2 * i;
                        sum += f[i2].y * this.Mother((f[i2].x - b) / a) + 2 * f[i2 - 1].y * this.Mother((f[i2 - 1].x - b) / a);
                    }

                    if (f.Length % 2 == 1)
                        sum0 += 4 * f[f.Length - 2].y * this.Mother((f[f.Length - 2].x - b) / a);

                    return con * (2 * sum + sum0).Conjugate;
                };
            else
                s = (double a, double b) =>
                {
                    if (a == 0) return 0;
                    double con = h3 / Math.Sqrt(Math.Abs(a));
                    double arev = 1.0 / a;
                   
                    Complex sum0 = f[0].y * this.Mother((f[0].x - b)*arev) + f[f.Length - 1].y * this.Mother((f[f.Length - 1].x - b)*arev);
                    Complex sum = 0;
                    Complex tmp;
                    int i2;
                    Point p1, p2;

                    void niter(int i)
                    {
                        i2 = 2 * i;
                        p1 = f[i2 - 1];
                        p2 = f[i2];
                        tmp = this.Mother((p2.x - b)*arev);
                        sum += p2.y * tmp + 2.0 * p1.y * this.Mother((p1.x - b)*arev);
                    }

                    for (int i = 1; i <= up; i++)
                        niter(i);

                    for (int i = up+1; i <= n - 1; i++)
                    {
                        niter(i);
                        if (tmp.Re < epsForWaveletValues * sum.Re)//так будет быстрее
                            break;
                    }


                    if (f.Length % 2 == 1)
                        sum0 += 4 * f[f.Length - 2].y * this.Mother((f[f.Length - 2].x - b)*arev);

                    return con * (2 * sum + sum0).Conjugate;
                };

            return MemoizeAndReturn(s);
        }
        /// <summary>
        /// Возвратить индексы элементов массива точек, в которых компонента x наиболее близка к указанным значениям
        /// </summary>
        /// <param name="farr"></param>
        /// <param name="tmin"></param>
        /// <param name="tmax"></param>
        /// <returns></returns>
        private static Tuple<int, int> GetMinMaxIndexies(Point[] farr, double tmin, double tmax)
        {
            if (double.IsNaN(tmin))
                tmin = farr[0].x;
            if (double.IsNaN(tmax))
                tmax = farr.Last().x;
            var tmpmas = farr.Select(p => p.x).ToArray();
            var tmpvec = new Vectors(tmpmas);
            int indmin = Array.IndexOf(tmpmas, tmpvec.BinaryApproxSearch(tmin));
            int indmax = Array.IndexOf(tmpmas, tmpvec.BinaryApproxSearch(tmax));
            return new Tuple<int, int>(indmin, indmax);
        }

        /// <summary>
        /// Вейвлет-преобразование от замера, записанного в файл
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="step"></param>
        /// <param name="count"></param>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public Func<double, double, Complex> GetAnalys(double begin, double step, int count, double tmin, double tmax, string filename, string path, int byevery = 1, double epsForWaveletValues = 0) => GetAnalys(Point.CreatePointArray(begin, step, count, filename, path, byevery), tmin, tmax, epsForWaveletValues);
        private Func<double, double, Complex> MemoizeAndReturn(Func<double, double, Complex> s)
        {
            Resultmems = new Memoize<Point, Complex>((Point p) => s(p.x, p.y));
            ResultMemoized = new Func<double, double, Complex>((double a, double b) => Resultmems.Value(new Point(a, b)));
            return ResultMemoized;
        }

        /// <summary>
        /// Обратное вейвлет-преобразование указанной функции
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        /// <remarks>Сам несобственный интеграл считается параллельно, так что рисовать эту функцию лучше последовательно, что ввиду мемоизации будет раз в 6 быстрее, чем рисовать параллельно и считать последовательно</remarks>
        public Func<double, double> GetSyntesis(Func<double, double, Complex> F = null)
        {
            //вычисление коэффициента С
            //надо добавить какое-нибудь ограничение на <inf 
            Complex C = Ccoef;

            //tex:$ f(t) =\dfrac{1}{C} \int_{R_* \times R} Wf(a,b) \psi_{a,b}(t) \dfrac{da db}{a^2}$
            Func<double, double> GetRes(Func<Point, Complex> func) =>
                (double t) => (MathNet.Numerics.Integration.GaussLegendreRule.Integrate((x, y) => (this.Mother((t - y) / x) * func(new Point(x, y)) / x / x).Re, 0.01, 3, -8, 8, 96) / C).Re / 2;
            //(DefInteg.DoubleIntegralIn_FULL(
            //    (Point p) => (this.Mother((t - p.y) / p.x) * func(p) / p.x / p.x).Re, 
            //    eps: eps, 
            //    parallel: true, 
            //    M: DefInteg.Method.GaussKronrod61, changestepcount: 0, a: 1, b: 10) / C).Re;

            //задание промежуточных переменных
            if (F != null)
            {
                Memoize<Point, Complex> f = new Memoize<Point, Complex>((Point p) => F(p.x, p.y));

                return GetRes(f.Value);
            }
            else
                return GetRes(p => ResultMemoized(p.x, p.y));
        }


        //tex:$ C_{\psi}= \int_{-\infty}^{\infty}  \dfrac{|\psi(\omega)|^2}{|\omega|} d \omega$
        private double Ccoef
        {
            get
            {
                double C;
                if (!Cpsi.ContainsKey(this.Type))
                {
                    switch (this.Type)
                    {
                        case Wavelets.LP:
                            C = Math.Log(2) * Math.Sqrt(2) / Math.PI;
                            break;
                        case Wavelets.WAVE:
                            C = 2 * Math.PI;
                            break;
                        case Wavelets.Gabor:
                            C = 1 / Math.PI;
                            break;
                        default:
                            C = DefInteg.GaussKronrod.DINN_GKwith0Full(
                                (Complex w) =>
                                {
                                    if (w == 0) return 0;
                                    return (this.FMother(w).Sqr() / w).Abs;
                                },
                            eps: eps, nodesCount: countNodes).Re;
                            break;
                    }
                    Cpsi[this.Type] = C;
                }
                else
                    C = Cpsi[this.Type];
                Console.WriteLine($"C coefficent = {C}");
                return C;
            }
        }


        public void Dispose()
        {
            Resultmems.Dispose();
        }
    }
}