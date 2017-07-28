using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Tabs.DataModels;

namespace Tabs
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<DogModel> notHotDogTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://dogornot8888.azurewebsites.net");
            var q = 5;

            this.notHotDogTable = this.client.GetTable<DogModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<DogModel>> GetHotDogInformation()
        {
            return await this.notHotDogTable.ToListAsync();
        }
        public async Task PostHotDogInformation(DogModel notHotDogModel)
        {
            await this.notHotDogTable.InsertAsync(notHotDogModel);
        }
    }
}