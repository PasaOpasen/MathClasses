using System;
using System.Linq;
using System.IO;

namespace МатКлассы
{
    /// <summary>
    /// Класс СЛАУ с методами их решения
    /// </summary>
    public class SLAU
    {
        private int size;
        /// <summary>
        /// Размерность системы
        /// </summary>
        public int Size => size;

        /// <summary>
        /// Матрица системы
        /// </summary>
        public double[,] A;
        /// <summary>
        /// Свободный вектор системы
        /// </summary>
        public double[] b;
        /// <summary>
        /// Вектор решения
        /// </summary>
        public double[] x;

        /// <summary>
        /// Невязка решения
        /// </summary>
        public static double? NEVA = null;

        /// <summary>
        /// Класс функций для элементов системы (написано в процедурном стиле, перенёс из курсача, так как матрица системы - это массив массивов)
        /// </summary>
        public static class Func_in_matrix
        {
            /// <summary>
            /// Частичное произведение матрицы на вектор
            /// </summary>
            /// <param name="Ax"></param>
            /// <param name="a"></param>
            /// <param name="x"></param>
            /// <param name="k"></param>
            public static void Matrix_power(ref double[] Ax, double[,] a, double[] x, int k)
            {
                for (int i = 0; i < k; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < k; j++)
                    {
                        sum += a[i, j] * x[j];
                    }
                    Ax[i] = sum;
                }
            }
            /// <summary>
            /// Разность двух векторов
            /// </summary>
            /// <param name="r"></param>
            /// <param name="Ax"></param>
            /// <param name="b"></param>
            /// <param name="t"></param>
            public static void Vector_difference(ref double[] r, double[] Ax, double[] b, int t)
            {
                for (int i = 0; i < t; i++)
                {
                    r[i] = Ax[i] - b[i];
                }
            }
            /// <summary>
            /// Классическое скалярное произведение двух векторов
            /// </summary>
            /// <param name="r"></param>
            /// <param name="rr"></param>
            /// <param name="t"></param>
            /// <returns></returns>
            public static double Scalar_power(double[] r, double[] rr, int t)
            {
                double s = 0;
                for (int i = 0; i < t; i++)
                {
                    s += r[i] * rr[i];
                }
                return s;
            }
            /// <summary>
            /// Умножение вектора на скаляр
            /// </summary>
            /// <param name="s"></param>
            /// <param name="tau"></param>
            /// <param name="r"></param>
            /// <param name="t"></param>
            public static void Vector_on_scalar(ref double[] s, double tau, double[] r, int t)
            {
                for (int i = 0; i < t; i++)
                {
                    s[i] = tau * r[i];
                }
            }
            /// <summary>
            /// Сумма векторов
            /// </summary>
            /// <param name="sum"></param>
            /// <param name="s"></param>
            /// <param name="x"></param>
            /// <param name="t"></param>
            public static void Vector_sum(ref double[] sum, double[] s, double[] x, int t)
            {
                for (int i = 0; i < t; i++)
                {
                    sum[i] = x[i] + s[i];
                }
            }
            /// <summary>
            /// Присваивание одному вектору другого
            /// </summary>
            /// <param name="x"></param>
            /// <param name="s"></param>
            /// <param name="t"></param>
            public static void Vector_assingment(ref double[] x, double[] s, int t)
            {
                Vector_on_scalar(ref x, 1, s, t);
            }
        }

        /// <summary>
        /// Функция частичной невязки
        /// </summary>
        /// <param name="A"></param>
        /// <param name="x"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Nev(double[,] A, double[] x, double[] b, int t)
        {
            double[] Ax = new double[t];
            Func_in_matrix.Matrix_power(ref Ax, A, x, t);
            double s = 0;
            for (int i = 0; i < t; i++)
            {
                s += ((Ax[i] - b[i]) * (Ax[i] - b[i]));
            }
            return Math.Sqrt(s);
        }
        /// <summary>
        /// Частичная невязка используемой системы
        /// </summary>
        /// <param name="t">Размерность подсистемы</param>
        /// <returns></returns>
        public double Nev(int t) { return SLAU.Nev(this.A, this.x, this.b, t); }
        public double Nevaska => Nev(this.size);
        /// <summary>
        /// Невязка системы
        /// </summary>
        public double Discrep
        {
            get { return this.Nev(this.size); }
        }

        //public double Error(int k) //частичная погрешность
        //{
        //    double p = myCurve.Firstkind(N, N);
        //    double sum = 0;

