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
            List<DogModel> dogornot8888 = await AzureManager.AzureManagerInstance.GetInfo();
            
            LocList.ItemsSource = dogornot8888;
            
        }

      
    }
}