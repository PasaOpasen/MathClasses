# Math Classes in CSharp

[![NuGet version](https://buildstats.info/nuget/MathClassesDmPa)](https://www.nuget.org/packages/MathClassesDmPa/)

## About

Set of simple mathematical classes in C# including

* Vectors (with complex cases)
* Matrixes (not only square)
* Polynoms (including intergation and derivatives)
* Systems of linear equations
* Integrals methods (including Gauss-Kronrod)
* Complex numbers, complex vectors, matrixes and methods for complex functions
* Rational numbers
* Graphs 
* Methods for solving simple differential equations
* Function approximation/interpolation
* Net functions methods
* Function optimization (including swarm algorithm for 1/2/n dimentions)
* Function values memorising (memoize) for parallel/non-parallel cases 

I've been creating this library since 2017 for my university homework and firts job. Writing it I was practicing my C# skills from C# 3.0 to C# 7.1, getting coding experience.

There are .NET Framework version and newest supported .NET Core (named MathClassesLibrary) copy with few upgrading.

See some examples of using and results [here](https://github.com/PasaOpasen/Old_Math_CSharpCpp_Projects) or [here](https://github.com/PasaOpasen/Search-for-defects-in-plates)

## How to download

Downloading in VS:

![1](https://github.com/PasaOpasen/MathClasses/blob/master/gifs/download.gif)

## Usage:

![1](https://github.com/PasaOpasen/MathClasses/blob/master/gifs/usage.gif)

## Examples

Below you can see examples of using most used classes of this library.

Firstly load the objects and set aliases:

```csharp
using MathNet.Numerics;
using System;
using System.Linq;
using МатКлассы;
using Complex = МатКлассы.Number.Complex;
using Rational = МатКлассы.Number.Rational;
```

### Complex numbers

```csharp
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
```

### Rational numbers

```csharp
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
```

### 2D Points

```csharp
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
```

### Function parser

```csharp

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

```
### Real vectors

```csharp
            var v = new Vectors(5);
            v.Show(); // (       0       0       0       0       0       )

            v = new Vectors(5, 1.0);
            v.Show(); // (       1       1       1       1       1       )

            v = new Vectors(1, 2, 3, 4, 5, 6, 7, 9.5, -2, 3);
            v.Show(); // (       1       2       3       4       5       6       7       9,5     -2      3       )

            v = new Vectors(new double[] { 1, 2, 3,-3,-2,-1,0 });
            v.Show(); // (       1       2       3       -3      -2      -1      0       )


            v[6].Show(); // 0

            v.EuqlidNorm.Show(); // 0,7559289460184545

            v.Normalize(0, 1).Show(); // (       0,6666666666666666      0,8333333333333333      1       0       0,16666666666666666     0,3333333333333333     0,5      )

            v.Range.Show(); // 6

            v.SubVector(4).Show(); // (       1       2       3       -3      )

            v.Average.Show(); // 1,7142857142857142

            v.Contain(3).Show(); // True

            v.Normalize(-0.5, 0.5).ToRationalStringTab().Show(); // (       1/6     1/3     1/2     -1/2    -1/3    -1/6    0       )

            var p = Vectors.Create(dim: 7, min: 0, max: 2);
            p.Show(); // (       1,4585359040647745      1,7510524201206863      1,4706563879735768      0,45403700647875667     0,022686069831252098    1,9943826524540782      0,3851787596940994      )

            Vectors.Mix(v, p).Show(); 
            // (       1       1,4585359040647745      2       1,7510524201206863      3       1,4706563879735768      -3      0,45403700647875667      -2      0,022686069831252098    -1      1,9943826524540782      0       0,3851787596940994      )
            
            (v + p).Show(); // (       2,4585359040647745      3,7510524201206863      4,470656387973577       -2,5459629935212433     -1,977313930168748      0,9943826524540782      0,3851787596940994      )
            (v * p).Show(); // 5,970744096674025
            Vectors.CompMult(v, p).Show(); // (       1,4585359040647745      3,5021048402413726      4,41196916392073        -1,36211101943627       -0,045372139662504196   -1,9943826524540782     0       )

            (2.4*v.AbsVector - p/2).Sort.BinaryApproxSearch(1.5).Show(); // 1,4028086737729608

```

### Complex vectors

```csharp
           var v1 = new Vectors(1, 2, 3, 4, 5);
            var v2 = new Vectors(0, 3, 4, 3, -5);

            var c = new CVectors(R: v1, I: v2);

            c.Show(); // (1    2 + 3i   3 + 4i   4 + 3i   5 - 5i)
            c.Re.Show(); // (       1       2       3       4       5       )
            c.Normalize.Show(); // (0,1414213562373095    0,282842712474619 + 0,42426406871192845i   0,42426406871192845 + 0,565685424949238i   0,565685424949238 + 0,42426406871192845i   0,7071067811865475 - 0,7071067811865475i)
            c.Conjugate.Show(); // (1    2 - 3i   3 - 4i   4 - 3i   5 + 5i)

            var b = new CVectors(new Complex[] { new Complex(1, 2), new Complex(4, 5), new Complex(4.4, 0), new Complex(), new Complex(4.5) });

            (c/5 + b*(0.2-Complex.I)).Show(); // (2,4000000000000004 - 0,6i   6,2 - 2,4i   1,48 - 3,6000000000000005i   0,8 + 0,6i   1,9 - 5,5i)

```














