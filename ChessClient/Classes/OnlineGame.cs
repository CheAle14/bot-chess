using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChessClient.Classes
{
    public class OnlineGame : APIObject
    {
        public ChessPlayer White { get; set; }
        public ChessPlayer Black { get; set; }
        public PlayerSide Waiting { get; set; }
        public Dictionary<PlayerSide, Move> LastMoves = new Dictionary<PlayerSide, Move>()
        {
            { PlayerSide.White, null},
            { PlayerSide.Black, null }
        };
        public bool Ended { get; set; }


        public override void FromJson(JObject json)
        {
            var wId = json["white"].ToObject<int>();
            var bId = json["black"].ToObject<int>();
            if(wId != 0)
                White = StartForm.GetPlayer(wId);
            if (bId != 0)
                Black = StartForm.GetPlayer(bId);
            Waiting = json["wait"].ToObject<PlayerSide>();
            if(json.ContainsKey("board"))
            {
                var board = json["board"];
                var BOARD = StartForm.INSTANCE.GameForm.Board;
                foreach(var side in new PlayerSide[] { PlayerSide.White, PlayerSide.Black })
                {
                    var content = board[side.ToString()];
                    var PIECES = BOARD.Pieces[side];
                    foreach(JProperty pieceMoved in content.Children())
                    {
                        var id = int.Parse(pieceMoved.Name.Substring("P#".Length));
                        var piece = PIECES.FirstOrDefault(x => x.Id == id);
                        if(piece.Location != null)
                        {
                            piece.Location.PieceHere = null;
                        }
                        var location = pieceMoved.ToObject<string>();
                        if(location == "null")
                        { // piece was taken
                            piece.Location = null;
                        } else
                        {
                            piece.Location = BOARD.GetButtonAt(location);
                            piece.Location.PieceHere = piece;
                        }
                    }
                }
            }
        }

        public override JObject ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