        //    double[] Ax = new double[N];
        //    Func_in_matrix.Matrix_power(Ax, A, x, k);
        //    for (int i = 0; i < k; i++)
        //    {
        //        sum += x[i] * Ax[i];
        //    }
        //    double EPS = Math.Abs(p - sum);
        //    return Math.Sqrt(EPS);
        //}

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public SLAU() { Make(1); }
        /// <summary>
        /// Прочитать систему из файла
        /// </summary>
        /// <param name="fs"></param>
        public SLAU(StreamReader fs)//конструктор через файл
        {
            string s = fs.ReadLine();
            try//standart
            {
                string[] st = s.Split(' ');
                //if(st.Length==2 &&)
                Make(Convert.ToInt32(st[0]));

                for (int k = 0; k < this.size; k++)
                {
                    s = fs.ReadLine();
                    st = s.Split(' ');//в аргументах указывается массив символов, которым разделяются числа
                    for (int i = 0; i < this.size; i++) this.A[k, i] = Convert.ToDouble(st[i]);
                }
                s = fs.ReadLine();
                //for (int k = 0; k < this.size; k++) { s = fs.ReadLine(); st = s.Split(' '); this.x[k] = Convert.ToDouble(st[0]); }
                //s = fs.ReadLine();
                for (int k = 0; k < this.size; k++) { s = fs.ReadLine(); st = s.Split(' '); this.b[k] = Convert.ToDouble(st[0]); }
            }
            catch
            {
                string[] st = s.Split(' ', '|', '\t');
                st = st.Where(n => n.Length > 0).ToArray();
                try//with vector x
                {
                    Make(Convert.ToInt32(st.Length - 2));
                    for (int i = 0; i < this.size; i++) this.A[0, i] = Convert.ToDouble(st[i]);
                    this.x[0] = Convert.ToDouble(st[st.Length - 2]);
                    this.b[0] = Convert.ToDouble(st[st.Length - 1]);
                    for (int k = 1; k < this.size; k++)
                    {
                        s = fs.ReadLine();
                        st = s.Split(' ', '|', '\t');//в аргументах указывается массив символов, которым разделяются числа
                        st = st.Where(n => n.Length > 0).ToArray();
                        for (int i = 0; i < this.size; i++) this.A[k, i] = Convert.ToDouble(st[i]);
                        this.x[k] = Convert.ToDouble(st[st.Length - 2]);
                        this.b[k] = Convert.ToDouble(st[st.Length - 1]);
                    }
                    s = fs.ReadLine();
                    st = s.Split(' ', '|', '\t');
                    st = st.Where(n => n.Length > 0).ToArray();

                    if (st.Length != 0) throw new Exception("Вектор x отсутствует");
                }
                catch//without vector x
                {
                    fs.BaseStream.Position = 0;
                    s = fs.ReadLine();
                    st = s.Split(' ', '|', '\t');
                    st = st.Where(n => n.Length > 0).ToArray();
                    //st.Show();
                    Make(Convert.ToInt32(st.Length - 1));
                    for (int i = 0; i < this.size; i++) this.A[0, i] = Convert.ToDouble(st[i]);
                    this.b[0] = Convert.ToDouble(st[st.Length - 1]);
                    for (int k = 1; k < this.size; k++)
                    {
                        s = fs.ReadLine();
                        st = s.Split(' ', '|', '\t');//в аргументах указывается массив символов, которым разделяются числа
                        st = st.Where(n => n.Length > 0).ToArray();
                        for (int i = 0; i < this.size; i++) this.A[k, i] = Convert.ToDouble(st[i]);
                        this.b[k] = Convert.ToDouble(st[st.Length - 1]);
                    }
                }

            }
            finally { fs.Close(); }
        }

        /// <summary>
        /// Создать нулевую систему заданной размерности
        /// </summary>
        /// <param name="k"></param>
        public SLAU(int k) { Make(k); }//создать систему такой размерности
                                       /// <summary>
                                       /// Задать систему по её расширенной матрице
                                       /// </summary>
                                       /// <param name="M"></param>
        public SLAU(Matrix M)
        {
            SLAU g = new SLAU(M.n);
            for (int i = 0; i < M.n; i++)
            {
                g.b[i] = M[i, M.m - 1];
                for (int j = 0; j < M.m - 1; j++) g.A[i, j] = M[i, j];
            }

            size = M.n;
            A = new double[size, size];
            b = new double[size];
            x = new double[size];

            this.A = g.A;
            this.b = g.b;
            this.x = g.x;
        }
        /// <summary>
        /// Создать систему, заполненную скалярными произведениями
        /// </summary>
        /// <param name="p">Функция из некоторой системы</param>
        /// <param name="f">Действительная функция (которую требуется аппроксимирвать)</param>
        /// <param name="k">Количество используемых функций системы</param>
        /// <param name="a">Начало отрезка интегрирования</param>
        /// <param name="b">Конец отрезка интегрирования</param>
        public SLAU(SequenceFunc p, Func<double,double> f, int k, double a, double b, SequenceFuncKind kind = SequenceFuncKind.Other)
        {
            Make(k);
            Func<double,double>[] fi = new Func<double,double>[k];
            for (int i = 0; i < k; i++)
            {
                fi[i] = new Func<double,double>((double x) => p(x, i));
                //Console.WriteLine(fi[i](3)+" "+p(3,i));
                this.b[i] = FuncMethods.RealFuncMethods.ScalarPower(f, fi[i], a, b);
            }
            switch (kind)
            {
                case SequenceFuncKind.Other:
                    for (int i = 0; i < k; i++)
                    {
                        fi[i] = new Func<double,double>((double x) => p(x, i));//массив функций дохуя неустойчив и выдаёт какую-то дичь! функции всегда надо определять заново!!!!!!
                        A[i, i] = FuncMethods.RealFuncMethods.ScalarPower(fi[i], fi[i], a, b);
                        for (int j = i + 1; j < k; j++)
                        {
                            fi[j] = new Func<double,double>((double x) => p(x, j));
                            A[i, j] = FuncMethods.RealFuncMethods.ScalarPower(fi[j], fi[i], a, b);
                            A[j, i] = A[i, j];
                        }
                    }
                    break;
                case SequenceFuncKind.Orthogonal:
                    for (int i = 0; i < k; i++)
                    {
                        fi[i] = new Func<double,double>((double x) => p(x, i));
                        A[i, i] = FuncMethods.RealFuncMethods.NormScalar(fi[i], a, b);
                    }

                    break;
                default:
                    for (int i = 0; i < k; i++)
                    {
                        A[i, i] = 1.0 / (b - a);
                        //this.b[i] *= (b - a);
                    }
                    break;
            }
            this.f = f;
            this.p = p;
            this.begin = a;
            this.end = b;
            this.Show();
        }
        /// <summary>
        /// Создать систему, заполненную скалярными произведениями по сетке
        /// </summary>
        /// <param name="p">Функция из некоторой системы</param>
        /// <param name="f">Сеточная функция (которую требуется аппроксимирвать)</param>
        public SLAU(SequenceFunc p, FuncMethods.NetFunc f, int k = 0)
        {
            if (k == 0) k = f.CountKnots;
            Make(k);
            Func<double,double>[] fi = new Func<double,double>[k];
            //for (int i = 0; i < k; i++) fi[i] = new Func<double,double>((double x) => p(x, i));
            for (int i = 0; i < k; i++)
            {
                fi[i] = new Func<double,double>((double x) => p(x, i));
                //Console.WriteLine(fi[i](3)+" "+p(3,i));
                this.b[i] = FuncMethods.NetFunc.ScalarP(f, fi[i]);
                A[i, i] = FuncMethods.NetFunc.ScalarP(fi[i], fi[i], f.Arguments);
                for (int j = i + 1; j < k; j++)
                {
                    fi[j] = new Func<double,double>((double x) => p(x, j));
                    A[i, j] = FuncMethods.NetFunc.ScalarP(fi[j], fi[i], f.Arguments);
                    A[j, i] = A[i, j];
                }
            }
        }
        /// <summary>
        /// Создать систему, заполненную скалярными произведениями
        /// </summary>
        /// <param name="p">Полином из некоторой системы</param>
        /// <param name="f">Действительная функция (которую требуется аппроксимирвать)</param>
        /// <param name="k">Количество используемых функций системы</param>
        /// <param name="a">Начало отрезка интегрирования</param>
        /// <param name="b">Конец отрезка интегрирования</param>
        public SLAU(SequencePol p, Func<double,double> f, int k, double a, double b, SequenceFuncKind kind = SequenceFuncKind.Other)
        {
            Make(k);
            Func<double,double>[] fi = new Func<double,double>[k];
            for (int i = 0; i < k; i++)
            {
                fi[i] = p(i).Value;
                this.b[i] = FuncMethods.RealFuncMethods.ScalarPower(f, fi[i], a, b);
            }
            switch (kind)
            {
                case SequenceFuncKind.Other:
                    for (int i = 0; i < k; i++)
                    {
                        fi[i] = p(i).Value;
                        A[i, i] = FuncMethods.RealFuncMethods.ScalarPower(fi[i], fi[i], a, b);
                        for (int j = i + 1; j < k; j++)
                        {
                            fi[j] = p(j).Value;
                            A[i, j] = FuncMethods.RealFuncMethods.ScalarPower(fi[j], fi[i], a, b);
                            A[j, i] = A[i, j];
                        }
                    }
                    break;
                case SequenceFuncKind.Orthogonal:
                    for (int i = 0; i < k; i++)
                    {
                        fi[i] = p(i).Value;
                        A[i, i] = FuncMethods.RealFuncMethods.ScalarPower(fi[i], fi[i], a, b);
                    }
                    break;
                default:
                    for (int i = 0; i < k; i++)
                        A[i, i] = 1;
                    break;
            }
        }

