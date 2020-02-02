using System;
using System.Collections.Generic;
using System.Linq;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using static Computator.NET.Core.NumericalCalculations.FunctionRoot;

namespace МатКлассы
{
    /// <summary>
    /// Класс методов поиска корней
    /// </summary>
    public static class Roots
    {
        /// <summary>
        /// Простой поиск корней комплексной функции на действительном отрезке методом дихотомии
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static Vectors MyHalfc(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12)
        {
            List<double> list = new List<double>();
            double a = beg, a2 = beg + step, b = end, t1, t2, s;
            Complex fa = f(a), fa2 = f(a2), fc;
            while (a < b)
            {
                if (fa.Abs < eps) list.Add(a);
                else
                if (Math.Sign(fa.Re) * Math.Sign(fa2.Re) <= 0 && Math.Sign(fa.Im) * Math.Sign(fa2.Im) <= 0)//написал условие именно так, чтобы избежать переполнения
                {
                    t1 = a; t2 = a2;
                    while (t2 - t1 > eps)
                    {
                        s = (t1 + t2) / 2;
                        fc = f(s);//fc.Show();
                        if (fc.Abs < eps)
                            break;
                        if (Math.Sign(fa.Re) * Math.Sign(fc.Re) <= 0 && Math.Sign(fa.Im) * Math.Sign(fc.Im) <= 0)
                            t2 = s;
                        else t1 = s;
                    }
                    s = (t1 + t2) / 2;
                    if (f(s).Abs < 3) list.Add(s);
                }
                a = a2;
                a2 += step;
                fa = new Complex(fa2);
                fa2 = f(a2);
            }
            return new Vectors(list.ToArray());
        }
        /// <summary>
        /// Простой поиск корней как поиск минимумов модуля функции (которые должны быть равны 0)
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static Vectors RootsByMinAbs(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12)
        {
            Func<double,double> fabs = (double c) => f(c).Abs;
            List<double> list = new List<double>();

            double a = beg, a2 = beg + step, t1, t2, s, d1, d2, ds;
            double fa = fabs(a), fa2 = fabs(a2), fc;

            double der(double c) => fabs(c + eps) - fabs(c - eps);

            while (a < end)
            {
                d1 = der(a);
                d2 = der(a2);
                //$"{d1} {d2}".Show();
                if (fa < eps) list.Add(a);
                else if (fa2 < eps) list.Add(a2);
                else if (d1 < 0 && d2 > 0)
                {
                    t1 = a; t2 = a2;
                    while (t2 - t1 > eps)
                    {
                        //$"{d1} {d2}".Show();
                        s = (t1 + t2) / 2;
                        fc = fabs(s);//fc.Show();
                        ds = der(s);
                        if (fc < eps) break;
                        if (ds > 0)
                        { t2 = s; d2 = ds; }
                        else { t1 = s; d1 = ds; }
                    }
                    s = (t1 + t2) / 2;
                    list.Add(s);
                }

                a = a2;
                a2 += step;
                fa = fa2;
                fa2 = fabs(a2);
            }

            for (int i = 0; i < list.Count; i++)
                if (fabs(list[i]) > eps)
                {
                    list.RemoveAt(i);
                    i--;
                }

            return new Vectors(list.Distinct().ToArray());
        }

        /// <summary>
        /// Метод локального поиска корня
        /// </summary>
        public enum MethodRoot : byte
        {
            Brent,
            Broyden,
            Bisec,
            Secant,
            NewtonRaphson,
            /// <summary>
            /// Комбинация методов Brent, Secant и Broyden
            /// </summary>
            Combine
            //Halfc
        }
        /// <summary>
        /// Поиск корней одним из специальных методов
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <param name="m">Метод</param>
        /// <param name="withMuller">Дополнять ли корни корнями метода парабол</param>
        /// <param name="countpoles">Требуемое количество корней. Если это число заранее известно, его указание может сильно ускорить вычисление. Иначе надо взять большое число</param>
        /// <returns></returns>
        public static Vectors OtherMethod(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12, MethodRoot m = MethodRoot.Brent, bool withMuller = false, int countpoles = 2)
        {
            List<double> list = new List<double>();
            Func<double, double> func = (double c) => f(c).ReIm;
            double a = beg, b = beg + step;
            Complex fa = f(a), fb = f(b);
            MethodR g;
            //MethodR half = (Func<double, double> ff, double begf, double endf, double epsf, uint N) => Optimization.Halfc((double c) => f(c).ReIm, begf, endf, step, eps);
            switch (m)
            {
                case MethodRoot.Brent:
                    g = BrentMethod;
                    break;
                case MethodRoot.Bisec:
                    g = bisectionMethod;
                    break;
                case MethodRoot.Secant:
                    g = secantMethod;
                    break;
                case MethodRoot.NewtonRaphson:
                    g = secantNewtonRaphsonMethod;
                    break;
                default:
                    g = BroydenMethod;
                    break;
            }

            if (withMuller)
                while (a < end)
                {
                    if (fa.Re * fb.Re <= 0 && fa.Im * fb.Im <= 0)
                    {
                        if (m == MethodRoot.Combine)
                        {
                            list.Add(BrentMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(BroydenMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(secantMethod(func, a, b, 1e-12, N: 1000));
                        }
                        else
                            list.Add(g(func, a, b, 1e-12, N: 1000));
                        //Optimization.Muller(f, a, new Complex((a + b) / 2, 0), new Complex(b, 0)).Re.Show();
                        list.Add(Optimization.Muller(f, a, new Complex((a + b) / 2, 0.01), new Complex((a + b) / 2, -0.01)).Re);
                    }
                    if (list.Count == countpoles) break;

                    a = b;
                    b += step;
                    fa = new Complex(fb);
                    fb = f(b);
                }
            else
                while (a < end)
                {
                    if (fa.Re * fb.Re <= 0 && fa.Im * fb.Im <= 0)
                    {
                        if (m == MethodRoot.Combine)
                        {
                            list.Add(BrentMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(BroydenMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(secantMethod(func, a, b, 1e-12, N: 1000));
                        }
                        else
                            list.Add(g(func, a, b, 1e-12, N: 1000));
                    }

                    if (list.Count == countpoles) break;
                    a = b;
                    b += step;
                    fa = new Complex(fb);
                    fb = f(b);
                }

            //new Vectors(list.ToArray()).Show();

            return new Vectors(list.Distinct().Where(n => !Double.IsNaN(n) && f(n).Abs <= eps && n >= beg && n <= end).ToArray());
        }
    }

}