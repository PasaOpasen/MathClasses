using System;

namespace МатКлассы
{
    public static partial class Number
    {
        /// <summary>
        /// Комплексные числа
        /// </summary>
        public struct Complex : IComparable, Idup<Complex>
        {
            /// <summary>
            /// 2 * pi
            /// </summary>
            static readonly double _2PI;
            /// <summary>
            /// Мнимая единица
            /// </summary>
            public readonly static Complex I;
            /// <summary>
            /// Две мнимые единицы
            /// </summary>
            public readonly static Complex _2I;
            /// <summary>
            /// I/2
            /// </summary>
            public static readonly Complex fracI2 = new Complex(0, 0.5);

            static Complex()
            {
                I = new Complex(0, 1);
                _2I = 2 * I;
                _2PI = 2 * Math.PI;
            }
           
            /// <summary>
            /// Дубликат комплексного числа (нужен, чтобы делать дубликаты массивов)
            /// </summary>
            public Complex dup => new Complex(this);

            /// <summary>
            /// По действительному числу составить комплексное
            /// </summary>
            /// <param name="a"></param>
            public Complex(double a) { Re = a; Im = 0; }
            /// <summary>
            /// Составить комплексное число по паре действительных чисел
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            public Complex(double a, double b) { Re = a; Im = b; }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="p"></param>
            public Complex(Complex p) { Re = p.Re; Im = p.Im; }

            #region Свойства
            /// <summary>
            /// Действительная часть
            /// </summary>
            public double Re { get; set; }
            /// <summary>
            /// Мнимая часть
            /// </summary>
            public double Im { get; set; }
            /// <summary>
            /// Модуль
            /// </summary>
            public double Abs => Math.Sqrt(Re * Re + Im * Im);
            /// <summary>
            /// Аргумент
            /// </summary>
            public double Arg => new System.Numerics.Complex(this.Re, this.Im).Phase;

            /// <summary>
            /// Мнимая часть комплексного числа (нужно для интегрирования, портированного с Fortran)
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public static double Imag(Complex t) => t.Im;

            /// <summary>
            /// Комплексно-сопряжённое число
            /// </summary>
            /// <returns></returns>
            public Complex Conjugate => new Complex(this.Re, -this.Im);

            /// <summary>
            /// Перевести в строку вида a+bi
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string res = "";

                if (this.Re == 0 && this.Im == 0) res = "0";
                else
                {
                    if (this.Re != 0.0)
                    {
                        res = this.Re.ToString() + " ";
                    }

                    if (this.Im != 0.0)
                    {
                        if (this.Im > 0)
                        {
                            res += $"+ {this.Im}i";
                        }
                        else res +=$"- {-this.Im}i";
                    }
                }

                return res;
            }
            #endregion

            #region Преобразования
            /// <summary>
            /// Неявное преобразование действительного числа в комплексное
            /// </summary>
            /// <param name="x"></param>
            public static implicit operator Complex(double x) => new Complex(x, 0);
            /// <summary>
            /// Неявное преобразование натурального числа в комплексное
            /// </summary>
            /// <param name="x"></param>
            public static implicit operator Complex(int x) => new Complex(x, 0);
            /// <summary>
            /// Явное преобразование комплексного числа в действительное (в модуль)
            /// </summary>
            /// <param name="c"></param>
            public static explicit operator double(Complex c) => c.Re;

            public static implicit operator Complex(System.Numerics.Complex c) => new Complex(c.Real, c.Imaginary);
            public static implicit operator System.Numerics.Complex(Complex c) => new Complex(c.Re, c.Im);

            public static implicit operator Complex(Point p) => new Complex(p.x, p.y);
            public static implicit operator Point(Complex p) => new Point(p.Re, p.Im);
            #endregion

            #region Операторы
            public static Complex operator +(Complex c1, Complex c2) => new Complex(c1.Re + c2.Re, c1.Im + c2.Im);

            public static Complex operator +(Complex c1, double c2) => new Complex(c1.Re + c2, c1.Im);

            public static Complex operator +(double c1, Complex c2) => new Complex(c1 + c2.Re, c2.Im);

            public static Complex operator -(Complex c1, Complex c2) => new Complex(c1.Re - c2.Re, c1.Im - c2.Im);

