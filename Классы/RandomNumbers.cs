namespace МатКлассы
{
    /// <summary>
    /// Класс, выдающий случайное число
    /// </summary>
    public static class RandomNumbers
    {
        private static MathNet.Numerics.Random.CryptoRandomSource r=new MathNet.Numerics.Random.CryptoRandomSource();

        /// <summary>
        /// Случайное положительное число типа int
        /// </summary>
        /// <returns></returns>
        public static int NextNumber() => r.Next();
        /// <summary>
        /// Случайное число int до 0 до ceiling
        /// </summary>
        /// <param name="ceiling"></param>
        /// <returns></returns>
        public static int NextNumber(int ceiling) => r.Next(ceiling);
        /// <summary>
        /// Случаное число от 0 до 1
        /// </summary>
        public static double NextDouble => r.NextDouble();

        /// <summary>
        /// Возвращает случайное число из указанного диапазона
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double NextDouble2(double min = 0, double max = 1) => min + NextDouble * (max - min);
    }
}