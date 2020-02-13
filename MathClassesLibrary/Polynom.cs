using System;
using System.Linq;
using System.IO;

namespace МатКлассы
{
    /// <summary>
    /// Класс полиномов вида a+bx+...
    /// </summary>
    public class Polynom:Idup<Polynom>
    {
        /// <summary>
        /// Степень полинома
        /// </summary>
        private int? degree;
        /// <summary>
        /// Массив коэффициентов полинома в порядке возрастания степеней
        /// </summary>
        public double[] coef;
        /// <summary>
        /// Массив точек, через которые проходит полином
        /// </summary>
        public Point[] points;

        //конструкторы
        /// <summary>
        /// Никакой полином
        /// </summary>
        public Polynom() { degree = null; coef = null; points = null; }//по умолчанию (хотя зачем?)
                                                                       /// <summary>
                                                                       /// Конструктор копирования
                                                                       /// </summary>
                                                                       /// <param name="p"></param>
        public Polynom(Polynom p)
        {
            this.degree = p.degree;
            this.coef = new double[p.coef.Length];
            this.points = new Point[p.coef.Length];//p.SavePoints();
            for (int i = 0; i <= p.degree; i++) { this.coef[i] = p.coef[i]; /*this.points[i] = p.points[i]; */}
        }
        /// <summary>
        /// Задать полином через массив коэффициентов
        /// </summary>
        /// <param name="c"></param>
        public Polynom(double[] c)
        {
            this.degree = c.Length - 1;
            this.coef = new double[c.Length];
            for (int i = 0; i <= this.degree; i++) this.coef[i] = c[i];
        }
        /// <summary>
        /// Задать полином по вектору коэффициентов
        /// </summary>
        /// <param name="v"></param>
        public Polynom(Vectors v)
        {
            this.degree = v.Deg - 1;
            this.coef = new double[v.Deg];
            for (int i = 0; i <= this.degree; i++) this.coef[i] = v[i];
        }
        /// <summary>
        /// Прочитать массив коэффициентов полинома из файла и задать полином
        /// </summary>
        /// <param name="fs"></param>
        public Polynom(StreamReader fs)
        {
            string s = fs.ReadToEnd();
            fs.Close();
            string[] st = s.Split(' ');//в аргументах указывается массив символов, которым разделяются числа

            this.degree = st.Length - 1;
            this.coef = new double[st.Length];
            for (int i = 0; i <= this.degree; i++) this.coef[i] = Convert.ToDouble(st[i]);
        }
        /// <summary>
        /// Нулевой полином нужной степени
        /// </summary>
        /// <param name="deg"></param>
        public Polynom(int deg)
        {
            this.degree = deg;
            this.coef = new double[deg + 1];
        }
        /// <summary>
        /// Приведённый одночлен по единственному корню (то есть 1(х-х0))
        /// </summary>
        /// <param name="x">Корень одночлена</param>
        public Polynom(double x)
        {
            this.degree = 1;
            this.coef = new double[2];
            //коэффициенты для приведённого полинома первой степени
            this.coef[0] = -x;
            this.coef[1] = 1;
        }
        /// <summary>
        /// Задать полином по старшему коэффициенту и набору его корней
        /// </summary>
        /// <param name="aN"></param>
        /// <param name="x"></param>
        public Polynom(double aN, params double[] x)//суть метода: последовательно умножать многочлен на одночлены
        {
            this.degree = (x.Length);
            this.coef = new double[x.Length + 1];
            //коэффициенты для приведённого полинома первой степени
            this.coef[0] = -x[0];
            this.coef[1] = 1;
            if (x.Length > 1)
            {//вычисление коэффициентов полинома степени k
             //Console.WriteLine("{0}", this.coef[0]);
                for (int k = 2; k <= this.degree; k++)
                {
                    this.coef[k] = this.coef[k - 1];
                    for (int i = k - 1; i > 0; i--) this.coef[i] = this.coef[i - 1] - this.coef[i] * x[k - 1];
                    this.coef[0] *= -x[k - 1];
                }
            }
            //умножение на старший коэффициент
            for (int i = 0; i <= this.degree; i++) this.coef[i] *= aN;
        }
        /// <summary>
        /// Создание полинома по старшему коэффициенту и вектору корней
        /// </summary>
        /// <param name="aN"></param>
        /// <param name="x"></param>
        public Polynom(double aN, Vectors x)
        {
            double[] root = Vectors.ToDoubleMas(x);
            Polynom p = new Polynom(aN, root);
            this.degree = p.degree;
            this.coef = new double[p.coef.Length];
            this.points = new Point[p.coef.Length];//p.SavePoints();
            for (int i = 0; i <= p.degree; i++) this.coef[i] = p.coef[i];
        }
        /// <summary>
        /// Полином (Лагранжа), проходящий через точки массива
        /// </summary>
        /// <param name="p"></param>
        public Polynom(Point[] p)//интерполяционных полином Лагранжа, проходящий через точки из массива p; представляется как сумма n+1 полиномов Pk, для которых известны корни  
        {
            this.degree = p.Length - 1;
            this.coef = new double[p.Length];
            Polynom PL = new Polynom((int)this.degree);//конечный полином
            double[] roots = new double[(int)this.degree];//массив корней

            if (p.Length == 1) PL = Polynom.ToPolynom(p[0].y);//если узел один
            else
            {
                for (int k = 0; k <= this.degree; k++)
                {   //задание корней полиномов Pk
                    for (int i = 0; i < k; i++) roots[i] = p[i].x;
                    for (int i = k + 1; i <= this.degree; i++) roots[i - 1] = p[i].x;

                    Polynom c = new Polynom(1, roots);//приведённый полином

                    double An = p[k].y / c.Value(p[k].x);//вычисление старшего коэффициента
                    Polynom Pk = new Polynom(An, roots);//создание полинома Pk
                    PL += Pk;//прибавление к общему
                }
            }
            this.points = new Point[this.coef.Length];
            for (int k = 0; k <= this.degree; k++) { this.coef[k] = PL.coef[k]; this.points[k] = p[k]; }
        }
        /// <summary>
        /// Интерполяционный полином функции f с n+1 узлами интерполяции (значит, n-й степени) на отрезке от a до b
        /// </summary>
        /// <param name="f">Интерполируемая функция</param>
        /// <param name="n">Степень полинома</param>
        /// <param name="a">Начало отрезна интерполирования</param>
        /// <param name="b">Конец отрезка интерполирования</param>
        public Polynom(Func<double,double> f, int n, double a, double b)
        {
            Polynom p = new Polynom(Point.Points(f, n, a, b));
            this.degree = p.degree;
            this.coef = new double[(int)(p.degree + 1)];
            for (int k = 0; k <= this.degree; k++) this.coef[k] = p.coef[k];
        }

