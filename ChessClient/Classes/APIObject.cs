using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class APIObject
    {
        internal MLAPI API { get; set; }
        
    }
    public class APIObject<T> : APIObject
    {
        public T Id { get; set; }
    }
}
