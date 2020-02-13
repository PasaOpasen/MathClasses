using System;
using static МатКлассы.Number;

namespace МатКлассы
{
    /// <summary>
    /// Квадратные комплексные матрицы
    /// </summary>
    public class CSqMatrix : Idup<CSqMatrix>
    {
        /// <summary>
        /// Действительные и мнимые части комплексной матрицы
        /// </summary>
        public SqMatrix Re
        {
            get
            {
                SqMatrix res = new SqMatrix(matr.GetLength(0));
                for (int i = 0; i < res.RowCount; i++)
                    for (int j = 0; j < res.RowCount; j++)
                        res[i, j] = matr[i, j].Re;
                return res;
            }
        }
        public SqMatrix Im
        {
            get
            {
                SqMatrix res = new SqMatrix(matr.GetLength(0));
                for (int i = 0; i < res.RowCount; i++)
                    for (int j = 0; j < res.RowCount; j++)
                        res[i, j] = matr[i, j].Im;
                return res;
            }
        }

        //public SqMatrix RE
        //{
        //    get
        //    {
        //        if(needretr)
        //        for (int i = 0; i < RowCount; i++)
        //            for (int j = 0; j < RowCount; j++)
        //                Re[i, j] = matr[i, j].Re;
        //        return Re;
        //    }
        //}
        //public SqMatrix IM
        //{
        //    get
        //    {
        //        if (needretr)
        //            for (int i = 0; i < RowCount; i++)
        //                for (int j = 0; j < RowCount; j++)
        //                    Im[i, j] = matr[i, j].Im;
        //        return Im;
        //    }
        //}

        private Complex[,] matrtmp = null;
        private bool needretr = false;
        private Complex[,] matr;
        //{
        //    get
        //    {
        //        if (matrtmp == null)
        //        {
        //            var matrtmp = new Complex[Re.n, Re.m];
        //            for (int i = 0; i < matrtmp.GetLength(0); i++)
        //                for (int j = 0; j < matrtmp.GetLength(1); j++)
        //                    matrtmp[i, j] = new Complex(Re[i, j], Im[i, j]);
        //        }
        //        return matrtmp;
        //    }
        //    set
        //    {
        //        needretr = true;
        //        matrtmp = new Complex[value.GetLength(0), value.GetLength(1)];
        //        for (int i = 0; i < matrtmp.GetLength(0); i++)
        //            for (int j = 0; j < matrtmp.GetLength(1); j++)
        //                matrtmp[i, j] = new Complex(value[i, j]);
        //    }
        //}

        /// <summary>
        /// Вернуть массив комплексных чисел исходной матрицы
        /// </summary>
        public Complex[,] ComplexMas
        {
            get
            {
                Complex[,] res = new Complex[this.ColCount, this.ColCount];
                for (int i = 0; i < this.ColCount; i++)
                    for (int j = 0; j < this.ColCount; j++)
                        res[i, j] = new Complex(this[i, j]);
                return res;
            }
        }

        /// <summary>
        /// Число строк
        /// </summary>
        public int RowCount => Re.RowCount;
        /// <summary>
        /// Число столбцов
        /// </summary>
        public int ColCount => Re.ColCount;

        /// <summary>
        /// Кубическая норма матрицы как сумма кубических норма её действительной и мнимой части
        /// </summary>
        public double CubeNorm => Re.CubeNorm + Im.CubeNorm;

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Complex this[int i, int j]
        { get { return this.matr[i, j]; } set { this.matr[i, j] = new Complex(value); } }