        public Polynom dup => new Polynom(this);


        //операции
        public static Polynom operator +(Polynom a, Polynom b)//сложение полиномов
        {
            int degree = Math.Max((sbyte)a.degree, (sbyte)b.degree);
            double[] coef = new double[degree + 1];
            for (int i = 0; i <= Math.Min((sbyte)a.degree, (sbyte)b.degree); i++) coef[i] = a.coef[i] + b.coef[i];
            for (int i = Math.Min((sbyte)a.degree, (sbyte)b.degree) + 1; i <= degree; i++)
            {
                if (a.degree > b.degree) coef[i] = a.coef[i];
                else coef[i] = b.coef[i];
            }
            return new Polynom(coef);
        }
        public static Polynom operator +(Polynom a, double Ch) { return a + ToPolynom(Ch); }
        public static Polynom operator -(Polynom a)//унарная операция -
        {
            Polynom p = new Polynom(a);
            for (int i = 0; i <= p.degree; i++) p.coef[i] *= -1;
            return p;
        }
        public static Polynom operator -(Polynom a, Polynom b)//разность полиномов
        {
            return new Polynom(a + (-b));
        }
        public static Polynom operator *(Polynom a, Polynom b)//произведение полиномов
        {
            int degree = (int)(a.degree + b.degree);
            double[] coef = new double[degree + 1];
            for (int i = 0; i <= degree; i++)
            {
                double s = 0;
                for (int k = 0; k <= i; k++) if ((k <= a.degree) && ((i - k) <= b.degree)) s += a.coef[k] * b.coef[i - k];
                coef[i] = s;
            }
            return new Polynom(coef);
        }
        public static Polynom operator *(Polynom t, double x)//произведение полинома с числом
        {
            Polynom e = new Polynom(t);
            for (int i = 0; i <= t.degree; i++) e.coef[i] *= x;
            return e;
        }
        public static Polynom operator *(double x, Polynom t)
        {
            return t * x;
        }
        public static Polynom operator /(Polynom t, double x)//деление полинома на число
        {
            return t * (1.0 / x);
        }
        public static Polynom operator /(Polynom end, Polynom er)//деление полиномов "нацело"
        {
            double[] quotient, remainder;
            Deconv(end.coef, er.coef, out quotient, out remainder);
            return new Polynom(quotient);
        }
        public static Polynom operator %(Polynom end, Polynom er)//остаток от деления полиномов
        {
            double[] quotient, remainder;
            Deconv(end.coef, er.coef, out quotient, out remainder);
            return new Polynom(remainder);
        }
        public static Polynom operator |(Polynom p, int k)//производная полинома
        {
            if (k >= p.degree + 1) return Polynom.ToPolynom(0);
            if (k == 0) return p;

            double[] d = new double[(int)p.degree - k + 1];
            if (k > 0)
                for (int i = 0; i < d.Length; i++)
                    d[i] = p.coef[k + i] * Combinatorik.A(i, k + i);
            else
                for (int i = 0; i < p.coef.Length; i++)
                    d[-k + i] = p.coef[i] / Combinatorik.A(i, -k + i);

            return new Polynom(d);
        }
        public static bool operator !=(Polynom p, Polynom q)
        {
            if (p.degree != q.degree) return true;
            for (int i = 0; i <= p.degree; i++) if (p.coef[i] != q.coef[i]) return true;
            return false;
        }
        public static bool operator ==(Polynom p, Polynom q) { return !(p != q); }

        public override bool Equals(object obj)
        {
            return this == (Polynom)obj;
        }

