using System;
using System.Collections.Generic;

namespace МатКлассы
{
    public static partial class Number
    {
        /// <summary>
        /// Рациональные числа (числа, представимые в виде m/n)
        /// </summary>
        public struct Rational : Idup<Rational>
        {
            public Rational dup => new Rational(this);

            /// <summary>
            /// Делимое
            /// </summary>
            public long Numerator { get; }
            /// <summary>
            /// Делитель
            /// </summary>
            public long Denominator { get; }
            /// <summary>
            /// Сообщает, равен н
            /// </summary>
            public bool IsNan => Denominator == 0;

            /// <summary>
            /// Ноль и единица во множестве рациональных чисел
            /// </summary>
            public static readonly Rational ZERO, ONE;

            #region Конструкторы
            static Rational()
            {
                ZERO = new Rational(0, 1);
                ONE = new Rational(1, 1);
            }

            /// <summary>
            /// Рациональное число по целому числу
            /// </summary>
            /// <param name="a"></param>
            public Rational(long a) { this.Numerator = a; this.Denominator = 1; }
            /// <summary>
            /// Несократимая дробь, эквивалентная частному аргументов
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            public Rational(long a, long b)
            {
                if (b == 0)
                {
                    Numerator = 0;
                    Denominator = 0;
                    return;
                }
                if (b < 0) { b = -b; a = -a; }
                long d = Nod(a, b); d = Math.Abs(d);
                Numerator = a / d; Denominator = b / d;
            }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="a"></param>
            public Rational(Rational a) { this.Denominator = a.Denominator; this.Numerator = a.Numerator; }
            /// <summary>
            /// Рациональное число по "действительному" числу
            /// </summary>
            /// <param name="x"></param>
            public Rational(double x)
            {
                Numerator = 0;
                Denominator = 0;
                ToRational(x);
            }
            #endregion

            #region Вспомогательные методы
            /// <summary>
            /// Наибольший общий делитель
            /// </summary>
            /// <param name="c"></param>
            /// <param name="d"></param>
            /// <returns></returns>
            public static long Nod(long c, long d)
            {
                long p = 0;
                if (c < 0) c = -c;
                if (d < 0) d = -d;
                do
                {
                    p = c % d; c = d; d = p;
                } while (d != 0);
                return c;
            }

            /// <summary>
            /// Перевести число в строку, где число имеет вид неправильной дроби
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (this.Denominator == 1) return this.Numerator.ToString();
                return $"{Numerator}/{Denominator}";
            }
            /// <summary>
            /// Привести число в строку, где оно имеет вид смешанной дроби
            /// </summary>
            /// <returns></returns>
            public string ToStringMixed()
            {
                string s;
                long k = this.Numerator / this.Denominator;
                Rational r = new Rational(this.Numerator - this.Denominator * k, this.Denominator);
                s = String.Format("{0} + {1}", k, r.ToString());
                return s;
            }
            /// <summary>
            /// Вывести смешанную дробь
            /// </summary>
            public void ShowMixed() { Console.WriteLine(this.ToStringMixed()); }

