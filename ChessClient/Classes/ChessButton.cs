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
            if (this.FlatAppearance.BorderColor == getDefaultColor())
                this.FlatAppearance.BorderColor = Color.Orange;
            return GetPins(side);
        }

        public PlayerSide Threats { get; set; }

        public List<ChessPiece> CanMoveHere { get; set; }

        public int CenterX => this.Width / 2;
        public int CenterY => this.Height / 2;

        public override Color ForeColor => Color.Red;

        public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }

        public Color getDefaultColor()
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
            if (this.FlatAppearance.BorderColor != Color.Red)
                this.FlatAppearance.BorderColor = getDefaultColor();
        }

        public ChessButton GetRelative(PlayerSide side, int right, int forward, bool absolute = false)
        {
            var coord = Board.GetPoint(this.Name);
            int direction = absolute ? 1 : side == PlayerSide.White ? 1 : -1;
            var newCoord = new Point(coord.X + right, coord.Y + (forward * direction));
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
            try
            {
                base.OnPaint(pevent);
            }
            catch { }
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
            {
                evalPawn(highlight);
                return;
            }
            if(PieceHere.Type == PieceType.Knight)
            {
                evalHorse(highlight);
                return;
            }
            if(PieceHere.Type == PieceType.King)
            {
                evalKing(highlight);
                return;
            }
            if (PieceHere.Type == PieceType.Rook || PieceHere.Type == PieceType.Queen)
                evalVerticals(highlight);
            if (PieceHere.Type == PieceType.Bishop || PieceHere.Type == PieceType.Queen)
                evalDiagonals(highlight);
        }

        public void SetCanMove(ChessPiece piece, bool highlight, bool threaten = true)
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
                    if(PieceHere.Type == PieceType.King)
                    {
                        if(!Board.CheckingKing.Contains(piece))
                            Board.CheckingKing.Add(piece);
                        piece.Location.FlatAppearance.BorderColor = Color.Purple;
                    }
                    if(highlight)
                        this.BackColor = Color.Red;
                } else
                {
                    return;
                }
            }
            if(!CanMoveHere.Contains(piece))
                CanMoveHere.Add(piece);
            if(threaten)
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
                    var btnAt = GetRelative(PieceHere.Owner, i * direction[0], i * direction[1], true);
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

        void evalHorse(bool highlight)
        {
            if (GetPins(PieceHere.Opponent) != Pin.None)
                return; // cannot move under any pin.
            int[] topL = new int[] { -1, 2 };
            int[] topR = new int[] { 1, 2 };
            int[] rightU = new int[] { 2, 1 };
            int[] rightD = new int[] { 2, -1 };
            int[] bottomL = new int[] { -1, -2 };
            int[] bottomR = new int[] { 1, -2 };
            int[] leftU = new int[] { -2, 1 };
            int[] leftD = new int[] { -2, -1 };
            foreach(var direction in new List<int[]> { topL, topR, rightU, rightD, bottomL, bottomR, leftU, leftD})
            {
                var loc = GetRelative(PieceHere.Owner, direction[0], direction[1]);
                if(loc == null)
                    continue;
                loc.SetCanMove(PieceHere, highlight);
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

        void evalLeftCastle(bool highlight)
        {
            var rook = GetLeft(PieceHere.Owner, 4);
            if (rook == null || rook.PieceHere == null || rook.PieceHere.HasMoved)
                return;
            // castle left moves king from Ex to Cx
            var adjacant = GetLeft(PieceHere.Owner);
            var final = adjacant.GetLeft(PieceHere.Owner);
            // king cannot move through locations under threat, or non-empty.
            foreach (var loc in new ChessButton[] { adjacant, final })
            {
                if (loc == null || loc.PieceHere != null || loc.Threats.HasFlag(PieceHere.Opponent))
                    return;
            }
            // rook can move through threat, but must be empty
            var next = final.GetLeft(PieceHere.Owner);
            if (next == null || next.PieceHere != null)
                return;
            // if we get here then:
            // a) King and rook are on player's starting rank
            // b) Neither have moved
            // c) No pieces between King and Rook
            // d) King is not in threat
            // e) King does not pass through a square in threat
            // -> Hence: Castle is permitted.
            final.SetCanMove(PieceHere, highlight);
        }

        void evalRightCastle(bool highlight)
        {
            var rook = GetRight(PieceHere.Owner, 3);
            if (rook == null || rook.PieceHere == null || rook.PieceHere.HasMoved)
                return;
            // castle right moves king from Ex to Gx
            var adjacant = GetRight(PieceHere.Owner);
            var final = adjacant.GetRight(PieceHere.Owner);
            // king cannot move through locations under threat, or non-empty.
            foreach (var loc in new ChessButton[] { adjacant, final })
            {
                if (loc == null || loc.PieceHere != null || loc.Threats.HasFlag(PieceHere.Opponent))
                    return;
            }
            // if we get here then:
            // a) King and rook are on player's starting rank
            // b) Neither have moved
            // c) No pieces between King and Rook
            // d) King is not in threat
            // e) King does not pass through a square in threat
            // -> Hence: Castle is permitted.
            final.SetCanMove(PieceHere, highlight);
        }

        void evalCastle(bool highlight)
        {
            if (PieceHere.HasMoved)
                return; // permenantly gives up if moved king.
            if (this.Threats.HasFlag(PieceHere.Opponent))
                return; // temporary inabiliy if threatened
            if (this.GetPins(PieceHere.Opponent) != Pin.None)
                return; // technically if pinned, should be threatened but just in case.
            evalLeftCastle(highlight);
            evalRightCastle(highlight);
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
            evalCastle(highlight);
        }

        void evalPawn(bool highlight)
        {
            var pins = this.GetPins(PieceHere.Opponent);
            if(pins.HasFlag(Pin.Horizontal)) 
                return; // cant move at all
            var forward = this.GetForward(PieceHere.Owner);
            if (forward.PieceHere == null)
            {
                forward?.SetCanMove(PieceHere, highlight, false);
                if (!PieceHere.HasMoved)
                {
                    var twice = forward?.GetForward(PieceHere.Owner);
                    if (twice?.PieceHere == null)
                    {
                        twice?.SetCanMove(PieceHere, highlight, false);
                    }
                }
            }
            // can move forward, but now we must verify vertical pins since we're aboutt to move off the file:
            if (pins.HasFlag(Pin.Vertical))
                return;
            var left = forward?.GetLeft(PieceHere.Owner);
            var right = forward?.GetRight(PieceHere.Owner);
            foreach(var thing in new ChessButton[] { left, right})
            {
                if(thing.Name == left.Name && pins.HasFlag(Pin.RightDiagonal))
                { // cant move on the left since pinned.
                    continue;
                } else if (thing.Name == right.Name && pins.HasFlag(Pin.LeftDiagonal))
                {
                    continue;
                }
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
            // determine if we can enpassant capture:
            foreach(var thing in new ChessButton[] { this.GetLeft(PieceHere.Owner), this.GetRight(PieceHere.Owner)})
            { // opponent pawn would be by our side
                // we need to check it is a pawn, and that the last move by opponent was to move this pawn two places.
                if (thing == null)
                    continue;
                if (thing.PieceHere == null || thing.PieceHere.Type != PieceType.Pawn)
                    continue;
                if (thing.PieceHere.Owner == PieceHere.Owner)
                    continue;
                // get the opponent's last move.
                var move = StartForm.INSTANCE.Game.LastMoves[PieceHere.Opponent];
                if (move == null)
                    continue; // opponent has made no moves, so obviously no enpassant
                if(move.To.Name == thing.Name)
                { // move was to this location, now we need to ensure it was two squares moved.
                    var to = Board.GetPoint(move.To.Name);
                    var from = Board.GetPoint(move.From.Name);
                    int diffX = Math.Abs(to.X - from.X);
                    int diffY = Math.Abs(to.Y - from.Y);
                    if(diffY == 2 && diffX == 0)
                    {
                        thing.GetBack(PieceHere.Opponent).SetCanMove(PieceHere, highlight);
                    }
                }
            }
        }
    }
}
