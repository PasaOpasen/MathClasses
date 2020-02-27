using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Complex = МатКлассы.Number.Complex;
using МатКлассы;

namespace MathClassesTests
{
    class ComplexTests
    {

        [TestCase("1+2i",1,2)]
        [TestCase("1,09 - 2i", 1.09, -2)]
        [TestCase("0,00001-2,2i", 0.00001, -2.2)]
        [TestCase("1e-4 - 2,4e-1i", 1e-4, -2.4e-1)]
        [TestCase("-1,4E-4 + 2,434e-1i", -1.4e-4, 2.434e-1)]
        [TestCase("2", 2, 0)]
        [TestCase("-2i", 0, -2)]
        public void ToComplexTest(string s,double x,double y)
        {
            var v = Complex.ToComplex(s);
            Assert.IsTrue(v.Re == x && v.Im == y, $"Expected {new Complex(x, y)}, but was {v}");
        }

        [TestCase(0,0)]
        [TestCase(0.123, 0)]
        [TestCase(0, -0.02)]
        [TestCase(-1e-4, 1e-15)]
        [TestCase(2e4, -43.56)]
        [TestCase(1, 0.1223)]
        public void ConvertTest(double x,double y)
        {
            var c = new Complex(x, y);
            Assert.IsTrue(c == Complex.ToComplex(c.ToString()),$"Was {Complex.ToComplex(c.ToString())} from {c.ToString()}");
        }
    }

    class ComplexParserTests
    {
        //[Test]
        public void ParsingTest1()
        {
            const string formula = "cos(z)+2+sh(z)";
            Func<Complex, Complex> expected = (Complex z) => Complex.Cos(z)+2+Complex.Sh(z);
            Func<Complex, Complex> result = ParserComplex.GetDelegate(formula);

            Random rnd = new Random(22);
            Complex tmp,v1,v2;
            for(int i = 0; i < 50; i++)
            {
                tmp = new Complex(rnd.NextDouble()*10-5, rnd.NextDouble()*10-5);
                v1 = expected(tmp);
                v2 = result(tmp);
                Assert.IsTrue(v1 == v2, $"Expected {v1} but was {v2} on arg = {tmp}; iter = {i+1}");
            }

        }
    }
}
