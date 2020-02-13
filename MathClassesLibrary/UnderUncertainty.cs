using System;

namespace МатКлассы
{
    /// <summary>
    /// Критерии принятия решений в условиях неопределённости
    /// </summary>
    public static class UnderUncertainty
    {
        private static void MAX(Vectors v, out int k)
        {
            double m = v[0];
            k = 0;
            for (int i = 1; i < v.Deg; i++)
                if (v[i] > m) { m = v[i]; k = i; }
        }
        private static void MIN(Vectors v, out int k)
        {
            double m = v[0];
            k = 0;
            for (int i = 1; i < v.Deg; i++)
                if (v[i] < m) { m = v[i]; k = i; }
        }

        /// <summary>
        /// Критерий среднего выйгрыша
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="q">Вектор вероятностей</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void AverageGain(Matrix S, out Vectors v, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.Deg;
                for (int i = 0; i < q.Deg; i++) q[i] = w;
            }
            for (int i = 0; i < S.n; i++)
            {
                for (int j = 0; j < S.m; j++) v[i] += S[i, j] * q[j];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию среднего значения с вектором вероятностей " + q.ToString() + " оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий минимакса
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void MiniMax(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MIN(v, out s);
            Console.WriteLine("По критерию минимакса оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий максимакса
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void MaxiMax(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию максимакса оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Лапласа
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Laplas(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            Vectors q = new Vectors(S.n);
            double w = 1.0 / S.m;
            for (int i = 0; i < q.Deg; i++) q[i] = w;

            for (int i = 0; i < S.n; i++)
            {
                for (int j = 0; j < S.m; j++) v[i] += S[i, j];
                v[i] *= q[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Лапласа оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Вальда (максимин)
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Vald(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MIN(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Вальда (максимина) оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Сэвиджа
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Savage(Matrix S, out Vectors v)
        {
            Vectors v0 = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v0[i] = S[i, tmp];
            }
            Matrix M = new Matrix(S.n, S.m);
            for (int i = 0; i < M.n; i++)
                for (int j = 0; j < M.m; j++)
                    M[i, j] = v0[i] - S[i, j];//M.PrintMatrix();

            v = new Vectors(M.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = M.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = M[i, tmp];
            }
            int s; MIN(v, out s);
            Console.WriteLine("По критерию Сэвиджа оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Гурвица
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        /// <param name="a">Коэффициент оптимизма</param>
        public static void Hurwitz(Matrix S, out Vectors v, double a = 0.5)
        {
            v = new Vectors(S.n);
            Vectors l = new Vectors(v), r = new Vectors(v);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp1, tmp2;
                MAX(c, out tmp1);
                MIN(c, out tmp2);
                l[i] = S[i, tmp1]; r[i] = S[i, tmp2];
                v[i] = a * l[i] + (1 - a) * r[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Гурвица с параметром {2} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], a);
        }
        /// <summary>
        /// Критерий Ходжа-Лемана
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        /// <param name="a">Коэффициент метода (вес)</param>
        /// <param name="q">Вектор вероятностей</param>
        public static void HodgeLeman(Matrix S, out Vectors v, double a = 0.5, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.Deg;
                for (int i = 0; i < q.Deg; i++) q[i] = w;
            }
            Vectors l = new Vectors(v), r = new Vectors(v);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp2;
                for (int j = 0; j < S.m; j++) l[i] += S[i, j] * q[j];
                MIN(c, out tmp2);
                r[i] = S[i, tmp2];
                v[i] = a * l[i] + (1 - a) * r[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Ходжа-Лемана с параметром {2} и вектором вероятностей {3} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], a, q.ToString());
        }
        /// <summary>
        /// Критерий Гермейера
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Germeier(Matrix S, out Vectors v, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.Deg;
                for (int i = 0; i < q.Deg; i++) q[i] = w;
            }
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                for (int j = 0; j < S.m; j++) c[j] *= q[j];
                int tmp; MIN(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Гермейера с вектором вероятностей {2} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], q.ToString());
        }
        /// <summary>
        /// Критерий произведений
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Powers(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                v[i] = 1;
                for (int j = 0; j < S.m; j++)
                    v[i] *= S[i, j];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию произведений оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
    }
}