        public override int GetHashCode()
        {
            double s = 0;
            for (int i = 0; i <= this.degree; i++) s += this.coef[i];
            return (int)s;
        }


        //методы
        private static void Deconv(double[] dividend, double[] divisor, out double[] quotient, out double[] remainder)//деление многочлена на многочлен с выводом остатка и частного
        {
            if (dividend.Last() == 0)
            {
                throw new ArithmeticException("Старший член многочлена делимого не может быть 0");
            }
            if (divisor.Last() == 0)
            {
                throw new ArithmeticException("Старший член многочлена делителя не может быть 0");
            }
            remainder = (double[])dividend.Clone();
            quotient = new double[remainder.Length - divisor.Length + 1];
            for (int i = 0; i < quotient.Length; i++)
            {
                double coeff = remainder[remainder.Length - i - 1] / divisor.Last();
                quotient[quotient.Length - i - 1] = coeff;
                for (int j = 0; j < divisor.Length; j++)
                {
                    remainder[remainder.Length - i - j - 1] -= coeff * divisor[divisor.Length - j - 1];
                }
            }
        }
        /// <summary>
        /// Сохранить в массив точки, через которые проходит полином
        /// </summary>
        private void SavePoints()
        {
            this.points = new Point[this.coef.Length];//Console.WriteLine(this.points[1].x);
            double h = 20.0 / (this.points.Length);
            for (int i = 0; i < this.points.Length; i++)
            {
                this.points[i] = new Point(-10 + h * i, this.Value(-10 + h * i));
                //this.points[i].x = -10 + h * i;
                //this.points[i].y = this.Value(this.points[i].x);
            }
        }
        /// <summary>
        /// Разделённая разность без рекурсии
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static double W(Point[] p)
        {
            double sum = 0, pow = 1;
            for (int j = 0; j < p.Length; j++)
            {
                for (int l = 0; l < p.Length; l++)
                    if (j != l) pow *= p[j].x - p[l].x;
                sum += p[j].y / pow;
                pow = 1;
            }
            return sum;
        }
        /// <summary>
        /// Разделённая разность по массиву точек (с рекурсией)
        /// </summary>
        /// <param name="p">Массив точек</param>
        /// <param name="i">Номер начального элемента в разности</param>
        /// <param name="j">Номер конечного элемента в разности</param>
        /// <returns></returns>
        internal static double W(Point[] p, int i, int j)
        {
            if (j - i == 1) return (p[j].y - p[i].y) / (p[j].x - p[i].x);
            return (W(p, i + 1, j) - W(p, i, j - 1)) / (p[j].x - p[i].x);
        }

        /// <summary>
        /// Вычисление значения в точке по схеме Горнера
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Value(double x)//вычисление значения в точке по схеме Горнера
        {
            double sum = this.coef[(int)this.degree];
            if (this.degree > 0)
            {
                for (int i = (int)(this.degree - 1); i >= 0; i--)
                {
                    sum *= x;
                    sum += this.coef[i];
                }
            }

            return sum;
        }
        /// <summary>
        /// Полином от матрицы
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public SqMatrix Value(SqMatrix A)
        {
            SqMatrix I = SqMatrix.I(A.n);
            SqMatrix sum = this.coef[(int)this.degree] * I;
            if (this.degree > 0)
            {
                for (int i = (int)(this.degree - 1); i >= 0; i--)
                {
                    sum *= A;
                    sum += this.coef[i] * I;
                }
            }

            return sum;
        }
        /// <summary>
        /// Полином от полинома
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public Polynom Value(Polynom A)
        {
            Polynom sum = Polynom.ToPolynom(this.coef[(int)this.degree]);
            if (this.degree > 0)
            {
                for (int i = (int)(this.degree - 1); i >= 0; i--)
                {
                    sum *= A;
                    sum += this.coef[i];
                }
            }

            return sum;
        }

