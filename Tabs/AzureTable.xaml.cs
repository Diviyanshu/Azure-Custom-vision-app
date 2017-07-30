using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Tabs.DataModels;
using Plugin.Geolocator;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {

        public AzureTable()
        {
            InitializeComponent();

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<DogModel> dogornot8888 = await AzureManager.AzureManagerInstance.GetHotDogInformation();
            
            HotDogList.ItemsSource = dogornot8888;
            //await PostLocationAsync();
        }

       // async Task PostLocationAsync()
       // {

           // var locator = CrossGeolocator.Current;
           // locator.DesiredAccuracy = 50;

           // var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

           // DogModel model = new DogModel()
           // {
               // Longitude = (float)position.Longitude,
               // Latitude = (float)position.Latitude

           // };

            //await AzureManager.AzureManagerInstance.PostHotDogInformation(model);
        //}
    }
}