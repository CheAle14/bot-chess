using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient.Classes
{
    public class GameBoard
    {
        /// <summary>
        /// +
        /// 8
        /// 7
        /// 6
        /// 5
        /// 4
        /// 3
        /// 2
        /// 1
        /// +  A  B  C  D  E  F  G +
        /// </summary>
        ChessButton[,] board;

        public Dictionary<PlayerSide, List<ChessPiece>> Pieces = new Dictionary<PlayerSide, List<ChessPiece>>()
        {
            { PlayerSide.Black, new List<ChessPiece>() },
            { PlayerSide.White, new List<ChessPiece>() }
        };

        public ChessPiece GetPiece(int id)
        {
            foreach(var lst in Pieces.Values)
            {
                foreach(var item in lst)
                {
                    if (item.Id == id)
                        return item;
                }
            }
            return null;
        }

        public List<ChessPiece> CheckingKing { get; set; }

        public string GetReference(int x, int y) 
        {
            string t = "";
            char letter = (char)(x + 65);
            t += letter.ToString();
            return t + y.ToString();
        }

        public Point GetPoint(string reference)
        {
            char letter = reference[0];
            int x = (int)letter - 65;
            int y = int.Parse(reference[1].ToString());
            return new Point(x, y);
        }

        bool isGrey(int x, int y)
        {
            return (x % 2 == 0) != (y % 2 == 0);
        }

        public ChessButton GetButtonAt(string reference) => GetButtonAt(GetPoint(reference));

        public ChessButton GetButtonAt(Point ptn)
        {
            ChessButton btn = null;
            try
            {
                btn = board[ptn.X, ptn.Y - 1];
            } catch { }
            return btn;
        }

        public GameForm Form;

        public GameBoard(GameForm form)
        {
            Form = form;
            board = new ChessButton[8, 8];
            Size btnSize = new Size(64, 64);
            int btnGap = 2;
            Point topLeft = new Point(100, 30);
            form.MinimumSize = new Size((btnSize.Width * 9) + (btnGap * 8) + topLeft.X,
                (btnSize.Height * 9) + (btnGap * 8) + topLeft.Y);
            for(int y = 8; y >= 1; y--)
            {
                for(int x = 0; x <= 7; x++)
                {
                    var btn = new ChessButton(this);
                    btn.Name = GetReference(x, y);
                    btn.Size = btnSize;
                    int true_y = 8 - y;
                    btn.Location = new Point(topLeft.X + (x * btnSize.Width) + (x * btnGap),
                        topLeft.Y + (true_y * btnSize.Height) + (true_y * btnGap));
                    btn.Text = btn.Name;
                    btn.BackColor = isGrey(x, y) ? Color.Gray : Color.FromKnownColor(KnownColor.Control);
                    btn.Click += form.Btn_Click;
                    form.Controls.Add(btn);
                    board[x, y-1] = btn;
                }
            }
            var a8 = GetButtonAt("A8");
            for(int i = 0; i <= 7; i++)
            {
                var lbl = new Label();
                lbl.Text = $"{8 - i}";
                lbl.Size = btnSize;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                lbl.Location = new Point(a8.Location.X - btnSize.Width - 1,
                    a8.Location.Y + (btnSize.Height * i) + (i * btnGap));
                lbl.Name = "lbly" + i.ToString();
                form.Controls.Add(lbl);
            }
            var a1 = GetButtonAt("A1");
            for(int i = 0; i <= 7; i++)
            {
                var lbl = new Label();
                lbl.Text = $"{(char)(i + 65)}";
                lbl.Size = btnSize;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
                lbl.Location = new Point(a1.Location.X + (btnSize.Width * i) + (i * btnGap),
                    a1.Location.Y + 1 + btnSize.Height);
                lbl.Name = "lblx" + i.ToString();
                form.Controls.Add(lbl);
            }

            int PIECE_ID = 0;
            for(int i = 1; i <= 16; i++)
            {
                // set pawns
                int num = i <= 8 ? 2 : 7;
                var refer = GetReference(i <= 8 ? i-1 : i-9, num);
                var btn = GetButtonAt(refer);
                var piece = new ChessPiece(this)
                {
                    Owner = i <= 8 ? PlayerSide.White : PlayerSide.Black,
                    Location = btn,
                    Type = PieceType.Pawn,
                    Id = PIECE_ID++,
                };
                Pieces[piece.Owner].Add(piece);
                btn.PieceHere = piece;
                btn.Text = piece.Type.ToString();
            }
            for(int i = 0; i <= 7; i++)
            {
                var black = GetReference(i, 8);
                var white = GetReference(i, 1);
                foreach(var thing in new string[] { black, white})
                {
                    var btn = GetButtonAt(thing);
                    var piece = new ChessPiece(this)
                    {
                        Location = btn,
                        Id = PIECE_ID++,
                    };
                    if (i == 0 || i == 7)
                        piece.Type = PieceType.Rook;
                    else if (i == 1 || i == 6)
                        piece.Type = PieceType.Knight;
                    else if (i == 2 || i == 5)
                        piece.Type = PieceType.Bishop;
                    else if (i == 3)
                        piece.Type = PieceType.Queen;
                    else
                        piece.Type = PieceType.King;
                    piece.Owner = thing.EndsWith("1") ? PlayerSide.White : PlayerSide.Black;
                    Pieces[piece.Owner].Add(piece);
                    btn.PieceHere = piece;
                    btn.Text = $"{piece.Owner}\n{piece.Type}";
                }
            }
            Evaluate();
        }

        public void Evaluate()
        {
            CheckingKing = new List<ChessPiece>();
            foreach(var x in board)
                x.Reset();
            foreach (var btn in board)
                btn.Evaluate(false);
        }
    }
}
