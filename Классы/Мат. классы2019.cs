using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using Excel = Microsoft.Office.Interop.Excel;

namespace МатКлассы
{
    /// <summary>
    /// Комплексная функция многих переменных
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate Complex CnToCFunction(CVectors v);
    /// <summary>
    /// Матричная функция от векторного аргумента
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate CSqMatrix CVecToCMatrix(CVectors v);
    /// <summary>
    /// Функция R->C
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Complex RToC(double x);

    public delegate double MethodR(Func<double, double> f, double beg, double end, double eps, uint N);

    /// <summary>
    /// Класс функции, осуществляющей отображение Ck -> Cn
    /// </summary>
    public class CkToCnFunc
    {
        /// <summary>
        /// Делегат, отождествляемый с унитарным отображением
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public delegate CVectors VecToVec(CVectors v);

        private CnToCFunction[] FuncMas;
        private VecToVec func = null;

        /// <summary>
        /// Размерность области значений
        /// </summary>
        public int EDimention => FuncMas.Length;

        /// <summary>
        /// Значение функции от вектора через индексатор
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public CVectors this[CVectors v]
        {
            get
            {
                if (func == null)
                {
                    CVectors res = new CVectors(EDimention);
                    for (int i = 0; i < EDimention; i++)
                        res[i] = FuncMas[i](v);
                    return res;
                }
                else
                    return func(v);
            }
        }
        /// <summary>
        /// Функция отдельного измерения
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CnToCFunction this[int i] => new CnToCFunction(FuncMas[i]);
        /// <summary>
        /// Метод, возвращающий значение функции от вектора
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public CVectors Value(CVectors v) => this[v];
        /// <summary>
        /// Значение функции от вектора
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public CVectors Value(params Complex[] c)
        {
            CVectors v = new CVectors(c);
            return this[v];
        }

        /// <summary>
        /// Задание функции как совокупности комплексных функций многих переменных
        /// </summary>
        /// <param name="mas"></param>
        public CkToCnFunc(params CnToCFunction[] mas)
        {
            FuncMas = new CnToCFunction[mas.Length];
            for (int i = 0; i < FuncMas.Length; i++)
                FuncMas[i] = new CnToCFunction(mas[i]);
        }
        /// <summary>
        /// Задать унитарную функции как произведение унитарной функции на комплексную матрицу
        /// </summary>
        /// <param name="M"></param>
        /// <param name="F"></param>
        public CkToCnFunc(CSqMatrix M, CkToCnFunc F)
        {
            this.FuncMas = new CnToCFunction[F.EDimention];
            for (int i = 0; i < this.EDimention; i++)
                this.FuncMas[i] = (CVectors v) => M.GetLine(i) * F.Value(v);
        }
        /// <summary>
        /// Задать унитарную функции как произведение унитарной функции на кматричную функцию
        /// </summary>
        /// <param name="M"></param>
        /// <param name="F"></param>
        public CkToCnFunc(CVecToCMatrix M, CkToCnFunc F)
        {
            this.FuncMas = null;
            func = (CVectors v) =>
              {
                  CSqMatrix Mat = M(v);
                  CVectors Vec = F.Value(v);
                  CVectors res = new CVectors(Vec.Degree);

                  for (int i = 0; i < this.EDimention; i++)
                      res[i] = new Complex(Mat.GetLine(i) * Vec);
                  return res;
              };

        }
        /// <summary>
        /// Задать функцию через делегат отображения
        /// </summary>
        /// <param name="f"></param>
        public CkToCnFunc(VecToVec f) { this.func = new VecToVec(f); }

