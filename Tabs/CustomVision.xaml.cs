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
using System.Collections.Generic;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadphoneCamera(object sender, EventArgs e)
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
            }
            );

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            await MakePredictionRequest(file);
            await postLocationAsync();
            
        }

        async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 30;

            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

            DogModel model = new DogModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude

            };

            await AzureManager.AzureManagerInstance.PostInfo(model);
        }





        static byte[] GetByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        
        async Task MakePredictionRequest(MediaFile file)
        {
            var c = new HttpClient();
            HttpResponseMessage res;
            c.DefaultRequestHeaders.Add("Prediction-Key", "7b8c0ade1a6b40928ebc64344757a657");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/e0a6a1d1-1c4b-4a13-a718-62ecc7a5b584/image?iterationId=7d4d7c49-617d-452a-83e0-d61923184510";

            byte[] byteData = GetByteArray(file);

            using (var info = new ByteArrayContent(byteData))
            {
                info.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                res = await c.PostAsync(url, info);


                if (res.IsSuccessStatusCode)
                {
                    var responseString = await res.Content.ReadAsStringAsync();
                    JObject recived = JObject.Parse(responseString);
                    var Probabilitystring = from p in recived["Predictions"] select (string)p["Probability"];
                    
                    var Tagstring = from p in recived["Predictions"] select (string)p["Tag"];
                    double temp;
                    string result;

                    foreach (var i in Tagstring) {TagLabel.Text += i + ":\n";    }
                    foreach (var i in Probabilitystring) {PredictionLabel.Text += ( temp = Double.Parse(i, CultureInfo.InvariantCulture)).ToString("N2", CultureInfo.InvariantCulture) + "% Probability" + "\n"; }

                    
                    
                    //--Math.Round(Convert.ToDecimal(TagLabel.Text), 2);
                    
                    
                    

                }

              
                file.Dispose();
            }
        }
    }
}