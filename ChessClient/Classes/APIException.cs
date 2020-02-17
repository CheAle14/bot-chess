using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class APIException : Exception
    {
        public string Call { get; set; }
        public APIException(string call, string message) : base(message) { }
    }
}
