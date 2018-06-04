using System;
using Newtonsoft.Json;

namespace Gardener.Crawler.Api
{
    public class DefaultResponse
    {
        [JsonProperty("status")]
        public string Status
        {
            get;
            set;
        }
    }
}
