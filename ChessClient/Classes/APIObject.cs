using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public abstract class APIObject
    {
        internal MLAPI API { get; set; }

        public abstract JObject ToJson();
        public abstract void FromJson(JObject json);
        
    }
    public abstract class APIObject<T> : APIObject
    {
        public T Id { get; set; }
    }
}
