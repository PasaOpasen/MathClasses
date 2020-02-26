using System;
using System.Collections.Generic;
using System.Text;
using МатКлассы;
using static МатКлассы.FuncMethods.Optimization;
using NUnit.Framework;
using Complex=МатКлассы.Number.Complex;
using System.Linq;

namespace MathClassesTests
{
    class MullerMethodTests
    {
        [TestCase(0,0,1)]
        [TestCase(-2, 2, 1)]
        [TestCase(-8, 2, 1)]
        [TestCase(1, 11, 2)]
        [TestCase(4, 41, 5)]
        [TestCase(4, 1, 7)]
        public void OneRootInDegree(double rootx,double rooty,double degree)
        {
            Complex root = new Complex(rootx, rooty);
            ComplexFunc f = (Complex z) => Complex.Pow(z - root, degree);
            Complex val = Muller(f, new Complex(0, 1), new Complex(3, 3), new Complex(9, 5),1e-16);
            Assert.IsTrue((root - val).Abs < 1e-2,message:$"Expected {root}, but was {val} (distance = {(root - val).Abs})");
        }


        [TestCase(1, 0, 1,2,3,5,1)]
        [TestCase(3, 4, 1, 2, 6, 5, 0)]
        [TestCase(5, 0, 1, 4, 3, 5, 1)]
        [TestCase(2, 5, 1, 2, 3, -5, 1)]
        [TestCase(-1, 4, 1, -20, -6, 5, 1)]
        public void RootofSin(double coef,double x1,double y1, double x2,double y2,double x3,double y3)
        {
            Complex val = Muller((Complex z) => Complex.Sin(coef * z), new Complex(x1, y1), new Complex(x2, y2), new Complex(x3, y3));
            Assert.IsTrue(Complex.Sin(coef * val).Abs < 1e-12,message:$"|Sin| was {Complex.Sin(coef * val).Abs}");
        }


        [TestCase(1, 0, 1, 20, 3.9, 50)]
        [TestCase(13, 4, 1, 2, 6, 50)]
        [TestCase(55.8, 0, 1, -4, 3.5, 100)]
        [TestCase(20, 5, 10, 2, 3, 100)]
        [TestCase(-10.8, 4, 10, -20, 60, 100)]
        public void RootofSinTrying(double coef, double xmin, double xmax, double ymin, double ymax,int count)
        {
            bool val = MullerTrying((Complex z) => Complex.Sin(coef * z), xmin,xmax,ymin,ymax,out Complex res,maxcount:count);
            Assert.IsTrue(val, message: $"|Sin(root)| was {Complex.Sin(coef * res).Abs}");
        }


        [TestCase(1, 0, 10, 20, 3.9, 50)]
        [TestCase(13, 4, 1, 2, 6, 50)]
        [TestCase(55.8, 0.008, 1, -4, 3.5, 100)]
        [TestCase(2, 50, 10, 2, 3, 100)]
        [TestCase(-10.8, 4, 10, -20, 60, 100)]
        public void RootofShTryMany(double coef, double xmin, double xmax, double ymin, double ymax, int count)
        {
            bool val = MullerTryMany((Complex z) => Complex.Sh(coef * z), xmin, xmax, ymin, ymax, out List<Complex> res, maxcount: count);
            Assert.IsTrue(val, message: $"|Sh(root)| was {res.ToArray().Select(s=>Complex.Sh(coef*s).Abs).ToArray().ToStringMas().Aggregate((s,r)=>$"{s} {r}")}");
        }
    }
}
