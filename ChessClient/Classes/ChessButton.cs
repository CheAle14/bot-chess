using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient.Classes
{
    public class ChessButton : Button
    {
        public ChessButton(GameBoard b)
        {
            Board = b;
        }
        GameBoard Board;
        public Pin WhitePins { get; set; }
        public Pin BlackPins { get; set; }
        public ChessPiece PieceHere { get; set; }

        public Pin GetPins(PlayerSide side) => side == PlayerSide.White ? WhitePins : BlackPins;
        public Pin SetPins(PlayerSide side, Pin pins)
        {
            if (side == PlayerSide.White)
                WhitePins |= pins;
            else
                BlackPins |= pins;
            return GetPins(side);
        }

        public PlayerSide Threats { get; set; }

        public List<ChessPiece> CanMoveHere { get; set; }

        public int CenterX => this.Width / 2;
        public int CenterY => this.Height / 2;

        public override Color ForeColor => Color.Red;

        public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }

        Color getDefaultColor()
        {
            int code = ((int)this.Name[0]) - 65;
            int num = int.Parse(Name[1].ToString());
            return ((code % 2 == 0) != (num % 2 == 0)) ? Color.Gray : Color.FromKnownColor(KnownColor.Control);
        }

        public void Reset()
        {
            CanMoveHere = new List<ChessPiece>();
            this.BackColor = getDefaultColor();
            WhitePins = Pin.None;
            BlackPins = Pin.None;
            Threats = PlayerSide.None;
            this.BackgroundImage = PieceHere?.Image ?? null;
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        public ChessButton GetRelative(PlayerSide side, int right, int forward)
        {
            var coord = Board.GetPoint(this.Name);
            int direction = side == PlayerSide.White ? 1 : -1;
            var newCoord = new Point(coord.X + (right * direction), coord.Y + (forward * direction));
            return Board.GetButtonAt(newCoord);
        }

        public ChessButton GetForward(PlayerSide side, int count = 1)
        {
            return GetRelative(side, 0, 1);
        }

        public ChessButton GetLeft(PlayerSide side, int count = 1)
        {
            return GetRelative(side, -1 * count, 0);
        }

        public ChessButton GetRight(PlayerSide side, int count = 1)
        { 
            return GetRelative(side, count, 0);
        }

        public ChessButton GetBack(PlayerSide side, int count = 1)
        { 
            return GetRelative(side, 0, -1 * count);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (!Program.DEBUG)
                return;
            var pen = new Pen(Color.Red, 3);
            var Pins = WhitePins | BlackPins;
            if (Pins.HasFlag(Pin.Horizontal))
            {
                pevent.Graphics.DrawLine(pen, 0, CenterY, this.Width, CenterY);
            }
            if(Pins.HasFlag(Pin.Vertical))
            {
                pevent.Graphics.DrawLine(pen, CenterX, 0, CenterX, Height);
            }
            if(Pins.HasFlag(Pin.LeftDiagonal))
            {
                pevent.Graphics.DrawLine(pen, 0, 0, Width, Height);
            }
            if(Pins.HasFlag(Pin.RightDiagonal))
            {
                pevent.Graphics.DrawLine(pen, 0, Height, Width, 0);
            }
        }
    
        public void Evaluate(bool highlight)
        {
            if (PieceHere == null)
                return; // nothing to do as nothing can move
            if (PieceHere.Type == PieceType.Pawn)
                evalPawn(highlight);
            if (PieceHere.Type == PieceType.Rook || PieceHere.Type == PieceType.Queen)
                evalVerticals(highlight);
            if (PieceHere.Type == PieceType.Bishop || PieceHere.Type == PieceType.Queen)
                evalDiagonals(highlight);
            if (PieceHere.Type == PieceType.King)
                evalKing(highlight);
        }

        public void SetCanMove(ChessPiece piece, bool highlight)
        {
            this.BackColor = getDefaultColor();
            if(PieceHere == null)
            {
                if(highlight)
                    this.BackColor = Color.Blue;
            } else
            {
                if(piece.Owner != PieceHere.Owner)
                {
                    if(highlight)
                        this.BackColor = Color.Red;
                } else
                {
                    return;
                }
            }
            if(!CanMoveHere.Contains(piece))
                CanMoveHere.Add(piece);
            Threats |= piece.Owner;
        }

        public override string Text
        {
            get
            {
                if (!Program.DEBUG)
                    return "";
                string t = Name;
                if(PieceHere != null)
                {
                    t += $"\n{PieceHere.Owner.ToString()[0]} {PieceHere.Type}";
                }
                if (Threats.HasFlag(PlayerSide.White))
                    t += "\nACK W";
                if (Threats.HasFlag(PlayerSide.Black))
                    t += "\nACK B";
                return t;
            }
            set { }
        }

        void evalDiagonals(bool highlight)
        {
            var upRight = new int[2] { 1, 1 };
            var downRight = new int[2] { 1, -1 };
            var upLeft = new int[2] { -1, 1};
            var downLeft = new int[2] { -1, -1};
            var dirList = new List<int[]>() { upRight, upLeft, downRight, downLeft };
            foreach(int[] direction in dirList)
            {
                ChessButton lastValid = null;
                var blocked = false;
                int amountInWay = 0;
                var currentlyPinning = direction[0] - direction[1] == 0 ? Pin.RightDiagonal : Pin.LeftDiagonal;
                for (int i = 1; i <= 8; i++)
                {
                    var btnAt = GetRelative(PieceHere.Owner, i * direction[0], i * direction[1]);
                    if (btnAt == null)
                        continue;
                    if (btnAt.Name == this.Name)
                        continue;
                    if (!blocked && btnAt.PieceHere?.Owner != PieceHere.Owner)
                    {
                        var opposite = (PlayerSide)((int)PieceHere.Owner ^ 0b11);
                        var pinsOnUs = GetPins(opposite);
                        if (currentlyPinning != pinsOnUs)
                        {
                            btnAt.SetCanMove(PieceHere, highlight);
                            lastValid = btnAt;
                        }
                    }
                    if (btnAt.PieceHere != null)
                    {
                        if (btnAt.PieceHere.Owner == PieceHere.Owner)
                        {
                            btnAt.Threats |= PieceHere.Owner; // eg, so King cannot take this piece if we defend it
                            break;
                        }
                        blocked = true;
                        amountInWay++;
                        if (btnAt.PieceHere.Type == PieceType.King && btnAt.PieceHere.Owner != PieceHere.Owner)
                        {
                            if (amountInWay == 2)
                            {
                                lastValid.SetPins(PieceHere.Owner, currentlyPinning);
                            }
                        }
                    }
                    if (amountInWay > 1)
                        break;
                }
            }
        }

        void evalVerticals(bool highlight)
        {
            var down = new int[2] { 0, -1 };
            var up = new int[2] { 0, 1 };
            var right = new int[2] { 1, 0 };
            var left = new int[2] { -1, 0 };
            var dirList = new List<int[]>() { left, right, up, down };
            foreach(int[] direction in dirList)
            { // 1 = forward (or left), -1 = back (or right)
                ChessButton lastValid = null;
                var blocked = false;
                int amountInWay = 0;
                var currentlyPinning = direction[0] == 0 ? Pin.Vertical : Pin.Horizontal;
                for (int i = 1; i <= 8; i++)
                {
                    var btnAt = GetRelative(PieceHere.Owner, i * direction[0], i * direction[1]);
                    if (btnAt == null)
                        continue;
                    if (btnAt.Name == this.Name)
                        continue;
                    if(!blocked && btnAt.PieceHere?.Owner != PieceHere.Owner)
                    {
                        var opposite = (PlayerSide)((int)PieceHere.Owner ^ 0b11);
                        var pinsOnUs = GetPins(opposite);
                        if(currentlyPinning != pinsOnUs)
                        {
                            btnAt.SetCanMove(PieceHere, highlight);
                            lastValid = btnAt;
                        }
                    }
                    if(btnAt.PieceHere != null)
                    {
                        if(btnAt.PieceHere.Owner == PieceHere.Owner)
                        {
                            btnAt.Threats |= PieceHere.Owner; // eg, so King cannot take this piece if we defend it
                            break;
                        }
                        blocked = true;
                        amountInWay++;
                        if(btnAt.PieceHere.Type == PieceType.King && btnAt.PieceHere.Owner != PieceHere.Owner)
                        {
                            if(amountInWay == 2)
                            {
                                lastValid.SetPins(PieceHere.Owner, currentlyPinning);
                            }
                        }
                    }
                    if (amountInWay > 1)
                        break;
                }
            }
        }

        void evalKing(bool highlight)
        {
            var btns = new ChessButton[8];
            int i = 0;
            btns[i++] = GetRelative(PieceHere.Owner, -1, 1); // f-l
            btns[i++] = GetRelative(PieceHere.Owner, 0, 1); // forward
            btns[i++] = GetRelative(PieceHere.Owner, 1, 1); // f-r

            btns[i++] = GetRelative(PieceHere.Owner, -1, 0); // left
            btns[i++] = GetRelative(PieceHere.Owner, 1, 0); // right

            btns[i++] = GetRelative(PieceHere.Owner, -1, -1); // back-l
            btns[i++] = GetRelative(PieceHere.Owner,  0, -1); // back
            btns[i++] = GetRelative(PieceHere.Owner,  1, -1); // back-r
            foreach(var item in btns)
            {
                if(item != null)
                {
                    if(item.PieceHere != null)
                    {
                        if (item.PieceHere.Owner == PieceHere.Owner)
                            continue; // cant move to own position
                    }
                    if(!item.Threats.HasFlag(PieceHere.Opponent))
                    {
                        item.SetCanMove(PieceHere, highlight);
                    }
                }
            }
        }

        void evalPawn(bool highlight)
        {
            var pins = this.GetPins(PieceHere.Opponent);
            if(pins.HasFlag(Pin.Horizontal) || pins.HasFlag(Pin.LeftDiagonal) || pins.HasFlag(Pin.RightDiagonal)) 
                return; // cant move at all
            var forward = this.GetForward(PieceHere.Owner);
            forward?.SetCanMove(PieceHere, highlight);
            if(!PieceHere.HasMoved)
            {
                forward?.GetForward(PieceHere.Owner)?.SetCanMove(PieceHere,highlight);
            }
            // can move forward, but now we must verify vertical pins since we're aboutt to move off the file:
            if(pins.HasFlag(Pin.Vertical))
                return;
            var left = forward?.GetLeft(PieceHere.Owner);
            var right = forward?.GetRight(PieceHere.Owner);
            foreach(var thing in new ChessButton[] { left, right})
            {
                if(thing != null)
                {
                    if(thing.PieceHere != null)
                    {
                        if(thing.PieceHere.Owner != PieceHere.Owner)
                        {
                            thing.SetCanMove(PieceHere, highlight);
                        }
                    }
                }
            }
        }
    }
}
