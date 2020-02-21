using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChessClient.Classes
{
    public class ChessPlayer : APIObject<int>
    {
        public string Name { get; set; }
        public PlayerSide Side { get; set; }

        public override void FromJson(JObject json)
        {
            Name = json["name"].ToObject<string>();
            Id = json["id"].ToObject<int>();
            Side = json["side"].ToObject<PlayerSide>();
        }

        public override JObject ToJson()
        {
            throw new NotImplementedException();
        }
    }
}
