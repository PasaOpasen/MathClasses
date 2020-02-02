using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static МатКлассы.Number;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Обычные векторы
    /// </summary>
    public sealed class Vectors : Idup<Vectors>
    {
        /// <summary>
        /// Массив, с которым отождествляется вектор
        /// </summary>
        internal double[] vector;

        #region Свойства

        /// <summary>
        /// Размерность вектора
        /// </summary>
        public int Deg => vector.Length;
        /// <summary>
        /// Массив, соответствующий вектору
        /// </summary>
        public double[] DoubleMas => vector;

        /// <summary>
        /// Евклидова норма вектора
        /// </summary>
        public double EuqlidNorm
        {
            get
            {
                double s = 0;
                for (int i = 0; i < this.Deg; i++)
                    s += vector[i] * vector[i];
                return Math.Sqrt(s) / this.Deg;
            }
        }

        /// <summary>
        /// Возвращает нормированный вектор
        /// </summary>
        public Vectors Normalizing => this.dup / this.EuqlidNorm;

        /// <summary>
        /// Использование вектора как массива
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double this[int i]
        {
            get { return vector[i]; }
            set { vector[i] = value; }
        }
        /// <summary>
        /// Среднее арифметическое значений в векторе
        /// </summary>
        public double ArithmeticAv
        {
            get
            {
                int n = this.Deg;
                double sun = 0;
                for (int i = 0; i < n; i++) sun += this[i];
                return sun / n;
            }
        }
        /// <summary>
        /// Среднее арифметическое
        /// </summary>
        public double Center => ArithmeticAv;
        /// <summary>
        /// Размах значений в векторе
        /// </summary>
        public double Range => Math.Abs(this.Max - this.Min);
        /// <summary>
        /// Сумма элементов вектора
        /// </summary>
        public double Sum => this.vector.Sum();
        /// <summary>
        /// Среднее отклонение значений в векторе
        /// </summary>
        public double Average
        {
            get
            {
                double sun = 0;
                for (int i = 0; i < this.Deg; i++) sun += this.Av(i);
                return sun / this.Deg;
            }
        }
        /// <summary>
        /// Относительная погрешность значений в векторе
        /// </summary>
        public double RelAc
        {
            get { if (this.ArithmeticAv != 0) return this.Average / this.ArithmeticAv; else throw new DivideByZeroException("Деление на 0!"); }
        }
        /// <summary>
        /// Вектор отклонений от среднего значения в векторе
        /// </summary>
        public Vectors RelAcVec
        {
            get
            {
                Vectors v = new Vectors(this.Deg);
                for (int i = 0; i < v.Deg; i++) v[i] = this.Av(i);
                return v;
            }
        }
        /// <summary>
        /// Вектор квадратов отклонений от среднего значения в векторе
        /// </summary>
        public Vectors RelAcSqr
        {
            get
            {
                Vectors v = new Vectors(this.Deg);
                for (int i = 0; i < v.Deg; i++) v[i] = this.Av(i) * this.Av(i);
                return v;
            }
        }
        /// <summary>
        /// Максимальное значение
        /// </summary>
        public double Max
        {
            get
            {
                double t = vector[0];
                for (int i = 1; i < this.Deg; i++)
                    if (vector[i] > t) t = vector[i];
                return t;
            }
        }
        /// <summary>
        /// Минимальное значение
        /// </summary>
        public double Min
        {
            get
            {
                double t = vector[0];
                for (int i = 1; i < this.Deg; i++)
                    if (vector[i] < t) t = vector[i];
                return t;
            }
        }
        /// <summary>
        /// Отсортированный вектор
        /// </summary>
        public Vectors Sort
        {
            get
            {
                Vectors e = new Vectors(this);
                Array.Sort(e.vector);
                return e;
            }
        }
        /// <summary>
        /// Вектор модулей
        /// </summary>
        public Vectors AbsVector
        {
            get
            {
                Vectors r = new Vectors(this);
                for (int i = 0; i < r.Deg; i++)
                    r[i] = r[i].Abs();
                return r;
            }
        }
        /// <summary>
        /// Максимальный элемент по модулю
        /// </summary>
        public double MaxAbs => AbsVector.Max;
        /// <summary>
        /// Минимальный элемент по модулю
        /// </summary>
        public double MinAbs => AbsVector.Min;
        /// <summary>
        /// Усреднённый вектор
        /// </summary>
        public Vectors ToAver
        {
            get
            {
                Vectors res = new Vectors(this);
                double a = res.ArithmeticAv;
                for (int i = 0; i < res.Deg; i++)
                    res[i] -= a;
                return res;
            }
        }
        /// <summary>
        /// Вектор, делённый на своё среднее
        /// </summary>
        public Vectors ToAverDel
        {
            get
            {
                Vectors res = new Vectors(this);
                double a = res.ArithmeticAv;
                for (int i = 0; i < res.Deg; i++)
                    res[i] /= a;
                return res;
            }
        }

        /// <summary>
        /// Последний элемент вектора
        /// </summary>
        public double LastElement => vector.Last();
        /// <summary>
        /// Дубликат вектора
        /// </summary>
        public Vectors dup => new Vectors(this);           
        #endregion

        #region Конструкторы
        /// <summary>
        /// Вектор (0)
        /// </summary>
        public Vectors()
        {
            this.vector = Array.Empty<double>();
        }
        /// <summary>
        /// Нулевой вектор
        /// </summary>
        /// <param name="n">Размерность вектора</param>
        public Vectors(int n)
        {
   this.vector = new double[n];
        }
        /// <summary>
        /// Вектор, заполненный одинаковыми числами
        /// </summary>
        /// <param name="n">Размерность вектора</param>
        /// <param name="c">Конпонент вектора</param>
        public Vectors(int n, double c) : this(n)
        {
            for (int i = 0; i < n; i++) this[i] = c;
        }
        /// <summary>
        /// Считать вектор из файла
        /// </summary>
        /// <param name="fs"></param>
        public Vectors(StreamReader fs)
        {
            string s = fs.ReadLine();
            string[] st = s.Split(' ');
            this.vector = new double[st.Length];
            for (int i = 0; i < this.Deg; i++) this.vector[i] = Convert.ToDouble(st[i]);
            fs.Close();
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="V"></param>
        public Vectors(Vectors v)
        {
            this.vector = new double[v.Deg];
            for (int i = 0; i < v.Deg; i++) this[i] = v[i];
        }
        /// <summary>
        /// Задать вектор перечислением координат или массивом
        /// </summary>
        /// <param name="x"></param>
        public Vectors(params double[] x)
        {
            vector = new double[x.Length];
            for (int i = 0; i < x.Length; i++) this.vector[i] = x[i];
        }
        /// <summary>
        /// Задать вектор массивом целых чисел
        /// </summary>
        /// <param name="c"></param>
        public Vectors(int[] c)
        {
            vector = new double[c.Length];
            for (int i = 0; i < c.Length; i++) this.vector[i] = c[i];
        }
        /// <summary>
        /// Задать вектор по вектору-столбцу
        /// </summary>
        /// <param name="M"></param>
        public Vectors(Matrix M)
        {
            this.vector = new double[M.n];
            for (int i = 0; i < M.n; i++) this.vector[i] = M[i, 0];
        }
        /// <summary>
        /// Вектор как кусок кругого вектора
        /// </summary>
        /// <param name="v">Образец</param>
        /// <param name="a">Коэффициент начала из образца</param>
        /// <param name="b">Коэффициент конца из образца</param>
        public Vectors(Vectors v, int a, int b)
        {
            int n = b - a + 1;
            this.vector = new double[n];
            for (int i = 0; i < n; i++) this[i] = v[a + i];
        }
        /// <summary>
        /// Метод, обратный ToString
        /// </summary>
        /// <param name="s"></param>
        public Vectors(string s)
        {
            string[] st = s.Replace('(', ' ').Replace(')', ' ').Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
           int n = st.Length;
            vector = new double[n];
            for (int i = 0; i < n; i++)
                vector[i] = st[i].ToDouble();
        }
        #endregion

        #region Преобразования
        public static implicit operator Vectors(double[] x) => new Vectors(x);
        public static explicit operator double[] (Vectors v) => ToDoubleMas(v);
        public static explicit operator Complex[] (Vectors v)
        {
            Complex[] res = new Complex[v.Deg];
            for (int i = 0; i < res.Length; i++)
                res[i] = new Complex(v[i]);
            return res;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Находятся ли все значения вектора в указанно промежутке
        /// </summary>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IsIn(double beg,double end)
        {
            for (int i = 0; i < Deg; i++)
                if (vector[i] < beg || vector[i] > end)
                    return false;
            return true;
        }
        /// <summary>
        /// Отклонение элемента от среднего значения
        /// </summary>
        /// <param name="i">Номер элемента</param>
        /// <returns></returns>
        public double Av(int i) { return Math.Abs(this[i] - this.ArithmeticAv); }

        /// <summary>
        /// Вывести истинное значение величины на консоль
        /// </summary>
        public void TrueValShow() { Console.WriteLine(this.ArithmeticAv + " +/- " + this.Average); }
        /// <summary>
        /// Показать всю информацию о векторе как о реализации величины
        /// </summary>
        public void TrueValShowFull()
        {
            Console.WriteLine("Исходный вектор:");
            this.Show();
            Console.WriteLine("Среднее арифметическое значений в векторе равно " + this.ArithmeticAv);
            Console.WriteLine("Среднее отклонение значений в векторе равно " + this.Average);
            Console.WriteLine("Относительная погрешность значений в векторе равна " + this.RelAc);

            Console.WriteLine("Вектор отклонений от среднего значения:"); this.RelAcVec.Show();
            Console.WriteLine("Вектор квадратов отклонений от среднего значения:"); this.RelAcSqr.Show();
            Console.WriteLine("Истинное значение величины:"); this.TrueValShow();

        }

        /// <summary>
        /// Перевести вектор в массив чисел
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(Vectors x)
        {
            double[] r = new double[x.Deg];
            for (int i = 0; i < x.Deg; i++) r[i] = x[i];
            return r;
        }
        /// <summary>
        /// Содержится ли указанное число в векторе
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool Contain(double x)
        {
            for (int i = 0; i < this.Deg; i++)
                if (this[i] == x) return true;
            return false;
        }
        /// <summary>
        /// Слияние нескольких векторов со состыковкой крайних вершин (конец предыдущего вектора должен совпадать с началом последующего)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vectors Merge(params Vectors[] v)
        {
            int l = 0;
            for (int i = 0; i < v.Length; i++) l += v[i].Deg;
            l -= v.Length - 1;
            Vectors r = new Vectors(l);
            r[0] = v[0].vector[0];
            int k = 0;
            for (int i = 0; i < v.Length; i++)
            {
                for (int j = 1; j < v[i].Deg; j++)
                    r[k + j] = v[i].vector[j];
                k += v[i].Deg - 1;
            }

            return r;
        }
        /// <summary>
        /// Описывает ли вектор простой цикл (первая и последняя вершины должны повторяться, но больше в нём не должно содержаться повторяющихся вершин)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsSimpleCycle(Vectors v)
        {
            if (v[0] != v[v.Deg - 1]) return false;
            double[] x = new double[v.Deg - 1];
            for (int i = 0; i < x.Length; i++) x[i] = v[i + 1];
            Array.Sort(x);
            for (int i = 0; i < x.Length - 1; i++)
                if (x[i] == x[i + 1]) return false;

            return true;
        }

        /// <summary>
        /// Задать коэффициенты через консоль
        /// </summary>
        public  void CreateMatrix()
        {
            for (int i = 0; i < this.Deg; i++)
            {
                Console.Write("Введите элемент [" + i.ToString() + "]" + "\t");
                vector[i] = Convert.ToDouble(Console.ReadLine());
            }
        }
        /// <summary>
        /// Вывести вектор на консоль
        /// </summary>
        public  void PrintMatrix()
        {
            for (int i = 0; i < this.Deg; i++)
            {
                Console.Write(vector[i].ToString() + " \t");
            }
            Console.WriteLine();
        }
        /// <summary>
        /// Нулевой ли вектор?
        /// </summary>
        /// <returns></returns>
        public  bool Nulle()
        {
            for (int i = 0; i < this.Deg; i++)           
               if (vector[i] != 0)
                    return false;           
            return true;
        }
        /// <summary>
        /// Перевод вектора в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //string s = "( ";
            //for (int i = 0; i < this.Deg; i++) s += String.Format("\t{0} ", this[i].ToRString());
            //s += "\t)";
            //return s;
            System.Text.StringBuilder s = new System.Text.StringBuilder("( ");
            for (int i = 0; i < this.Deg; i++) s.Append($"\t{vector[i].ToRString()} ");
            s.Append("\t)");
            return s.ToString();
        }

        /// <summary>
        /// Перевод вектора (все координаты которого увеличены на 1) в строку
        /// </summary>
        /// <returns></returns>
        public string ToStringPlusOne()
        {
            string s = "( ";
            for (int i = 0; i < this.Deg; i++) s += String.Format("\t{0} ", this[i] + 1);
            s += "\t)";
            return s;
        }
        /// <summary>
        /// Перевод вектора в строку с рациональными числами
        /// </summary>
        /// <returns></returns>
        public string ToRationalString()
        {
            string s = "( ";
            for (int i = 0; i < this.Deg; i++) s += String.Format("{0} ", Number.Rational.ToRational(this[i]));
            s += ")";
            return s;
        }
        /// <summary>
        /// Перевод вектора в строку с рациональными числами с табуляцией
        /// </summary>
        /// <returns></returns>
        public string ToRationalStringTab()
        {
            string s = "( ";
            for (int i = 0; i < this.Deg; i++) s += String.Format("\t{0} ", Number.Rational.ToRational(this[i]));
            s += "\t)";
            return s;
        }
        /// <summary>
        /// Вывести вектор на консоль
        /// </summary>
        public void Show() { Console.WriteLine(this.ToString()); }
        /// <summary>
        /// Вывести вектор (все координаты которого увеличены на 1) на консоль
        /// </summary>
        public void ShowPlusOne() { Console.WriteLine(this.ToStringPlusOne()); }

        /// <summary>
        /// Вывести вектор в рациональном виде на консоль
        /// </summary>
        public void ShowRational() { Console.WriteLine(this.ToRationalString()); }
        private void ShowRational(StreamWriter sf) { sf.WriteLine(this.ToRationalString()); }
        /// <summary>
        /// Вывести вектор в рациональном виде на консоль с табуляцией
        /// </summary>
        public void ShowRationalTab() { Console.WriteLine(this.ToRationalStringTab()); }
        private void ShowRationalTab(StreamWriter sf) { sf.WriteLine(this.ToRationalStringTab()); }
        /// <summary>
        /// Вывести массив векторов на консоль
        /// </summary>
        /// <param name="lines"></param>
        public static void Show(Vectors[] lines)
        {
            Console.Write(" \t\tb "); for (int i = 1; i < lines[0].Deg; i++) Console.Write("\tx[{0}] ", i);
            Console.WriteLine();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                Console.Write("x[{0}]\t", Vectors.NumberColumn(lines, i));
                lines[i].Show();
            }
            Console.Write("L\t"); lines[lines.Length - 1].Show();
        }
        /// <summary>
        /// Вывести массив векторов в файл
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sf"></param>
        private static void Show(Vectors[] lines, StreamWriter sf)
        {
            sf.Write(" \t\tb "); for (int i = 1; i < lines[0].Deg; i++) sf.Write("\tx[{0}] ", i);
            sf.WriteLine();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                sf.Write("x[{0}]\t", Vectors.NumberColumn(lines, i));
                lines[i].Show();
            }
            sf.Write("L\t"); lines[lines.Length - 1].Show();
        }
        /// <summary>
        /// Вывести информацию об векторе в файл
        /// </summary>
        /// <param name="v"></param>
        /// <param name="fs"></param>
        public static void ShowInfo(Vectors v, StreamWriter fs)
        {
            fs.Write("Исходный вектор: "); fs.WriteLine(v.ToString());
            fs.Write("Вектор отклонений от среднего значения: "); fs.WriteLine(v.RelAcVec.ToString());
            fs.Write("Вектор квадратов отклонений: "); fs.WriteLine(v.RelAcSqr.ToString());
            fs.Write("Истинное значение элементов в векторе: "); fs.WriteLine(v.ArithmeticAv + " +/- " + v.Average);
            fs.Close();
        }
        private static int NumberColumn(Vectors[] l, int k)
        {
            for (int j = 1; j < l[0].Deg; j++)
                if (l[k].vector[j] == 1)
                {
                    int t = 0;
                    for (int i = 0; i < l.Length; i++)
                        if (l[i].vector[j] == 0) t++;
                    if (t == l.Length - 1) return j;

                }
            return 0;
        }

        /// <summary>
        /// Вывести массив векторов c рациональными коэффициентами на консоль
        /// </summary>
        /// <param name="lines"></param>
        public static void ShowRational(Vectors[] lines)
        {
            //for (int i = 0; i < lines.Length; i++) lines[i].ShowRational();
            Console.Write(" \t\tb "); for (int i = 1; i < lines[0].Deg; i++) Console.Write("\tx[{0}] ", i);
            Console.WriteLine();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                Console.Write("x[{0}]\t", Vectors.NumberColumn(lines, i));
                lines[i].ShowRationalTab();
            }
            Console.Write("L\t"); lines[lines.Length - 1].ShowRationalTab();
        }

        /// <summary>
        /// Нахождение максимума функционала методами симплекс-таблицы, считанной из файла
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="fs">Файл с матрицей (симплекс-таблицей) размера (x.Lenth+1)X(xbase.Lenth+1) (образец)</param>
        /// <returns>Наибольшее значение функционала, который задан последней строкой таблицы</returns>
        /// <remarks>Не предусматривается возможность вырожденного решения</remarks>
        public static double SimpleSimplex(ref Vectors result, StreamReader fs)
        {
            //чтение данных
            int k = 0;//число единиц
            for (int i = 0; i < result.Deg; i++) if (result[i] != 0) k++;
            Vectors[] lines = new Vectors[k + 1];
            for (int i = 0; i < k + 1; i++)
            {
                lines[i] = new Vectors(result.Deg + 1);
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                for (int j = 0; j < lines[i].Deg; j++) lines[i].vector[j] = Convert.ToDouble(st[j]);
            }
            fs.Close();
            Console.WriteLine("Исходная сиплекс-таблица:");
            Vectors.Show(lines);
            Console.WriteLine("Вектор, указывающий на базис:");
            result.ShowRational(); Console.WriteLine(); Console.WriteLine();

            //Решение
            int it_num = 1;
            while (Vectors.ExistColumn(lines) != 0)
            {
                if (Vectors.ExistInfinity(lines) != 0)
                {
                    Console.WriteLine("Функция неограничена на области допустимых решений (столбец {0} таблицы полностью отрицателен)", Vectors.ExistInfinity(lines) + 1);
                    return Double.PositiveInfinity;
                }
                int m = Vectors.ExistColumn(lines);
                Console.WriteLine("На итерации {0} разрещающим выбран элемент на пересечении строки {1} и столбца {2}", it_num, Vectors.FindPosElem(lines, m) + 1, m + 1);
                Vectors.Transform(ref lines, Vectors.FindPosElem(lines, m), m, ref result);
                Console.WriteLine("Сиплекс-таблица после итерации {0}:", it_num);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines);
                Console.WriteLine();
                it_num++;
                //result.Show(); Console.WriteLine(); Console.WriteLine();
            }
            //отредактировать вектор решения
            //int t = 0;

            //for (int j = 1; j < lines[0].Deg; j++)
            //{
            //    int zero = 0, one = 0;
            //    for (int i = 0; i < lines.Length; i++)
            //        if (lines[i].vector[j] == 0) zero++;
            //        else if (lines[i].vector[j] == 1) one++;
            //    if (zero == lines.Length - 1 && one == 1) result[j - 1] = 1;
            //    else result[j - 1] = 0;
            //}

            //for (int i = 0; i < result.Deg; i++) { if (result[i] != 0) { result[i] = lines[t].vector[0]; t++; } }

            result = new Vectors(Vectors.GetSolutionVec(lines));
            //result.Show();
            //Console.WriteLine("Вектор решения {0}", result.ToString());
            Console.WriteLine("Вектор решения {0}", result.ToRationalString());
            Console.WriteLine("Оптимальное значение функции равно {0}", lines[k].vector[0]);

            return lines[k].vector[0];
        }
        /// <summary>
        /// Нахождение максимума функционала методами симплекс таблицы, заданной массивом векторов
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="lines">Симплекс-таблица, заданная массивом векторов</param>
        /// <returns></returns>
        public static double SimpleSimplex(ref Vectors result, ref Vectors[] lines)
        {
            Console.WriteLine("Исходная сиплекс-таблица:");
            Vectors.ShowRational(lines);
            Console.WriteLine("Вектор, указывающий на базис:");
            result.ShowRational(); Console.WriteLine(); Console.WriteLine();

            //Решение
            int it_num = 1;
            while (Vectors.ExistColumn(lines) != 0)
            {
                if (Vectors.ExistInfinity(lines) != 0)
                {
                    Console.WriteLine("Функция неограничена на области допустимых решений (столбец {0} таблицы полностью отрицателен)", Vectors.ExistInfinity(lines) + 1);
                    return Double.PositiveInfinity;
                }
                int m = Vectors.ExistColumn(lines);
                int n = Vectors.FindPosElem(lines, m);
                //Console.WriteLine("+");
                Console.WriteLine("На итерации {0} разрещающим выбран элемент на пересечении строки {1} и столбца {2}", it_num, n + 1, m + 1);
                //Console.WriteLine("++");
                Vectors.Transform(ref lines, Vectors.FindPosElem(lines, m), m, ref result);
                Console.WriteLine("Сиплекс-таблица после итерации {0}:", it_num);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines);
                Console.WriteLine();
                it_num++;
                //result.Show(); Console.WriteLine(); Console.WriteLine();
            }

            //отредактировать вектор решения
            //int t = 0;

            //for (int j = 1; j < lines[0].Deg; j++)
            //{
            //    int zero = 0, one = 0;
            //    for (int i = 0; i < lines.Length; i++)
            //        if (lines[i].vector[j] == 0) zero++;
            //        else if (lines[i].vector[j] == 1) one++;
            //    if (zero == lines.Length - 1 && one == 1) result[j - 1] = 1;
            //    else result[j - 1] = 0;
            //}

            //for (int i = 0; i < result.Deg; i++) { if (result[i] != 0) { result[i] = lines[t].vector[0]; t++; } }

            result = new Vectors(Vectors.GetSolutionVec(lines));

            //result.Show();
            //Console.WriteLine("Вектор решения {0}", result.ToString());
            Console.WriteLine("Вектор решения {0}:", result.ToRationalString());
            Console.WriteLine("Оптимальное значение функции равно: {0}", lines[lines.Length - 1].vector[0]);

            return lines[lines.Length - 1].vector[0];
        }
        /// <summary>
        /// Проверяет симплекс-таблицу на наличие столбца с положительным элементом, где коэффициент при линейной форме отрицателен
        /// </summary>
        /// <param name="l"></param>
        /// <returns>Номер найденного столбца или 0, если таковой отсутствует</returns>
        /// <remarks>Выбирается стобец с наибольшей по модулю отрицательной частью в конце</remarks>
        private static int ExistColumn(Vectors[] l)
        {
            int k = 0;
            double max = 0;
            for (int j = 1; j < l[0].Deg; j++)
                if (l[l.Length - 1].vector[j] < 0)
                    if (Math.Abs(l[l.Length - 1].vector[j]) > Math.Abs(max))
                    {
                        max = l[l.Length - 1].vector[j];
                        k = j;
                    }
            //if (k >= 0)
            for (int i = 0; i < l.Length - 1; i++)
                if (l[i].vector[k] > 0)
                {
                    return k;
                }

            return k;
            //return 0;
        }
        /// <summary>
        /// Найти позицию элемента, который годится под центр в симплекс-таблице
        /// </summary>
        /// <param name="l">Симплекс-таблица</param>
        /// <param name="k">Номер столбца, где этот элемент ищется</param>
        /// <returns>Номер строки, где этот элемент находится</returns>
        private static int FindPosElem(Vectors[] l, int k)
        {
            int fix = 0;//Номер столбца, с которым сравниваем
            int t = 0;
            while (l[t].vector[k] <= 0) t++;
            double min = l[t].vector[fix] / l[t].vector[k];//может возникнуть ошибка деления на 0
            int u = t;
            for (int i = t + 1; i < l.Length - 1; i++)
            {
                if (l[i].vector[k] > 0)
                {
                    double tmp = l[i].vector[fix] / l[i].vector[k];
                    if (tmp < min)
                    {
                        min = tmp;
                        u = i;
                    }
                }
            }
            //Console.WriteLine("{0} ---- {1}", u, k);
            return u;
            //return 0;
        }
        /// <summary>
        /// Преобразует симплекс-таблицу по годному элементу в центре
        /// </summary>
        /// <param name="l">Симплекс-таблица</param>
        /// <param name="k">Номер строки, где находится элемент</param>
        /// <param name="m">Номер столбца, где находится элемент</param>
        private static void Transform(ref Vectors[] l, int k, int m, ref Vectors r)
        {
            //Vectors.Show(l);r.Show();Console.WriteLine(l[k].vector[m]);
            l[k] /= l[k].vector[m];//сделать в главной строке коэффициент 1
            for (int i = 0; i < k; i++) l[i] -= l[k] * l[i].vector[m];//отнимать эту строку от остальных
            for (int i = k + 1; i < l.Length; i++) l[i] -= l[k] * l[i].vector[m];
        }
        /// <summary>
        /// Проверяет симплекс-таблицу на наличие столбца, характеризующего отсутствие решения
        /// </summary>
        /// <param name="l"></param>
        /// <returns>Номер такого столбца и 0 при его отсутствии</returns>
        private static int ExistInfinity(Vectors[] l)
        {
            int k = 0;
            for (int j = 1; j < l[0].Deg; j++)
                if (l[l.Length - 1].vector[j] < 0)
                {
                    for (int i = 0; i < l.Length - 1; i++)
                        if (l[i].vector[j] < 0) k++;
                    if (k == l.Length - 1) return j;
                    k = 0;
                }

            return k;
        }
        /// <summary>
        /// (Старый) Поиск целочисленного решения задачи линейного программирования, заданной симплекс-таблицей
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="fs">Файл с матрицей (симплекс-таблицей) размера (x.Lenth+1)X(xbase.Lenth+1)</param>
        /// <returns></returns>
        public static double SimplexInteger(ref Vectors result, StreamReader fs)
        {
            //Считать таблицу
            //чтение данных
            int k = 0;//число единиц
            for (int i = 0; i < result.Deg; i++) if (result[i] != 0) k++;
            Vectors[] lines = new Vectors[k + 1];
            for (int i = 0; i < k + 1; i++)
            {
                lines[i] = new Vectors(result.Deg + 1);
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                for (int j = 0; j < lines[i].Deg; j++) lines[i].vector[j] = Convert.ToDouble(st[j]);
            }
            fs.Close();
            //Найти решение обычным симплекс-методом
            double end = Vectors.SimpleSimplex(ref result, ref lines);
            //Vectors.Show(lines); //не решает, потому что double =3, (int)3=2...

            if (Vectors.IsIntDesigion(lines)) return end;
            int y = 0;
            //Если решение не целочисленное, то пока оно не является целочисленным и пока его разумно искать...
            while ((!Vectors.IsIntDesigion(lines)) && (!Vectors.ImpossibleFoundIntDesigion(lines)) && (y <= 10))
            {
                Console.WriteLine();
                Console.WriteLine("-------------------------Итерация метода Гомори номер {0}", y + 1);
                //Создаётся новая симплекс-таблица
                Vectors[] newlines = new Vectors[lines.Length + 1];
                for (int i = 0; i < lines.Length - 1; i++)//Переписать строки до строки с функционалом
                {
                    newlines[i] = new Vectors(lines[0].Deg + 1);
                    for (int j = 0; j < lines[0].Deg; j++) newlines[i].vector[j] = lines[i].vector[j];
                    newlines[i].vector[lines[0].Deg] = 0;
                }
                int tmp = Vectors.NumberOfNotIntDes(lines);//Номер строки, где решение не целочисленное
                Console.WriteLine("----------За образец новой строки взята строка {0}", tmp + 1);
                newlines[lines.Length - 1] = new Vectors(lines[0].Deg + 1);
                newlines[lines.Length] = new Vectors(lines[0].Deg + 1);

                //Записать новую строку
                for (int j = 0; j < lines[0].Deg; j++) newlines[lines.Length - 1].vector[j] = -(double)Number.Rational.ToRational(lines[tmp].vector[j]).FracPart;
                newlines[lines.Length - 1].vector[lines[0].Deg] = 1;
                //Переписать строку функции с изменением знака
                for (int j = 0; j < lines[0].Deg; j++) newlines[lines.Length].vector[j] = -lines[lines.Length - 1].vector[j];
                newlines[lines.Length].vector[lines[0].Deg] = 0;
                //Записать новый вектор result
                Vectors newresult = new Vectors(result.Deg + 1);
                for (int i = 0; i < result.Deg; i++)
                {
                    if (result[i] == 0) newresult[i] = result[i];
                    else newresult[i] = 1;
                }
                newresult[result.Deg] = /*newlines[lines.Length - 1].vector[0]*/1;

                //Console.WriteLine("--------Новая таблица создана");

                //Переписать новые данные в старые
                result = new Vectors(newresult);
                lines = new Vectors[newlines.Length];
                for (int i = 0; i < newlines.Length; i++) lines[i] = new Vectors(newlines[i]);
                //Console.WriteLine("--------Новая таблица перезаписана");
                //Console.WriteLine("-----------Новое использование сиплекс-метода-----------");

                //Решить двойственным симплекс-методом
                end = Vectors.SimpleSimplex(ref result, ref lines);
                y++;
            }

            Console.WriteLine("Целочисленное решение найдено либо целочисленного решения не существует");

            //Вернуть значение
            Console.WriteLine("Конечная симплекс-таблица: "); Vectors.ShowRational(lines);
            Console.Write("Вектор решения:"); result.ShowRational();
            return -end;
        }
        /// <summary>
        /// Проверка на то, содержится ли в симплекс-таблице целочисленное решение
        /// </summary>
        /// <param name="l"></param>
        /// <returns>True, если содержится</returns>
        private static bool IsIntDesigion(Vectors[] l)
        {/*Math.Truncate(l[i].vector[0])!= l[i].vector[0]*/
            for (int i = 0; i < l.Length - 1; i++)
                if (/*l[i].vector[0] != (int)l[i].vector[0]*/ Number.Rational.ToRational(l[i].vector[0]).IsFract())
                {
                    //Console.WriteLine("{0} != {1}", l[i].vector[0], Number.Rational.ToRational(l[i].vector[0]).IntPart);
                    return false;
                }
            return true;
        }
        /// <summary>
        /// Возвращает номер оптимальной строки, где решение не является целочисленными
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private static int NumberOfNotIntDes(Vectors[] l)
        {
            double t, max = 0;
            int k = 0;
            bool an = false;
            for (int i = 0; i < l.Length - 1; i++)
                if (Number.Rational.ToRational(l[i].vector[0]).IsFract())
                {
                    an = true;
                    t = Number.Rational.ToRational(l[i].vector[0]).FracPart;
                    if (t > max) { max = t; k = i; }
                }
            if (an) return k;
            throw new Exception("Решение является целочисленным!");
        }
        /// <summary>
        /// Проверка невозможности нахождения целочисленного решения
        /// </summary>
        /// <param name="l"></param>
        /// <returns>True, если целочисленное решение найти невозможно</returns>
        /// <remarks>Целочисленного решения не существует, если в какой-то столбце имеется дробный свободный коэффициент, а все остальные коэффициенты - целые</remarks>
        private static bool ImpossibleFoundIntDesigion(Vectors[] l)
        {
            for (int i = 0; i < l.Length - 1; i++)
                if (/*!*/Number.Rational.ToRational(l[i].vector[0]).IsFract())
                {
                    int k = 1;
                    for (int j = 1; j < l[0].Deg; j++)
                        if (l[i].vector[j] == (int)l[i].vector[j]) k++;
                    if (k == l[0].Deg) return true;
                }
            return false;
        }
        /// <summary>
        /// Получить вектор решения по симплекс-таблице
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private static Vectors GetSolutionVec(Vectors[] lines)
        {
            //int t = 0;
            Vectors r = new Vectors(lines[0].Deg - 1);
            int ii = 0;

            for (int j = 1; j < lines[0].Deg; j++)
            {
                int zero = 0, one = 0;
                for (int i = 0; i < lines.Length; i++)
                    if (lines[i].vector[j] == 0) zero++;
                    else if (lines[i].vector[j] == 1) { one++; ii = i; }
                if (zero == lines.Length - 1 && one == 1) { r[j - 1] = lines[ii].vector[0]; ii = 0; }
                else r[j - 1] = 0;
            }

            //for (int i = 0; i < r.Deg; i++)
            //{
            //    if (r[i] != 0)
            //    {
            //        r[i] = lines[t].vector[0];
            //        t++;
            //    }
            //}
            return r;
        }


        /// <summary>
        /// Вывести вектор в файл
        /// </summary>
        private void Show(StreamWriter sf) { sf.WriteLine(this.ToString()); }
        /// <summary>
        /// Вывести массив векторов c рациональными коэффициентами в файл
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sf"></param>
        private static void ShowRational(Vectors[] lines, StreamWriter sf)
        {
            //for (int i = 0; i < lines.Length; i++) lines[i].ShowRational();
            sf.Write(" \t\tb "); for (int i = 1; i < lines[0].Deg; i++) sf.Write("\tx[{0}] ", i);
            sf.WriteLine();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                sf.Write("x[{0}]\t", Vectors.NumberColumn(lines, i));
                lines[i].ShowRationalTab(sf);
            }
            sf.Write("L\t"); lines[lines.Length - 1].ShowRationalTab(sf);
        }
        /// <summary>
        /// Нахождение максимума функционала методами симплекс-таблицы, считанной из файла, с выводом в файл
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="fs">Файл с матрицей (симплекс-таблицей) размера (x.Lenth+1)X(xbase.Lenth+1) (образец)</param>
        /// <param name="sf"></param>
        /// <returns>Наибольшее значение функционала, который задан последней строкой таблицы</returns>
        /// <remarks>Не предусматривается возможность вырожденного решения</remarks>
        private static double SimpleSimplex(ref Vectors result, StreamReader fs, StreamWriter sf)
        {
            //чтение данных
            int k = 0;//число единиц
            for (int i = 0; i < result.Deg; i++) if (result[i] != 0) k++;
            Vectors[] lines = new Vectors[k + 1];
            for (int i = 0; i < k + 1; i++)
            {
                lines[i] = new Vectors(result.Deg + 1);
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                for (int j = 0; j < lines[i].Deg; j++) lines[i].vector[j] = Convert.ToDouble(st[j]);
            }
            fs.Close();
            sf.WriteLine("Исходная сиплекс-таблица:");
            Vectors.Show(lines, sf);
            sf.WriteLine("Вектор, указывающий на базис:");
            result.ShowRational(sf); sf.WriteLine(); sf.WriteLine();

            //Проверка на отрицательные свободные члены и их исправление
            while (Vectors.ExistColumnDual(lines) != -1)
            {
                int m = Vectors.ExistColumnDual(lines);
                int n = Vectors.FindPosElemDual(lines, m);
                sf.WriteLine("Для избавления от отрицательных свободных коэффициентов в базисных переменных разрещающим выбран элемент на пересечении строки {0} и столбца {1}", m + 1, n + 1);
                Vectors.Transform(ref lines, m, n, ref result);
                sf.WriteLine("Сиплекс-таблица после избавления от отрицательных свободных коэффициентов в базисных переменных");
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                //sf.WriteLine();
            }

            //Решение
            int it_num = 1;
            while (Vectors.ExistColumn(lines) != 0)
            {
                if (Vectors.ExistInfinity(lines) != 0)
                {
                    sf.WriteLine("Функция неограничена на области допустимых решений (столбец {0} таблицы полностью отрицателен)", Vectors.ExistInfinity(lines) + 1);
                    return Double.PositiveInfinity;
                }
                int m = Vectors.ExistColumn(lines);
                sf.WriteLine("На итерации {0} разрещающим выбран элемент на пересечении строки {1} и столбца {2}", it_num, Vectors.FindPosElem(lines, m) + 1, m + 1);
                Vectors.Transform(ref lines, Vectors.FindPosElem(lines, m), m, ref result);
                sf.WriteLine("Сиплекс-таблица после итерации {0}:", it_num);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                sf.WriteLine();
                it_num++;
                //result.Show(); sf.WriteLine(); sf.WriteLine();
            }

            result = new Vectors(Vectors.GetSolutionVec(lines));
            //result.Show();
            //sf.WriteLine("Вектор решения {0}", result.ToString());
            sf.WriteLine("Вектор решения {0}", result.ToRationalString());
            sf.WriteLine("Оптимальное значение функции равно {0}", lines[k].vector[0]);

            return lines[k].vector[0];
        }
        /// <summary>
        /// Нахождение максимума функционала методами симплекс таблицы, заданной массивом векторов
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="lines">Симплекс-таблица, заданная массивом векторов</param>
        /// <param name="sf"></param>
        /// <returns></returns>
        public static double SimpleSimplex(ref Vectors result, ref Vectors[] lines, StreamWriter sf)
        {
            sf.WriteLine("Исходная сиплекс-таблица:");
            Vectors.ShowRational(lines, sf);
            sf.WriteLine("Вектор, указывающий на базис:");
            result.ShowRational(sf); sf.WriteLine(); sf.WriteLine();

            //Проверка на отрицательные свободные члены и их исправление
            while (Vectors.ExistColumnDual(lines) != -1)
            {
                int m = Vectors.ExistColumnDual(lines);
                int n = Vectors.FindPosElemDual(lines, m);
                sf.WriteLine($"Для избавления от отрицательных свободных коэффициентов в базисных переменных разрещающим выбран элемент на пересечении строки {m + 1} и столбца {n + 1}");
                Vectors.Transform(ref lines, m, n, ref result);
                sf.WriteLine("Сиплекс-таблица после избавления от отрицательных свободных коэффициентов в базисных переменных");
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                //sf.WriteLine();
            }

            //Решение
            int it_num = 1;
            while (Vectors.ExistColumn(lines) != 0)
            {
                if (Vectors.ExistInfinity(lines) != 0)
                {
                    sf.WriteLine("Функция неограничена на области допустимых решений (столбец {0} таблицы полностью отрицателен)", Vectors.ExistInfinity(lines) + 1);
                    sf.Close();
                    return Double.PositiveInfinity;
                }
                int m = Vectors.ExistColumn(lines);
                int n = Vectors.FindPosElem(lines, m);
                //sf.WriteLine("+");
                sf.WriteLine("На итерации {0} разрещающим выбран элемент на пересечении строки {1} и столбца {2}", it_num, n + 1, m + 1);
                //sf.WriteLine("++");
                Vectors.Transform(ref lines, Vectors.FindPosElem(lines, m), m, ref result);
                sf.WriteLine("Сиплекс-таблица после итерации {0}:", it_num);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                sf.WriteLine();
                it_num++;
                //result.Show(); sf.WriteLine(); sf.WriteLine();
            }
            result = new Vectors(Vectors.GetSolutionVec(lines));

            //result.Show();
            //sf.WriteLine("Вектор решения {0}", result.ToString());
            sf.WriteLine("Вектор решения {0}:", result.ToRationalString());
            sf.WriteLine("Оптимальное значение функции равно: {0}", lines[lines.Length - 1].vector[0]);

            return lines[lines.Length - 1].vector[0];
        }
        /// <summary>
        /// Поиск целочисленного решения задачи линейного программирования, заданной симплекс-таблицей
        /// </summary>
        /// <param name="result">Вектор вида (0,0,1,0,1), показывающий, какие переменные являются базисными</param>
        /// <param name="fs">Файл с матрицей (симплекс-таблицей) размера (x.Lenth+1)X(xbase.Lenth+1)</param>
        /// <param name="sf"></param>
        /// <returns></returns>
        public static double SimplexInteger(ref Vectors result, StreamReader fs, StreamWriter sf)
        {
            //Считать таблицу
            //чтение данных
            int k = 0;//число единиц
            for (int i = 0; i < result.Deg; i++) if (result[i] != 0) k++;
            Vectors[] lines = new Vectors[k + 1];
            for (int i = 0; i < k + 1; i++)
            {
                lines[i] = new Vectors(result.Deg + 1);
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                for (int j = 0; j < lines[i].Deg; j++) lines[i].vector[j] = Convert.ToDouble(st[j]);
            }
            fs.Close();
            //Найти решение обычным симплекс-методом
            double end = Vectors.SimpleSimplex(ref result, ref lines, sf);

            if (Vectors.IsIntDesigion(lines) || end == Double.PositiveInfinity) { sf.Close(); return end; }
            int y = 0;
            sf.WriteLine("-------------->Оптимальное решение найдено. Начинается решение двойственной задачи с целью поиска целочисленности.");
            sf.WriteLine(); sf.WriteLine();

            //Если решение не целочисленное, то пока оно не является целочисленным и пока его разумно искать...
            while ((!Vectors.IsIntDesigion(lines)) && (!Vectors.ImpossibleFoundIntDesigion(lines)) && (y <= 15))
            {
                sf.WriteLine();
                sf.WriteLine("----------------------------------->Итерация метода Гомори номер {0}", y + 1);
                //Создаётся новая симплекс-таблица
                Vectors[] newlines = new Vectors[lines.Length + 1];
                for (int i = 0; i < lines.Length - 1; i++)//Переписать строки до строки с функционалом
                {
                    newlines[i] = new Vectors(lines[0].Deg + 1);
                    for (int j = 0; j < lines[0].Deg; j++) newlines[i].vector[j] = lines[i].vector[j];
                    newlines[i].vector[lines[0].Deg] = 0;
                }
                int tmp = Vectors.NumberOfNotIntDes(lines);//Номер строки, где решение не целочисленное
                sf.WriteLine("------------------------->За образец новой строки взята строка {0}", tmp + 1);
                newlines[lines.Length - 1] = new Vectors(lines[0].Deg + 1);
                newlines[lines.Length] = new Vectors(lines[0].Deg + 1);

                //Записать новую строку
                for (int j = 0; j < lines[0].Deg; j++) newlines[lines.Length - 1].vector[j] = -(double)Number.Rational.ToRational(lines[tmp].vector[j]).FracPart;
                newlines[lines.Length - 1].vector[lines[0].Deg] = 1;
                //Переписать строку функции с изменением знака, если итерация первая (и без изменения знака в противном случае)
                for (int j = 0; j < lines[0].Deg; j++)
                    if (y + 1 == 1) newlines[lines.Length].vector[j] = -lines[lines.Length - 1].vector[j];
                    else newlines[lines.Length].vector[j] = lines[lines.Length - 1].vector[j];
                newlines[lines.Length].vector[lines[0].Deg] = 0;
                //Записать новый вектор result
                Vectors newresult = new Vectors(result.Deg + 1);
                for (int i = 0; i < result.Deg; i++)
                {
                    if (result[i] == 0) newresult[i] = result[i];
                    else newresult[i] = 1;
                }
                newresult[result.Deg] = /*newlines[lines.Length - 1].vector[0]*/1;

                //sf.WriteLine("--------Новая таблица создана");

                //Переписать новые данные в старые
                result = new Vectors(newresult);
                lines = new Vectors[newlines.Length];
                for (int i = 0; i < newlines.Length; i++) lines[i] = new Vectors(newlines[i]);
                //sf.WriteLine("--------Новая таблица перезаписана");
                //sf.WriteLine("-----------Новое использование сиплекс-метода-----------");

                //Решать двойственным симплекс-методом
                //end = Vectors.SimpleSimplex(ref result, ref lines, sf);
                end = Vectors.SimplexDual(ref result, ref lines, sf, y + 1);
                y++;
            }

            sf.WriteLine();
            sf.WriteLine("----->------>--------> Целочисленное решение найдено либо целочисленного решения не существует");

            //Вернуть значение
            sf.WriteLine("Конечная симплекс-таблица: "); Vectors.ShowRational(lines, sf);
            sf.Write("Вектор решения:"); result.ShowRational(sf);
            sf.WriteLine($"Оптимальное значение функции равно {-lines[lines.Length - 1].vector[0]}");
            sf.Close();
            return end;
        }
        private static double SimplexDual(ref Vectors result, ref Vectors[] lines, StreamWriter sf, int y)
        {
            sf.WriteLine("Исходная сиплекс-таблица до избавления от отрицательных свободных коэффициентов в базисе:");
            Vectors.ShowRational(lines, sf);
            int k = 1;
            //if
            while (Vectors.ExistColumnDual(lines) != -1)
            {
                int m = Vectors.ExistColumnDual(lines);
                int n = Vectors.FindPosElemDual(lines, m);
                //sf.WriteLine("+");
                sf.WriteLine("На итерации {0}.{1} метода Гомори для избавления от отрицательных свободных коэффициентов в базисных переменных разрещающим выбран элемент на пересечении строки {2} и столбца {3}", y, k, m + 1, n + 1);
                //sf.WriteLine("++");
                Vectors.Transform(ref lines, m, n, ref result);
                sf.WriteLine("Сиплекс-таблица после итерации {0}.{1} алгоритма Гомори:", y, k);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                //sf.WriteLine();
                k++;
            }
            if (!Vectors.NotExistColumn(lines))//если среди коэффициентов функционала есть положительные
            {
                result = new Vectors(Vectors.GetSolutionVec(lines));
                sf.WriteLine("Использование обычного симплекс-метода после {0}.{1} итерации алгоритма Гомори:", y, k);
                Vectors.SimpleSimplexD(ref result, ref lines, sf);
                //Vectors.SimpleSimplex(ref result, ref lines, sf);
            }
            result = new Vectors(Vectors.GetSolutionVec(lines));

            //sf.WriteLine("Вектор решения {0}:", result.ToRationalString());
            //sf.WriteLine("Оптимальное значение функции равно: {0}", -lines[lines.Length - 1].vector[0]);

            return -lines[lines.Length - 1].vector[0];
        }
        /// <summary>
        /// Проверяет симплекс-таблицу на наличие столбца (то есть строки, так как решается двойственная задача) с положительным элементом, где коэффициент при линейной форме отрицателен
        /// </summary>
        /// <param name="l"></param>
        /// <returns>Номер найденного столбца или 0, если таковой отсутствует</returns>
        /// <remarks>Выбирается стобец с наибольшей по модулю отрицательной частью в конце</remarks>
        private static int ExistColumnDual(Vectors[] l)
        {
            int k = -1;
            double max = 0;
            for (int j = 0; j < l.Length - 1; j++)
                if (l[j].vector[0] < 0)
                    if (Math.Abs(l[j].vector[0]) > Math.Abs(max))
                    {
                        max = l[j].vector[0];
                        k = j;
                    }
            if (k >= 0)
                for (int i = 1; i < l[0].Deg; i++)
                    if (l[k].vector[i] < 0) return k;
            return -1;
        }
        /// <summary>
        /// Найти позицию элемента, который годится под центр в симплекс-таблице (при решении двойственной задачи)
        /// </summary>
        /// <param name="l">Симплекс-таблица</param>
        /// <param name="k">Номер столбца, где этот элемент ищется</param>
        /// <returns>Номер строки, где этот элемент находится</returns>
        private static int FindPosElemDual(Vectors[] l, int k)
        {
            int t = 1;
            while (l[k].vector[t] >= 0) t++;
            double min = Math.Abs(l[l.Length - 1].vector[t] / l[k].vector[t]);//может возникнуть ошибка деления на 0
            int u = t;
            for (int i = t + 1; i < l[0].Deg - 1; i++)
            {
                if (l[k].vector[i] < 0)
                {
                    double tmp = Math.Abs(l[l.Length - 1].vector[i] / l[k].vector[i]);
                    if (tmp < min)
                    {
                        min = tmp;
                        u = i;
                    }
                }
            }
            return u;
        }
        private static double SimpleSimplexD(ref Vectors result, ref Vectors[] lines, StreamWriter sf)
        {
            sf.WriteLine("Исходная сиплекс-таблица:");
            Vectors.ShowRational(lines, sf);
            sf.WriteLine("Вектор, указывающий на базис:");
            result.ShowRational(sf); sf.WriteLine(); sf.WriteLine();

            //Решение
            int it_num = 1;
            while (Vectors.ExistColumnM(lines) != 0)
            {
                if (Vectors.ExistInfinity(lines) != 0)
                {
                    sf.WriteLine("Функция неограничена на области допустимых решений (столбец {0} таблицы полностью отрицателен)", Vectors.ExistInfinity(lines) + 1);
                    return Double.PositiveInfinity;
                }
                int m = Vectors.ExistColumnM(lines);
                int n = Vectors.FindPosElemM(lines, m);
                //sf.WriteLine("+");
                sf.WriteLine("На итерации {0} разрещающим выбран элемент на пересечении строки {1} и столбца {2}", it_num, n + 1, m + 1);
                //sf.WriteLine("++");
                Vectors.Transform(ref lines, n, m, ref result);
                sf.WriteLine("Сиплекс-таблица после итерации {0}:", it_num);
                //Vectors.Show(lines);
                Vectors.ShowRational(lines, sf);
                sf.WriteLine();
                it_num++;
            }

            result = new Vectors(Vectors.GetSolutionVec(lines));
            sf.WriteLine("Вектор решения {0}:", result.ToRationalString());
            sf.WriteLine("Оптимальное значение функции равно: {0}", -lines[lines.Length - 1].vector[0]);

            return -lines[lines.Length - 1].vector[0];
        }
        private static int ExistColumnM(Vectors[] l)
        {
            int k = 0;
            double max = 0;
            for (int j = 1; j < l[0].Deg; j++)
                if (l[l.Length - 1].vector[j] > 0)
                    if (Math.Abs(l[l.Length - 1].vector[j]) > Math.Abs(max))
                    {
                        max = l[l.Length - 1].vector[j];
                        k = j;
                    }
            for (int i = 0; i < l.Length - 1; i++)
                if (l[i].vector[k] < 0)
                {
                    return k;
                }
            return 0;
        }
        private static int FindPosElemM(Vectors[] l, int k)
        {
            int fix = 0;//Номер столбца, с которым сравниваем
            int t = 0;
            while (l[t].vector[k] <= 0) t++;
            double min = l[t].vector[fix] / l[t].vector[k];//может возникнуть ошибка деления на 0
            int u = t;
            for (int i = t + 1; i < l.Length - 1; i++)
            {
                if (l[i].vector[k] > 0)
                {
                    double tmp = l[i].vector[fix] / l[i].vector[k];
                    if (tmp < min)
                    {
                        min = tmp;
                        u = i;
                    }
                }
            };
            return u;
        }
        /// <summary>
        /// Проверяет симплекс-таблицу на отсутствие столбца с положительным элементом, где коэффициент при линейной форме отрицателен
        /// </summary>
        /// <param name="l"></param>
        /// <returns>Номер найденного столбца или 0, если таковой отсутствует</returns>
        /// <remarks>Выбирается стобец с наибольшей по модулю отрицательной частью в конце</remarks>
        private static bool NotExistColumn(Vectors[] l)
        {
            for (int j = 1; j < l[0].Deg; j++)
                if (l[l.Length - 1].vector[j] > 0)
                {
                    return false;
                }

            return true;
        }

        /// <summary>
        /// Перевод массива строк в матрицу
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Matrix LinesToMatrix(Vectors[] l)
        {
            Matrix A = new Matrix(l.Length, l[0].Deg);
            for (int i = 0; i < A.n; i++)
                for (int j = 0; j < A.m; j++)
                    A[i, j] = l[i].vector[j];
            return A;
        }
        /// <summary>
        /// Перевод матрицы в массив строк
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Vectors[] MatrixToLines(Matrix A)
        {
            Vectors[] l = new Vectors[A.n];
            for (int i = 0; i < l.Length; i++)
            {
                l[i] = new Vectors(A.m);
                for (int j = 0; j < A.m; j++) l[i].vector[j] = A[i, j];
            }
            return l;
        }
        /// <summary>
        /// Содержат ли векторы общий элемент
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ExistIntersection(Vectors a, Vectors b)
        {
            for (int i = 0; i < a.Deg; i++)
                for (int j = 0; j < b.Deg; j++)
                    if (a[i] == b[j]) return true;
            return false;
        }
        /// <summary>
        /// Быстрое прибавление вектора
        /// </summary>
        /// <param name="v"></param>
        public void FastAdd(Vectors v)
        {
            for (int i = 0; i < v.Deg; i++)
                vector[i] += v[i];
        }
        #endregion

        #region Операторы
        public static Vectors operator +(Vectors A, Vectors B)
        {
            Vectors C = new Vectors(A.Deg);
            for (int i = 0; i < A.Deg; i++)
            {
                C[i] = A[i] + B[i];
            }
            return C;
        }

        public static Vectors operator +(Vectors a, Double b)
        {
            return a + new Vectors(a.Deg, b);
        }
       
        public static Vectors operator -(Vectors A, Vectors B)
        {
            Vectors R = new Vectors(A.Deg);
            for (int i = 0; i < A.Deg; i++)
            {
                R[i] = A[i] - B[i];
            }
            return R;

        }
        public static Vectors operator -(Vectors A)
        {
            Vectors R = new Vectors(A.Deg);
            for (int i = 0; i < A.Deg; i++)
            {
                R[i] = -A[i];
            }
            return R;

        }

        public static Vectors operator -(Vectors v, double c)
        {
            Vectors r = new Vectors(v);
            for (int i = 0; i < r.Deg; i++)
                r[i] -= c;
            return r;
        }
        public static Vectors operator -(double c, Vectors v) => -(v - c);
       
        public static double operator *(Vectors A, Vectors B)
        {
            double sum = 0;
            for (int i = 0; i < A.Deg; i++)
            {
                sum += A[i] * B[i];

            }
            return sum;
        }
      
        public static Vectors operator *(Vectors A, double Ch)
        {
            Vectors q = new Vectors(A.Deg);
            for (int i = 0; i < A.Deg; i++)
            {
                q[i] = A[i] * Ch;
            }
            return q;
        }
        public static Vectors operator *(double Ch, Vectors A) { return A * Ch; }
        
        public static Vectors operator /(Vectors A, double Ch)
        {
            if (Ch == 0) throw new DivideByZeroException("Деление на ноль!");
            return A * (1.0 / Ch);
        }

        //public static bool operator ==(Vectors a, Vectors b)
        //{
        //    if (a.Deg != b.Deg) throw new Exception("Векторы не совпадают по длине!");
        //    for (int i = 0; i < a.Deg; i++)
        //        if (a[i] != b[i]) return false;
        //    return true;
        //}
        //public static bool operator !=(Vectors a,Vectors b) { return !(a==b); }

        /// <summary>
        /// Сравнение всех элементов с числом
        /// </summary>
        /// <param name="a"></param>
        /// <param name="Ch"></param>
        /// <returns></returns>
        public static bool operator ==(Vectors a, double Ch)
        {
            for (int i = 0; i < a.Deg; i++)
                if (a[i] != Ch) return false;
            return true;
        }
        public static bool operator !=(Vectors a, double b) { return !(a == b); }
        public static bool operator >(Vectors a, double Ch)
        {
            for (int i = 0; i < a.Deg; i++)
                if (a[i] <= Ch) return false;
            return true;
        }
        public static bool operator <(Vectors a, double Ch)
        {
            for (int i = 0; i < a.Deg; i++)
                if (a[i] >= Ch) return false;
            return true;
        }
        #endregion


        public override bool Equals(object vv)
        {
            Vectors v = vv as Vectors;
            if (this.Deg != v.Deg) return false;
            for (int i = 0; i < v.Deg; i++)
                if (this.vector[i] != v[i]) return false;
            return true;
        }
        public override int GetHashCode()
        {
            var hashCode = 1350185542;
            hashCode = hashCode * -1521134295 + this.Deg.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double[]>.Default.GetHashCode(vector);
            hashCode = hashCode * -1521134295 + Deg.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double[]>.Default.GetHashCode(DoubleMas);
            hashCode = hashCode * -1521134295 + ArithmeticAv.GetHashCode();
            hashCode = hashCode * -1521134295 + Average.GetHashCode();
            hashCode = hashCode * -1521134295 + RelAc.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(RelAcVec);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(RelAcSqr);
            hashCode = hashCode * -1521134295 + Max.GetHashCode();
            hashCode = hashCode * -1521134295 + Min.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(Sort);
            hashCode = hashCode * -1521134295 + EuqlidNorm.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(AbsVector);
            hashCode = hashCode * -1521134295 + MaxAbs.GetHashCode();
            hashCode = hashCode * -1521134295 + MinAbs.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(ToAver);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vectors>.Default.GetHashCode(ToAverDel);
            return hashCode;
        }

        /// <summary>
        /// Евклидово расстояние между векторами
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Distance(Vectors v1, Vectors v2) => (v1 - v2).EuqlidNorm;

        /// <summary>
        /// Смешение векторов abcd и xyz в вектор axbyczd
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vectors Mix(Vectors a, Vectors b)
        {
            Vectors v = new Vectors(a.Deg + b.Deg);
            int aa = 0, bb = 0, i = 0;
            while (i < v.Deg)
            {
                if (aa < a.Deg) { v[i] = a[aa++]; i++; }
                if (bb < b.Deg) { v[i] = b[bb++]; i++; }

            }
            return v;
        }
        /// <summary>
        /// Объединение с другим вектором, вдобавок сортировка
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public void UnionWith(Vectors v)
        {
            double[] mas = Expendator.Union(this.DoubleMas, v.DoubleMas);
            mas = mas.Distinct().ToArray();
            Array.Sort(mas);
            this.vector = new double[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                this.vector[i] = mas[i];
            //this.Show();
        }
        public static Vectors Union(Vectors a, Vectors b) { Vectors tmp = new Vectors(a); tmp.UnionWith(b); return new Vectors(tmp); }

        /// <summary>
        /// Простое объединение массива векторов (без сортировки!)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vectors Union(Vectors[] v)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                c += v[i].Deg;
            Vectors res = new Vectors(c);
            int k = 0;
            for (int i = 0; i < v.Length; i++)
                for (int j = 0; j < v[i].Deg; j++)
                    res[k++] = v[i][j];
            return res;
      
        }
        /// <summary>
        /// Прострое объединение двух векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vectors Union2(Vectors a, Vectors b) => Union(new Vectors[] { a, b });

        /// <summary>
        /// Повторить указанный вектор указанно число раз
        /// </summary>
        /// <param name="v"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vectors Repeat(Vectors v,int count)
        {
            Vectors[] mas = new Vectors[count];
            for (int i = 0; i < count; i++)
                mas[i] = v.dup;
            return Union(mas);
        }

       
        /// <summary>
        /// Покомпонентное произведение векторов
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vectors CompMult(Vectors v1, Vectors v2)
        {
            Vectors res = new Vectors(v1.Deg);
            for (int i = 0; i < res.Deg; i++)
                res[i] = v1[i] * v2[i];
            return res;
        }

        /// <summary>
        /// Создать вектор указанной размерности со случайными числами из указанного диапазона
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vectors Create(int dim, double min = 0, double max = 1)
        {
            Vectors res = new Vectors(dim);
            var t = new MathNet.Numerics.Random.MersenneTwister();
            Random r = new Random(Environment.TickCount);
            for (int i = 0; i < dim; i++)
                //res[i] = min + r.NextDouble()*(max-min);
                res[i] = min + t.NextDouble() * (max - min);
            return res;
        }
        /// <summary>
        /// Перевести матрицу в вектор
        /// </summary>
        /// <param name="A"></param>
        /// <param name="col">Если true, элементы вектора собираются в первую очередь слева-направо, иначе сверху-вниз</param>
        /// <returns></returns>
        public static Vectors Create(Matrix A, bool col = true)
        {
            Vectors res = new Vectors(A.ColCount * A.RowCount);
            int k = 0;
            if (col)
                for (int i = 0; i < A.RowCount; i++)
                    for (int j = 0; j < A.ColCount; j++)
                        res[k++] = A[i, j];
            else
                for (int j = 0; j < A.ColCount; j++)
                    for (int i = 0; i < A.RowCount; i++)
                        res[k++] = A[i, j];
            return res;
        }
        /// <summary>
        /// Создать случайный вектор из delta-окрестности указанного вектора
        /// </summary>
        /// <param name="center"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vectors Create(Vectors center, double delta)
        {
            var r = new MathNet.Numerics.Random.CryptoRandomSource();
            Vectors res = new Vectors(center);
            for (int i = 0; i < res.Deg; i++)
                res[i] += r.NextDouble() * 2 * delta - delta;
            return res;
        }
        /// <summary>
        /// Создать случайный вектор между двумя заданными векторами
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vectors Create(Vectors min, Vectors max)
        {
            var r = new MathNet.Numerics.Random.CryptoRandomSource();
            Vectors res = new Vectors(min);
            for (int i = 0; i < res.Deg; i++)
                res[i] += r.NextDouble() * (max[i]-min[i]);
            return res;
        }
        /// <summary>
        /// Создать вектор по массиву чисел
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Vectors Create(params double[] mas) => new Vectors(mas);
        /// <summary>
        /// Вектор, заполненный одинаковыми числами
        /// </summary>
        /// <param name="number"></param>
        /// <param name="repeatcount"></param>
        /// <returns></returns>
        public static Vectors Create(double number, int repeatcount) => new Vectors(repeatcount, number);

        /// <summary>
        /// Случайный вектор указанной размерности
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vectors Random(int degree, double min=0,double max=1)
        {
            var r =new  MathNet.Numerics.Random.CryptoRandomSource();
            Vectors v = new Vectors(degree);
            for (int i = 0; i < degree; i++)
                v[i] = min + r.NextDouble() * (max - min);
            return v;
            
        }

        /// <summary>
        /// Cумма квадратов элементов вектора
        /// </summary>
        public double DistNorm
        {
            get
            {
                double sum = 0;
                for (int i = 0; i < Deg; i++)
                    sum += this[i] * this[i];
                return sum;
            }
        }

        /// <summary>
        /// Записать массив векторов в файл
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="filename"></param>
        public static void VectorsToFile(Vectors[] mas, string filename = "vectors.txt")
        {
            StreamWriter fs = new StreamWriter(filename);
            for (int i = 0; i < mas.Length; i++)
                fs.WriteLine(mas[i].ToString());
            fs.Close();
        }

        /// <summary>
        /// Получить массив векторов из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Vectors[] VectorsFromFile(string filename)
        {
            StreamReader fs = new StreamReader(filename);
            List<Vectors> l = new List<Vectors>(10);
            string s = fs.ReadLine();
            while (s != null)
            {
                l.Add(new Vectors(s));
                s = fs.ReadLine();
            }
            fs.Close();
            return l.ToArray();
        }

        /// <summary>
        /// Нормализовать значения в векторе к отрезку
        /// </summary>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Vectors Normalize(double c=0,double d = 1)
        {
            double a = this.Min;
            double b = this.Max;

            return (this - a) * ((d - c) / (b - a)) + c;
        }

        /// <summary>
        /// Исходный вектор, делённый на своё максимальное значение, чтобы принимать значения из [0,1] (предполагается, что в нём нет отрицательных элементов)
        /// </summary>
        public Vectors Norming => this / this.Max;

        /// <summary>
        /// Подвектор из первых len элементов
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public Vectors SubVector(int len)
        {
            Vectors r = new Vectors(len);
            for (int i = 0; i < len; i++)
                r[i] = this[i];
            return r;
        }

        /// <summary>
        /// Минимальная разница между соседними компонентами в векторе
        /// </summary>
        public double MinDist
        {
            get
            {
                double d = Math.Abs(this[1] - this[0]);
                for(int i=2;i< this.Deg; i++)
                {
                    double c= Math.Abs(this[i] - this[i-1]);
                    if (c < d) d = c;
                }
                return d;
            }
        }

        /// <summary>
        /// Ищет наиболее близкий к d элемент отсортированного массива
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double BinaryApproxSearch(double d)
        {
            int i = 0, j = this.Deg - 1;
            int k;
            if (this[i] == d) return this[i];
            if (this[j] == d) return this[j];
            while (j - i > 1)
            {
                k = (i + j) / 2;
                if (this[k] == d) return this[k];
                if (this[k] < d) i = k;
                else j = k;
            }

            return (Math.Abs(this[i] - d) < Math.Abs(this[j] - d)) ? this[i] : this[j];
        }

        /// <summary>
        /// Быстро создаёт вектор за счёт копирования ссылки на массив, а не дублирования массива
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vectors CreateFast(double[] m)=> new Vectors
            {
                vector = m
            };

        

        /// <summary>
        /// Прочитать вектор из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public  static Vectors VectorFromFile(string filename)
        {
            Vectors res;
            using (StreamReader f = new StreamReader(filename))
                res= Vectors.CreateFast( f.ReadToEnd().Replace('.', ',').Replace("NA","NaN").ToDoubleMas());
                return res;
          }

        /// <summary>
        /// Записать вектор в файл
        /// </summary>
        /// <param name="filename"></param>
        public void ToFile(string filename, bool NanIsNA=true)
        {
            if (!NanIsNA)
                using (StreamWriter f = new StreamWriter(filename))
                    for (int i = 0; i < Deg; i++)
                        f.Write($"{this[i]} ".Replace(',', '.'));
            else
                using (StreamWriter f = new StreamWriter(filename))
                    for (int i = 0; i < Deg; i++)
                        if (Double.IsNaN(this[i]))
                            f.Write($"NA ");
                        else
                            f.Write($"{this[i]} ".Replace(',', '.'));


        }

        /// <summary>
        /// Перенести вектор без создания новых векторов
        /// </summary>
        /// <param name="t"></param>
        public void MoveTo(Vectors t)
        {
            for (int i = 0; i < vector.Length; i++)
                vector[i] = t.vector[i];
        }


        public void Save(string filename)
        {
            using (StreamWriter r = new StreamWriter(filename))
                foreach (var d in this.vector)
                    r.WriteLine(d);
        }
    }

    /// <summary>
    /// Множество векторов
    /// </summary>
    public class SetOfVectors
    {
        private Vectors[] mas;
        /// <summary>
        /// Мощность множества
        /// </summary>
        public int M => mas.Length;

        public SetOfVectors(params Vectors[] vec)
        {
            mas = vec.dup();
        }

        /// <summary>
        /// Расстояние до множества от вектора
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Dist(Vectors v)
        {
            double[] m = new double[M];
            for (int i = 0; i < M; i++)
                m[i] = (mas[i] - v).EuqlidNorm;

            return m.Min();
        }
    }
}