        /// <summary>
        /// Вывод полинома на консоль
        /// </summary>
        public void Show() { Console.WriteLine(this.ToString()); }
        /// <summary>
        /// Вывод полинома с рациональными коэффициентами на консоль
        /// </summary>
        public void ShowRational() { Console.WriteLine(this.ToStringRational()); }
        /// <summary>
        /// Преобразование полинома в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "";
            for (int i = (int)(this.degree); i > 0; i--)
            {
                s += String.Format("{0}x^{1} + ", this.coef[i], i);
            }
            s += String.Format("{0}"/*x^{1}*/, this.coef[0]/*, 0*/);
            return s;
        }
        /// <summary>
        /// Преобразовать полином в строку с отображением рациональных коэффициентов
        /// </summary>
        /// <returns></returns>
        public string ToStringRational()
        {
            string s = "";
            for (int i = (int)(this.degree); i > 0; i--)
            {
                s += String.Format("({0}x^{1}) + ", Number.Rational.ToRational(this.coef[i]), i);
            }
            s += String.Format("{0}"/*x^{1}*/, Number.Rational.ToRational(this.coef[0])/*, 0*/);
            return s;
        }
        /// <summary>
        /// Приведённый полином по текущему полиному
        /// </summary>
        /// <returns></returns>
        public Polynom ToLeadPolynom()
        {
            Polynom p = new Polynom(this);
            double tmp = p.coef[(int)p.degree];
            for (int i = (int)p.degree; i >= 0; i--) p.coef[i] /= tmp;
            return p;
        }
        /// <summary>
        /// Приведённый полином такого-то полинома
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Polynom ToLeadPolynom(Polynom p) { return p.ToLeadPolynom(); }
        /// <summary>
        /// Перевод числа в полином нулевой степени с соответствующим коэффициентом
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Polynom ToPolynom(double x) //перевод числа в полином нулевой степени с соответствующим коэффициентом
        {
            Polynom p = new Polynom(0);
            if (x == 0) return p;
            p.coef[0] = x;
            return p;
        }
        /// <summary>
        /// Перевод массива чисел в полином
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Polynom ToPolynom(double[] x) { return new Polynom(x); }
        /// <summary>
        /// Полином, близкий к производной функции f порядка k
        /// </summary>
        /// <param name="f"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Polynom Derivative(Func<double,double> f, int n, double a, double b, int k)
        {
            Polynom p = new Polynom(f, n + k, a, b);
            return p | k;
        }
        /// <summary>
        /// Характеристический многочлен заданной матрицы
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static Polynom CharactPol(SqMatrix M)
        {
            Polynom p = new Polynom(M.n);

            double sum;
            SqMatrix A =new SqMatrix( M);
            //заполнение массива треков
            double[] tr = new double[A.n];
            for (int i = 0; i < A.n; i++)
            {
                tr[i] = A.Track;
                A *= M;
            }

            p.coef[(int)p.degree] = 1 * Math.Pow(-1, A.n);
            int k = 0;
            for (int i = (int)p.degree - 1; i >= 0; i--)
            {
                sum = 0; k++;
                for (int j = 0; j < k; j++) sum += tr[k - j - 1] * p.coef[(int)p.degree - j];
                sum *= -1;
                sum /= k;
                p.coef[i] = sum;
            }

            return p;
        }
        /// <summary>
        /// Добавить ещё одну точку, через которую проходит полином (методами Ньютона)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Polynom AddPoint(Point a)
        {
            Polynom p = new Polynom(this);
            if (p.Value(a.x) == a.y) return p;

            Point[] mas = new Point[p.points.Length + 1];
            double[] xmas = new double[mas.Length - 1];
            for (int i = 0; i < mas.Length - 1; i++)
            {
                mas[i] = p.points[i];
                xmas[i] = mas[i].x;
            }
            mas[mas.Length - 1] = a; //Point.Show(mas);

            Polynom w = new Polynom(Polynom.W(mas), xmas);
            Polynom q = p + w;
            q.points = new Point[mas.Length]; for (int i = 0; i < mas.Length; i++) q.points[i] = mas[i];
            return q;
        }
        /// <summary>
        /// Точное вычисление определённого интеграла по формуле Ньютона-Лейбница
        /// </summary>
        /// <param name="a">Начало отрезка интегрирования</param>
        /// <param name="b">Конец отрезка интегрирования</param>
        /// <returns></returns>
        public double S(double a, double b)
        {
            Polynom p = this | -1;
            return p.Value(b) - p.Value(a);
        }

