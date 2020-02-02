using System;
using System.IO;

namespace МатКлассы
{
    /// <summary>
    /// Квадратные матрицы
    /// </summary>
    public class SqMatrix : Matrix
    {
        //Конструктор
        /// <summary>
        /// Матрица (0)
        /// </summary>
        public SqMatrix() : base()//по умолчанию
        { }
        /// <summary>
        /// Нулевая квадратная матрица
        /// </summary>
        /// <param name="n">Размерность матрицы</param>
        public SqMatrix(int n) : base(n, n)//по размерности
        { }
        /// <summary>
        /// Считать матрицу из файла
        /// </summary>
        /// <param name="fs"></param>
        public SqMatrix(StreamReader fs) : base(fs)//через файл
        { }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="M"></param>
        public SqMatrix(SqMatrix M)
        {
            this.n = this.m = M.n;
            this.matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) this[i, j] = M[i, j];
        }
        /// <summary>
        /// Создание матрицы по двумерному массиву
        /// </summary>
        /// <param name="S"></param>
        public SqMatrix(int[,] S)
        {
            if (S.GetLength(0) != S.GetLength(1)) throw new Exception("Таблица не является квадратной!");
            this.m = this.n = S.GetLength(0);
            this.matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) this[i, j] = S[i, j];
        }
        /// <summary>
        /// Создание матрицы по двумерному массиву
        /// </summary>
        /// <param name="S"></param>
        public SqMatrix(double[,] S)
        {
            if (S.GetLength(0) != S.GetLength(1)) throw new Exception("Таблица не является квадратной!");
            this.m = this.n = S.GetLength(0);
            this.matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) this[i, j] = S[i, j];
        }
        /// <summary>
        /// Создать матрицу как угловую подматрицу размерности k
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        public SqMatrix(SqMatrix A, int k) : base(k, k)
        {
            for (int i = 0; i < this.n; i++)
                for (int j = 0; j < this.n; j++)
                    this[i, j] = A[i, j];
        }
        /// <summary>
        /// Создать матрицу как угловую подматрицу размерности k
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        public SqMatrix(double[,] A, int k) : base(k, k)
        {
            for (int i = 0; i < this.n; i++)
                for (int j = 0; j < this.n; j++)
                    this[i, j] = A[i, j];
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="M"></param>
        public SqMatrix(Matrix M) : base(M) { }
        /// <summary>
        /// Создание матрицы по одномерному массиву
        /// </summary>
        /// <param name="mas"></param>
        public SqMatrix(double[] mas)
        {
            this.n = this.m = (int)Math.Sqrt(mas.Length);
            this.matrix = new double[n, n];
            int y = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) this[i, j] = mas[y++];
        }

        /// <summary>
        /// Единичная матрица
        /// </summary>
        public static SqMatrix I(int n)
        {
            SqMatrix A = new SqMatrix(n);
            for (int i = 0; i < n; i++) A[i, i] = 1;
            return A;
        }

        //методы

        ///// <summary>
        ///// Задать коэффициенты в матрице через консоль
        ///// </summary>
        //public override void CreateMatrix()
        //{

        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n; j++)
        //        {
        //            Console.Write("Введите элемент [" + i.ToString() + ";" + j.ToString() + "]" + "\t");
        //            matrix[i, j] = Convert.ToDouble(Console.ReadLine());
        //        }
        //    }

        //}
        ///// <summary>
        ///// Вывести матрицу на консоль
        ///// </summary>
        //public override void PrintMatrix()
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n; j++)
        //        {
        //            Console.Write(matrix[i, j].ToString() + " \t");
        //        }
        //        Console.WriteLine();
        //    }
        //}

        /// <summary>
        /// Диагональная ли матрица?
        /// </summary>
        /// <returns></returns>
        public bool Diagonal()
        {
            uint r = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if ((matrix[i, j] != 0) && (matrix[j, i] != 0)) return false;
                    if (matrix[i, i] != 0) r++;
                }
            }
            if (r > 0) return true;
            return false;
        }
        /// <summary>
        /// Является ли матрица симметрической
        /// </summary>
        /// <returns></returns>
        public bool IsSymmetric()
        {
            for (int i = 0; i < this.n; i++)
                for (int j = 0; j < this.n; j++)
                    if (this[i, j] != this[j, i]) return false;
            return true;
        }
        /// <summary>
        /// Является ли матрица положительно определённой
        /// </summary>
        /// <returns></returns>
        public bool IsPositCertain()
        {
            //if (!this.IsSymmetric()) return false;

            for (int i = 0; i < this.n; i++)
            {
                SqMatrix M = new SqMatrix(this, i + 1);
                double s = M.Det;
                if (s <= 0) return false;
            }
            return true;
        }
        /// <summary>
        /// Является ли матрица тридиагональной
        /// </summary>
        /// <returns></returns>
        public bool IsTreeDiag()
        {
            for (int i = 0; i < this.n; i++)
                for (int j = i + 2; j < this.n; j++)
                    if (this[i, j] != 0 || this[j, i] != 0) return false;
            return true;
        }

        //public override bool Nulle()//нулевая ли матрица?
        //{
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n; j++)
        //        {
        //            if (matrix[i, j] != 0) return false;
        //        }
        //    }

        //    return true;
        //}

        /// <summary>
        /// Единичная матрица
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static SqMatrix E(int n)
        {
            SqMatrix M = new SqMatrix(n);
            for (int i = 0; i < n; i++) M[i, i] = 1;
            return M;
        }
        /// <summary>
        /// Определитель матрицы
        /// </summary>
        public double Det
        {
            get
            {
                SqMatrix matrix = new SqMatrix(this);

                double m = 0;
                for (int j = 0; j < this.n; j++)
                {
                    for (int i = j + 1; i < this.n; i++)
                    {
                        if (matrix[j, j] != 0)
                        {
                            m = matrix[i, j] / matrix[j, j];

                            for (int h = j; h < this.n; h++)
                                matrix[i, h] -= m * matrix[j, h];
                        }
                    }
                }
                m = 1;
                for (int i = 0; i < this.n; i++) m *= matrix[i, i];
                // PrintMatrix();
                return m;
            }
        }
        /// <summary>
        /// Минор элемента матрицы (точнее, алгебраическое дополнение)
        /// </summary>
        /// <returns></returns>
        public double Minor(int i, int j)
        {
            if ((i >= n) || (j >= n)) throw new Exception("Вызов элемента, которого нет в матрице");
            if (this.n <= 1) throw new Exception("Размерность матрицы слишком мала");
            if (this.n == 2) return Math.Pow(-1, i + j) * this[3 - i - 2, 3 - j - 2];
            //int Inew = i--,Jnew=j--;
            SqMatrix M = new SqMatrix(this.n - 1);
            int a = 0, b = 0;
            for (int ii = 0; ii < this.n; ii++)
            {
                if (ii != i)
                {
                    for (int jj = 0; jj < this.n; jj++)
                    {
                        if (jj != j)
                        {
                            M[a, b] = this[ii, jj]; //Console.WriteLine("{0} {1} {2} {3}", a,b,ii,jj);
                            b++;
                        }
                    }
                    a++; b = 0;
                }
            }
            //M.PrintMatrix();
            return Math.Pow(-1, i + 1 + j + 1) * M.Det;

        }

        /// <summary>
        /// Трек матрицы
        /// </summary>
        /// <returns></returns>
        public double Track
        {
            get
            {
                double s = 0;
                for (int i = 0; i < this.n; i++) s += this[i, i];
                return s;
            }
        }
        /// <summary>
        /// Характеристический многочлен заданной матрицы
        /// </summary>
        public Polynom CharactPol
        {
            get { return Polynom.CharactPol(this); }
        }
        /// <summary>
        /// Обратная матрица
        /// </summary>
        public SqMatrix Reverse//из теоремы Гамильтона-Кели
        {
            get
            {
                //if (this.Det == 0) throw new ArithmeticException("Матрица вырождена");
                SqMatrix M = SqMatrix.E(this.n);
                SqMatrix A = new SqMatrix(this);
                Polynom p = this.CharactPol;
                M *= p.coef[1];
                for (int i = 2; i <= n; i++)
                {
                    M += p.coef[i] * A;
                    A *= this;
                }
                M *= -1;
                M /= p.coef[0];
                return M;
            }
        }
        /// <summary>
        /// Обратная матрица по Гауссу
        /// </summary>
        /// <returns></returns>
        public SqMatrix Invert()
        {
            SqMatrix mResult = SqMatrix.E(this.ColCount);
            /*
             * Получать "1" на элементе главной диагонали, а потом
             * Занулять оставшиеся элементы
             * */
            SqMatrix mCur = new SqMatrix(this);
            //mCur.PrintMatrix(); Console.WriteLine();
            //mResult.PrintMatrix(); Console.WriteLine();

            for (int i = 0; i < this.ColCount; i++) //Цикл по строкам сверху-вниз
            {
                //Заединичить вервую строку
                double dItem = mCur[i, i];
                mCur.DivByLine(i, dItem);
                mResult.DivByLine(i, dItem);

                //mCur.PrintMatrix(); Console.WriteLine();
                //mResult.PrintMatrix(); Console.WriteLine();

                Vectors rTmp = mCur.GetLine(i);
                Vectors eTmp = mResult.GetLine(i);
                //Забить нулями вертикаль
                for (int j = 0; j < this.ColCount; j++)
                    if (i != j)
                    {
                        double con = mCur[j, i];
                        mCur.MinusVector(j, rTmp * con);
                        mResult.MinusVector(j, eTmp * con);
                    }
                //mCur.PrintMatrix(); Console.WriteLine();
                //mResult.PrintMatrix(); Console.WriteLine();
            }

            return mResult;
        }
        /// <summary>
        /// Уточнение обратной матрицы
        /// </summary>
        /// <param name="A">Исходная матрица</param>
        /// <param name="Reverse">Обратная марица</param>
        /// <param name="eps">Точность</param>
        /// <returns></returns>
        public static SqMatrix ReverseCorrect(SqMatrix A, SqMatrix Reverse, double eps = 0.001, int stepcount = 1000, bool existnorm = false)
        {
            SqMatrix E = SqMatrix.E(A.RowCount), R = new SqMatrix(Reverse);
            int i = 0;
            if ((E - A * R).CubeNorm < 1 || existnorm)
                while ((E - A * R).CubeNorm > eps && i < stepcount)
                {
                    R *= (2 * E - A * R);
                    //(E - A * R).CubeNorm.Show();
                    i++;
                }
            return R;
        }

        /// <summary>
        /// Обратная матрица через MathNet
        /// </summary>
        public SqMatrix Invertion
        {
            get
            {
                //var m = (MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfArray(this.matrix));
                //SqMatrix A = new SqMatrix(m.ToArray());
                //SqMatrix R = new SqMatrix(m.Inverse().ToArray());

                //return ReverseCorrect(A, R, 0.0001, 1000, false);

                return new SqMatrix((MathNet.Numerics.LinearAlgebra.CreateMatrix.DenseOfArray(this.matrix)).Inverse().ToArray());
            }
        }

        /// <summary>
        /// Квадратная подматрица, порождённая пересечением таких строк и столбцов
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        /// <remarks>Нумерация строк должна начинаться с единицы</remarks>
        public SqMatrix SubMatrix(params int[] m)
        {
            SqMatrix M = new SqMatrix(m.Length);
            for (int i = 0; i < m.Length; i++)
                for (int j = 0; j < m.Length; j++)
                    M[i, j] = this[m[i] - 1, m[j] - 1];
            return M;
        }

        /// <summary>
        /// Замена столбца матрицы на указанный вектор (для метода Крамера)
        /// </summary>
        /// <param name="ColumnNumber">Номер стоблца, начиная с 1</param>
        /// <param name="NewColumn">Сам вектор (если вектор)</param>
        /// <remarks>Если вектор короткий, заменится лишь часть колонны, а если длинный, будет исключение</remarks>
        /// <returns></returns>
        public SqMatrix ColumnSwap(int ColumnNumber, Vectors NewColumn)
        {
            SqMatrix mat = new SqMatrix(this);
            if (ColumnNumber > mat.ColCount || ColumnNumber <= 0 || NewColumn.Deg > mat.RowCount) throw new Exception("В матрице нет столбца с таким номером либо вектор слишком длинный");
            ColumnNumber--;
            for (int i = 0; i < NewColumn.Deg; i++)
                mat[i, ColumnNumber] = NewColumn[i];
            return mat;
        }

        public SqMatrix Transpose()
        {
            return new SqMatrix(base.Transpose().matrix);
        }
        /// <summary>
        /// Подобная матрица, если задана ортогональная матрица преобразования
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public SqMatrix ConvertToSimilar(SqMatrix M, bool directly = true)
        {
            if (directly)
                return (SqMatrix)(M.Transpose() * this * M);
            else
                return (SqMatrix)(M * this * M.Transpose());
        }

        //операторы
        //сложение
        public static SqMatrix operator +(SqMatrix A, SqMatrix B)
        {
            SqMatrix C = new SqMatrix(A.n);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.n; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }
            return C;
        }

        //вычитание
        public static SqMatrix operator -(SqMatrix A, SqMatrix B)
        {
            SqMatrix R = new SqMatrix(A.n);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.n; j++)
                {
                    R[i, j] = A[i, j] - B[i, j];
                }
            }
            return R;

        }

        //произведение
        public static SqMatrix operator *(SqMatrix A, SqMatrix B)
        {
            SqMatrix r = new SqMatrix(A.n);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.n; j++)
                {
                    for (int k = 0; k < B.n; k++)
                    {
                        r[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return r;
        }

        //Умножение на число
        public static SqMatrix operator *(SqMatrix A, double Ch)
        {
            SqMatrix q = new SqMatrix(A.n);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.n; j++)
                {
                    q[i, j] = A[i, j] * Ch;
                }
            }
            return q;
        }
        public static SqMatrix operator *(double Ch, SqMatrix A) { return A * Ch; }
        public static SqMatrix operator /(SqMatrix A, double Ch) { return A * (1 / Ch); }
        public static Vectors operator *(SqMatrix A, Vectors v)
        {
            if (v.Deg != A.ColCount) v=Vectors.Union(new Vectors[] {v, new Vectors(A.ColCount - v.Deg, 0.0) });
            Matrix V = new Matrix(v);
            Matrix res = A * V;
            Vectors w = new Vectors(v.Deg);
            for (int i = 0; i < v.Deg; i++)
                w[i] = res[i, 0];
            return w;
        }

        /// <summary>
        /// Перевод матрицы в одномерный массив
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(SqMatrix M)
        {
            double[] res = new double[M.m * M.m];
            int y = 0;
            for (int i = 0; i < M.n; i++)
                for (int j = 0; j < M.m; j++)
                    res[y++] = M[i, j];
            return res;
        }

        public double[,] MatrixAsMas
        {
            get
            {
                double[,] r = new double[this.ColCount, this.ColCount];
                for (int i = 0; i < ColCount; i++)
                    for (int j = 0; j < ColCount; j++)
                        r[i, j] = this[i, j];

                return r;
            }
        }

        /// <summary>
        /// Подматрица размерности len
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public SqMatrix SubMatrix(int len)
        {
            SqMatrix r = new SqMatrix(len);
            for (int i = 0; i < len; i++)
                for (int j = 0; j < len; j++)
                    r[i, j] = this[i, j];
            return r;
        }

        /// <summary>
        /// Вектор x решения системы Ax=b
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vectors Solve(Vectors b)
        {
            var bb = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(b.DoubleMas);
            var A = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(this.matrix);
            var x = A.Solve(bb);

           // $"nevaska = {((new SqMatrix(this)) * (new Vectors(x.ToArray())) - b).EuqlidNorm}".Show();

            return new Vectors(x.ToArray());
        }
    }
}

