using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace Gardener.Crawler.Api.Util
{
    class HttpUtil
    {
        string cookie = string.Empty;

        public void SetCookie(string cookie)
        {
            this.cookie = cookie;
        }

        public async Task<string> Do(Uri address, Uri proxy = null)
        {
            string result = string.Empty;

            try
            {
                WebCrawlerHttpClientHandler webCrawlerHttpClientHandler = new WebCrawlerHttpClientHandler(cookie);

                if(proxy != null)
                {
                    webCrawlerHttpClientHandler.Proxy = new WebProxy(proxy);
                }

                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(webCrawlerHttpClientHandler);

                result = await httpClient.GetStringAsync(address);
            }
            catch
            {

            }

            return result;
        }

        public async Task<string> Do(string address)
        {
            string result = string.Empty;

            try
            {
                if(Uri.TryCreate(address, UriKind.Absolute, out Uri _address))
                {
                    result = await Do(_address);
                }
            }
            catch
            {

            }

            return result;
        }
    }

    class WebCrawlerHttpClientHandler : System.Net.Http.HttpClientHandler
    {
        private string cookie = string.Empty;

        public WebCrawlerHttpClientHandler(string cookie = "")
        {
            this.cookie = cookie;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            request.Headers.Add("Host", request.RequestUri.Host);

            if(!string.IsNullOrEmpty(this.cookie))
            {
                request.Headers.Add("Cookie", this.cookie);
            }            

            request.Method = HttpMethod.Get;

            this.ClientCertificateOptions = ClientCertificateOption.Automatic;

            this.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
