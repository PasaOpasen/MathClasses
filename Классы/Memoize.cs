using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace МатКлассы
{
    /// <summary>
    /// Мемоизированная функция
    /// </summary>
    /// <typeparam name="TVal">Класс аргумента</typeparam>
    /// <typeparam name="TResult">Класс результата</typeparam>
    public sealed class Memoize<TVal, TResult>:IDisposable //where TVal : class, struct//, ICloneable
    {
        private readonly Func<TVal, TResult> M;
        /// <summary>
        /// Текущий словарь
        /// </summary>
        public ConcurrentDictionary<TVal, TResult> dic; 
        /// <summary>
        /// Число элементов в словаре
        /// </summary>
        public int Lenght => dic.Count;

        /// <summary>
        /// Вывести информацию у размерах словаря
        /// </summary>
        public void ShowSizeInfo() => Debug.WriteLine($"Count = {Lenght} \tElementSize = {System.Runtime.InteropServices.Marshal.SizeOf(dic.ToArray())/Lenght}");

        /// <summary>
        /// Только добавить значение в словарь (по идее это должно быть быстрее, чем GetOrAdd)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="res"></param>
        public void OnlyAdd(TVal val, TResult res) => dic.TryAdd(val, res);

        /// <summary>
        /// Очистить словарь
        /// </summary>
        public void Dispose()
        {
            dic.Clear();
           
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Конструктор по обычной функции
        /// </summary>
        /// <param name="Memoize">Исходная функция</param>
        /// <param name="capacity">Начальная ёмкость</param>
        /// <param name="concurrencyLevel">Уровень параллелизма</param>
        public Memoize(Func<TVal, TResult> Memoize, int capacity=0, int concurrencyLevel=0)
        {
            if (concurrencyLevel == 0)
                concurrencyLevel = Environment.ProcessorCount;

            dic=new ConcurrentDictionary<TVal, TResult>(concurrencyLevel,capacity);
            M = new Func<TVal, TResult>(Memoize);
        }

        /// <summary>
        /// Делегат, возвращающий оптимизированную за счёт мемоизации функцию
        /// </summary>
        public Func<TVal, TResult> Value => (TVal val) => dic.GetOrAdd(val,M  /* id => M(id)*/);

    }
}