        /// <summary>
        /// Тип каррирования
        /// </summary>
        public enum CarringType : byte
        {
            /// <summary>
            /// По первым аргументам
            /// </summary>
            FirstArgs,
            /// <summary>
            /// По последним аргументам
            /// </summary>
            LastArgs
        }
        /// <summary>
        /// Каррирование отображения в соответствии с параметрами
        /// </summary>
        /// <param name="C">Параметр каррирования</param>
        /// <param name="c">Фиксированные аргументы</param>
        /// <returns></returns>
        public CkToCnFunc CarrByFirstOrLastArgs(CarringType C = CarringType.LastArgs, params Complex[] c)
        {
            if (func == null)
            {
                CnToCFunction[] mas = new CnToCFunction[EDimention];

                switch (C)
                {
                    case CarringType.FirstArgs:
                        for (int i = 0; i < mas.Length; i++)
                            mas[i] = (CVectors v) => this.FuncMas[i](new CVectors(Expendator.Union(c, v.ComplexMas)));
                        break;
                    default:
                        for (int i = 0; i < mas.Length; i++)
                            mas[i] = (CVectors v) => this.FuncMas[i](new CVectors(Expendator.Union(v.ComplexMas, c)));
                        break;
                }
                return new CkToCnFunc(mas);
            }
            else
            {
                VecToVec h;
                switch (C)
                {
                    case CarringType.FirstArgs:
                        h = (CVectors v) => func(new CVectors(Expendator.Union(c, v.ComplexMas)));
                        break;
                    default:
                        h = (CVectors v) => func(new CVectors(Expendator.Union(v.ComplexMas, c)));
                        break;
                }
                return new CkToCnFunc(h);
            }

        }

        /// <summary>
        /// Интеграл от отображения по одному аргументу (другие зафиксированы)
        /// </summary>
        /// <param name="beforeArg">Фиксированные аргументы до изменяемого</param>
        /// <param name="afterArg">Фиксированные аргументы после изменяемого</param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="tm"></param>
        /// <param name="tp"></param>
        /// <param name="eps"></param>
        /// <param name="pr"></param>
        /// <param name="gr"></param>
        /// <returns></returns>
        public CVectors IntegralAmoutOneArg(CVectors beforeArg, CVectors afterArg, double t1, double t2, double t3, double t4, double tm, double tp, double eps, double pr, double gr)
        {
            FuncMethods.DefInteg.GaussKronrod.ComplexVectorFunc tmp = (Complex x, int N) => this.Value(Expendator.Union(beforeArg.ComplexMas, new Complex[] { x }, afterArg.ComplexMas)).ComplexMas;
            return new CVectors(FuncMethods.DefInteg.GaussKronrod.DINN5_GK(tmp, t1, t2, t3, t4, tm, tp, eps, pr, gr, this.EDimention));
        }
    }

