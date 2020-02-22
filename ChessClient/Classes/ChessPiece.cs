using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class ChessPiece
    {
        public ChessPiece(GameBoard b)
        {
            Board = b;
        }
        public int Id { get; set; }
        GameBoard Board;
        public PlayerSide Owner { get; set; }
        public PlayerSide Opponent => (PlayerSide)((int)Owner ^ 0b11);
        public PieceType Type { get; set; }
        public ChessButton Location { get; set; }
        public bool HasMoved { get; set; }
        public Image Image => (Image)Properties.Resources.ResourceManager.GetObject($"{Owner.ToString()[0]}_{Type}");
    }
}
