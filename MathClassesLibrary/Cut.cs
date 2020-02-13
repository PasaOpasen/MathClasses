using System;

namespace МатКлассы
{
    /// <summary>
    /// Класс отрезков на плоскости
    /// </summary>
    public class Cut
    {
        private Point First, Second;
        private Line2D line;


        /// <summary>
        /// Первая точка на прямой (начало отрезка)
        /// </summary>
        public Point FirstPoint => new Point(First);
        /// <summary>
        /// Вторая точка на прямой (конец отрезка)
        /// </summary>
        public Point SecondPoint => new Point(Second);
        /// <summary>
        /// Прямая, на которой лежит отрезок
        /// </summary>
        public Line2D Line => new Line2D(line);
        /// <summary>
        /// Длина отрезка
        /// </summary>
        public double Length => Point.Eudistance(First, Second);

        /// <summary>
        /// Задать отрезок по его концам
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Cut(Point first, Point second)
        {
            this.line = new Line2D(first, second, Line2D.EquType.Normal);
            First = new Point(first);
            Second = new Point(second);
        }

        /// <summary>
        /// Выводит точку на отрезке, соответствующую естественному параметру
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Point Transfer(double t)
        {
            if (t > this.Length) throw new Exception("Параметр выходит за границы");
            double k = line.Corner_k;
            if (k == 0) return new Point(First.x + t * Math.Sign(Second.x - First.x), First.y);
            if (k == Double.PositiveInfinity) return new Point(First.x, First.y + t * Math.Sign(Second.y - First.y));



            //если прямая не параллельна какой-либо оси          
            double cor = Math.Atan(k);
            double x = First.x + t * Math.Cos(cor);
            return new Point(x, line.Func(x));
        }

    }
}