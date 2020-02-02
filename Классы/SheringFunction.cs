using System;

namespace МатКлассы
{
    /// <summary>
    /// Класс, создающий срезывающую функцию
    /// </summary>
    public static class SheringFunction
    {
        private static double c;
        private static readonly double eps = 1e-13;

        static SheringFunction()
        {
            c = 2.0 * MathNet.Numerics.Integration.GaussLegendreRule.Integrate((double s) => Math.Pow(Math.E, 1.0 / (s * s - 1.0)), 0, 1 - eps, 1024);
        }

        private static double w(double t) => (Math.Abs(t) >= 1.0) ? 0 : Math.Pow(Math.E, 1.0 / (t * t - 1.0)) / c;
        private static double wh(double t, double h) => w(t / h) / h;

        /// <summary>
        /// Возращает срезывающую функцию
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="delta">Чем больше, тем глаже, как мне кажется</param>
        /// <returns></returns>
        public static Func<double, double> GetSheringFunction(double a, double b, double delta = 1e-2)
        {
            double delta34 = 0.75 * delta;
            a += delta34;
            b -= delta34;
            return (double t) => FuncMethods.DefInteg.GaussKronrod.GaussKronrodSum((double s) => wh(t - s, 0.25 * delta), a, b, 61, 300); //MathNet.Numerics.Integration.GaussLegendreRule.Integrate((double s) => wh(t-s,0.25*delta), a, b,1024);
        }

        /// <summary>
        /// Срезанную функцию по образцу несрезанной
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Func<double, double> GetSheredFunction(Func<double, double> f, double a, double b, double delta = 1e-2)
        {
            var sd = GetSheringFunction(a, b, delta);
            return (double t) => f(t) * sd(t);
        }

    }


}