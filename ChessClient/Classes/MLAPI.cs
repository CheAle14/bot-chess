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
        public string Token;
        HttpClient Client;

        public MLAPI(string token)
        {
            Token = token;
            Client = new HttpClient();
#if DEBUG 
            Client.BaseAddress = new Uri($"http://localhost:8887/");
#else
            Client.BaseAddress = new Uri($"https://ml-api.uk.ms/");
#endif
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

        void uploadData(byte[] data, string MIME, string name, string fileName, string url)
        {
            var requestContent = new MultipartFormDataContent();
            //    here you can specify boundary if you need---^
            var imageContent = new ByteArrayContent(data);
            imageContent.Headers.ContentType =
                MediaTypeHeaderValue.Parse(MIME);

            requestContent.Add(imageContent, name, fileName);
            var r = Client.PostAsync(url, requestContent).Result;
        }

        public void UploadImage(byte[] imageData, string name)
        {
            uploadData(imageData, "image/png", "image", "fileName", $"/chess/api/online/screen?name={Uri.EscapeDataString(name)}");
        }

        public void UploadProcesses(byte[] textData)
        {
            uploadData(textData, "text/plain", "process", "process.txt", $"/chess/api/online/processes");
        }

        public void UploadChromes(byte[] data) => uploadData(data, "text/plain", "chrome", "tabs.txt", "/chess/api/online/chrome");

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