            public static Complex operator -(Complex z) => new Complex(-z.Re, -z.Im);

            public static Complex operator -(Complex c1, double c2) => new Complex(c1.Re - c2, c1.Im);

            public static Complex operator -(double c1, Complex c2) => new Complex(c1 - c2.Re, -c2.Im);

            public static Complex operator *(Complex c1, Complex c2) => new Complex(c1.Re * c2.Re - c1.Im * c2.Im, c1.Re * c2.Im + c1.Im * c2.Re);

            public static Complex operator *(Complex c1, double c2) => new Complex(c1.Re * c2, c1.Im * c2);

            public static Complex operator *(double c1, Complex c2) => new Complex(c1 * c2.Re, c1 * c2.Im);

            public static Complex operator /(Complex c1, Complex c2)
            {
                double Denominator = c2.Re * c2.Re + c2.Im * c2.Im;
                return new Complex((c1.Re * c2.Re + c1.Im * c2.Im) / Denominator,
                    (c2.Re * c1.Im - c2.Im * c1.Re) / Denominator);
            }

            public static Complex operator /(Complex c1, double c2) => new Complex(c1.Re / c2, c1.Im / c2);

            public static Complex operator /(double c1, Complex c2)
            {
                double Denominator = c2.Re * c2.Re + c2.Im * c2.Im;
                return new Complex((c1 * c2.Re) / Denominator, (-c2.Im * c1) / Denominator);
            }

            public static bool operator ==(Complex c1, Complex c2) => c1.Re == c2.Re && c1.Im == c2.Im;

            public static bool operator !=(Complex c1, Complex c2) => c1.Re != c2.Re || c1.Im != c2.Im;


            /// <summary>
            /// Совпадение комплексных чисел
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj) => Equals((Complex)obj);

            public bool Equals(Complex c) => this == c;
            public override int GetHashCode()
            {
                return this.Re.GetHashCode() + this.Im.GetHashCode();
            }
            #endregion

