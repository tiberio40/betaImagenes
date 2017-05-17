using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;

using System.IO;
using betaImagenes.json;
using Newtonsoft.Json;

namespace betaImagenes
{
    class ComputerVision
    {
        public ComputerVision(string direccionImagen)
        {
            MakeRequest(direccionImagen);
        }

        public Person consulta { get; set; }
        

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async void MakeRequest(string imageFilePath)
        {
            var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2ca6d229b5c2430bbc0b123096b17bb8");

            // Request parameters
            //queryString["maxCandidates"] = "1";
            //var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?" + queryString;
            var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(imageFilePath);
            string contentString;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                contentString = await response.Content.ReadAsStringAsync();

            }

            Person deserializedProduct = JsonConvert.DeserializeObject<Person>(contentString);

            
        }
        

    }
}
