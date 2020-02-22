using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException(string m, Exception inner) : base(m, inner) { }
        public IllegalStateException(string m) : this(m, null) { }
    }
}
