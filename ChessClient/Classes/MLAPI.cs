using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class MLAPI
    {
#if DEBUG
        public const string URL = "http://localhost:8887/";
#else
        public const string URL = "https://ml-api.uk.ms/";
#endif
        public string Token;
        HttpClient Client;

        public MLAPI(string token)
        {
            Token = token;
            Client = new HttpClient();
            Client.BaseAddress = new Uri(URL);
            Client.DefaultRequestHeaders.Add("User-Agent", "bot-chess");
            Client.DefaultRequestHeaders.Add("X-TOKEN", Token);
        }

        public ChessPlayer GetIdentity()
        {
            var r = Client.GetAsync("/chess/api/identity").Result;
            var content = r.Content.ReadAsStringAsync().Result;
            if(!r.IsSuccessStatusCode)
                throw new APIException("GetIdentity", content);
            var player = JsonConvert.DeserializeObject<ChessPlayer>(content);
            player.API = this;
            return player;
        }

        public void CreateNewGame()
        {
            var req = new HttpRequestMessage(HttpMethod.Put, "/chess/api/online/create");
            var r = Client.SendAsync(req).Result;
            var content = r.Content.ReadAsStringAsync().Result;
            if (!r.IsSuccessStatusCode)
                throw new APIException("CreateNewGame", content);
        }
    }
}
