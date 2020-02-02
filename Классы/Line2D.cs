using System;
using static МатКлассы.Number;

namespace МатКлассы
{
    /// <summary>
    /// Класс прямых на плоскости (Ax+By+C=0)
    /// </summary>
    public class Line2D
    {
        /// <summary>
        /// Коэффициент при X
        /// </summary>
        public double A = 0;
        /// <summary>
        /// Коэффициент при Y
        /// </summary>
        public double B = 0;
        /// <summary>
        /// Свободный коэффициент
        /// </summary>
        public double C = 0;

        /// <summary>
        /// Тип прямой
        /// </summary>
        public enum Type : byte { ParallelOx, ParallelOy, Other }
        /// <summary>
        /// Тип прямой
        /// </summary>
        public Type LineType
        {
            get
            {
                if (A == 0) return Type.ParallelOx;
                if (B == 0) return Type.ParallelOy;
                else return Type.Other;
            }
        }
        /// <summary>
        /// Тип взаимного отношения прямых
        /// </summary>
        public enum Mode : byte { Parallel, Perpendicular }
        /// <summary>
        /// Тип уравнения прямой
        /// </summary>
        public enum EquType : byte { Normal, Other }
        /// <summary>
        /// Тангенс угла наклона
        /// </summary>
        public double Corner_k
        {
            get
            {
                if (LineType == Type.ParallelOx) return 0;
                if (LineType == Type.ParallelOy) return Double.PositiveInfinity;
                return -A / B;
            }
        }

        /// <summary>
        /// Задать прямую по её коэффициентам из уравнения ax+by+c=0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Line2D(double a, double b, double c, EquType t = EquType.Other)
        {
            A = a;
            B = b;
            C = c;
            Modif(t);
        }
        /// <summary>
        /// Задать прямую проходящую через две точки
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Line2D(Point p1, Point p2, EquType t = EquType.Other) : this(p2.y - p1.y, p1.x - p2.x, p1.y * (p2.x - p1.x) - p1.x * (p2.y - p1.y), t) { }
        /// <summary>
        /// Задать прямую, находящуюся относительно указанной прямой и указанной точки в заданном соотношении
        /// </summary>
        /// <param name="line"></param>
        /// <param name="p"></param>
        /// <param name="m"></param>
        public Line2D(Line2D line, Point p, Mode m, EquType t = EquType.Other)
        {
            if (m == Mode.Parallel)
            {
                A = line.A; B = line.B;
                C = -A * p.x - B * p.y;
            }
            else
            {
                A = -line.B;
                B = line.A;
                C = -line.A * p.y + line.B * p.x;
            }
            Modif(t);
        }
        /// <summary>
        /// Задать прямую через коэффициенты уравнения из y=ax+b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Line2D(double a, double b, EquType t = EquType.Other) : this(-a, 1, -b, t) { }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="l"></param>
        public Line2D(Line2D l) { A = l.A; B = l.B; C = l.C; }

        private void Modif(EquType t = EquType.Other)
        {
            if (A == 0 && B == 0) throw new Exception("Главные коэффициенты оба равны 0!");
            if (A <= 0 && B <= 0 && C <= 0) { A *= -1; B *= -1; C *= -1; }
            if (t == EquType.Other)
            {
                if (LineType == Type.Other) GiveDivisionByNOD();
            }
            else
            {
                double d = new Complex(A, B).Abs;
                A /= d; B /= d; C /= d;
            }


        }
        private void GiveDivisionByNOD()
        {
            int a = A.DimOfFractionalPath(), b = B.DimOfFractionalPath(), c = C.DimOfFractionalPath();
            int max = Expendator.Max(a, b, c);
            for (int i = 0; i < max; i++)
            {
                A *= 10; B *= 10; C *= 10;
            }
            max = (int)Number.Rational.Nod((long)A, (long)B);
            A /= max;
            B /= max;
            C /= max;
        }

        /// <summary>
        /// Функция, соответствующая прямой, от аргумента x, кроме того случая, когда B=0
        /// </summary>
        public Func<double,double> Func
        {
            get
            {
                if (A == 0) return (double x) => -C / B;
                if (B == 0) return (double x) => -C / A;
                return (double x) => -(A * x + C) / B;
            }
        }

        /// <summary>
        /// Точка пересечения двух прямых либо null, когда прямые не пересекаются или совпадают
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point? InterSecPoint(Line2D a, Line2D b)
        {
            if (IsParallel(a, b)) return null;
            SLAU s = new SLAU(new Matrix(new double[,] { { a.A, a.B, -a.C }, { b.A, b.B, -b.C } }));
            s.GaussSelection();
            return new Point(s.x[0], s.x[1]);
        }
        /// <summary>
        /// Угол между прямыми
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Corner(Line2D x, Line2D y)
        {
            if (IsPerpendicular(x, y)) return Math.PI / 2;
            return Math.Atan2(x.A * y.B - x.B * y.A, x.A * y.A + x.B * y.B);
        }
        /// <summary>
        /// Расстояние от точки до прямой
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double Distance(Point p) => form(p) / ((Complex)p).Abs;

        private double form(Point p) => A * p.x + B * p.y + C;

        public static bool IsPerpendicular(Line2D x, Line2D y) => x.A * y.A + x.B * y.B == 0;
        public static bool IsParallel(Line2D a, Line2D b) => a.A / b.A == a.B / b.B;
        public static bool operator ==(Line2D a, Line2D b)
        {
            return (a.A == b.A) && (a.B == b.B) && (a.C == b.C);
        }
        public static bool operator !=(Line2D a, Line2D b) => !(a == b);

        public override string ToString()
        {
            string b = (B < 0) ? "-" : "+", c = (C < 0) ? "-" : "+";
            string s = $"{A}x {b} {B.Abs()}y {c} {C.Abs()} = 0";
            return s;
        }

        public override bool Equals(object obj)
        {
            var d = obj as Line2D;
            return d != null &&
                   A == d.A &&
                   B == d.B &&
                   C == d.C;
        }

        public override int GetHashCode()
        {
            var hashCode = 793064651;
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            hashCode = hashCode * -1521134295 + C.GetHashCode();
            return hashCode;
        }
    }
}