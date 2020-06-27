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

### Square matrixes

```csharp
var mat = new SqMatrix(new double[,] { { 1, -5 }, { -40, 0.632 } });

            mat.PrintMatrix();
//  1      -5
//- 40     0,632

            var i = SqMatrix.I(mat.RowCount);

            i.PrintMatrix();
            //1       0
            //0       1

            var mat2 = mat + i * 40;

            mat2.PrintMatrix();
            //41      -5
            //-40     40,632

            var inv = mat.Invertion;
            inv.PrintMatrix();
            //-0,003170017254524297   -0,02507925043136311
            //-0,20063400345090485 - 0,0050158500862726215

                      (inv * mat).PrintMatrix();
            //1       0
            //2,7755575615628914E-17  0,9999999999999999


                        (inv * mat).Track.Show(); // 2


            (inv * mat).CubeNorm.Show(); // 1

            (inv * mat).Det.Show(); // 0,9999999999999999

            mat.Solve(new Vectors(4.0, -5)).Show(); // (       0,11271618313871837     -0,7774567633722562     )

```

### Random numbers

```csharp
RandomNumbers.SetSeed(0);

            RandomNumbers.NextDouble.Show(); // 0,7262432699679598

            RandomNumbers.NextDouble2(min: 40, max: 50).Show(); // 48,173253595909685

            RandomNumbers.NextNumber().Show(); // 1649316166
```


### Double nets

```csharp
void show(NetOnDouble n) => new Vectors(n.Array).Show();

            var net = new NetOnDouble(begin: -1, end: 1, count: 8); // like numpy.linspase

            show(net); // (       -1      -0,7142857142857143     -0,4285714285714286     -0,1428571428571429     0,1428571428571428      0,4285714285714284      0,7142857142857142      1       )

            net = new NetOnDouble(begin: -1, end: 1, step: 0.3); // like numpy.arange

            show(net); // (       -1      -0,7    -0,4    -0,10000000000000009    0,19999999999999996     0,5     0,7999999999999998      )

            var net2 = new NetOnDouble(net, newCount: 5);

            show(net2); // (       -1      -0,6    -0,19999999999999996    0,20000000000000018     0,6000000000000001      )

```

### Integration

```csharp
Func<double, double> freal = x => (x-4 + Math.Sin(x*10)) / (1 + x * x);

            double integral = FuncMethods.DefInteg.GaussKronrod.GaussKronrodSum(freal, a: -3, b: 3, n: 61, count: 5);

            integral.Show(); // -9,992366179186035

            Complex integ = FuncMethods.DefInteg.GaussKronrod.GaussKronrodSum(
                z => (Math.Exp(-z.Abs) + Complex.Sin(z + Complex.I)) / (1 + z * z / 5), 
                a: new Complex(-1,-4.3), b: 3+Complex.I*2, n: 61, count: 10);

            integ.Show(); // -3,325142834912312 + 10,22008333462534i
```


### Memoization

```csharp
Func<double, double> f = t => 
            FuncMethods.DefInteg.GaussKronrod.GaussKronrodSum(x=>Math.Exp(-(x-t).Sqr()-x*x), a: -20, b: 10, n: 61, count: 12);

            var f_Memoized = new Memoize<double,double>(f, capacity: 10, concurrencyLevel: 1).Value;

            var t = DateTime.Now;
            void show_time()
            {
                (DateTime.Now-t).Ticks.Show();
                t = DateTime.Now;
            }

            f_Memoized(2).Show(); // 0,16961762375804412

            show_time(); // 842753

            f_Memoized(2.5).Show(); // 0,05506678006050927

            show_time(); // 8179

            f_Memoized(3).Show(); // 0,013923062412768037

            show_time(); // 7991

            f_Memoized(2.5).Show(); // 0,05506678006050927

            show_time(); // 1442



            // not only real functions!

            Func<(double, Complex, bool), (int, int)> c = tuple =>
               {
                   var x = tuple.Item1;
                   var z = tuple.Item2;
                   var b = tuple.Item3;

                   if (b)
                       return ((int)(x + z.Re), (int)(x + z.Im));
                   else
                       return (0, 0);
               };

            var c_tmp = new Memoize<(double, Complex, bool), (int, int)>(c, 100, 4).Value;

            Func<double, Complex, bool,(int,int)> c_Memoized = (double x, Complex z, bool b) => c_tmp((x, z, b));
```

### Polynoms

