using ChessClient.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    public partial class GameForm : Form
    {
        public GameForm(StartForm main)
        {
            Main = main;
            InitializeComponent();
        }

        public StartForm Main;
        public GameBoard Board;
        public OnlineGame Game;

        private void GameForm_Load(object sender, EventArgs e)
        {
            Board = new GameBoard(this);
        }

        private void GameForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Program.DEBUG = !Program.DEBUG;
        }

        public ChessButton firstClick;
        public void Btn_Click(object sender, EventArgs e)
        {
            if (sender is ChessButton btn)
            {
                if (Control.ModifierKeys.HasFlag(Keys.Shift))
                {
                    string move = "Can Move:";
                    foreach (var piece in btn.CanMoveHere)
                    {
                        move += $"\n{piece.Owner} - {piece.Type} @ {piece.Location.Name}";
                    }
                    MessageBox.Show(move);
                    return;
                }
                if(Game.Waiting != Main.Self.Side)
                    return;
                if (firstClick == null)
                {
                    if (btn.PieceHere == null)
                        return;
                    firstClick = btn;
                    firstClick.Evaluate(true);
                }
                else
                {
                    if (firstClick == btn)
                    {
                        firstClick.Evaluate(false);
                    }
                    else if (btn.BackColor == Color.FromKnownColor(KnownColor.Control) || btn.BackColor == Color.Gray)
                    {
                        firstClick.Evaluate(false);
                    }
                    else
                    {
                        var piece = firstClick.PieceHere;
                        firstClick.PieceHere = null;
                        piece.Location = btn;
                        btn.PieceHere = piece;
                        piece.HasMoved = true;
                        Board.Evaluate();
                    }
                    firstClick = null;
                }
            }
        }
    
        public void AuthorativeMove(Packet ping)
        {
            var from = ping.Content["from"].ToObject<string>();
            var fromBtn = Board.GetButtonAt(from);
            var to = ping.Content["to"].ToObject<string>();
            var toBtn = Board.GetButtonAt(to);
            if (toBtn.PieceHere != null)
                toBtn.PieceHere.Location = null;
            toBtn.PieceHere = fromBtn.PieceHere;
            if (fromBtn.PieceHere != null)
                fromBtn.PieceHere.Location = toBtn;
            Board.Evaluate();
        }
    
    }
}
