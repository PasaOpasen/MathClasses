using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace МатКлассы
{
    //------------------------------------------матрицы
    /// <summary>
    /// Произвольных размеров матрицы
    /// </summary>
    public class Matrix:Idup<Matrix>
    {

        public Matrix dup => new Matrix(this);

        //размерность и сам массив
        /// <summary>
        /// Число столбцов в матрице
        /// </summary>
        protected internal int m;
        /// <summary>
        /// Число строк в матрице
        /// </summary>
        protected internal int n;
        /// <summary>
        /// Массив, отождествлённый с матрицей
        /// </summary>
        protected internal double[,] matrix;

        //Свойства-методы
        /// <summary>
        /// Обращение к матрице как к двумерному массиву
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        /// <summary>
        /// Обращение к матрице как к одномерному массиву (при условии, что в ней число строк либо число столбцов равно 1)
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double this[int i]
        {
            get
            {
                if (this.n == 1) return this[0, i];
                else if (this.m == 1) return this[i, 0];
                else throw new Exception(String.Format("Матрица размерности {0}x{1} не конвертируется в вектор, поэтому к её элементам нельзя обращаться как к векторам", n, m));
            }
            set
            {

            }
        }

        /// <summary>
        /// Количество строк в матрице
        /// </summary>
        public int RowCount => this.n;
        /// <summary>
        /// Количество столбцов в матрице
        /// </summary>
        public int ColCount => this.m;

        //Конструктор
        /// <summary>
        /// Матрица (0)
        /// </summary>
        public Matrix()//по умолчанию
        {
            this.n = this.m = 0;
            this.matrix = new double[n, m];
        }

        /// <summary>
        /// Квадратная нулевая матрица
        /// </summary>
        /// <param name="n">Размерность матрицы</param>
        public Matrix(int n)//по размерности (квадратная)
        {
            this.n = this.m = n;
            this.matrix = new double[n, n];
        }
        /// <summary>
        /// Прямоугольная нулевая матрица
        /// </summary>
        /// <param name="n">Число строк</param>
        /// <param name="m"></param>
        public Matrix(int n, int m)
        {
            this.n = n;
            this.m = m;
            this.matrix = new double[n, m];
        }
        /// <summary>
        /// Матрица из файла
        /// </summary>
        /// <param name="fs"></param>
        public Matrix(StreamReader fs)//через файл
        {

            string s = fs.ReadLine();
            string[] st = s.Split(' ', '\t');
            st = st.Where(n => n.Length != 0).ToArray();
            this.n = Convert.ToInt32(st[0]);
            try { this.m = Convert.ToInt32(st[1]); } catch { this.m = this.n; }
            this.matrix = new double[n, m];

            for (int k = 0; k < this.n; k++)
            {
                s = fs.ReadLine();
                s = s.Replace('.', ',');
                st = s.Split(' ', '\t');//в аргументах указывается массив символов, которым разделяются числа
                st = st.Where(n => n.Length != 0).ToArray();
                //st.Show();
                // try
                // {
                for (int i = 0; i < this.m; i++) this.matrix[k, i] = Convert.ToDouble(st[i]);
                //}
                //catch { throw new Exception("Тут"); }
            }


            fs.Close();
        }
        /// <summary>
        /// Генерировние матрицы по массиву её строк
        /// </summary>
        /// <param name="l"></param>
        public Matrix(Vectors[] l)
        {
            this.n = l.Length; this.m = l[0].Deg;
            for (int i = 0; i < this.n; i++)
                for (int j = 0; j < this.m; j++)
                    this[i, j] = l[i].vector[j];
        }
        /// <summary>
        /// Создать матрицу как вектор-столбец
        /// </summary>
        /// <param name="b"></param>
        public Matrix(double[] b)
        {
            this.n = b.Length;
            this.m = 1;
            this.matrix = new double[n, m];
            for (int i = 0; i < b.Length; i++) this.matrix[i, 0] = b[i];
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="M"></param>
        public Matrix(Matrix M)
        {
            this.m = M.m;
            this.n = M.n;
            this.matrix = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    this.matrix[i, j] = M[i, j];
        }
        /// <summary>
        /// Матрица по двумерному массиву
        /// </summary>
        /// <param name="mas"></param>
        public Matrix(double[,] mas)
        {
            this.n = mas.GetLength(0);
            this.m = mas.GetLength(1);
            this.matrix = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    this.matrix[i, j] = mas[i, j];
        }
        /// <summary>
        /// Создать матрицу по её размерности и массиву диагональных элементов
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="m">Число строк</param>
        /// <param name="n">Число столбцов</param>
        public Matrix(double[] mas, int m, int n) : this(m, n)
        {
            for (int i = 0; i < mas.Length; i++)
                this.matrix[i, i] = mas[i];
        }
        /// <summary>
        /// Преобразовать вектор в матрицу
        /// </summary>
        /// <param name="v"></param>
        public Matrix(Vectors v) : this(v.Deg, 1)
        {
            for (int i = 0; i < v.Deg; i++)
                this[i, 0] = v[i];
        }

        //методы
        /// <summary>
        /// Задание матрицы с помощью консоли
        /// </summary>
        public /*virtual*/ void CreateMatrix()//задать коэффициенты в матрице через консоль
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write("Введите элемент [" + i.ToString() + ";" + j.ToString() + "]" + "\t");
                    matrix[i, j] = Convert.ToDouble(Console.ReadLine());
                }
            }

        }
        /// <summary>
        /// Вывести матрицу на консоль
        /// </summary>
        public /*virtual*/ void PrintMatrix()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(matrix[i, j].ToString() + " \t");
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Число нулей в матрице
        /// </summary>
        protected internal int NullValue
        {
            get
            {
                int val = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (matrix[i, j] == 0) val++;
                    }
                }
                return val;
            }
        }
        /// <summary>
        /// Нулевая ли матрица?
        /// </summary>
        /// <returns></returns>
        public /*virtual*/ bool Nulle()
        {
            //for (int i = 0; i < n; i++)
            //{
            //    for (int j = 0; j < m; j++)
            //    {
            //        if (matrix[i, j] != 0) return false;
            //    }
            //}

            return this.NullValue == n * m;
        }
        /// <summary>
        /// Отнять от строки матрицы другую строку (i-k*j)
        /// </summary>
        /// <param name="i">Номер строки, от которой отнимают</param>
        /// <param name="j">Номер строки, которая отнимается</param>
        /// <param name="val">Коэффициент, на который умножается отнимаемая строка</param>
        public void LinesDiff(int i, int j, double val)
        {
            for (int k = 0; k < this.m; k++) this[i, k] -= val * this[j, k];
        }
        /// <summary>
        /// Отнять от строки матрицы вектор
        /// </summary>
        /// <param name="i"></param>
        /// <param name="c"></param>
        public void MinusVector(int i, Vectors c)
        {
            for (int j = 0; j < this.ColCount; j++)
                this[i, j] -= c[j];
        }

        /// <summary>
        /// Переставить строки
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void LinesSwap(int i, int j)
        {
            for (int k = 0; k < this.m; k++)
            {
                double tmp = this[i, k];
                this[i, k] = this[j, k];
                this[j, k] = tmp;
            }
        }
        /// <summary>
        /// Переставить столбцы
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void ColumnSwap(int i, int j)
        {
            for (int k = 0; k < this.n; k++)
            {
                double tmp = this[k, i];
                this[k, i] = this[k, j];
                this[k, j] = tmp;
            }
        }
        /// <summary>
        /// Удалить столбец матрицы
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Matrix ColumnDelete(int k)
        {
            Matrix A = new Matrix(this.n, this.m - 1);
            for (int i = 0; i < this.n; i++)
                for (int j = 0; j < k; j++)
                    A[i, j] = this[i, j];
            for (int i = 0; i < this.n; i++)
                for (int j = k + 1; j < this.m; j++)
                    A[i, j - 1] = this[i, j];
            return A;
        }
        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static Matrix Transpose(Matrix M)
        {
            Matrix MT = new Matrix(M.m, M.n);
            for (int i = 0; i < MT.n; i++)
                for (int j = 0; j < MT.m; j++) MT[i, j] = M[j, i];
            return MT;
        }
        /// <summary>
        /// Возврат транспонированной матрицы
        /// </summary>
        /// <returns></returns>
        public virtual Matrix Transpose() { return Matrix.Transpose(this); }
        /// <summary>
        /// Норма Фробениуса
        /// </summary>
        public double Frobenius
        {
            get
            {
                double sum = 0;
                for (int i = 0; i < this.n; i++)
                    for (int j = 0; j < this.m; j++)
                        sum += this[i, j] * this[i, j];
                return Math.Sqrt(sum);
            }
        }
        /// <summary>
        /// Перевести строку матрицы в вектор
        /// </summary>
        /// <returns></returns>
        public Vectors GetLine(int k)
        {
            Vectors v = new Vectors(this.m);
            for (int j = 0; j < v.Deg; j++) v[j] = this[k, j];
            return v;
        }
        /// <summary>
        /// Получить строку матрицы в строковом формате
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public string GetLineString(int k)
        {
            string v = "";
            for (int j = 0; j < this.ColCount; j++) v += this[k, j].ToString()+" ";
            return v;
        }
        /// <summary>
        /// Перевести строку матрицы в вектор
        /// </summary>
        /// <returns></returns>
        public Vectors GetLine(int k, int beg, int end)
        {
            Vectors v = new Vectors(end - beg + 1);
            for (int j = beg; j <= end; j++) v[j - beg] = this[k, j];
            return v;
        }

        /// <summary>
        /// Прибавить ко всеми элементам строки число
        /// </summary>
        /// <param name="line">Номер строки</param>
        /// <param name="val">Число, которое прибавляется</param>
        public void AddByLine(int line, double val)
        {
            line--;
            for (int j = 0; j < this.ColCount; j++)
                this[line, j] += val;
        }
        /// <summary>
        /// Прибавить ко всем элементам столбца число
        /// </summary>
        /// <param name="col">Номер столбца</param>
        /// <param name="val"></param>
        public void AddByColumn(int col, double val)
        {
            col--;
            for (int j = 0; j < this.RowCount; j++)
                this[j, col] += val;
        }
        /// <summary>
        /// Поделить строку в матрице на число
        /// </summary>
        /// <param name="i"></param>
        /// <param name="val"></param>
        public void DivByLine(int i, double val)
        {
            for (int j = 0; j < this.ColCount; j++)
                this[i, j] /= val;
        }

        /// <summary>
        /// Кубическая норма
        /// </summary>
        public double CubeNorm
        {
            get
            {
                double[] mas = new double[this.RowCount];
                for (int i = 0; i < mas.Length; i++)
                    for (int j = 0; j < this.ColCount; j++)
                        mas[i] += Math.Abs(this[i, j]);
                return mas.Max();
            }
        }
        /// <summary>
        /// Октаэдрическая норма
        /// </summary>
        public double OctNorn
        {
            get
            {
                double[] mas = new double[this.ColCount];
                for (int i = 0; i < mas.Length; i++)
                    for (int j = 0; j < this.RowCount; j++)
                        mas[i] += Math.Abs(this[j, i]);
                return mas.Max();
            }
        }
        /// <summary>
        /// Максимальная абсолютная величина в матрице
        /// </summary>
        public double MaxofMod
        {
            get
            {
                double[] mas = new double[this.RowCount];
                for (int i = 0; i < mas.Length; i++)
                {
                    double[] mas2 = new double[this.ColCount];
                    for (int j = 0; j < this.ColCount; j++)
                        mas2[j] = Math.Abs(this[i, j]);
                    mas[i] = mas2.Max();
                }

                return mas.Max();
            }
        }

        public double Min
        {
            get
            {
                Vectors[] v = new Vectors[this.RowCount];
                double[] mas = new double[v.Length];
                for (int i = 0; i < n; i++)
                {
                    v[i] = this.GetLine(i);
                    mas[i] = v[i].Min;
                }
                return mas.Min();
            }
        }
        public double Max => -(-this).Min;

        //операторы
        //сложение
        public static Matrix operator +(Matrix A, Matrix B)
        {
            if ((A.n != B.n) || (A.m != B.m)) return new Matrix();
            Matrix C = new Matrix(A.n, B.m);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < B.m; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }
            return C;
        }

        //вычитание
        public static Matrix operator -(Matrix A, Matrix B)
        {
            if ((A.n != B.n) || (A.m != B.m)) return new Matrix();
            Matrix R = new Matrix(A.n, B.m);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.m; j++)
                {
                    R[i, j] = A[i, j] - B[i, j];
                }
            }
            return R;

        }
        public static Matrix operator -(Matrix A)
        {
            Matrix R = new Matrix(A);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.m; j++)
                {
                    R[i, j] *= -1;
                }
            }
            return R;
        }

        //произведение
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.m != B.n) return new Matrix();
            Matrix R = new Matrix(A.n, B.m);
            Parallel.For(0, A.n, (int i) => { 
           // for (int i = 0; i < A.n; i++)
            //{
                for (int j = 0; j < B.m; j++)
                {
                    for (int k = 0; k < B.n; k++)
                    {
                        R[i, j] += A[i, k] * B[k, j];
                    }
                }
            //}
            });
            return R;
        }

        //Умножение на число
        public static Matrix operator *(Matrix A, double Ch)
        {
            Matrix q = new Matrix(A.n, A.m);
            for (int i = 0; i < A.n; i++)
            {
                for (int j = 0; j < A.m; j++)
                {
                    q[i, j] = A[i, j] * Ch;
                }
            }
            return q;
        }

        public static Matrix operator /(Matrix A, double c) => A * (1.0 / c);

        public static Matrix operator *(double c, Matrix A) => A * c;

        public static Vectors operator *(Vectors v,Matrix A)
        {
            Vectors res = new Vectors(A.ColCount);
            Parallel.For(0, res.Deg, (int i) => {
                for (int j = 0; j < v.Deg; j++)
                    res[i] += v[j] * A[j,i ];
            });
            return res;
        }
        public static Vectors operator *(Matrix A,Vectors v )
        {
            Vectors res = new Vectors(A.RowCount);
            Parallel.For(0, res.Deg, (int i) => {
                for (int j = 0; j < v.Deg; j++)
                    res[i] += v[j] * A[i,j ];
            });
            return res;
        }

        //public static implicit operator SqMatrix(Matrix M) => new SqMatrix(M.matrix);

        /// <summary>
        /// Сингулярное разложение матрицы через библиотеку alglib
        /// </summary>
        /// <param name="A">Исходная матрица</param>
        /// <param name="U">Левая матрица</param>
        /// <param name="w">Вектор сингулярных чисел</param>
        /// <param name="VT">Правая матрица</param>
        public static void SVD(Matrix A, out Matrix U, out double[] w, out Matrix VT)
        {
            double[,] mas = A.matrix, u = new double[1, 1], vt = new double[1, 1];
            w = new double[0];
            bool resjust = alglib.svd.rmatrixsvd(mas, A.RowCount, A.ColCount, 2, 2, 2, ref w, ref u, ref vt);
            U = new Matrix(u);
            VT = new Matrix(vt);
        }

        /// <summary>
        /// Умножение матрицы на вектор с использованием распараллеливания
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vectors FastMult(Matrix A, Vectors b)
        {
            Vectors res = new Vectors(A.n);

            Parallel.For(0, A.n, (int i) => {
                for (int j = 0; j < A.m; j++)
                    res[i] += A[i, j] * b[j];
            });

            return res;
        }

        /// <summary>
        /// Создать матрицу по стобцам
        /// </summary>
        /// <param name="a">Вектор чисел, на которые умножается эталонный столбец</param>
        /// <param name="v">Вектор, ставящийся в столбец</param>
        /// <returns></returns>
        public static Matrix Create(Vectors a,Vectors v)
        {
            Matrix res = new Matrix(v.Deg, a.Deg);
            for (int i = 0; i < res.n; i++)
                for (int j = 0; j < res.m; j++)
                    res[i, j] = v[i] * a[j];
            return res;

        }

        /// <summary>
        /// Сумма элементов матрицы
        /// </summary>
        public double Sum
        {
            get
            {
                double s = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < m; j++)
                        s += this[i, j];
                return s;
            }
        }

        /// <summary>
        /// Размах элементов в матрице
        /// </summary>
        public double Range => Math.Abs(this.Max-this.Min);

        /// <summary>
        /// Среднее арифметическое элементов матрицы
        /// </summary>
        public double Center => Sum / (n * m);

        /// <summary>
        /// Создать матрицу по вектору
        /// </summary>
        /// <param name="v"></param>
        /// <param name="dim">Число строк или столбцов в зависимости от col</param>
        /// <param name="col">Если true, заполняются сначала строки матрицы, иначе --- столбцы</param>
        /// <returns></returns>
        public static Matrix Create(Vectors v,int dim=1, bool col = true)
        {
            Matrix res;
            int k = 0;

            if (col)
            {
                res = new Matrix(v.Deg / dim, dim);
                for (int i = 0; i < res.RowCount; i++)
                    for (int j = 0; j < res.ColCount; j++)
                        res[i, j] = v[k++];
            }
            else
            {
                res = new Matrix(dim,v.Deg / dim );
                for (int j = 0; j < res.ColCount; j++)
                for (int i = 0; i < res.RowCount; i++)
                        res[i, j] = v[k++];
            }


            return res;
        }

        public void MoveTo(Matrix t)
        {
            throw new NotImplementedException();
        }
    }
}

