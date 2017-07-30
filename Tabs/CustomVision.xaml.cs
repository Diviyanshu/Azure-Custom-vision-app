using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tabs.Model;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Tabs.DataModels;
using System.Globalization;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            await postLocationAsync();
            await MakePredictionRequest(file);
        }

        async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

            DogModel model = new DogModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude

            };

            await AzureManager.AzureManagerInstance.PostHotDogInformation(model);
        }





        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }
        
        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "7b8c0ade1a6b40928ebc64344757a657");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/e0a6a1d1-1c4b-4a13-a718-62ecc7a5b584/image?iterationId=7d4d7c49-617d-452a-83e0-d61923184510";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject rss = JObject.Parse(responseString);
                    var Probability = from p in rss["Predictions"] select (string)p["Probability"];

                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];
                    double temp;
                    string result;
                    foreach (var item in Tag) {TagLabel.Text += item + ":\n";    }
                    foreach (var item in Probability) {PredictionLabel.Text += ( temp = Double.Parse(item, CultureInfo.InvariantCulture)).ToString("N2", CultureInfo.InvariantCulture) + "%" + "\n"; }

                    
                    //EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);
                    //--Math.Round(Convert.ToDecimal(TagLabel.Text), 2);
                    //double max = responseModel.Predictions.Max(m => m.Probability);
                    //--String TagLabel = TagLabel.ToString();
                    //TagLabel.Text = (max >= 0.4) ? "Its a dog!" : "Not a dog :(";

                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }
    }
}