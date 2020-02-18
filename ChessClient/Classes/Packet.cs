using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class Packet
    {
        public PacketId Id { get; set; }
        public JObject Content { get; set; }
        public Packet(PacketId id, JObject content)
        {
            Id = id;
            Content = content;
        }

        public override string ToString()
        {
            var jobj = new JObject();
            jobj["id"] = Id.ToString();
            jobj["payload"] = Content.ToString();
            return jobj.ToString();
        }
    }

    public enum PacketId
    {
        #region Client -> Server Messages
        /// <summary>
        /// Initial connection request
        /// Content: <see cref="string"/>[] of Token and join/spectate
        /// </summary>
        ConnRequest,

        /// <summary>
        /// Request that we move a piece
        /// Content: <see cref="string"/> 'from', and 'to'
        /// </summary>
        MoveRequest,

        /// <summary>
        /// Requests full information of a <seealso cref="ChessPlayer"/>
        /// Content: <see cref="int"/> Id
        /// </summary>
        IdentRequest,

        #endregion

        #region Server -> Client Messages
        /// <summary>
        /// Sends the current status of the game.
        /// Content: Game content
        /// </summary>
        GameStatus,

        /// <summary>
        /// Sends full information of a player.
        /// Content: ChessPlayer
        /// </summary>
        PlayerIdent,

        /// <summary>
        /// Informs Client to move a piece to a location
        /// Content: <see cref="string"/> From, To.
        /// </summary>
        MoveMade,

        /// <summary>
        /// Orders the Client to reply how they are to promote a pawn
        /// Content: <see cref="string"/> Location
        /// </summary>
        ChoosePromotion,

        /// <summary>
        /// Orders Client to reflect an update to a Piece,
        /// Content: Full Chess piece
        /// </summary>
        PieceUpdated,

        /// <summary>
        /// Orders Client to reflec an update to a Location
        /// Content: Full ChessLocation
        /// </summary>
        LocationUpdated,

        /// <summary>
        /// Informs Clients that a new Player has joined
        /// Content: Full ChessPlayer, and mode
        /// </summary>
        ConnectionMade,

        #endregion

        #region Multipurpose Messages

        #endregion
    }
}
