using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Complex = МатКлассы.Number.Complex;

namespace MathClassesTests
{
    class ComplexTests
    {

        [TestCase("1+2i",1,2)]
        [TestCase("1,09 - 2i", 1.09, -2)]
        [TestCase("0,00001-2,2i", 0.00001, -2.2)]
        [TestCase("1e-4 - 2,4e-1i", 1e-4, -2.4e-1)]
        [TestCase("2", 2, 0)]
        [TestCase("-2i", 0, -2)]
        public void ToComplexTests(string s,double x,double y)
        {
            var v = Complex.ToComplex(s);
            Assert.IsTrue(v.Re == x && v.Im == y, $"Expected {new Complex(x, y)}, but was {v}");
        }
    }
}