        //специальные полиномы
        /// <summary>
        /// Интерполяционных полином Лагранжа, проходящий через точки из массива p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Polynom Lag(Point[] p) { return new Polynom(p); }
        /// <summary>
        /// Интерполяционный полином функции f с n+1 узлами интерполяции (значит, n-й степени) на отрезке от a до b
        /// </summary>
        /// <param name="f"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Polynom Lag(Func<double,double> f, int n, double a, double b) { return new Polynom(f, n, a, b); }
        /// <summary>
        /// Вывод полинома Чебышёва соответсвующего рода и степени
        /// </summary>
        /// <param name="r"></param>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static Polynom Cheb(int deg, Kind r = Kind.FirstKind)//полиномы Чебышёва; можно уменьшить код, так как отличаются только полиномы степени 1, но тогда что делать с проверкой на род, которых может быть больше двух?
        {
            if (r == Kind.FirstKind)
            {
                switch (deg)
                {
                    case 0:
                        return Polynom.ToPolynom(1);
                    case 1:
                        return new Polynom(0.0);
                    default:
                        Polynom p = (new Polynom(0.0)) * 2;
                        return p * Polynom.Cheb(deg - 1, r) - Polynom.Cheb(deg - 2, r);
                }
            }
            if (r == Kind.SecondKind)
            {
                switch (deg)
                {
                    case 0:
                        return Polynom.ToPolynom(1);
                    case 1:
                        return (new Polynom(0.0)) * 2;
                    default:
                        Polynom p = (new Polynom(0.0)) * 2;
                        return p * Polynom.Cheb(deg - 1, r) - Polynom.Cheb(deg - 2, r);
                }
            }
            return Polynom.ToPolynom(0);
        }
        /// <summary>
        /// Полином Лежандра
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static Polynom Lezh(int deg)
        {
            switch (deg)
            {
                case 0:
                    return Polynom.ToPolynom(1);
                case 1:
                    return new Polynom(0.0);
                default:
                    Polynom p = (new Polynom(0.0));
                    return p * Polynom.Lezh(deg - 1) * (2 * deg - 1) / deg - Polynom.Lezh(deg - 2) * (deg - 1) / deg;
            }
        }
        /// <summary>
        /// Полиномы Лагерра
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static Polynom Lagerr(int deg)
        {
            switch (deg)
            {
                case 0:
                    return Polynom.ToPolynom(1);
                case 1:
                    return new Polynom(new double[] { 1, -1 });
                default:
                    Polynom p = new Polynom(new double[] { 2 * (deg - 1) + 1, -1 });
                    return p * Polynom.Lagerr(deg - 1) - Polynom.Lagerr(deg - 2) * (deg - 1) * (deg - 1);
            }
        }
        /// <summary>
        /// Полиномы Эрмита (ортогональные)
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static Polynom Hermit(int deg)
        {
            switch (deg)
            {
                case 0:
                    return Polynom.ToPolynom(1);
                case 1:
                    return new Polynom(new double[] { 0, 2 });
                default:
                    Polynom p = new Polynom(new double[] { 0, 2 });
                    return p * Polynom.Hermit(deg - 1) - Polynom.Hermit(deg - 2) * (deg - 1) * 2;
            }
        }
        /// <summary>
        /// Полином Ньютона через разделённые разности по массиву точек
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public static Polynom Neu(Point[] P/*,out Polynom[] np*/)
        {
            Polynom pol = Polynom.ToPolynom(P[0].y);//Первый элемент суммы полиномов
                                                    //np = new Polynom[P.Length];

            double[][] mas = new double[P.Length][];
            for (int i = 0; i < P.Length; i++)
            {
                mas[i] = new double[i + 1];
                for (int j = 0; j <= i; j++) mas[i][j] = P[j].x;//Заполнить массив массивов корней
            }
            for (int i = 0; i < P.Length - 1; i++)
            {
                pol += new Polynom(W(P, 0, i + 1), mas[i]);//Просуммировать полиномы
                                                           //np[i] = new Polynom(pol);
            }

            return pol;
        }

        /// <summary>
        /// Полином Ньютона через разделённые разности по массиву точек
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        public static Polynom NeuNew(Point[] P, out Polynom[] np, out Polynom[] pn)
        {
            Polynom pol = Polynom.ToPolynom(P[0].y);//Первый элемент суммы полиномов
            np = new Polynom[P.Length];
            pn = new Polynom[P.Length];
            np[0] = new Polynom(pol);
            //pn[pn.Length-1]= new Polynom(pol);


            double[][] mas = new double[P.Length][];
            for (int i = 0; i < P.Length; i++)
            {
                mas[i] = new double[i + 1];
                for (int j = 0; j <= i; j++) mas[i][j] = P[j].x;//Заполнить массив массивов корней
            }
            for (int i = 0; i < P.Length - 1; i++)
            {
                pol += new Polynom(W(P, 0, i + 1), mas[i]);//Просуммировать полиномы
                np[i + 1] = new Polynom(pol);
            }

            pn[pn.Length - 1] = new Polynom(W(P, 0, P.Length - 1), mas[P.Length - 2]);
            for (int i = P.Length - 3; i >= 0; i--) pn[i + 1] = pn[i + 1 + 1] + new Polynom(W(P, 0, i + 1), mas[i]);
            pn[0] = pn[1] + np[0];
            //np[P.Length - 1] = new Polynom(pol);

            return pol;
        }

        /// <summary>
        /// Объект для хранения вспомогательной системы
        /// </summary>
        public static SLAU syst = null;
        /// <summary>
        /// Строковое представление рациональной функции
        /// </summary>
        public static string Rat = null;
        /// <summary>
        /// Интерполяция рациональной функцией по точкам
        /// </summary>
        /// <param name="P">Массив точек</param>
        /// <param name="p">Степень полинома в числителе</param>
        /// <param name="q">Степень полинома в знаменателе</param>
        /// <param name="bq">Старший коэффициент в знаменателе</param>
        /// <returns></returns>
        public static Func<double,double> R(Point[] P, int p, int q, double bq = 1)
        {
            if (p + q + 1 != P.Length) throw new Exception("Не выполняется равенство p+q+1=n !");
            if (bq == 0) throw new Exception("Старший коэффициент полинома не может быть нулевым!");

            Matrix M = new Matrix(P.Length, P.Length + 1);
            for (int i = 0; i < M.n; i++)
            {
                M[i, P.Length] = P[i].y * bq * Math.Pow(P[i].x, q);
                for (int j = 0; j <= p; j++) M[i, j] = Math.Pow(P[i].x, j);
                for (int j = p + 1; j < M.m - 1; j++) M[i, j] = -P[i].y * Math.Pow(P[i].x, j - p - 1);
            }

            SLAU S = new SLAU(M);
            //S.Show();
            S.Gauss();
            syst = S;
            //S.Show();
            Vectors numerator = new Vectors(p + 1);
            Vectors denominator = new Vectors(q + 1);
            for (int i = 0; i <= p; i++) numerator[i] = S.x[i];
            for (int i = 0; i < q; i++) denominator[i] = S.x[p + 1 + i];
            denominator[q] = bq;
            Polynom num = new Polynom(numerator);
            Polynom den = new Polynom(denominator);
            //num.Show();den.Show();
            Rat = String.Format("({0}) / ({1})", num.ToString(), den.ToString());
            return (double x) => { return num.Value(x) / den.Value(x); };
        }

