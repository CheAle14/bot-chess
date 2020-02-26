using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class CircularQueue<T>
    {
        protected T[] _array;
        private int next;

        public int Length { get; set; }
        public int Items { get; set; }

        public CircularQueue(int length)
        {
            Length = length;
            _array = new T[length];
            next = 0;
        }

        public void Add(T item)
        {
            _array[next] = item;
            next = (next + 1) % Length;
            Items++;
        }

        public T Remove()
        {
            var item = _array[next];
            next = (next - 1) % Length;
            Items--;
            return item;
        }
    }
}
