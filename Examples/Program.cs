using System;
using System.Linq;
using МатКлассы;
using Complex = МатКлассы.Number.Complex;
using Rational = МатКлассы.Number.Rational;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ex.ComplexNumberExamples();

            //Ex.RadionalNumberExamples();

            //Ex.PointExamples();

            Ex.ParserExamples();

            Console.ReadKey();
        }
    }

    static class Ex
    {
        public static void ComplexNumberExamples()
        {

            var a = new Complex(1);
            a.Show(); // 1

            var b = new Complex(2.5, 1.5);
            b.Show(); // 2,5 + 1,5i
            b.Conjugate.Show(); // 2,5 - 1,5i
            b.Re.Show(); // 2,5
            b.Im.Show(); // 1,5
            b.Abs.Show(); // 2,9154759474226504
            b.Arg.Show(); // 0,5404195002705842


            var c = a / b + 3 + 5.6 + a * b +
                Complex.Cos(b) + Complex.Sin(a + b) + Complex.Ch(a * b * b) +
                Complex.Ln(b) + Complex.Exp(a) + Complex.Expi(10);
            c.Show(); // 21,099555214728547 + 23,649577072514752i
            c.Round(4).Show(); // 21,0996 + 23,6496i

            c.Swap.Show(); // 23,649577072514752 + 21,099555214728547i
            c.Pow(3.5).Show(); // -175884,94745749474 + 34459,051603806576i

            new CVectors(Complex.Radical(a + b + b * b, 7)).Show(); // (1,4101632808375018 + 0,1774107754735756i   0,7405170949635271 + 1,2131238576267886i   -0,4867535672138724 + 1,3353299317700824i   -1,3474888653159458 + 0,4520053315239409i   -1,1935375640715047 - 0,771688502588176i   -0,14082813335184957 - 1,4142851546746704i   1,0179277541521443 - 0,9918962391315406i)

            Complex.ToComplex("1+2i").Show(); // 1 + 2i
            Complex.ToComplex("-1 + 2,346e-5i").Show(); // -1 + 2,346e-5i
        }

        public static void RadionalNumberExamples()
        {
            var a = new Rational(2123343454656);
            a.Show(); // 2123343454656

            var b = new Rational(455, 15);
            b.Show(); // 91/3
            b.ShowMixed(); // 30 + 1/3

            (a + b).IntPart.Show(); // 2123343454686
            b.FracPart.Show(); // 1/3

            var c = new Rational(22.16);
            c.Show(); // 554/25

            Rational.ToRational(-12.46572).Show(); // -311643/25000

            (b + c).ShowMixed(); // 52 + 37/75

            (a / 3213443 + b * c).ShowMixed(); // 661441 + 40262377/241008225
        }

        public static void PointExamples()
        {
            var p1 = new Point(1, 4);
            p1.Show(); // (1 , 4)

            Point.Add(p1, 3).Show(); // (4 , 7)
            Point.Add(p1, -1, -3).Show(); // (0 , 1)

            Point.Eudistance(new Point(0.4, -2), new Point(1, 1)).Show(); // 3,059411708155671

            var gen = Expendator.ThreadSafeRandomGen(100);

            var points = Enumerable.Range(0, 10).Select(s => new Point(gen.NextDouble(), gen.NextDouble())).ToArray();

            Point.Center(points).Show(); // (0,6655678990602344 , 0,531635367093438) 

            points = Point.Points(x => Math.Sin(x) + 2 * Math.Round(x), n: 10, a: -0.2, b: 0.2);

            foreach (var p in points)
                Console.WriteLine(p);

            //(0, 6655678990602344, 0, 531635367093438)
            //(-0, 2, -0, 19866933079506122)
            //(-0, 16, -0, 15931820661424598)
            //(-0, 12000000000000001, -0, 11971220728891938)
            //(-0, 08000000000000002, -0, 07991469396917271)
            //(-0, 04000000000000001, -0, 03998933418663417)
            //(0, 0)
            //(0, 03999999999999998, 0, 03998933418663414)
            //(0, 08000000000000002, 0, 07991469396917271)
            //(0, 12, 0, 11971220728891936)
            //(0, 15999999999999998, 0, 15931820661424595)
            //(0, 2, 0, 19866933079506122)
        }

        public static void ParserExamples()
        {
            var gen = Expendator.ThreadSafeRandomGen(1);

            Func<double, double> f1 = Parser.GetDelegate("cos(x)+sin(x/2+4,6)*exp(-sqr(x))+abs(x)^0,05");
            Func<double, double> f2 = x => Math.Cos(x) + Math.Sin(x / 2 + 4.6) * Math.Exp(-x * x) + Math.Pow(Math.Abs(x), 0.05);

            for (int i = 0; i < 5; i++)
            {
                var d = gen.NextDouble() * 50;
                $"{f1(d)} == {f2(d)}".Show();
            }
            //2,1254805141764708 == 2,1254805141764708
            //1,8237614071831993 == 1,8237614071831991
            //0,9607774340933849 == 0,9607774340933849
            //1,8366859282256982 == 1,8366859282256982
            //1,3013656833866554 == 1,3013656833866554

            Func<Complex, Complex> c1 = ParserComplex.GetDelegate("Re(z)*Im(z) + sh(z)*I + sin(z)/100");
            Func<Complex, Complex> c2 = z => z.Re * z.Im + Complex.Sh(z) * Complex.I + Complex.Sin(z) / 100;

            for (int i = 0; i < 5; i++)
            {
                var d = new Complex(gen.NextDouble() * 50, gen.NextDouble() * 10);

                $"{c1(d)} == {c2(d)}".Show();
            }

            // 485696399,00749403 - 1151202339,537752i == 485696398,8506223 - 1151202339,7349265i
            //- 1,328008130324937E+20 + 8,291419281824573E+19i == -1,328008130324937E+20 + 8,291419281824573E+19i
            // - 12609203585138,465 + 42821192159972,99i == -12609203585138,78 + 42821192159972,99i
            //7270,488386388151 - 121362,61308893963i == 7270,344179063162 - 121362,52416901031i
            //- 7,964336745137357E+20 + 1,3345594600169975E+21i == -7,964336745137357E+20 + 1,3345594600169975E+21i

        }
    }

}
