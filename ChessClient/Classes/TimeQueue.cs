using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class TimeQueue : CircularQueue<long>
    {
        public TimeQueue(int length) : base(length) { }
        public long GetSum() => _array.Sum();
        public double GetAverage() => GetSum() / (double)Length;
    }
}
