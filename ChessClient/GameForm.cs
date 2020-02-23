using ChessClient.Classes;
using Newtonsoft.Json.Linq;
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
            UpdateUI();
        }

        public void UpdateUI()
        {
            lblWhite.Text = Main.Game.White?.Name ?? "Waiting for White";
            lblBlack.Text = Main.Game.Black?.Name ?? "Waiting for Black";
            if(Main.Self == null)
            {
                this.Text = "Your account is unknown";
            } else
            {
                this.Text = $"You are {(Main.Self.Side == PlayerSide.None ? "spectating" : Main.Self.Side.ToString())}";
            }
            var wait = Main.Game?.Waiting ?? PlayerSide.None;
            lblWhite.ForeColor = wait == PlayerSide.White ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
            lblBlack.ForeColor = wait == PlayerSide.Black ? Color.Red : Color.FromKnownColor(KnownColor.ControlText);
            Main.AdminForm?.UpdateUI();
        }

        public StartForm Main;
        public GameBoard Board;

        private void GameForm_Load(object sender, EventArgs e)
        {
            Board = new GameBoard(this);
        }

        private void GameForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Program.DEBUG = !Program.DEBUG;
        }

        public ChessButton firstClick;
        public bool handlingMove = false;
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
                if(Main.Game.Waiting != Main.Self.Side)
                    return;
                if (handlingMove)
                    return;
                if (firstClick == null)
                {
                    if (btn.PieceHere == null)
                        return;
                    if (btn.PieceHere?.Owner != Main.Self.Side)
                        return; // cant move opponent's pieces
                    firstClick = btn;
                    firstClick.Evaluate(true);
                }
                else
                {
                    if (firstClick == btn)
                    {
                        firstClick.Evaluate(false);
                    }
                    else if (btn.BackColor == Color.FromKnownColor(KnownColor.Control) || btn.BackColor == Color.Gray || btn.BackColor == Color.Purple)
                    {
                        firstClick.Evaluate(false);
                    }
                    else
                    {
                        if(willKingStillCheck(firstClick, btn))
                        {
                            MessageBox.Show("Your King is in Check!\nYou must defend your king.");
                            return;
                        }
                        handlingMove = true;
                        var jobj = new JObject();
                        jobj["from"] = firstClick.Name;
                        jobj["to"] = btn.Name;
                        StartForm.Send(new Packet(PacketId.MoveRequest, jobj));
                    }
                    firstClick = null;
                }
            }
        }

        bool willKingStillCheck(ChessButton from, ChessButton to)
        {
            if (Board.CheckingKing.Count == 0)
                return false;
            Dictionary<int, ChessButton> original = new Dictionary<int, ChessButton>();
            original[from.PieceHere.Id] = from;
            if (to.PieceHere != null)
            {
                original[to.PieceHere.Id] = to;
                to.PieceHere.Location = null;
            }
            to.PieceHere = from.PieceHere;
            from.PieceHere.Location = to;
            var ourKing = Board.Pieces[Main.Self.Side].FirstOrDefault(x => x.Type == PieceType.King);
            ourKing.Location.Reset();
            foreach(var t in Board.CheckingKing)
            {
                t?.Location?.Evaluate(false);
            }
            foreach (var t in original.Values)
                t.Evaluate(false);
            var can = ourKing.Location.CanMoveHere.Count > 0;
            from.PieceHere = null;
            to.PieceHere = null;
            foreach(var t in original)
            {
                var piece = Board.GetPiece(t.Key);
                t.Value.PieceHere = piece;
                piece.Location = t.Value;
            }
            Board.Evaluate();
            return can;
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
            fromBtn.PieceHere = null;
            toBtn.PieceHere.HasMoved = true;
            Main.Game.Waiting = (PlayerSide)((int)Main.Game.Waiting ^ 0b11);
            Board.Evaluate();
        }
    
    }
}
