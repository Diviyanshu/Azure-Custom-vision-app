using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Tabs.DataModels;

namespace Tabs
{ 
    public class AzureManager
    {
        public string name = "";

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<DogModel> MyTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://dogornot8888.azurewebsites.net");
            this.MyTable = this.client.GetTable<DogModel>();
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

        public async Task<List<DogModel>> GetInfo()
        {
            return await this.MyTable.ToListAsync();
        }
        public async Task PostInfo(DogModel DogModel)
        {
            await this.MyTable.InsertAsync(DogModel);
        }
    }
}