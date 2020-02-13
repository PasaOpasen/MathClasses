using System;

namespace МатКлассы
{
    /// <summary>
    /// Числовые классы
    /// </summary>
    public static partial class Number
    {

        /// <summary>
        /// Возвращает действительную, мнимую части и модули массива
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Tuple<double[], double[], double[]> ReImAbs(this Complex[] v)
        {
            double[] re = new double[v.Length], im = new double[v.Length], abs = new double[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                re[i] = v[i].Re;
                im[i] = v[i].Im;
                abs[i] = v[i].Abs;
            }
            return new Tuple<double[], double[], double[]>(re, im, abs);
        }
    }
}

