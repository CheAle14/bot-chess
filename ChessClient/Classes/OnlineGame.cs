using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class OnlineGame : APIObject
    {
        public ChessPlayer White { get; set; }
        public ChessPlayer Black { get; set; }
        public PlayerSide Waiting { get; set; }
    }
}
