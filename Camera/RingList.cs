using System;
using System.Collections;

namespace Task2
{
    /// <summary>
    /// Кольцевой список. Нужен для переключения между id камер по кругу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingList<T>
    {
        IEnumerator enumerator;

        public RingList(T[] array)
        {
            enumerator = array.GetEnumerator();
        }

        /// <summary>
        /// Возвращает следующий элемент
        /// </summary>
        /// <returns></returns>
        public T Next()
        {
            if (!enumerator.MoveNext())
            {
                enumerator.Reset();
                enumerator.MoveNext();
            }

            return (T)enumerator.Current;
        }
    }
}