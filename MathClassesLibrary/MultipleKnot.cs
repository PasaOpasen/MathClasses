
namespace МатКлассы
{
    /// <summary>
    /// Класс кратных узлов
    /// </summary>
    public class MultipleKnot
    {
        /// <summary>
        /// Абцисса кратного узла
        /// </summary>
        public double x;
        /// <summary>
        /// Массив ординат кратного узла
        /// </summary>
        public double[] y;

        /// <summary>
        /// Полный конструктор
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MultipleKnot(double x, params double[] y)
        {
            this.x = x;
            this.y = new double[y.Length];
            for (int i = 0; i < y.Length; i++) this.y[i] = y[i];
        }

        /// <summary>
        /// Кратность узла
        /// </summary>
        public int Multiplicity { get { return this.y.Length; } }

    }
}

