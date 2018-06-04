using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gardener.Crawler.Api.Interface;
using Gardener.Crawler.Api;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.IO;

namespace Gardener.Crawler.Client.UWP.Util
{
    class ApiHelper
    {
        private static IApi apiHelper { get; set; }

        public async static Task<IApi> GetApi()
        {
            if (apiHelper is null)
            {
                StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///rule.json"));

                var randomAccessStream = await storageFile.OpenReadAsync();

                Stream stream = randomAccessStream.AsStreamForRead();

                apiHelper = CrawlerApi.GetApi(ApiType.BCY, stream);
            }

            return apiHelper;
        }
    }
}