        /// <summary>
        /// Создать систему как подсистему исходной системы с заданной разменостью 
        /// </summary>
        /// <param name="M"></param>
        /// <param name="t">Размерность подсистемы</param>
        public SLAU(SLAU M, int t)
        {
            this.size = t;
            A = new double[size, size];
            b = new double[size];
            x = new double[size];

            for (int i = 0; i < size; i++)
            {
                b[i] = M.b[i];
                x[i] = M.x[i];
                for (int j = 0; j < size; j++) A[i, j] = M.A[i, j];
            }
        }

        public SLAU(SLAU M) : this(M, M.Size) { }
        /// <summary>
        /// Задание СЛАУ по матрице и свободному вектору
        /// </summary>
        /// <param name="M"></param>
        /// <param name="b"></param>
        public SLAU(SqMatrix M, Vectors b)
        {
            Make(b.Deg);
            for (int i = 0; i < b.Deg; i++)
            {
                for (int j = 0; j < b.Deg; j++)
                    this.A[i, j] = M[i, j];
                this.b[i] = b[i];
            }
        }

        /// <summary>
        /// Создание двумерного и одномерных динамических массивов с заданной размерностью
        /// </summary>
        /// <param name="k"></param>
        public virtual void Make(int k)
        {
            size = k;
            A = new double[size, size];
            //for (int i = 0; i < size; i++)
            //{
            //    A[i] = new double[size];
            //}
            b = new double[size];
            x = new double[size];
            UltraCount = 0;
            ultraval = -1;
            ErrorsMas = new double[k];
            ErrorMasP = new double[k];
            //for (int i = 0; i < size; i++)
            //{
            //    x[i] = 0;
            //}
        }
        /// <summary>
        /// Решение методом прогонки системы с трёхдиагональной матрицей
        /// </summary>
        public void ProRace()
        {
            SqMatrix t = new SqMatrix(this.A);
            //if (!t.IsTreeDiag()) throw new Exception("Матрица не тридиагональная!");
            int n = this.size;
            double[] a = new double[n + 1];
            double[] b = new double[n + 1];
            double[] c = new double[n + 1];
            double[] alp = new double[n + 1];
            double[] bet = new double[n + 1];

            //Заполнение массивов диагоналей
            b[1] = this.A[1 - 1, 1 - 1];
            c[1] = this.A[1 - 1, 2 - 1];
            for (int i = 2; i < n; i++)
            {
                b[i] = this.A[i - 1, i - 1];
                c[i] = this.A[i - 1, i + 1 - 1];
                a[i] = this.A[i - 1, i - 1 - 1];
            }
            b[n] = this.A[n - 1, n - 1];
            a[n] = this.A[n - 1, n - 1 - 1];

            //Vectors t = new Vectors(c);t.Show();

            //прямой ход 
            alp[2] = -c[1] / b[1];
            bet[2] = this.b[1 - 1] / b[1];
            for (int i = 2; i < n; i++)
            {
                double val = b[i] + a[i] * alp[i];
                alp[i + 1] = -c[i] / val;
                bet[i + 1] = (-a[i] * bet[i] + this.b[i - 1]) / val;
            }

            //обратный ход 
            this.x[n - 1] = (-a[n] * bet[n] + this.b[n - 1]) / (b[n] + a[n] * alp[n]);
            for (int i = n - 1; i >= 1; i--)
            {
                this.x[i - 1] = alp[i + 1] * this.x[i + 1 - 1] + bet[i + 1];
            }
        }

