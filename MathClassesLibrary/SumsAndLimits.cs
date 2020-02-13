using System;
using System.Linq;
using System.Threading.Tasks;
using static МатКлассы.Number;

namespace МатКлассы
{
    /// <summary>
    /// Класс методов суммирования
    /// </summary>
    public static class SumsAndLimits
    {
        /// <summary>
        /// Сумма частичного ряда
        /// </summary>
        /// <param name="n0">Начальный номер члена ряда</param>
        /// <param name="nmax">Конечный номер члена ряда</param>
        /// <param name="f">Функция, сопоставляющая член ряда по номеру</param>
        /// <returns></returns>
        public static Complex Sum(int n0, int nmax, Func<int, Complex> f)
        {
            int tmp = nmax - n0 + 1;
            Complex[] mas = new Complex[tmp];
            for (int i = 0; i < tmp; i++)
                mas[i] = f(n0 + i);
            Array.Sort(mas);
            return mas.Sum();
        }
        /// <summary>
        /// Сумма бесконечного ряда
        /// </summary>
        /// <param name="n0">Начальный номер члена ряда</param>
        /// <param name="f">Функция, сопоставляющая член ряда номеру</param>
        /// <param name="eps">Точность</param>
        /// <param name="ndo">Минимальное количество членов, которые должны быть суммированы</param>
        /// <param name="ndomax">Максимальное число суммируемых членов</param>
        /// <returns></returns>
        public static Complex Sum(int n0, Func<int, Complex> f, double eps = 1e-8, int ndo = 10, int ndomax = 100)
        {
            Complex sum = Sum(n0, n0 + ndo, f);
            int i = 1;
            Complex sum2 = 0;
            Complex tmp = f(n0 + ndo + i);
            do
            {
                sum2 += tmp;
                i++;
                tmp = f(ndo + i);
            }
            while (tmp.Abs >= eps && i + ndo <= ndomax);
            return sum + sum2;
        }
        /// <summary>
        /// Сумма ряда от -inf до inf
        /// </summary>
        /// <param name="f">Функция, сопоставляющая член ряда номеру</param>
        /// <param name="eps">Точность</param>
        /// <param name="ndo">Минимальное количество членов, которые должны быть суммированы в одну сторону</param>
        /// <returns></returns>
        public static Complex Sum(Func<int, Complex> f, double eps = 1e-8, int ndo = 10)
        {
            Func<int, Complex> f2 = (int n) => f(-n);
            Complex sum1 = 0, sum2 = 0;
            Parallel.Invoke(() => sum1 = Sum(0, f, eps, ndo), () => sum2 = Sum(1, f2, eps, ndo));
            return sum1 + sum2;
        }

        /// <summary>
        /// Сумма частичного ряда
        /// </summary>
        /// <param name="n0">Начальный номер члена ряда</param>
        /// <param name="nmax">Конечный номер члена ряда</param>
        /// <param name="f">Функция, сопоставляющая член ряда по номеру</param>
        /// <returns></returns>
        public static CVectors Sum(int n0, int nmax, Func<int, CVectors> f)
        {
            int tmp = nmax - n0 + 1;
            CVectors[] mas = new CVectors[tmp];
            for (int i = 0; i < tmp; i++)
                mas[i] = f(n0 + i);
            Array.Sort(mas);
            CVectors sum = new CVectors(mas[0].Degree);
            for (int i = 0; i < sum.Degree; i++)
                sum += mas[i];
            return sum;
        }
        /// <summary>
        /// Сумма бесконечного ряда
        /// </summary>
        /// <param name="n0">Начальный номер члена ряда</param>
        /// <param name="f">Функция, сопоставляющая член ряда номеру</param>
        /// <param name="eps">Точность</param>
        /// <param name="ndo">Минимальное количество членов, которые должны быть суммированы</param>
        /// <returns></returns>
        public static CVectors Sum(int n0, Func<int, CVectors> f, double eps = 1e-8, int ndo = 10, int ndomax = 1000)
        {
            CVectors sum = Sum(n0, n0 + ndo, f);
            int i = 1;
            CVectors sum2 = new CVectors(sum.Degree);
            CVectors tmp = f(n0 + ndo + i);
            do
            {
                sum2 += tmp;
                i++;
                tmp = f(ndo + i);
            }
            while (tmp.Abs >= eps && i + ndo <= ndomax);
            return sum + sum2;
        }
        /// <summary>
        /// Сумма ряда от -inf до inf
        /// </summary>
        /// <param name="f">Функция, сопоставляющая член ряда номеру</param>
        /// <param name="eps">Точность</param>
        /// <param name="ndo">Минимальное количество членов, которые должны быть суммированы в одну сторону</param>
        /// <returns></returns>
        public static CVectors Sum(Func<int, CVectors> f, double eps = 1e-8, int ndo = 10)
        {
            Func<int, CVectors> f2 = (int n) => f(-n);
            CVectors sum1 = null, sum2 = null;
            Parallel.Invoke(() => sum1 = Sum(0, f, eps, ndo), () => sum2 = Sum(1, f2, eps, ndo));
            return sum1 + sum2;
        }

