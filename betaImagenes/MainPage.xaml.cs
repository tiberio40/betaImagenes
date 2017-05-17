using betaImagenes.json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace betaImagenes
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile imagenCamara;
        private string RutaImagen;
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async System.Threading.Tasks.Task cameraAsync()
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            Size aspectRatio = new Size(16, 9);
            dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;

            imagenCamara = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            
        }


        private async void capturaImagen_Click(object sender, RoutedEventArgs e)
        {
            await cameraAsync();
            try
            {
                RutaImagen = imagenCamara.Path;
                imagenCamaraCapturada.Source = new BitmapImage(new Uri(RutaImagen, UriKind.Relative));
                listaCaracteristicas.Items.Clear();
                listaCaracteristicas.Visibility = Visibility.Collapsed;
                listaCaracteristicas.IsEnabled = false;
            }
            catch (System.NullReferenceException)
            {

            }
            

        }

        private async void BotonAnalizar_Click(object sender, RoutedEventArgs e)
        {

            imagenCarga.Visibility = Visibility.Visible;
            await Consultar();
           
        }

        public async System.Threading.Tasks.Task Consultar()
        {
            
            Person consulta = await MakeRequest(RutaImagen);

            for (int i = 0; i < consulta.description.tags.Count; i++)
            {
                listaCaracteristicas.Items.Add(consulta.description.tags.ElementAt(i));
            }
            listaCaracteristicas.SelectedIndex = 0;
            imagenCarga.Visibility = Visibility.Collapsed;
            listaCaracteristicas.Visibility = Visibility.Visible;
            listaCaracteristicas.IsEnabled = true;


        }


        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async Task<Person> MakeRequest(string imageFilePath)
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

            return deserializedProduct;
        }


    }
}
