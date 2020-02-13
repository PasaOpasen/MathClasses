using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Интерфейс наличия дубликата как метода-свойства
    /// </summary>
   public interface Idup<T>
    {
       T dup { get; }
        void MoveTo(T t);
    }
}