```csharp
// create by coefs
            var pol = new Polynom(new double[] { 1, 2, 3, 4, 5 });
            pol.Show(); // 5x^4 + 4x^3 + 3x^2 + 2x^1 + 1

            // create by head coef and roots
            pol = new Polynom(aN: 2, -1, 0, 1, 2);
            pol.Show(); // 2x^4 + -4x^3 + -2x^2 + 4x^1 + -0


            var points = new Point[] { new Point(1, 2), new Point(3, 4), new Point(5, 7) };
            // interpolation polynom
            pol = new Polynom(points);

            pol.Show(); // 0,125x^2 + 0,5x^1 + 1,375
            pol.Value(1).Show(); // 2
            pol.Value(3).Show(); // 4
            pol.Value(5).Show(); // 7


            // interpolation polynom
            pol = new Polynom(x => Math.Sin(x) + x, n: 6, a: -1, b: 1);

            foreach (var val in new NetOnDouble(-1, 1, 12).Array)
                $"pol = {pol.Value(val)}   f = {Math.Sin(val)+val}".Show();

//pol = -1,841470984807902   f = -1,8414709848078965
//pol = -1,5480795026639842   f = -1,548086037891892
//pol = -1,230639272195443   f = -1,230638423911926
//pol = -0,8936010083785814   f = -0,8935994224990154
//pol = -0,5420855146782465   f = -0,5420861802628714
//pol = -0,18169222919395844   f = -0,1816930144179611
//pol = 0,18169222919395842   f = 0,1816930144179611
//pol = 0,5420855146782463   f = 0,5420861802628714
//pol = 0,8936010083785806   f = 0,8935994224990154
//pol = 1,2306392721954413   f = 1,230638423911926
//pol = 1,5480795026639818   f = 1,5480860378918924
//pol = 1,8414709848078967   f = 1,8414709848078965

            // operations

            var pol1 = new Polynom(new double[] { 1, 2, 3, 4, 5 });
            var pol2 = new Polynom(new double[] { 2.2, 3 });

            pol1.Show(); // 5x^4 + 4x^3 + 3x^2 + 2x^1 + 1
            pol2.Show(); // 3x^1 + 2,2
            pol2.ShowRational(); // (3x^1) + 11/5

            (pol1 * pol2).Show(); // 15x^5 + 23x^4 + 17,8x^3 + 12,600000000000001x^2 + 7,4x^1 + 2,2

            (pol1 / 2 + pol2 * 2.8-4.66).Show(); // 2,5x^4 + 2x^3 + 1,5x^2 + 9,399999999999999x^1 + 2

            var a = pol1 / pol2;
            var b = pol1 % pol2;

            $"{pol1} == {a*pol2+b}".Show(); // 5x^4 + 4x^3 + 3x^2 + 2x^1 + 1 == 5x^4 + 4x^3 + 3x^2 + 2x^1 + 1

            (a, b) = Polynom.Division(pol1 - 1.5, pol2);
            $"{pol1-1.5} == {a * pol2 + b}".Show(); // 5x^4 + 4x^3 + 3x^2 + 2x^1 + -0,5 == 5x^4 + 4x^3 + 3x^2 + 2x^1 + -0,5

            // derivative
            (pol1 | 2).Show(); // 60x^2 + 24x^1 + 6



            pol2.Value(pol1).Show(); // 

            pol2.Value(new SqMatrix(new double[,] { { 1, 2 }, { 3, 4 } })).PrintMatrix();

            //5,2     6
            //9       14,2


            // integration

            $"{pol1.S(-3,2)} == {FuncMethods.DefInteg.GaussKronrod.MySimpleGaussKronrod(pol1.Value,-3,2,n:15)}".Show();

            // 245 == 244,99999999999997
        
```

### Swarm algorithm

```csharp
double rastr(double v) => v * v - 10 * Math.Cos(2 * Math.PI * v);
            double shvel(double v) => -v * Math.Sin(Math.Sqrt(v.Abs()));
```
![1](https://github.com/PasaOpasen/MathClasses/blob/master/Examples/images/rast1d.png)
![1](https://github.com/PasaOpasen/MathClasses/blob/master/Examples/images/shvel1.png)

Use swarm algorithm to find global minimum of 1D-functions:

```csharp
            // 1D functions

            var (argmin, min) = BeeHiveAlgorithm.GetGlobalMin(rastr, minimum: -5, maximum: 5,eps:1e-15, countpoints: 100, maxiter: 100);

            $"min = {min}, argmin = {argmin}".Show(); // min = -9,973897874017823, argmin = 0,011472797486931086


            (argmin, min) = BeeHiveAlgorithm.GetGlobalMin(shvel, minimum: -150, maximum: 150, eps: 1e-15, countpoints: 100, maxiter: 100);

            $"min = {min}, argmin = {argmin}".Show(); // min = -122,45163537317933, argmin = -126,64228803478181
```
2D-functions
```csharp
            // 2D functions

            // u can write -func to find the maximum of function
            var (argmin2, _) = BeeHiveAlgorithm.GetGlobalMin((Point p) => -shvel(p.x) - shvel(p.y), 
                minimum: new Point(-150, -150), maximum: new Point(150, 150), eps: 1e-15, countpoints: 300, maxiter: 200);

            argmin2.Show(); // (125,85246982052922 , 133,86488312389702)


            // u don't need only smooth functions!
            (argmin2, _) = BeeHiveAlgorithm.GetGlobalMin((Point p) => -shvel(p.x) - shvel(p.y)+RandomNumbers.NextDouble2(-1,1), 
                minimum: new Point(-150, -150), maximum: new Point(150, 150), eps: 1e-15, countpoints: 500, maxiter: 200);

            argmin2.Show(); // (124,97163349762559 , 126,79389473050833)
```
![1](https://github.com/PasaOpasen/MathClasses/blob/master/Examples/images/shvel2.png)

And more than 2D functions:
```csharp

            // u can use 3D+ functions

            var (argmin3, _) = BeeHiveAlgorithm.GetGlobalMin((Vectors v) => Math.Sin(v[0]).Abs()*rastr(v[1])*shvel(v[2]).Abs()+shvel(v[3]),
                minimum: new Vectors(-100, -100, -100, -50),
                maximum: new Vectors(100, 50, 50, 50),
                eps: 1e-15,
                countpoints: 500,
                maxiter: 500);

            argmin3.Show(); // (       48,37734238244593       0,09753305930644274     -60,919505648780614     46,69636117760092       )
```