        private static Complex[,] mas(SqMatrix A, SqMatrix B)
        {
            var res = new Complex[A.RowCount, A.ColCount];
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.ColCount; j++)
                    res[i, j] = A[i, j] + Complex.I * B[i, j];
            return res;
        }
        /// <summary>
        /// Конструктор по двумерному комплексному массиву
        /// </summary>
        /// <param name="m"></param>
        public CSqMatrix(Complex[,] m)
        {
            //double[,] re = new double[m.GetLength(0), m.GetLength(1)], im = new double[m.GetLength(0), m.GetLength(1)];
            //for (int i = 0; i < m.GetLength(0); i++)
            //    for (int j = 0; j < m.GetLength(1); j++)
            //    {
            //        re[i, j] = m[i, j].Re;
            //        im[i, j] = m[i, j].Im;
            //    }
            //Re = new SqMatrix(re);
            //Im = new SqMatrix(im);
            matr = m;
        }
        /// <summary>
        /// Конструктор по двумерному действительному массиву
        /// </summary>
        /// <param name="m"></param>
        public CSqMatrix(double[,] m)
        {
            //double[,] re = new double[m.GetLength(0), m.GetLength(1)], im = new double[m.GetLength(0), m.GetLength(1)];
            Complex[,] tmp = new Complex[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < m.GetLength(0); i++)
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    //re[i, j] = m[i, j];
                    //im[i, j] = 0;
                    tmp[i, j] = new Complex(m[i, j], 0);
                }
            //Re = new SqMatrix(re);
            //Im = new SqMatrix(im);
            matr = tmp;
        }
        /// <summary>
        /// Конструктор по действительной и мнимой части матрицы
        /// </summary>
        /// <param name="R"></param>
        /// <param name="I"></param>
        public CSqMatrix(SqMatrix R, SqMatrix I) : this(mas(R, I)) { }
        /// <summary>
        /// Копирование комплексной матрицы
        /// </summary>
        /// <param name="M"></param>
        public CSqMatrix(CSqMatrix M) : this(M.Re, M.Im) { }

        //public CSqMatrix(CVectors v)
        //{
        //    this.Re =new SqMatrix( v.Re);
        //    this.Im = new SqMatrix(v.Im);
        //}

        /// <summary>
        /// Определитель матрицы
        /// </summary>
        public Complex Det
        {
            get
            {
                CSqMatrix matrix = new CSqMatrix(this);

                Complex m = 0;
                for (int j = 0; j < this.RowCount; j++)
                {
                    for (int i = j + 1; i < this.ColCount; i++)
                    {
                        if (matrix[j, j] != 0)
                        {
                            m = matrix[i, j] / matrix[j, j];

                            for (int h = j; h < this.ColCount; h++)
                                matrix[i, h] -= m * matrix[j, h];
                        }
                    }
                }
                m = 1;
                for (int i = 0; i < this.RowCount; i++) m *= matrix[i, i];
                // PrintMatrix();
                return m;
            }
        }

        /// <summary>
        /// Определитель с помощью MathNet
        /// </summary>
        public Complex DetByMathNet
        {
            get
            {
                var mat = this.ToSystemNumComplex();
                MathNet.Numerics.LinearAlgebra.Matrix<System.Numerics.Complex> matrix = MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix.OfArray(mat);
                return new Complex(matrix.Determinant());
            }
        }

        /// <summary>
        /// Преобразование матрицы в комплексный массив
        /// </summary>
        /// <param name="M"></param>
        public static explicit operator Complex[,] (CSqMatrix M) => mas(M.Re, M.Im);
        /// <summary>
        /// Выдать матрицу в консоль
        /// </summary>
        public void PrintMatrix()
        {
            for (int i = 0; i < this.RowCount; i++)
            {
                Console.Write("|| ");
                for (int j = 0; j < this.ColCount; j++)
                    Console.Write("\t" + this[i, j] + " ");
                Console.WriteLine("\t||");
            }
        }
        /// <summary>
        /// Замена колонны указанным вектором (для метода Крамера)
        /// </summary>
        /// <param name="ColumnNumber">Номер колонны</param>
        /// <param name="newColumn">Новая колонна</param>
        /// <returns></returns>
        public CSqMatrix ColumnSwap(int ColumnNumber, CVectors newColumn)
        {
            SqMatrix R = this.Re.ColumnSwap(ColumnNumber, newColumn.Re);
            SqMatrix I = this.Im.ColumnSwap(ColumnNumber, newColumn.Im);
            return new CSqMatrix(R, I);
        }
        /// <summary>
        /// Вернуть строку матрицы
        /// </summary>
        /// <param name="k">Номер строки, начиная от 0</param>
        /// <returns></returns>
        public CVectors GetLine(int k)
        {
            CVectors ew = new CVectors(this.ColCount);
            for (int i = 0; i < ew.Degree; i++)
                ew[i] = new Complex(this[k, i]);
            return ew;
        }
        /// <summary>
        /// Вернуть столбец матрицы
        /// </summary>
        /// <param name="k">Номер столбца, начиная от 0</param>
        /// <returns></returns>
        public CVectors GetColumn(int k)
        {
            CVectors ew = new CVectors(this.RowCount);
            for (int i = 0; i < ew.Degree; i++)
                ew[i] = new Complex(this[i, k]);
            return ew;
        }
        /// <summary>
        /// Переставить местами столбцы с указанными индексами, нумерация начинается с 1
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public void ReversColumns(int t1, int t2)
        {
            t1--; t2--;
            //Complex[,] res = this.matr;
            Complex tmp;
            for (int i = 0; i < this.ColCount; i++)
            {
                tmp = new Complex(this.matr[i, t1]);
                this.matr[i, t1] = new Complex(this.matr[i, t2]);
                this.matr[i, t2] = new Complex(tmp);
            }

        }
        /// <summary>
        /// Умножить в матрице какие-то строки на число
        /// </summary>
        /// <param name="c">Коэффициент</param>
        /// <param name="k">Массив индексов строк, индексы начинаются с 1</param>
        /// <returns></returns>
        public CSqMatrix MultplyRows(Complex c, params int[] k)
        {
            Complex[,] res = this.ComplexMas;
            for (int i = 0; i < k.Length; i++)
                for (int j = 0; j < this.ColCount; j++)
                    res[k[i] - 1, j] *= c;
            return new CSqMatrix(res);
        }

        /// <summary>
        /// Поделить строку в матрице на число
        /// </summary>
        /// <param name="i"></param>
        /// <param name="val"></param>
        public void DivByLine(int i, Complex val)
        {
            for (int j = 0; j < this.ColCount; j++)
                matr[i, j] /= val;
        }
        /// <summary>
        /// Отнять от строки матрицы вектор
        /// </summary>
        /// <param name="i"></param>
        /// <param name="c"></param>
        public void MinusVector(int i, CVectors c)
        {
            for (int j = 0; j < this.ColCount; j++)
                matr[i, j] -= c[j];
        }

        public void Show()
        {
            string s;
            for (int i = 0; i < RowCount; i++)
            {
                s = $"||{matr[i, 0]}";
                for (int j = 1; j < ColCount; j++)
                    s += $"\t {matr[i, j]}";
                s += "||";
                s.Show();
            }
        }

        /// <summary>
        /// Обратная матрица по Гауссу
        /// </summary>
        /// <returns></returns>
        public CSqMatrix Invert(bool correct = false)
        {
            CSqMatrix mResult = SqMatrix.E(this.ColCount);
            CSqMatrix mCur = new CSqMatrix(this);
            CVectors rTmp, eTmp;

            for (int i = 0; i < this.ColCount; i++) //Цикл по строкам сверху-вниз
            {
                int max = MaxLine(i, i);
                if (max != i)
                {
                    rTmp = mCur.GetLine(max);
                    eTmp = mResult.GetLine(max);
                    mCur.MinusVector(i, rTmp * (-1));
                    mResult.MinusVector(i, eTmp * (-1));

                    //Complex y = new Complex(mCur[i, max]);
                    //mCur.DivByLine(i, y);
                    //mResult.DivByLine(i, y);
                }

                //Заединичить вервую строку
                Complex dItem = new Complex(mCur[i, i]);
                mCur.DivByLine(i, dItem);
                mResult.DivByLine(i, dItem);

                rTmp = mCur.GetLine(i);
                eTmp = mResult.GetLine(i);
                //Забить нулями вертикаль
                for (int j = 0; j < this.ColCount; j++)
                    if (i != j)
                    {
                        Complex con = new Complex(mCur[j, i]);
                        mCur.MinusVector(j, rTmp * con);
                        mResult.MinusVector(j, eTmp * con);
                    }
                //mCur.PrintMatrix(); Console.WriteLine();
                //mResult.PrintMatrix(); Console.WriteLine();
            }

            if (!correct) return mResult;
            return ReverseCorrect(this, mResult, 1e-16, 100);
        }
        /// <summary>
        /// Выдаёт индекс строки, содержащей максимальный элемент по столбцу column в диапазоне rowbeg...column.Len
        /// </summary>
        /// <param name="rowbeg"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private int MaxLine(int rowbeg, int column)
        {
            CVectors v = this.GetColumn(column);
            double max = v[rowbeg].Abs, tmp;
            int k = rowbeg;

            for (int i = rowbeg + 1; i < v.Degree; i++)
            {
                tmp = v[i].Abs;
                if (tmp > max)
                {
                    max = tmp;
                    k = i;
                }
            }
            return k;
        }


        /// <summary>
        /// Обратная матрица через алглиб
        /// </summary>
        /// <returns></returns>
        public CSqMatrix InvertAlg()
        {
            alglib.complex[,] res = new alglib.complex[this.ColCount, this.ColCount];
            for (int i = 0; i < this.ColCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    res[i, j] = new alglib.complex(this[i, j].Re, this[i, j].Im);

            alglib.cmatrixinverse(ref res, out int ti, out alglib.matinvreport rep);

            CSqMatrix m = new CSqMatrix(new Complex[this.ColCount, this.ColCount]);
            for (int i = 0; i < this.ColCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    m[i, j] = new Complex(res[i, j].x, res[i, j].y);
            return m;
        }

        /// <summary>
        /// Обратная матрица через метод из библиотеки MathNet. Когда мой метод работает хорошо, этот в половине случаев работает ещё лучше. Но когда у меня плохо, у этого лучше только на доли процентов
        /// </summary>
        /// <returns></returns>
        public CSqMatrix InvertByMathNet()
        {
            System.Numerics.Complex[,] mas = new System.Numerics.Complex[this.ColCount, this.ColCount];
            for (int i = 0; i < this.ColCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    mas[i, j] = new System.Numerics.Complex(this[i, j].Re, this[i, j].Im);
            MathNet.Numerics.LinearAlgebra.Matrix<System.Numerics.Complex> matrix = MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix.OfArray(mas);
            mas = matrix.Inverse().ToArray();

            Complex[,] res = new Complex[this.ColCount, this.ColCount];
            for (int i = 0; i < this.ColCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    res[i, j] = new Complex(mas[i, j].Real, mas[i, j].Imaginary);
            return new CSqMatrix(res);
        }

        private static MathNet.Numerics.LinearAlgebra.Matrix<System.Numerics.Complex> ToMathMas(CSqMatrix t)
        {
            System.Numerics.Complex[,] mas = new System.Numerics.Complex[t.ColCount, t.ColCount];
            for (int i = 0; i < t.ColCount; i++)
                for (int j = 0; j < t.ColCount; j++)
                    mas[i, j] = new System.Numerics.Complex(t[i, j].Re, t[i, j].Im);
           return MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix.OfArray(mas);
        }
        private static Complex[,] ToCompMas(MathNet.Numerics.LinearAlgebra.Matrix<System.Numerics.Complex> t)
        {
            System.Numerics.Complex[,] mas= t.ToArray();

            Complex[,] res = new Complex[mas.GetLength(0), mas.GetLength(1)];
            for (int i = 0; i < mas.GetLength(0); i++)
                for (int j = 0; j < mas.GetLength(1); j++)
                    res[i, j] = new Complex(mas[i, j].Real, mas[i, j].Imaginary);
            return res;
        }


        /// <summary>
        /// Обращение матрицы 2х2
        /// </summary>
        /// <returns></returns>
        public CSqMatrix Invert2()
        {
            Complex det = this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];
            return new CSqMatrix(new Complex[,] { { this[1, 1], -this[0, 1] }, { -this[1, 0], this[0, 0] } }) / det;
        }
        /// <summary>
        /// Обращение матрицы 4х4
        /// </summary>
        /// <returns></returns>
        public CSqMatrix Invert4()
        {
            CSqMatrix A11 = new CSqMatrix(new Complex[,] { { this[0, 0], this[0, 1] }, { this[1, 0], this[1, 1] } });
            CSqMatrix A12 = new CSqMatrix(new Complex[,] { { this[0, 2], this[0, 3] }, { this[1, 2], this[1, 3] } });
            CSqMatrix A21 = new CSqMatrix(new Complex[,] { { this[2, 0], this[2, 1] }, { this[3, 0], this[3, 1] } });
            CSqMatrix A22 = new CSqMatrix(new Complex[,] { { this[2, 2], this[2, 3] }, { this[3, 2], this[3, 3] } });

            CSqMatrix C11, C12, C21, C22, A22i = A22.Invert2(), A11i = A11.Invert2();//A22.Show();
            C11 = (A11 - A12 * A22i * A21).Invert2();
            C22 = (A22 - A21 * A11i * A12).Invert2();
            C21 = -A22i * A21 * C11;
            C12 = -A11i * A12 * C22;

            return new CSqMatrix(new Complex[,] {
                {C11[0,0],C11[0,1],C12[0,0],C12[0,1]},
                { C11[1,0],C11[1,1],C12[1,0],C12[1,1]},
                { C21[0,0],C21[0,1],C22[0,0],C22[0,1]},
                { C21[1,0],C21[1,1],C22[1,0],C22[1,1]}
            });

        }

        /// <summary>
        /// Обратная матрица, полученная через сумму степеней
        /// </summary>
        public CSqMatrix InvertSum
        {
            get
            {
                //if (this.Det == 0) throw new ArithmeticException("Матрица вырождена");
                CSqMatrix M = SqMatrix.E(this.ColCount);
                CSqMatrix A = new CSqMatrix(this);
                CVectors p = CharactPol(this);
                M *= p[1];
                for (int i = 2; i <= p.Degree - 1; i++)
                {
                    M += p[i] * A;
                    A *= this;
                }
                M *= -1;
                M /= p[0];
                return M;
            }
        }
        /// <summary>
        /// Характеристический многочлен заданной матрицы
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        private static CVectors CharactPol(CSqMatrix M)
        {
            CVectors p = new CVectors(M.ColCount + 1);

            Complex sum;
            CSqMatrix A = new CSqMatrix(M);
            //заполнение массива треков
            Complex[] tr = new Complex[A.ColCount];
            for (int i = 0; i < A.ColCount; i++)
            {
                tr[i] = A.Track;
                A *= M;
            }

            p[p.Degree - 1] = 1 * Math.Pow(-1, A.ColCount);
            int k = 0;
            for (int i = p.Degree - 2; i >= 0; i--)
            {
                sum = 0; k++;
                for (int j = 0; j < k; j++) sum += tr[k - j - 1] * p[p.Degree - j - 1];
                sum *= -1;
                sum /= k;
                p[i] = new Complex(sum);
            }

            return p;
        }

        /// <summary>
        /// Уточнение обратной матрицы
        /// </summary>
        /// <param name="A">Исходная матрица</param>
        /// <param name="Reverse">Обратная марица</param>
        /// <param name="eps">Точность</param>
        /// <returns></returns>
        public static CSqMatrix ReverseCorrect(CSqMatrix A, CSqMatrix Reverse, double eps = 0.001, int stepcount = 1000, bool existnorm = false)
        {
            CSqMatrix E = new CSqMatrix(SqMatrix.E(A.RowCount)), E2 = E * 2, R = new CSqMatrix(Reverse), Rold = new CSqMatrix(Reverse);
            int i = 0;//i.Show();
            double epsold = (E - A * R).CubeNorm, epsnew = epsold;
            //if (epsold < 1 || existnorm)
            while (epsnew > eps && i < stepcount)
            {
                R *= (E2 - A * R);//epsold.Show();
                //(E - A * R).CubeNorm.Show();
                epsold = epsnew;
                epsnew = (E - A * R).CubeNorm;
                if (epsnew >= epsold) {/*$"{epsold} {epsnew}".Show();*/ return Rold; }
                Rold = new CSqMatrix(R);
                i++;
            }
            return R;
        }
        /// <summary>
        /// Track матрицы
        /// </summary>
        public Complex Track
        {
            get
            {
                Complex sum = 0;
                for (int i = 0; i < this.RowCount; i++)
                    sum += this[i, i];
                return sum;
            }
        }

        public CSqMatrix Transpose => new CSqMatrix(this.Re.Transpose(), this.Im.Transpose());

        public Complex DetSarius
        {
            get
            {
                //Complex[] mas = new Complex[6];
                //double[] re = new double[6], im = new double[6];
                //mas[0] = ;

                return this[0, 0] * this[1, 1] * this[2, 2] - this[2, 0] * this[1, 1] * this[0, 2] + this[2, 0] * this[0, 1] * this[1, 2] - this[1, 0] * this[0, 1] * this[2, 2] + this[1, 0] * this[2, 1] * this[0, 2] - this[0, 0] * this[2, 1] * this[1, 2];
            }
        }

        /// <summary>
        /// Дубликат
        /// </summary>
        public CSqMatrix dup => new CSqMatrix(this);

        /// <summary>
        /// Возвращает матрицу, полученную из исходной исключением строки row и столбца col (начинаются с 1)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public CSqMatrix GetMinMat(int row, int col)
        {
            int k = this.ColCount - 1, i = row - 1, j = col - 1;
            CSqMatrix M = new CSqMatrix(new Complex[k, k]);
            int a = 0, b = 0;
            for (int ii = 0; ii < k + 1; ii++)
            {
                if (ii != i)
                {
                    for (int jj = 0; jj < k + 1; jj++)
                    {
                        if (jj != j)
                        {
                            M[a, b] = new Complex(this[ii, jj]); //Console.WriteLine("{0} {1} {2} {3}", a,b,ii,jj);
                            b++;
                        }
                    }
                    a++; b = 0;
                }
            }
            return M;
        }

        public static CSqMatrix operator +(CSqMatrix A, CSqMatrix B)// => new CSqMatrix(A.Re + B.Re, A.Im + B.Im);
        {
            Complex[,] mat = new Complex[A.RowCount, A.RowCount];
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.RowCount; j++)
                    mat[i, j] = A[i, j] + B[i, j];
            return new CSqMatrix(mat);
        }
        public static CSqMatrix operator -(CSqMatrix A) => new CSqMatrix(new SqMatrix((-A.Re).matrix), new SqMatrix((-A.Im).matrix));
        public static CSqMatrix operator -(CSqMatrix A, CSqMatrix B)// => new CSqMatrix(A.Re - B.Re, A.Im - B.Im);
        {
            Complex[,] mat = new Complex[A.RowCount,A.RowCount];
            for (int i = 0; i < A.RowCount; i++)
                for (int j = 0; j < A.RowCount; j++)
                    mat[i, j] = A[i, j] - B[i, j];
            return new CSqMatrix(mat);
        }
        public static CSqMatrix operator *(CSqMatrix A, CSqMatrix B) => new CSqMatrix(A.Re * B.Re - A.Im * B.Im, A.Re * B.Im + B.Re * A.Im);
        public static CVectors operator *(CSqMatrix A, CVectors x)
        {
            CVectors res = new CVectors(x.Degree);
            for (int i = 0; i < res.Degree; i++)
                for (int j = 0; j < res.Degree; j++)
                    res[i] += A[i, j] * x[j];
            return res;

            //if (A.ColCount != x.Degree) throw new Exception("Размерность матрицы и вектора не совпадают");
            //CVectors res = new CVectors(x.Degree);
            //for (int i = 0; i < res.Degree; i++)
            //    res[i] = A.GetLine(i) * x;
            //return res;
        }
        public static CSqMatrix operator *(Complex c, CSqMatrix A)
        {
            //CSqMatrix R = new CSqMatrix(A);R.Show();
            //for (int i = 0; i < R.ColCount; i++)
            //    for (int j = 0; j < R.RowCount; j++)
            //        R[i, j] = R[i,j]*c;R.Show();"".Show();
            //return R;
            return new CSqMatrix(A.Re * c.Re - A.Im * c.Im, A.Re * c.Im + c.Re * A.Im);
        }
        public static CSqMatrix operator *(CSqMatrix A, Complex c) => c * A;
        public static CSqMatrix operator /(CSqMatrix A, Complex c) => (1.0/c) * A;

        /// <summary>
        /// Ускоренное отнятие другой матрицы от исходной
        /// </summary>
        /// <param name="M"></param>
        public void FastLessen(CSqMatrix M)
        {
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.RowCount; j++)
                    this.matr[i, j].FastLessen(M[i, j]);
        }
        /// <summary>
        /// Ускоренное добавление другой матрицы от исходной
        /// </summary>
        /// <param name="M"></param>
        public void FastAdd(CSqMatrix M)
        {
            //var c = ToMathMas(this);
            //var r = ToMathMas(M);

            //var m = c + r;
            //this.matr = ToCompMas(m);

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.RowCount; j++)
                    this.matr[i, j].FastAdd(M[i, j]);
        }

        public void MoveTo(CSqMatrix t)
        {
            throw new NotImplementedException();
        }

        public static implicit operator CSqMatrix(SqMatrix sq)
        {
            CSqMatrix res = new CSqMatrix(sq.matrix);
            return res;
        }
    }
}

