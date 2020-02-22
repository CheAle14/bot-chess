using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Classes
{
    public class MLAPI
    {
        public const string URL = "http://localhost:8887/";
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

        public void UploadImage(byte[] imageData, string name)
        {
            var requestContent = new MultipartFormDataContent();
            //    here you can specify boundary if you need---^
            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType =
                MediaTypeHeaderValue.Parse("image/png");

            requestContent.Add(imageContent, "image", "image.png");
            var r = Client.PostAsync($"/chess/api/online/screen?name={Uri.EscapeDataString(name)}", requestContent).Result;
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