        /// <summary>
        /// Вычисление предела комплексной матричной функции в точке через окружности
        /// </summary>
        /// <param name="f">Функция</param>
        /// <param name="point">Точка, где нужно найти предел</param>
        /// <param name="begeps">Начальный радиус</param>
        /// <param name="count">Число усредняющихся точек</param>
        /// <returns></returns>
        public static CSqMatrix LimitCircle(Func<Complex,CSqMatrix> f,Complex point, double begeps=0.1,int count = 10,int kmax=40)
        {
            double h = 2 * Math.PI / (count - 1),arg;
            double[] cos = new double[count],sin=new double[count];
            for (int i = 0; i < count; i++)
            {
                arg = i * h;
                cos[i] = Math.Cos(arg);
                sin[i] = Math.Sin(arg);
            }
                
            CSqMatrix[] mas = new CSqMatrix[count],tmp;
            CSqMatrix res=f(point+2),sum;
            while (true)
            {
                for (int i = 0; i < count; i++)               
                    mas[i] = f(new Complex(point.Re+begeps*cos[i], point.Im + begeps * sin[i]));
                tmp = mas.Where(n => !Double.IsNaN(n.CubeNorm)).ToArray();

                if (tmp.Length == 0) return res;
                
                sum =new CSqMatrix( tmp[0]);
                for (int i = 1; i < tmp.Length; i++)
                    sum += tmp[i];
                sum /=tmp.Length; 
                res = new CSqMatrix(sum);
                begeps /= 2;
                kmax--;
                if (kmax == 0) return res;
            }
        }
        /// <summary>
        /// Вычисление предела комплексной матричной функции в точке при случайном разбросе
        /// </summary>
        /// <param name="f">Функция</param>
        /// <param name="point">Точка, где нужно найти предел</param>
        /// <param name="radius">Радиус приближения</param>
        /// <param name="count">Число усредняющихся точек</param>
        /// <returns></returns>
        public static CSqMatrix LimitRandom(Func<Complex, CSqMatrix> f, Complex point, double radius = 0.01, int count = 100,bool onlyReplusAxis=true)
        {
            double p = 2 * Math.PI,ro,fi;       
            CSqMatrix[] mas = new CSqMatrix[count];
            CSqMatrix sum;
            Random r = new Random();

            if(onlyReplusAxis)
            for (int i = 0; i < count; i++)
            {
                ro = r.NextDouble() * radius;
                mas[i] = f(new Complex(point.Re + ro, point.Im));
            }
            else
            for (int i = 0; i < count; i++)
            {
                ro = r.NextDouble() * radius;
                fi = r.NextDouble() * p;
                mas[i] = f(new Complex(point.Re + ro * Math.Cos(fi), point.Im + ro * Math.Sin(fi)));
            }

            mas = mas.Where(n => !Double.IsNaN(n.CubeNorm)).ToArray();
            if (mas.Length == 0) return LimitRandom(f, point, radius * 2, count, onlyReplusAxis);
            sum = new CSqMatrix(mas[0]);
            for (int i = 1; i < mas.Length; i++)
                sum += mas[i];
            (sum / mas.Length).Show();
            return sum / mas.Length;
        }
    }
}