    /// <summary>
    /// Класс оптимизации функции двух аргументов
    /// </summary>
    public static class OptimizationDCompFunc
    {
        /// <summary>
        /// Поиск минимума функции на прямоугольнике, чьи стороны параллельны осям координат
        /// </summary>
        /// <param name="f">Оптимизируемая функция</param>
        /// <param name="x0"></param>
        /// <param name="X"></param>
        /// <param name="y0"></param>
        /// <param name="Y"></param>
        /// <param name="nodescount">Корень из числа точек, берущихся в прямоугольнике (нижняя граница, потому что если прямоугольник слишком далёк от квадрата, надо брять другое соотношение)</param>
        /// <param name="eps">Погрешность поиска</param>
        /// <param name="ogr">Через сколько максимально итераций нужно закончить цикл, если последние ogr итераций подряд точка максимума не изменялась</param>
        /// <returns></returns>
        public static (double ur , double uz) GetMaxOnRectangle(Func<double, double, double> f, double x0, double X, double y0, double Y, int nodescount = 10, double eps = 1e-7, int ogr = 3, bool useGradient = false, bool parallel = true)
        {
            double max = f(x0, y0);//max.Show();
            (double ur , double uz) res = (x0, y0);
            double x = (X - x0).Abs(), y = (Y - y0).Abs();
            int nodescI, nodescJ;
            if (x > y)
            {
                nodescJ = nodescount;
                nodescI = (int)(nodescount * (x / y));
            }
            else
            {
                nodescI = nodescount;
                nodescJ = (int)(nodescount * (x / y));
            }
            double[,] mas = new double[nodescI, nodescJ];
            int k = 0;

            while (((X - x0) * (Y - y0)).Abs() > eps && k <= ogr)
            {
                double xstep = (X - x0) / (nodescI /*+ 1*/);
                double ystep = (Y - y0) / (nodescJ /*+ 1*/);

                k++;
                if (!parallel)
                    for (int i = 0; i < nodescI; i++)
                        for (int j = 0; j < nodescJ; j++)
                        {
                            mas[i, j] = f(x0 + xstep * i, y0 + ystep * j);
                            if (mas[i, j] > max)
                            {
                                k = 0;
                                max = mas[i, j];//max.Show();
                                res = (x0 + xstep * i, y0 + ystep * j);
                            }
                        }
                else
                {
                    //параллельная версия
                    double[] maxmas = new double[nodescI];
                    (double ur , double uz)[] resmas = new (double ur , double uz)[nodescI];
                    for (int i = 0; i < nodescI; i++)
                    {
                        maxmas[i] = max;
                        resmas[i] = (res.Item1, res.Item2);
                    }

                    Parallel.For(0, nodescI, (int i) =>
                    {
                        for (int j = 0; j < nodescJ; j++)
                        {
                            mas[i, j] = f(x0 + xstep * i, y0 + ystep * j);
                            if (mas[i, j] > maxmas[i])
                            {
                                k = 0;
                                maxmas[i] = mas[i, j];//max.Show();
                                resmas[i] = (x0 + xstep * i, y0 + ystep * j);
                            }
                        }
                    });

                    max = maxmas.Max();
                    int tmp = Array.IndexOf(maxmas, max);
                    res = (resmas[tmp].Item1, resmas[tmp].Item2);
                }



                //double x = x0 + (X - x0) / 2;
                //double y = y0 + (Y - y0) / 2;
                //if (res.Item1 > x) x0 = x;
                //else X = x;
                //if (res.Item2 > y) y0 = y;
                //else Y = y;
                x = (X - x0) / 2;
                double x2 = x / 2, p1p = res.Item1 + x2, p1m = res.Item1 - x2;
                y = (Y - y0) / 2;
                double y2 = y / 2, p2p = res.Item2 + y2, p2m = res.Item2 - y2;

                if (p1m < x0) X = x0 + x;
                else if (p1p > X) x0 = X - x;
                else
                {
                    x0 = p1m;
                    X = p1p;
                }
                if (p2m < y0) Y = y0 + y;
                else if (p2p > Y) y0 = Y - y;
                else
                {
                    y0 = p2m;
                    Y = p2p;
                }

            }

            if (useGradient)
            {
                Complex point = new Complex(res.Item1, res.Item2);
                ComplexFunc cf = (Complex a) => f(a.Re, a.Im);
                Gradient(cf, ref point, eps: eps);
                res = (point.Re, point.Im);
            }

            return res;
        }

        /// <summary>
        /// Метод градиентного спуска к максимуму по модулю от функции
        /// </summary>
        /// <param name="f">Функция комплексного переменного</param>
        /// <param name="point">Начальная точка</param>
        /// <param name="alp">Коэффициент метода</param>
        /// <param name="maxcount">Максимальное число итераций</param>
        /// <param name="eps">Погрешность</param>
        private static void Gradient(ComplexFunc f, ref Complex point, double alp = 0.01, int maxcount = 100, double eps = 1e-14)
        {
            DefInteg.Residue.eps = eps;
            Complex gr = DefInteg.Residue.Derivative(f, point);
            Complex fp = f(point);
            int count = 0;
            while (gr.Abs > eps && count <= maxcount && alp > 10 * eps)
            {
                Complex p2 = point - alp * gr;
                Complex fp2 = f(p2);
                if (fp2.Abs > fp.Abs)
                {
                    point = new Complex(p2);
                    fp = new Complex(fp2);
                    gr = DefInteg.Residue.Derivative(f, p2);
                }
                else
                {
                    alp /= 2;
                }
                count++;
            }
            //point = new Complex(fp);
        }
    }