        /// <summary>
        /// Строковое представление сплайна (массив полиномов)
        /// </summary>
        public static string[] SplinePol = null;
        /// <summary>
        /// Первая и вторая производные сплайна
        /// </summary>
        public static Func<double,double> DSpline = null, D2Spline = null;
        /// <summary>
        /// Максимальный шаг между двумя соседними точками при интерполяции сплайном
        /// </summary>
        public static double hmax;
        /// <summary>
        /// Интерполяция кубическими сплайнами дефекта 1 по массиву точек
        /// </summary>
        /// <param name="P"></param>
        /// <param name="a">Граничное условие в начале отрезка</param>
        /// <param name="b">Граничное условие в конце отрезка</param>
        /// <param name="is0outcut">Должен ли сплайн равняться 0 вне отрезка задания</param>
        /// <returns></returns>
        public static Func<double,double> CubeSpline(Point[] P, double a = 0, double b = 0, bool is0outcut = false)
        {
            int n = P.Length - 1;//записать в новую переменную для облегчения
            double[] h = new double[n + 1];
            double[] y = new double[n + 1];

            hmax = P[1].x - P[0].x;

            for (int i = 1; i <= n; i++)
            {
                h[i] = P[i].x - P[i - 1].x;//Заполнение массива длин отрезков
                if (h[i] > hmax) hmax = h[i];
                y[i] = P[i].y - P[i - 1].y;
            }

            //создание, заполнение и решение системы с трёхдиагональной матрицей
            SLAU S = new SLAU(n + 1);
            S.A[0, 0] = -4.0 / h[1]; S.A[0, 1] = -2.0 / h[1]; S.b[0] = -6.0 * y[1] / (h[1] * h[1]) + a;
            S.A[n, n - 1] = 2.0 / h[n]; S.A[n, n] = 4.0 / h[n]; S.b[n] = 6.0 * y[n] / (h[n] * h[n]) + b;
            for (int i = 1; i <= n - 1; i++)
            {
                S.A[i, i - 1] = 1.0 / h[i];
                S.A[i, i] = 2 * (1.0 / h[i] + 1.0 / h[i + 1]);
                S.A[i, i + 1] = 1.0 / h[i + 1];
                S.b[i] = 3 * (y[i] / h[i] / h[i] + y[i + 1] / h[i + 1] / h[i + 1]);
            }
            //S.b[0] = a;
            //S.b[n] = b;
            S.ProRace();
            syst = S;
            //S.Show();

            //создание и заполнение массива полиномов
            Polynom[] mas = new Polynom[n + 1];
            Polynom.SplinePol = new string[n];
            for (int i = 1; i <= n; i++)
            {
                Polynom p1, p2, p3, p4;
                p1 = (new Polynom(1, P[i].x, P[i].x)) * (2 * new Polynom(P[i - 1].x) + h[i]) / Math.Pow(h[i], 3) * P[i - 1].y;
                p2 = (new Polynom(1, P[i - 1].x, P[i - 1].x)) * (-2 * new Polynom(P[i].x) + h[i]) / Math.Pow(h[i], 3) * P[i].y;
                p3 = (new Polynom(1, P[i].x, P[i].x)) * (new Polynom(P[i - 1].x)) / Math.Pow(h[i], 2) * S.x[i - 1];
                p4 = (new Polynom(1, P[i - 1].x, P[i - 1].x)) * (new Polynom(P[i].x)) / Math.Pow(h[i], 2) * S.x[i];
                mas[i] = p1 + p2 + p3 + p4;
                SplinePol[i - 1] = String.Format("[{0};{1}]: {2}", P[i - 1].x, P[i].x, mas[i].ToString());
                //mas[i].Show();
            }
            //mas[0] = mas[1];mas[n + 1] = mas[n];

            //создание производных сплайна
            Polynom[] mas1 = new Polynom[n + 1], mas2 = new Polynom[n + 1];
            for (int i = 1; i <= n; i++)
            {
                mas1[i] = mas[i] | 1;
                mas2[i] = mas1[i] | 1;
            }
            DSpline = (double x) =>
              {
                  if (x <= P[1].x) return mas1[1].Value(x);
                  if (x >= P[n].x) return mas1[n].Value(x);
                  int i = 1;
                  while (x > P[i].x) i++;
                  return mas1[i].Value(x);
              };
            D2Spline = (double x) =>
             {
                 if (x <= P[1].x) return mas2[1].Value(x);
                 if (x >= P[n].x) return mas2[n].Value(x);
                 int i = 1;
                 while (x > P[i].x) i++;
                 return mas2[i].Value(x);
             };

            //создание общей функции и вывод
            return (double x) =>
            {
                if (x <= P[1].x) return (is0outcut) ? 0 : mas[1].Value(x);
                if (x >= P[n].x) return (is0outcut) ? 0 : mas[n].Value(x);
                int i = 1;
                //while (x > P[i].x) i++;
                int i1 = 1, i2 = n;
                //реализация бинарного поиска
                while (i2 - i1 != 1)
                {
                    int tmp = Expendator.Average(i1, i2);
                    if (x > P[tmp].x) i1 = tmp;
                    else i2 = tmp;
                }
                i = i2;
                return mas[i].Value(x);
            };
        }
        /// <summary>
        /// Полиномы Эрмита для набора кратных узлов
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Polynom Hermit(params MultipleKnot[] mas)
        {
            int n = -1;//dergee of Pol
            for (int i = 0; i < mas.Length; i++) n += mas[i].Multiplicity;

            SLAU S = new SLAU(n + 1);//S.Show();
            int k = 0;
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = 0; j < mas[i].Multiplicity; j++)
                {
                    S.b[k + j] = mas[i].y[j];
                    for (int t = 0; t <= n - j; t++)
                    {
                        int s = n - j - t;
                        S.A[k + j, t] = Combinatorik.A(s, j + s) * Math.Pow(mas[i].x, s);
                    }
                }
                k += mas[i].Multiplicity;
            }
            S.GaussSelection();
            //S.Show();
            Array.Reverse(S.x);
            return new Polynom(S.x);
        }
        /// <summary>
        /// Оценка погрешности метода
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="Mn"></param>
        /// <returns></returns>
        private static double wn(Point[] p, double x, double Mn = 1)
        {
            double e = x - p[0].x;
            for (int i = 1; i < p.Length; i++) e *= (x - p[i].x);
            e /= Combinatorik.P(p.Length);
            e *= Mn;
            return Math.Abs(e);
        }
        /// <summary>
        /// Оценка погрешности метода
        /// </summary>
        /// <param name="f"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="Mn"></param>
        /// <returns></returns>
        private static double wn(Func<double,double> f, int n, double a, double b, double x, double Mn)
        {
            Point[] p = Point.Points(f, n, a, b);
            return wn(p, x, Mn);
        }
        /// <summary>
        /// Оценить погрешность интерполяционного полинома (Лагранжа) в точке при константе
        /// </summary>
        /// <param name="f">Интерполируемая функция</param>
        /// <param name="n">Стень полинома</param>
        /// <param name="a">Начало отрезка интерполирования</param>
        /// <param name="b">Конец отрезка интерполяции</param>
        /// <param name="x">Точка, в которой оценивается погрешность</param>
        /// <param name="Mn">Константа в погрешности</param>
        public static void LagEstimateErr(Func<double,double> f, int n, double a, double b, double x, double Mn = 0)
        {
            if (Mn <= 0)
            {
                double[] y = Point.PointsX(f, n, a, b);
                double q = Expendator.Min(y);
                double y1 = Math.Min(q, x);
                q = Expendator.Max(y);
                double y2 = Math.Max(q, x);
                //double e = y1 + (y2 - y1) / 2;//середина отрезка
                //Mn = Math.Abs(f(e));
                Mn = FuncMethods.RealFuncMethods.NormC(f, y1, y2);
            }
            Polynom p = new Polynom(f, n, a, b);

            Console.WriteLine("Узлы интерполяции: "); Point.Show(Point.Points(f, n, a, b));

            Console.WriteLine("Полученный полином: "); p.Show();
            Console.WriteLine($"Значение полинома в точке {x} = {p.Value(x)}");
            Console.WriteLine($"Значение функции в точке {x} = {f(x)}");
            double t = Math.Abs(p.Value(x) - f(x));
            Console.WriteLine($"Абсолютная величина погрешности в точке {x} = {t}");
            Console.WriteLine($"Оценка погрешности при Mn = {Mn}: {t} <= {wn(f, n, a, b, x, Mn)}");
        }
        /// <summary>
        /// Показать последовательно значение слагаемых в сумме полинома Ньютона в точке x
        /// </summary>
        /// <param name="h"></param>
        /// <param name="f"></param>
        /// <param name="x"></param>
        public static void ShowNeuNew(Point[] h, Func<double,double> f, double x)
        {
            Polynom[] u, v;
            Polynom r = Polynom.NeuNew(h, out u, out v);

            Console.WriteLine("Узлы интерполяции:");
            for (int i = 0; i < h.Length; i++)
                h[i].Show();
            Console.WriteLine();

            Console.WriteLine("Значение функции в точке {0} равно {1}", x, f(x));
            Console.WriteLine("Значение сумм полиномов (при суммировании от меньшего к большему):");
            for (int i = 0; i < h.Length; i++)
            {
                Console.WriteLine("P[сумма №{0}](в {1})= {2}", i + 1, x, u[i].Value(x));
            }
            Console.WriteLine("Значение сумм полиномов (при суммировании от большего к меньшему):");
            for (int i = 0; i < h.Length; i++)
            {
                //v[h.Length - 1 - i].Show();
                Console.WriteLine("P[сумма №{0}](в {1})= {2}", i + 1, x, v[h.Length - 1 - i].Value(x));
            }
        }

        /// <summary>
        /// Скалярное произведение между полиномами на отрезке
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ScalarP(Polynom p, Polynom q, double a, double b)
        {
            Polynom pol = p * q;
            double tmp = 1;
            if (b != a) { tmp /= Math.Abs(b - a); }
            return pol.S(a, b) * tmp;
        }
        /// <summary>
        /// Скалярное произведение между полиномами на отрезке
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double NormL(Polynom x, double a, double b) { return Math.Sqrt(ScalarP(x, x, a, b)); }
        /// <summary>
        /// Расстояние между полиномами на отрезке
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Distance(Polynom p, Polynom q, double a, double b) { return NormL(p - q, a, b); }

        /// <summary>
        /// Показать информацию о интерполяции указанной функции методами класса полиномов
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k">Число узлов интерполяции</param>
        /// <param name="a">Начало отрезка с узлами</param>
        /// <param name="b">Конец отрезка с узлами</param>
        /// <param name="p">Степень числителя у рациональной функции</param>
        /// <param name="q">Степень знаменателя у рациональной функции</param>
        /// <param name="bq">Старший коэффициент знаменателя рациональной функции</param>
        public static void PolynomTestShow(Func<double,double> f, int k, double a = -10, double b = 10, int p = 1, int q = -1, double bq = 1)
        {
            if (q == -1) q = k - 1 - p;
            Point[] P = Point.Points(f, k - 1, a, b);
            Console.WriteLine("Набор узлов интерполяции:"); Point.Show(P);
            Polynom l = Polynom.Lag(P);//l.Show();
            Polynom n = Polynom.Neu(P);
            Func<double,double> c = Polynom.CubeSpline(P);
            Func<double,double> r = Polynom.R(P, p, q, bq);

            Console.WriteLine("Погрешности в равномерной норме:");
            Console.WriteLine("У полинома Лагранжа {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, l.Value, a, b));
            Console.WriteLine("У полинома Ньютона {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, n.Value, a, b));
            Console.WriteLine("У рациональной функции {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, r, a, b));
            Console.WriteLine("У кубического сплайна {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, c, a, b));
            Console.WriteLine();
            Console.WriteLine("Погрешности в интегральной норме:");
            Console.WriteLine("У полинома Лагранжа {0}", FuncMethods.RealFuncMethods.NormDistance(f, l.Value, a, b));
            Console.WriteLine("У полинома Ньютона {0}", FuncMethods.RealFuncMethods.NormDistance(f, n.Value, a, b));
            Console.WriteLine("У рациональной функции {0}", FuncMethods.RealFuncMethods.NormDistance(f, r, a, b));
            Console.WriteLine("У кубического сплайна {0}", FuncMethods.RealFuncMethods.NormDistance(f, c, a, b));

        }
        /// <summary>
        /// Показать информацию о интерполяции указанной функции методами класса полиномов
        /// </summary>
        /// <param name="f"></param>
        /// <param name="c">Массив абцисс узлов интерполяции</param>
        /// <param name="a">Начало отрезка с узлами</param>
        /// <param name="b">Конец отрезка с узлами</param>
        /// <param name="p">Степень числителя у рациональной функции</param>
        /// <param name="q">Степень знаменателя у рациональной функции</param>
        /// <param name="bq">Старший коэффициент знаменателя рациональной функции</param>
        public static void PolynomTestShow(Func<double,double> f, double[] c, double a = 0, double b = 0, int p = 1, int q = -1, double bq = 1)
        {
            if (a == 0 && b == 0) { a = c[0]; b = c[c.Length - 1]; }
            if (q == -1) q = c.Length - 1 - p;
            Point[] P = Point.Points(f, c);
            Console.WriteLine("Набор узлов интерполяции:"); Point.Show(P);
            Polynom l = Polynom.Lag(P);//l.Show();
            Polynom n = Polynom.Neu(P);
            Func<double,double> cu = Polynom.CubeSpline(P);
            Func<double,double> r = Polynom.R(P, p, q, bq);

            Console.WriteLine("Погрешности в равномерной норме:");
            Console.WriteLine("У полинома Лагранжа {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, l.Value, a, b));
            Console.WriteLine("У полинома Ньютона {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, n.Value, a, b));
            Console.WriteLine("У рациональной функции {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, r, a, b));
            Console.WriteLine("У кубического сплайна {0}", FuncMethods.RealFuncMethods.NormDistanceС(f, cu, a, b));
            Console.WriteLine();
            Console.WriteLine("Погрешности в интегральной норме:");
            Console.WriteLine("У полинома Лагранжа {0}", FuncMethods.RealFuncMethods.NormDistance(f, l.Value, a, b));
            Console.WriteLine("У полинома Ньютона {0}", FuncMethods.RealFuncMethods.NormDistance(f, n.Value, a, b));
            Console.WriteLine("У рациональной функции {0}", FuncMethods.RealFuncMethods.NormDistance(f, r, a, b));
            Console.WriteLine("У кубического сплайна {0}", FuncMethods.RealFuncMethods.NormDistance(f, cu, a, b));

        }

        public void MoveTo(Polynom t)
        {
            for (int i = 0; i < this.coef.Length; i++)
                coef[i] = t.coef[i];
        }
    }
}

