using System;

namespace МатКлассы
{
    /// <summary>
    /// Многоугольник на плоскости
    /// </summary>
    public class Polygon
    {
        private Point center;
        private Point[] vertexes;
        private double corner, r, a;

        private Line2D[] lines;
        private PointFunc[] transfs;
        private void MakeLines()
        {
            lines = new Line2D[VertCount - 1];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = new Line2D(vertexes[i], vertexes[i + 1]);
                transfs[i] = new PointFunc(new Cut(vertexes[i], vertexes[i + 1]).Transfer);
            }

        }

        /// <summary>
        /// Центр многоугольника
        /// </summary>
        public Point Center => new Point(center);
        /// <summary>
        /// Вершины многоугольника
        /// </summary>
        public Point[] Vertexes
        {
            get
            {
                Point[] res = new Point[vertexes.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Point(vertexes[i]);
                return res;

            }
        }
        /// <summary>
        /// Количество вершин многоугольника
        /// </summary>
        public int VertCount => vertexes.Length;
        /// <summary>
        /// Периметр многоугольника
        /// </summary>
        public double Perimeter => a * VertCount;
        /// <summary>
        /// Длина стороны многоугольника
        /// </summary>
        public double CutLength => a;
        /// <summary>
        /// Площадь многоугольника
        /// </summary>
        public double S => VertCount * r * r / 2 * Math.Sin(corner);

        /// <summary>
        /// Создание многоугольника по его параметрам
        /// </summary>
        /// <param name="center">Центр многоугольника</param>
        /// <param name="vertcount">Число вершин</param>
        /// <param name="sidelenght">Длина стороны</param>
        /// <param name="somecorner">Угол между осью Х и отрезком, соединяющим центр многоугольника с какой-то его вершиной</param>
        public Polygon(Point center, int vertcount = 3, Double sidelenght = 1, double somecorner = 0)
        {
            int n = vertcount;
            corner = 2 * Math.PI / n;
            r = sidelenght / Math.Sqrt(2 * (1 - Math.Cos(corner)));
            this.center = new Point(center);
            this.vertexes = new Point[n];
            for (int i = 0; i < n; i++)
                vertexes[i] = new Point(center.x + r * Math.Cos(somecorner + i * corner), center.y + r * Math.Sin(somecorner + i * corner));
            a = sidelenght;
            MakeLines();
        }

        /// <summary>
        /// Возвращает точку на кривой в зависимости от значения естественного параметра
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Point Transfer(double t)
        {
            double div = t / a;
            int tmp = (int)Math.Ceiling(div);
            return transfs[tmp](t - a * tmp);

        }

        [Obsolete]
        /// <summary>
        /// Преобразование многоугольника в кривую (для интегрирования) 
        /// </summary>
        /// <param name="p"></param>
        /// <remarks>Преобразование красивое, но не очень хорошее, так как многие данные придётся вычислять повторно</remarks>
        public static implicit operator Curve(Polygon p) => new Curve(0, p.Perimeter,
            (double t) => p.Transfer(t).x, (double t) => p.Transfer(t).y, p.a,
         (double t, double r) => new Polygon(p.Center, p.VertCount, r, p.corner).Transfer(t).x,
         (double t, double r) => new Polygon(p.Center, p.VertCount, r, p.corner).Transfer(t).y,
         (double a, double b, double r) =>
         {
             Polygon t1 = new Polygon(p.Center, p.VertCount, p.a + b / 2, p.corner);
             Polygon t2 = new Polygon(p.Center, p.VertCount, p.a - b / 2, p.corner);
             return (t1.S - t2.S) * a / p.Perimeter;
         },
         (double t) => t * p.VertCount);
        /// <summary>
        /// Перевод многоугольника в кривую по более оптимальному алгоритму, рассчитанному на интегрирование
        /// </summary>
        public Curve ToCurve => new Curve(0, this.Perimeter, (double t, double r) => new Polygon(this.Center, this.VertCount, r, this.corner).Transfer(t), this.a,
                     (double a, double b, double r) =>
                     {
                         Polygon t1 = new Polygon(this.Center, this.VertCount, this.a + b / 2, this.corner);
                         Polygon t2 = new Polygon(this.Center, this.VertCount, this.a - b / 2, this.corner);
                         return (t1.S - t2.S) * a / this.Perimeter;
                     },
         (double t) => t * this.VertCount);
    }
}