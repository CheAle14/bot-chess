using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class Move
    {
        public ChessButton From;
        public ChessButton To;
        public ChessPiece Piece;
        public ChessPiece Taken;
    }
}