            /// <summary>
            /// Перевод десятичного числа в несократимую дробь
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public static Rational ToRational(double x)
            {
                if( Math.Truncate(x)==x) return new Rational((long)x);//если целое
                if (Math.Abs(x) <= 1e-15) return Rational.ZERO;
             
                string s = Math.Abs(x).ToString();
                int i = s.IndexOf(',');
                int n = s.Length - i;

                //если период возможен
                if (n >= 8)
                {
                    //---------Проверка на периодичность (полную)
                    Rational u = Rational.ToRational((long)x);
                    Rational z = new Rational(u.IntPart);//отделить целую часть

                    string mant = s.Substring(i + 1, n - 1);
                                                       
                    for (int beg = 0; beg <= n - 6; beg++)//если периоды проверять не с первого символа
                    {
                        int idx = 0;//индекс
                        int cnt = 0;//количество повторений подстроки
                        for (int k = 1; k < (n - beg) / 2 + 1;)//проход по подстрокам всех длин
                        {
                            for (int h = 0; h < n * n; h++)
                            {
                                idx = mant.IndexOf(mant.Substring(beg, k/*+beg*/), idx);
                                if (idx == -1) break;
                                else
                                {
                                    cnt += 1;
                                    idx += mant.Substring(beg, k/*+beg*/).Length;
                                }
                            }
                            if (k * cnt > 2.0 * (n - beg) / 3)//если нашёлся период
                            {
                                //mant = s.Substring(i++, s.Length);
                                long a, b;
                                if (beg > 0)
                                {
                                    a = Convert.ToInt64(mant.Substring(0, k + beg)) - Convert.ToInt64(mant.Substring(0, beg));
                                    b = (long)((Math.Pow(10, mant.Substring(beg, k /*+ beg*/).Length) - 1) * Math.Pow(10, mant.Substring(0, beg).Length));
                                }
                                else
                                {
                                    a = Convert.ToInt64(mant.Substring(0, k));
                                    b = (long)(Math.Pow(10, mant.Substring(beg, k /*+ beg*/).Length) - 1);
                                }
                                Rational r = new Rational(a, b);
                                if (x < 0) r = -r;
                                return z + r;
                            }
                            k++;
                            idx = 0;
                            cnt = 0;
                        }
                    }
                }

                //если периода нет
                return new Rational((long)(x * Math.Pow(10, n)), (long)Math.Pow(10, n));
            }
            /// <summary>
            /// Целая часть числа
            /// </summary>
            public long IntPart { get { return Rational.IntegerPart(this); } }
            /// <summary>
            /// Целая часть числа
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public static long IntegerPart(Rational t)
            {
                if (t.Denominator == 1) return t.Numerator;
                if (t.Numerator >= 0) return t.Numerator / t.Denominator;               
                return t.Numerator / t.Denominator - 1;
            }
            /// <summary>
            /// Дробная часть числа
            /// </summary>
            public Rational FracPart { get { return Rational.FractPart(this); } }
            /// <summary>
            /// Дробная часть числа
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public static Rational FractPart(Rational t) { return t - t.IntPart; }
            /// <summary>
            /// Является ли дробным
            /// </summary>
            /// <returns></returns>
            public static bool IsFractional(Rational r) { return !(r.FracPart == ZERO); }
            /// <summary>
            /// Является ли дробным
            /// </summary>
            /// <returns></returns>
            public bool IsFract() { return Rational.IsFractional(this); }

            /// <summary>
            /// Показать действительное число в виде смешанной дроби
            /// </summary>
            /// <param name="x"></param>
            public static void Show(double x) { ToRational(x).ShowMixed(); }
            /// <summary>
            /// Показать рациональное число в виде смешанной дроби
            /// </summary>
            /// <param name="x"></param>
            public static void Show(Rational x) { x.ShowMixed(); }
            /// <summary>
            /// Показать комплексное число с рациональными частями
            /// </summary>
            /// <param name="a"></param>
            public static void Show(Complex a) { Console.WriteLine("(" + ToRational(a.Re).ToStringMixed() + ") + (" + ToRational(a.Im).ToStringMixed() + ")i"); }

            public override bool Equals(object obj) => Equals((Rational)obj);

            public bool Equals(Rational rational)
            {
                return 
                       Numerator == rational.Numerator &&
                       Denominator == rational.Denominator;
            }

            public override int GetHashCode()
            {
                var hashCode = 893539880;
                hashCode = hashCode * -1521134295 + Numerator.GetHashCode();
                hashCode = hashCode * -1521134295 + Denominator.GetHashCode();
                hashCode = hashCode * -1521134295 + IntPart.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<Rational>.Default.GetHashCode(FracPart);
                return hashCode;
            }

            public void MoveTo(Rational t)
            {
            }
            #endregion

            #region Операторы
            public static Rational operator +(Rational a, Rational b) { return new Rational((a.Numerator * b.Denominator + a.Denominator * b.Numerator), (a.Denominator * b.Denominator)); }
            public static Rational operator -(Rational a) { return new Rational(-a.Numerator, a.Denominator); }
            public static Rational operator -(Rational a, Rational b) { return a + (-b); }
            public static Rational operator -(Rational a, long b)=> a - new Rational(b);    
            public static Rational operator *(Rational a, Rational b) { return new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator); }
            public static Rational operator /(Rational a, Rational b) { return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator); }
            public static bool operator ==(Rational a, Rational b) { return (a.Numerator == b.Numerator) && (a.Denominator == b.Denominator); }
            public static bool operator !=(Rational a, Rational b) { return !(a == b); }

            public static implicit operator double(Rational r) => ((double)r.Numerator) / r.Denominator;
            public static implicit operator Rational(long r) => new Rational(r);
            public static explicit operator int(Rational r)
            {
                if (r.Numerator == 0 && r.Denominator != 0) return 0;
                if (Math.Abs(r.Numerator) < r.Denominator) throw new Exception("Не конвертируется в int: числитель меньше знаменателя");
                if (Math.Abs(r.Numerator) % r.Denominator != 0) throw new Exception("Не конвертируется в int: числитель не делится на знаменатель нацело");
                return (int)(r.Numerator / r.Denominator);
            }
            #endregion
        }
    }
}

