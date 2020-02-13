using System;

namespace МатКлассы
{
    /// <summary>
    /// Методы комбинаторики
    /// </summary>
    public class Combinatorik
    {
        /// <summary>
        /// Число перестановок (факториал)
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int P(int k)
        {
            if (k < 2) return 1;
            int s = 1;
            for (int i = 2; i <= k; i++) s *= i;
            return s;
        }
        /// <summary>
        /// Перестановки с повторениями
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int P(params int[] k)
        {
            int sum = 0;
            for (int i = 0; i < k.Length; i++) sum += k[i];
            sum = P(sum);
            for (int i = 0; i < k.Length; i++) sum /= P(k[i]);
            return sum;
        }
        /// <summary>
        /// Число размещений из n по m 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int A(int m, int n)
        {
            int s = 1;
            for (int i = m + 1; i <= n; i++) s *= i;
            return s;
        }
        /// <summary>
        /// Размещения с повторениями
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int AA(int m, int n) { return (int)Math.Pow(n, m); }
        /// <summary>
        /// Число сочетаний из n по m
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int C(int m, int n) { return A(m, n) / P(m); }
        /// <summary>
        /// Сочетания с повторениями
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int CС(int m, int n) { return C(m, n + m - 1); }
    }
}

