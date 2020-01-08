using System.Collections.Generic;

namespace Task2
{
    /// <summary>
    /// Кольцевой список. Нужен для переключения между id камер по кругу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingList<T>
    {
        LinkedList<T> _list;
        LinkedListNode<T> _current;

        public RingList(T[] array)
        {
            _list = new LinkedList<T>(array);
            _current = _list.First;
        }

        public T Next
        {
            get
            {
                var value = _current.Value;

                if (_current.Next != null)
                    _current = _current.Next;
                else
                    _current = _list.First;

                return value;
            }
        }

        public T Previous
        {
            get
            {
                if (_current.Previous != null)
                    _current = _current.Previous;
                else
                    _current = _list.Last;

                return _current.Value;
            }
        }
    }
}