        /// <summary>
        /// Метод Гаусса (частичный)
        /// </summary>
        /// <param name="t">Количество строк, с которыми происходит преобразование</param>
        /// <param name="getdiv">Надо ли делить на среднее по модулю в строке</param>
        public void Gauss(int t, bool getdiv = false)
        {
            if (A[0, 0] < 1e-4) getdiv = true;

            //создание вспомогательной матрицы системы
            double[,] matrix = new double[size, size + 1];
            //for (int i = 0; i < size; i++)
            //{
            //    matrix[i] = new double[size + 1];
            //}
            //присваивание её элементам нужных значений
            for (int i = 0; i < size; i++)
            {
                matrix[i, size] = b[i];
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = A[i, j];
                }
            }

            double m; //промежуточный множитель

            Vectors res = new Vectors(size);
            if (getdiv)
            {
                Vectors v = new Vectors(size);
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                        v[j] = Math.Abs(matrix[i, j]);
                    res[i] = v.ArithmeticAv;
                    for (int j = 0; j < size + 1; j++)
                        matrix[i, j] /= res[i];
                }

            }

            //прямой ход (без вывода матрицы, потому что работает)
            for (int j = 0; j < t; j++)
            {

                for (int i = j + 1; i < t; i++)
                {
                    m = matrix[i, j] / matrix[j, j];

                    for (int _h = j; _h < t; _h++)
                    {
                        matrix[i, _h] -= m * matrix[j, _h];
                    }
                    matrix[i, size] -= matrix[j, size] * m;
                }
            }

            //обратный ход		
            for (int j = t - 1; j >= 0; j--)
            {
                z2:
                for (int i = j - 1; i >= 0; i--)
                {
                    if (matrix[j, j] == 0)
                    {
                        j--;
                        goto z2;
                    }
                    m = matrix[i, j] / matrix[j, j];

                    matrix[i, size] -= matrix[j, size] * m;
                    matrix[i, j] -= m * matrix[j, j];
                }
            }

            //заполнение решения
            for (int i = 0; i < t; i++)
            {
                x[i] = matrix[i, size] / matrix[i, i];
            }
            if (getdiv)
                for (int i = 0; i < t; i++)
                    x[i] *= res[i];

            NEVA = Nev(A, x, b, t); //невязка фиксируется
        }
        /// <summary>
        /// Стандартный метод Гаусса
        /// </summary>
        public void Gauss() { Gauss(this.size); }
        /// <summary>
        /// Метод Гаусса, годный и при нулевых коэффициентах в системе
        /// </summary>
        public void GaussSelection()
        {
            Matrix S = new Matrix(this.size, this.size + 1);
            for (int j = 0; j < this.size; j++)
            {
                for (int i = 0; i < this.size; i++) S[i, j] = this.A[i, j];
                S[j, this.size] = this.b[j];
            }

            //S.LinesDiff(2, 1, 2); S.PrintMatrix();

            for (int j = 0; j < this.size; j++)
            {
                int k = j;
                if (S[k, j] == 0)//если ведущий элемент равен нулю, поменять эту строку местами с ненулевой
                {
                    int h = k;
                    while (S[h, j] == 0) h++;
                    S.LinesSwap(k, h);
                }

                while (S[k, j] == 0 && k < this.size - 1) k++;//найти ненулевой элемент
                int l = k + 1;
                if (k != this.size - 1) while (l != this.size) { S.LinesDiff(l, k, S[l, j] / S[k, j]); l++; } //отнимать от строк снизу
                                                                                                              //S.PrintMatrix();Console.WriteLine();
                l = k - 1;
                if (k != 0) while (l != -1) { S.LinesDiff(l, k, S[l, j] / S[k, j]); l--; }//отнимать от строк сверху
            }

            for (int i = 0; i < this.size; i++) this.x[i] = S[i, this.size] / S[i, i];
        }
        public void GaussSelection(int t)
        {
            SLAU s = new SLAU(t);
            for (int i = 0; i < t; i++)
            {
                s.b[i] = this.b[i];
                s.A[i, i] = this.A[i, i];
                for (int j = i + 1; j < t; j++)
                {
                    s.A[i, j] = this.A[i, j];
                    s.A[j, i] = this.A[i, j];
                }
            }
            s.GaussSelection();
            for (int i = 0; i < t; i++)
                this.x[i] = s.x[i];
        }
        /// <summary>
        /// Решение уравнения Ах=b методом Холецкого, присвоение вектору х значений решения
        /// </summary>
        /// <param name="z"></param>
        public void Holets(int z)
        {
            SqMatrix M = new SqMatrix(this.A);
            if (!M.IsPositCertain() || !M.IsSymmetric()) throw new Exception("Матрица не симметрическая или не положительно определённая!"); "".Show();

            //создание вспомогательной матрицы
            SqMatrix t = new SqMatrix(z);

            //прямой ход метода
            t[0, 0] = Math.Sqrt(A[0, 0]);
            if (z == 1)
            {
                x[0] = b[0] / A[0, 0];
                return;
            }

            for (int j = 1; j < z; j++)
            {
                t[0, j] = A[0, j] / t[0, 0];
            }


            for (int i = 1; i < z; i++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                {
                    sum += t[k, i] * t[k, i];
                }
                t[i, i] = Math.Sqrt(/*Math.Abs*/(A[i, i] - sum)); //без модуля не получается

                for (int j = i + 1; j < z; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += t[k, i] * t[k, j];
                    }
                    t[i, j] = (A[i, j] - sum) / t[i, i];
                }
            }

            //t.PrintMatrix();

            //обратный ход метода     
            double[] y = new double[z];
            y[0] = b[0] / t[0, 0];
            for (int i = 1; i < z; i++)
            {
                double sum = 0;
                for (int k = 0; k <= i - 1; k++)
                {
                    sum += t[k, i] * y[k];
                }
                y[i] = (b[i] - sum) / t[i, i];
            }

            x[z - 1] = y[z - 1] / t[z - 1, z - 1];
            for (int i = z - 2; i >= 0; i--)
            {
                double sum = 0;
                for (int k = i + 1; k < z; k++)
                {
                    sum += t[i, k] * x[k];
                }
                x[i] = (y[i] - sum) / t[i, i];
            }

            NEVA = Nev(A, x, b, z);
        }
        /// <summary>
        /// Решение уравнения Ах=b методом Якоби (простой итерации), присвоение вектору х значений решения
        /// </summary>
        /// <param name="t"></param>
        public void Jak(int t, double eps = 0.000001, int maxit = 0)
        {
            //создать диагональное преобладание
            SqMatrix M = new SqMatrix(this.A);
            SqMatrix A = new SqMatrix(M * M.Transpose());
            Matrix bb = new Matrix(this.b);
            Vectors b = new Vectors(M.Transpose() * bb);
            //for (int i = 0; i < t; i++)
            //{
            //    x[i] = 0; //первое приближение - свободный вектор
            //}
            double E;
            double EPSJ = eps;
            double NE = Nev(this.A, this.x, this.b, t);
            int num = 0; //переменные, связанные с количеством итераций
            int maxI = maxit;
            if (maxI == 0) maxI = t * t;
            while ((Nev(this.A, this.x, this.b, t) > EPSJ) && (num <= maxI))
            {
                NE = Nev(this.A, this.x, this.b, t);
                //cout<<NE<<endl;
                for (int i = 0; i < t; i++)
                {
                    E = 0;
                    for (int j = 0; j < t; j++)
                    {
                        z1:
                        if (j == i)
                        {
                            j++;
                            if (j != t) goto z1;
                        }
                        else
                        {
                            E += A[i, j] * x[j];
                        }
                    }
                    x[i] = (b[i] - E) / A[i, i];
                }
                num++;
            }

            NEVA = Nev(this.A, x, this.b, t);
        }

        public static readonly double EPSS = 0;
        /// <summary>
        /// Метод наискорейшего спуска со свободным вектором в качестве первого приближения
        /// </summary>
        /// <param name="t"></param>
        public void Speedy(int t, double eps = 0.000001, int maxit = 2000)
        {
            for (int i = 0; i < t; i++)
            {
                x[i] = b[i]; //первое приближение - свободный вектор
            }
            double E = Nev(A, x, b, t);
            double EPSJ = eps;
            int num = 0; //переменные, связанные с количеством итераций
            int maxI = maxit;
            double[] Ax = new double[t];
            double[] r = new double[t];
            double[] Ar = new double[t];
            double[] s = new double[t];
            double[] sum = new double[t];
            while ((Nev(A, x, b, t) > EPSJ) && (num <= maxI)) //пока 1) невязка большая, 2) шагов ещё не много, 3) невязка убывает
            {
                Func_in_matrix.Matrix_power(ref Ax, A, x, t); //произведение матрицы на вектор Ax=A*x
                Func_in_matrix.Vector_difference(ref r, Ax, b, t); //разность двух векторов r=Ax-b
                Func_in_matrix.Matrix_power(ref Ar, A, r, t); //Ar=A*r
                double tau = Func_in_matrix.Scalar_power(r, r, t) / Func_in_matrix.Scalar_power(Ar, r, t); //скалярное произведение двух векторов tau=(r,r)/(Ar,r)
                Func_in_matrix.Vector_on_scalar(ref s, -tau, r, t); //умножение вектора на скаляр s=-tau*r
                Func_in_matrix.Vector_sum(ref sum, s, x, t); //сумма векторов sum=x+s=x-tau*r...

                E = Nev(A, x, b, t); //фиксируем невязку
                Func_in_matrix.Vector_assingment(ref x, sum, t); //присваивание одному вектору другого
                num++;
            }
            NEVA = Nev(A, x, b, t);
        }
        /// <summary>
        /// Метод наискорейшего спуска без начального присвоения (используется вектор х, изначально нулевой либо изменёный в другом методе)
        /// </summary>
        /// <param name="t"></param>
        private void SpeedyNext(int t)
        {
            double E = Nev(A, x, b, t);
            double EPSJ = EPSS;
            int num = 0; //переменные, связанные с количеством итераций
            int maxI = 1000;
            double[] Ax = new double[t];
            double[] r = new double[t];
            double[] Ar = new double[t];
            double[] s = new double[t];
            double[] sum = new double[t];

            SqMatrix a = new SqMatrix(this.A);
            //if(a.CubeNorm<1)
            while ((Nev(A, x, b, t) > EPSJ) && (num <= maxI) && (Nev(A, x, b, t) <= E)) //пока 1) невязка большая, 2) шагов ещё не много, 3) невязка убывает
            {
                //$"{Nev(A, x, b, t)} > {EPSJ} && {num} <= {maxI} && {Nev(A, x, b, t)}<={E} ".Show();
                E = Nev(A, x, b, t);
                Func_in_matrix.Matrix_power(ref Ax, A, x, t); //произведение матрицы на вектор Ax=A*x
                Func_in_matrix.Vector_difference(ref r, Ax, b, t); //разность двух векторов r=Ax-b
                Func_in_matrix.Matrix_power(ref Ar, A, r, t); //Ar=A*r
                double tau = Func_in_matrix.Scalar_power(r, r, t) / Func_in_matrix.Scalar_power(Ar, r, t); //скалярное произведение двух векторов tau=(r,r)/(Ar,r)
                Func_in_matrix.Vector_on_scalar(ref s, -tau, r, t); //умножение вектора на скаляр s=-tau*r
                Func_in_matrix.Vector_sum(ref sum, s, x, t); //сумма векторов sum=x+s=x-tau*r...
                                                             //E = Nev(A, x, b, t); //фиксируем невязку
                Func_in_matrix.Vector_assingment(ref x, sum, t); //присваивание одному вектору другого
                num++;
            }
        }

        /// <summary>
        /// Покоординатная минимизация коэффициентов
        /// </summary>
        /// <param name="t"></param>
        public void Minimize_coef(int t, int count = 50) //
        {
            Console.Write("Точность аппроксимации при числе функций ");
            Console.Write(t);
            Console.Write(" =");
            Console.WriteLine();
            Console.Write("до использования покоординатной минимизации:\t");
            Console.Write(Error(t) + $" ({this.Nev(t)}) - в скобках указана невязка системы");
            Console.WriteLine();
            double sum = 0;
            int r = count;
            for (int k = 1; k <= r; k++)
            {
                for (int i = t - 1; i >= 0; i--)
                {
                    for (int j = 0; j < t; j++)
                    {
                        if (j != i)
                        {
                            sum += x[j] * A[i, j];
                        }
                    }
                    x[i] = (b[i] - sum) / A[i, i];
                    sum = 0;
                }
                Console.Write("после использования покоординатной минимизации ");
                Console.Write(k);
                Console.Write(" раз:\t");
                Console.Write(Error(t) + $" ({this.Nev(t)})");
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Метод Гаусса с последующим улучшением решения методом наискорейшего спуска
        /// </summary>
        /// <param name="t"></param>
        public void GaussSpeedy(int t)
        {
            //Gauss(t);
            GaussSelection(t);
            Vectors v = new Vectors(this.x);

            if (A[0, 0] > 1)
            {
                double tmp = Nev(t);
                SpeedyNext(t);
                if (tmp < Nev(t))
                    for (int i = 0; i < t; i++)
                        x[i] = v[i];
            }

            try
            {
                SLAU tmpp = new SLAU(this, t);
                tmpp.Correction();

                for (int i = 0; i < t; i++)
                    this.x[i] = tmpp.x[i];
            }
            finally
            {
                NEVA = Nev(A, x, b, t);
            }
        }
        /// <summary>
        /// Метод Гаусса + метод наискорейшего спуска + метод поокрдинатной минимизации
        /// </summary>
        /// <param name="t"></param>
        public void GaussSpeedyMinimize(int t) //гибридный с использованием покоординатной минимизации
        {
            //Gauss(t);
            //SpeedyNext(t);
            GaussSpeedy(t);
            Minimize_coef(t);
            NEVA = Nev(A, x, b, t);
        }

        //public static double VALUE_FOR_ULTRA = 10;
        //public void UltraHybrid(int t) //гибридный с координатной минимизацией по последней координате
        //{
        //    double[] c = new double[t];
        //    for (int i = 0; i < t - 1; i++)
        //    {
        //        c[i] = x[i];
        //    }

        //    double sum = 0;
        //    GaussSpeedy(t);

        //    if ((VALUE_FOR_ULTRA < Error(t)) && (t >= 1)) //если погрешность выросла - исправить это
        //    {
        //        for (int i = 0; i < t - 1; i++)
        //        {
        //            x[i] = c[i];
        //        }
        //        x[t - 1] = 0;
        //        if (t != 0)
        //        {
        //            for (int j = 0; j < t - 1; j++)
        //            {
        //                sum += x[j] * A[t - 1,j];
        //            }
        //            x[t - 1] = (b[t - 1] - sum) / A[t - 1,t - 1];
        //            sum = 0;

        //            double tmp1 = Error(t);

        //            if (VALUE_FOR_ULTRA < tmp1) //погрешность опять выросла
        //            {
        //                for (int i = 0; i < t - 1; i++)
        //                {
        //                    x[i] = c[i];
        //                }
        //                x[t - 1] = 0;
        //            }
        //        }
        //    }
        //    VALUE_FOR_ULTRA = Error(t);
        //    NEVA = Nev(A, x, b, t);
        //}

        //перечисление методов
        /// <summary>
        /// Перечисление методов решения системы
        /// </summary>
        public enum Method : byte
        {
            Gauss,
            Holets,
            Jak,
            Speedy,
            GaussSpeedy,
            GaussSpeedyMinimize,
            UltraHybrid
        }

        /// <summary>
        /// Вывести систему на консоль
        /// </summary>
        public void Show()
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write("||");
                for (int j = 0; j < size - 1; j++) Console.Write("{0} \t", A[i, j]);
                Console.WriteLine("{0}\t|| \t||{1}|| \t{2}", A[i, size - 1], x[i], b[i]);
            }
        }
        /// <summary>
        /// Вывести систему c рациональными числами на консоль
        /// </summary>
        public void ShowRational()
        {
            for (int i = 0; i < size; i++)
            {
                Console.Write("||");
                for (int j = 0; j < size - 1; j++) Console.Write("{0} \t", Number.Rational.ToRational(A[i, j]));
                Console.WriteLine("{0}\t|| \t||{1}|| \t{2}", Number.Rational.ToRational(A[i, size - 1]), Number.Rational.ToRational(x[i]), Number.Rational.ToRational(b[i]));
            }

        }

        /// <summary>
        /// Вывести систему, её решение и невязки от разных методов
        /// </summary>
        public void ShowErrors()
        {
            Console.WriteLine("Система, решённая универсальным методом Гаусса:");
            this.GaussSelection();
            this.Show();
            Console.WriteLine("");
            Console.WriteLine("----------Невязки при использовании разных методов:");
            Console.WriteLine("Погрешность универсального метода Гаусса = {0}", this.Discrep);
            this.Gauss();
            Console.WriteLine("Погрешность экономного метода Гаусса = {0}", this.Discrep);

            SqMatrix M = new SqMatrix(this.A);
            if (M.IsPositCertain() && M.IsSymmetric())
            {
                this.Holets(this.size);
                Console.WriteLine("Погрешность метода Холетского = {0}", this.Discrep);
            }
            if (M.IsTreeDiag())
            {
                this.ProRace();
                Console.WriteLine("Погрешность метода прогонки = {0}", this.Discrep);
            }

            SqMatrix D = new SqMatrix(this.size), T = new SqMatrix(this.A);
            for (int i = 0; i < this.size; i++) D[i, i] = this.A[i, i];
            if (D.Det != 0)
            {


                SqMatrix B = SqMatrix.I(this.size) - D.Reverse * (T);

                if (B.Frobenius < 1)
                {
                    this.Jak(this.size);
                    Console.WriteLine("Погрешность метода Якоби = {0}", this.Discrep);
                }
            }

            this.Speedy(this.size);
            Console.WriteLine("Погрешность метода наискорейшего спуска = {0}", this.Discrep);
            this.GaussSpeedy(this.size);
            Console.WriteLine("Погрешность метода Гаусса с улучшением наискорейшим спуском (гибридный метод) = {0}", this.Discrep);
            this.Minimize_coef(this.size);
            if (M.IsPositCertain() && M.IsSymmetric())
            {
                Console.WriteLine("Погрешность метода покоординатной минимизации (10-минимизация) = {0}", this.Discrep);
                this.GaussSpeedyMinimize(this.size);
                Console.WriteLine("Погрешность метода Гаусса с улучшением наискорейшим спуском и покоординатной минимизацией (гипергибридный метод) = {0}", this.Discrep);
            }
        }


        public double[] ErrorsMas, ErrorMasP;
        public Func<double,double> f = null;
        public SequenceFunc p = null;
        public double begin = 0, end = 0;
        //public double Error(int k) //частичная погрешность
        //{
        //    double p = FuncMethods.RealFuncMethods.NormScalar(f, begin, end);
        //    double sum = 0;

        //    double[] Ax = new double[this.Size];
        //    Func_in_matrix.Matrix_power(ref Ax, A, x, k);
        //    for (int i = 0; i < k; i++)
        //    {
        //        sum += x[i] * Ax[i];
        //    }
        //    double EPS = Math.Abs(p - sum);
        //    return Math.Sqrt(EPS);
        //}
        public double Error(int k) //частичная погрешность
        {
            Func<double,double> po = (double xx) =>
              {
                  double sum = 0;
                  for (int i = 0; i < k; i++)

                      sum += x[i] * p(xx, i);
                  return sum;
              };
            return FuncMethods.RealFuncMethods.NormDistance(f, po, begin, end);
        }

        private double ultraval = -1;
        public double VALUE_FOR_ULTRA { get { if (ultraval == -1) ultraval = FuncMethods.RealFuncMethods.NormScalar(f, begin, end); return ultraval; } set { ultraval = value; } }
        public void Make(int k, double[,] AMAS)
        {
            Make(k);
            for (int i = 0; i < this.Size; i++)
                for (int j = 0; j < this.Size; j++)
                    this.A[i, j] = AMAS[i, j];
        }

        /// <summary>
        /// Число, которое показывает, какая часть системы уже была решена ультра-гибридом
        /// </summary>
        public int UltraCount = 0;
        public void UltraHybrid(int t, SequenceFuncKind kind = SequenceFuncKind.Other)
        {
            //ErrorsMas = new double[this.size];
            //ErrorMasP = new double[this.size];
            //UltraCount.Show();
            if (UltraCount == 0)//если вообще не решалось
            {
                //"Вошло".Show();
                x[0] = b[0] / A[0, 0];  //x[0].Show();
                                        //if (kind == SequenceFuncKind.Orthonormal) x[0] *= end - begin;
                VALUE_FOR_ULTRA = Error(1);
                ErrorsMas[0] = VALUE_FOR_ULTRA;
                Func<double,double> ff = (double xx) =>
                {
                    double sum = this.x[0] * p(xx, 0);
                    double s = sum - f(xx);
                    return s * s;
                };
                ErrorMasP[0] = FuncMethods.RealFuncMethods.NormScalar(ff, begin, end);

                UltraCount++;
                for (int i = UltraCount + 1; i <= t; i++)
                    UltraHybridLast(i, kind);
                UltraCount = t;
            }
            else if (UltraCount == t - 1)//если надо решить только по последней координате
            {
                UltraHybridLast(t, kind);
                UltraCount++;
            }
            else
            {
                for (int i = UltraCount + 1; i <= t; i++)
                    UltraHybridLast(i, kind);
                UltraCount = t;
            }
        }
        /// <summary>
        /// Ультра-гибридный метод суперского решения по последней координате
        /// </summary>
        /// <param name="t"></param>
        public void UltraHybridLast(int t, SequenceFuncKind kind = SequenceFuncKind.Other) //гибридный с координатной минимизацией по последней координате
        {
            double[] c = new double[t];
            for (int i = 0; i < t - 1; i++)
                c[i] = x[i];
            Vectors mk1 = new Vectors(c), mk2 = new Vectors(c);

            double sum = 0;
            switch (kind)
            {
                case SequenceFuncKind.Other:
                    GaussSpeedy(t);
                    break;
                //case SequenceFuncKind.Orthonormal:
                //    for (int i = 0; i < t; i++)
                //        x[i] = b[i]*(end-begin);
                //    break;
                default:
                    for (int i = 0; i < t; i++)
                        x[i] = b[i] / A[i, i];
                    break;
            }


            double tmp = Error(t);
            if (VALUE_FOR_ULTRA < tmp) //если погрешность выросла - исправить это, потому что новое решение не годится
            {
                $"{VALUE_FOR_ULTRA} < {tmp} при t = {t} (до покоординатной минимизации результата СПИДГАУССА)".Show();
                //покоординатная минимизация результата СПИДГАУССА
                for (int k = 0; k <= t - 1; k++)
                {
                    for (int j = 0; j < k; j++)
                        sum += x[j] * A[k, j];
                    for (int j = k + 1; j < t - 1; j++)
                        sum += x[j] * A[k, j];
                    x[k] = (b[k] - sum) / A[k, k];
                    sum = 0;
                }

                tmp = Error(t);
                if (VALUE_FOR_ULTRA < tmp)
                {
                    $"{VALUE_FOR_ULTRA} < {tmp} при t = {t} (до полно покоординатной минимизации вектора с1 с2 ... 0)".Show();
                    for (int i = 0; i < t; i++)//исправили, теперь пробуем новый метод                 
                        x[i] = c[i];

                    //покоординатная минимизация
                    for (int k = 0; k <= t - 1; k++)
                    {
                        for (int j = 0; j < k; j++)
                            sum += x[j] * A[k, j];
                        for (int j = k + 1; j < t - 1; j++)
                            sum += x[j] * A[k, j];
                        x[k] = (b[k] - sum) / A[k, k];
                        sum = 0;
                    }


                    double tmp1 = Error(t);
                    if (VALUE_FOR_ULTRA < tmp1) //погрешность опять выросла - тогда просто оставляем 0 на конце
                    {
                        $"{VALUE_FOR_ULTRA} < {tmp1} при t = {t} (до полной покоординатной минимизации на конце)".Show();
                        for (int i = 0; i < t; i++)//исправили, теперь пробуем новый метод                 
                            x[i] = c[i];
                        for (int j = 0; j < t - 1; j++)
                        {
                            sum += x[j] * A[t - 1, j];
                        }
                        x[t - 1] = (b[t - 1] - sum) / A[t - 1, t - 1];
                        sum = 0;
                        tmp = Error(t);
                        if (VALUE_FOR_ULTRA < tmp)
                        {
                            for (int i = 0; i < t - 1; i++)
                            {
                                x[i] = c[i];
                            }
                            x[t - 1] = 0;
                        }
                        else
                        {
                            $"Погрешность уменьшена МИНИМАКОЙ НА КОНЦЕ на {(VALUE_FOR_ULTRA - tmp) / VALUE_FOR_ULTRA * 100} %".Show();
                            VALUE_FOR_ULTRA = tmp;
                            Vectors v = new Vectors(this.x);
                            $"\t Новая аппроксимация {VALUE_FOR_ULTRA}; текущий вектор решения: {v.ToString()}".Show();
                        }

                    }
                    else
                    {
                        $"Погрешность уменьшена ПОЛНОЙ МИНИМАКОЙ на {(VALUE_FOR_ULTRA - tmp1) / VALUE_FOR_ULTRA * 100} %".Show();
                        VALUE_FOR_ULTRA = tmp1;
                        Vectors v = new Vectors(this.x);
                        $"\t Новая аппроксимация {VALUE_FOR_ULTRA}; текущий вектор решения: {v.ToString()}".Show();
                    }
                }
                else
                {
                    $"Погрешность уменьшена МИНИМАКОЙ СПИДГАУССА на {(VALUE_FOR_ULTRA - tmp) / VALUE_FOR_ULTRA * 100} %".Show();
                    VALUE_FOR_ULTRA = tmp;
                    Vectors v = new Vectors(this.x);
                    $"\t Новая аппроксимация {VALUE_FOR_ULTRA}; текущий вектор решения: {v.ToString()}".Show();
                }
            }
            else
            {
                $"Погрешность уменьшена СПИДГАУССОМ на {(VALUE_FOR_ULTRA - tmp) / VALUE_FOR_ULTRA * 100} %".Show();
                VALUE_FOR_ULTRA = tmp;
                Vectors v = new Vectors(this.x);
                $"\t Новая аппроксимация {VALUE_FOR_ULTRA}; текущий вектор решения: {v.ToString()}".Show();
            }

            ErrorsMas[t - 1] = VALUE_FOR_ULTRA;
            Func<double,double> ff = (double x) =>
            {
                sum = 0;

                for (int ii = 1; ii <= t; ii++)
                {
                    sum += this.x[ii - 1] * p(x, ii - 1);
                }
                double s = sum - f(x);
                return s * s;
            };
            ErrorMasP[t - 1] = FuncMethods.RealFuncMethods.NormScalar(ff, begin, end);
            //UltraCount = t;
            NEVA = Nev(A, x, b, t); "".Show();
        }

        /// <summary>
        /// Уточнение решения СЛАУ по Уилкинсону
        /// </summary>
        /// <param name="eps">Норма невязки, до которой нужно уточнять</param>
        /// <param name="maxcount">Максимальное число итераций</param>
        public void Correction(double eps = 0.0001, int maxcount = 10000)
        {
            SqMatrix A = new SqMatrix(this.A);
            SqMatrix AR = SqMatrix.ReverseCorrect(A, A.Reverse);
            Vectors b = new Vectors(this.b);

            double nev = Nevaska;
            int s = 0;
            while (Nevaska > eps && s <= maxcount && Nevaska < nev)
            {
                nev = Nevaska;
                Vectors xs = new Vectors(this.x);
                Vectors d = AR * (b - A * xs);
                xs += d;
                this.x = Vectors.ToDoubleMas(xs);
            }
        }
    }
}