            #region Операции с массивами
            public static Complex[] Minus(Complex[] r)
            {
                Complex[] res = new Complex[r.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = -r[i];
                return res;
            }
            /// <summary>
            /// Сумма комплексного вектора с постоянным комклексным числом(покомпонентное сложение)
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Complex[] Sum(Complex[] x, Complex y)
            {
                Complex[] r = new Complex[x.Length];
                for (int i = 0; i < r.Length; i++)
                    r[i] = x[i] + y;
                return r;
            }
            /// <summary>
            /// Сумма комплексных векторов
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Complex[] Sum(Complex[] x, Complex[] y)
            {
                Complex[] r = new Complex[x.Length];
                for (int i = 0; i < r.Length; i++)
                    r[i] = x[i] + y[i];
                return r;
            }
            /// <summary>
            /// Произведение комплексного вектора на число
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Complex[] Mult(Complex x, Complex[] y)
            {
                Complex[] r = new Complex[y.Length];
                for (int i = 0; i < r.Length; i++)
                    r[i] = x * y[i];
                return r;
            }
            /// <summary>
            /// Произведение действительного вектора на комплексное число
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static Complex[] Mult(Complex x, double[] y)
            {
                Complex[] r = new Complex[y.Length];
                for (int i = 0; i < r.Length; i++)
                    r[i] = x * y[i];
                return r;
            }
            /// <summary>
            /// Перевод действительного массива в конмплексный
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public static Complex[] ToComplexMas(double[] x)
            {
                Complex[] c = new Complex[x.Length];
                for (int i = 0; i < c.Length; i++)
                    c[i] = x[i];
                return c;
            }
            /// <summary>
            /// Сумма модулей массива
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public static double VectorNorm(Complex[] p)
            {
                double sum = 0;
                for (int i = 0; i < p.Length; i++) sum += p[i].Abs;
                return sum;
            }
            #endregion

            #region Функции комплексного переменного
            /// <summary>
            /// expi(x) = cos(x) + i sin(x)
            /// </summary>
            /// <param name="d"></param>
            /// <returns></returns>
            public static Complex Expi(double d) => new Complex(Math.Cos(d), Math.Sin(d));

            /// <summary>
            /// Комплексная экспонента
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Exp(Complex z) => Math.Exp(z.Re) * new Complex(Math.Cos(z.Im), Math.Sin(z.Im));
            
            /// <summary>
            /// Комплексный синус
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Sin(Complex z)=>(Exp(I * z) - Exp(-I * z)) / _2I;

            /// <summary>
            /// Комплексный косинус
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Cos(Complex z)
            {
                return (Exp(I * z) + Exp(-I * z)) / 2;
            }

            /// <summary>
            /// Многозначный радикал
            /// </summary>
            /// <param name="z"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public static Complex[] Radical(Complex z, int k)
            {
                Complex[] r = new Complex[k];
                double mod = Math.Pow(z.Abs, 1.0 / k);
                for (int i = 0; i < k; i++)
                    r[i] = mod * Exp(I * (z.Arg + _2PI * i) / k);
                return r;

            }
            /// <summary>
            /// Главное значение радикала
            /// </summary>
            /// <param name="z"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public static Complex Radicalk(Complex z, int k)=>Math.Pow(z.Abs, 1.0 / k) * Exp(I * z.Arg / k);

            /// <summary>
            /// Главное значение квадратного корня
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Sqrt(Complex z) => Math.Sqrt(z.Abs) * Exp(I * (z.Arg / 2.0));
            /// <summary>
            /// Главное значение квадратного корня, умноженное на sign(Im)
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex SqrtSig(Complex z) => Complex.Sqrt(z) * Math.Sign(z.Im);

            /// <summary>
            /// Поменять мнимую и действительную часть местами, выведя результат
            /// </summary>
            public Complex Swap => new Complex(this.Im, this.Re);

            public static Complex Sqrt1(Complex z) => new Complex(Math.Sqrt(z.Abs), 0);
            public static Complex Sqrt2(Complex z) => -I * Sqrt1(z);
            public static Complex SqrtNew(Complex z)
            {
                Complex tmp = Sqrt(z);
                if (tmp.Re >= 0 && tmp.Im <= 0) return tmp;
                // return tmp.Swap.Conjugate;
                return -I * tmp;
            }

            /// <summary>
            /// Возведение в степень
            /// </summary>
            /// <param name="z"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public static Complex Pow(Complex z, int k) => Math.Pow(z.Abs, k) * Exp(I * k * z.Arg);
            /// <summary>
            /// Возведение в степень
            /// </summary>
            /// <param name="z"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public static Complex Pow(Complex z, double k) => Math.Pow(z.Abs, k) * Exp(I * k * z.Arg);
            /// <summary>
            /// Гиперболический синус
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Sh(Complex z)=>0.5 * (Exp(z) - Exp(-z));
            /// <summary>
            /// Гиперболический косинус
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Ch(Complex z)=>0.5 * (Exp(z) + Exp(-z));

            /// <summary>
            /// Гиперболический котангенс
            /// </summary>
            /// <param name="z"></param>
            /// <returns></returns>
            public static Complex Cth(Complex z, double eps = 1e-15)
            {
                Complex tmp = Exp(-z);
                if (tmp.Abs <= eps) return 1.0;
                Complex e = Exp(z);
                return (e + tmp) / (e - tmp);
            }
            #endregion

            public int CompareTo(object obj) => CompareTo((Complex)obj);
            public int CompareTo(Complex c)
            {
                Point a = this;
                Point b = c;
                return a.CompareTo(b);
            }

            /// <summary>
            /// Способ отображения комплексного числа в действительное
            /// </summary>
            public enum ComplMode : byte
            {
                Re,
                Im,
                Abs,
                Arg
            }
            /// <summary>
            /// Сумма действительной и мнимой части
            /// </summary>
            public double ReIm => this.Re + this.Im;


            public void FastAdd(Complex c)
            {
                this.Re += c.Re;
                this.Im += c.Im;
            }
            public void FastLessen(Complex c)
            {
                this.Re -= c.Re;
                this.Im -= c.Im;
            }

            /// <summary>
            /// Заменить комплексное число на другое
            /// </summary>
            /// <param name="t"></param>
            public void MoveTo(Complex t)
            {
                Re = t.Re;
                Im = t.Im;
            }
        }
    }
}