    /// <summary>
    /// Некоторые специальные функции
    /// </summary>
    public static class SpecialFunctions
    {
        /// <summary>
        /// Функция Бесселя
        /// </summary>
        /// <param name="a">Порядок</param>
        /// <param name="x">Аргумент</param>
        /// <returns></returns>
        public static Complex MyBessel(double a, Complex x)
        {
            if (x.Im.Abs() < 1e-16) return MyBessel(a, x.Re);
            ComplexFunc f = (Complex c) => Complex.Cos(a * c - x * Complex.Sin(c));
            return FuncMethods.DefInteg.GaussKronrod.MySimpleGaussKronrod(f, 0, Math.PI, 61, true, 5) / Math.PI;
        }
        /// <summary>
        /// Функция Бесселя
        /// </summary>
        /// <param name="a">Порядок</param>
        /// <param name="x">Аргумент</param>
        /// <returns></returns>
        public static double MyBessel(double a, double x)=> Computator.NET.Core.Functions.SpecialFunctions.BesselJν(a, x);
        

        /// <summary>
        /// Функция Ханкеля первого рода
        /// </summary>
        /// <param name="a"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Complex Hankel(double a, double z) => Computator.NET.Core.Functions.SpecialFunctions.Hankel1(a, z);

       // public static Complex HankelApprox()
    }

    /// <summary>
    /// Класс разных методов, которые я затем использую в комбинации со скриптами
    /// </summary>
    public static class ForScripts
    {
        /// <summary>
        /// Создать файлы для последующего создания поверхностей. 
        /// </summary>
        /// <param name="x0">Минимальное значение аргумента х</param>
        /// <param name="X">Максимальное значение аргумента х</param>
        /// <param name="argcount">Число точек по аргументу</param>
        /// <param name="filename">Базовое имя файла</param>
        /// <param name="f">Массив функций, от которых нужны поверхности</param>
        /// <param name="filter">Фильтр принадлежности области, на которой надо рисовать</param>
        public static void MakeFilesForSurfaces(double x0, double X, double y0, double Y, int argcount, string filename, Functional[] f, Func<Point, bool> filter,bool parallel=false)
        {

            int xc = argcount, yc = argcount;

            double hx = (X - x0) / (xc - 1), hy = (Y - y0) / (yc - 1);

            double[] xa = new double[xc], ya = new double[yc];

            for (int i = 0; i < xc; i++)
                xa[i] = x0 + hx * i;
            for (int i = 0; i < yc; i++)
                ya[i] = y0 + hy * i;

            double[][,] val = new double[f.Length][,];
            for (int i = 0; i < f.Length; i++)
            {
                val[i] = new double[xc, yc];

                if(!parallel)
                for (int ix = 0; ix < xc; ix++)
                    for (int iy = 0; iy < yc; iy++)
                    {
                        Point t = new Point(xa[ix], ya[iy]), o = new Point(0);

                        if (filter(t))
                            val[i][ix, iy] = f[i](t);
                        else val[i][ix, iy] = Double.NaN;
                    }
                else             
                    Parallel.For(0, xc, (int ix) => 
                    {
                        for (int iy = 0; iy < yc; iy++)
                        {
                            Point t = new Point(xa[ix], ya[iy]), o = new Point(0);

                            if (filter(t))
                                val[i][ix, iy] = f[i](t);
                            else val[i][ix, iy] = Double.NaN;
                        }
                    });
                

            }

            StreamWriter args = new StreamWriter(filename + "(arg).txt");
            StreamWriter vals = new StreamWriter(filename + "(vals).txt");

            for (int i = 0; i < xc; i++)
                args.WriteLine($"{xa[i]} {ya[i]}");

            for (int i = 0; i < xc; i++)
                for (int j = 0; j < yc; j++)
                {
                    string st = "";
                    for (int s = 0; s < f.Length; s++)
                        st += (val[s][i, j] + " ");
                    vals.WriteLine(st.Replace("NaN", "NA"));
                }


            args.Close();
            vals.Close();
        }
    }

    /// <summary>
    /// B-сплайны дефекта 1
    /// </summary>
    public class BSpline
    {
        /// <summary>
        /// Массив узлов
        /// </summary>
        private CVectors xk;

        private int m => xk.Degree - 1;

        /// <summary>
        /// Создать сплайн по массиву узлов
        /// </summary>
        /// <param name="mas"></param>
        public BSpline(CVectors mas) { xk = mas.dup; }

    }

}