using System;
using static МатКлассы.Number;

namespace МатКлассы
{
    /// <summary>
    /// Класс комплексных СЛАУ
    /// Отличие от действительного случая в том, что реализация происходит через матрицы и векторы, а не через массивы
    /// Из-за комплексных чисел все методы надо переписывать
    /// </summary>
    public class CSLAU
    {
        private CSqMatrix A;
        private CVectors x, b;

        /// <summary>
        /// Размерность системы
        /// </summary>
        public int Dim => x.Degree;

        /// <summary>
        /// Конструктор по матрице и свободному вектору
        /// </summary>
        /// <param name="M"></param>
        /// <param name="v"></param>
        public CSLAU(CSqMatrix M, CVectors v)
        {
            x = new CVectors(v.Degree);
            b = new CVectors(v);
            A = new CSqMatrix(M);
        }

        private void GetDet(out Complex det)
        {
            det = A.Det;
            if (det == 0) throw new Exception("Матрица системы вырождена!");
        }

        /// <summary>
        /// Решение системы методом Крамера
        /// </summary>
        public void KramerSolve()
        {
            GetDet(out Complex det);
            for (int i = 0; i < this.Dim; i++)
                x[i] = A.ColumnSwap(i + 1, b).Det / det;
        }
        /// <summary>
        /// Метод Гаусса, годный и при нулевых коэффициентах в системе
        /// </summary>
        public void GaussSelection()
        {
            CMatrix S = new CMatrix(this.Dim, this.Dim + 1);
            for (int j = 0; j < this.Dim; j++)
            {
                for (int i = 0; i < this.Dim; i++) S[i, j] = this.A[i, j];
                S[j, this.Dim] = this.b[j];
            }

            for (int j = 0; j < this.Dim; j++)
            {
                int k = j;
                if (S[k, j] == 0)//если ведущий элемент равен нулю, поменять эту строку местами с ненулевой
                {
                    int h = k;
                    while (S[h, j] == 0) h++;
                    S.LinesSwap(k, h);
                }

                while (S[k, j] == 0 && k < this.Dim - 1) k++;//найти ненулевой элемент
                int l = k + 1;
                if (k != this.Dim - 1) while (l != this.Dim) { S.LinesDiff(l, k, S[l, j] / S[k, j]); l++; } //отнимать от строк снизу
                                                                                                            //S.PrintMatrix();Console.WriteLine();
                l = k - 1;
                if (k != 0) while (l != -1) { S.LinesDiff(l, k, S[l, j] / S[k, j]); l--; }//отнимать от строк сверху
            }

            for (int i = 0; i < this.Dim; i++) this.x[i] = S[i, this.Dim] / S[i, i];
        }
        private class CMatrix
        {
            private Matrix R, I;
            public Complex this[int i, int j]
            {
                get
                {
                    return R[i, j] + Complex.I * I[i, j];
                }
                set
                {
                    R[i, j] = value.Re;
                    I[i, j] = value.Im;
                }
            }

            public CMatrix(int n, int m)
            {
                R = new Matrix(n, m);
                I = new Matrix(n, m);

            }

            public void LinesSwap(int a, int b)
            {
                R.LinesSwap(a, b);
                I.LinesSwap(a, b);
            }
            public void LinesDiff(int a, int b, Complex coef)
            {
                for (int k = 0; k < R.m; k++) this[a, k] -= coef * this[b, k];
            }
        }

        /// <summary>
        /// Вывести систему на консоль
        /// </summary>
        public void Show()
        {
            for (int i = 0; i < this.Dim; i++)
            {
                string s = "||";
                for (int j = 0; j < this.Dim - 1; j++)
                    s += "\t" + A[i, j].ToString() + " ";
                s += "\t" + A[i, this.Dim - 1].ToString() + "|| \t" + x[i].ToString() + " \t||" + b[i].ToString() + "||";
                s.Show();
            }
        }
    }
}