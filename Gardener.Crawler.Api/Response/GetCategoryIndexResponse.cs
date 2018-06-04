using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Gardener.Crawler.Api
{
    public class GetCategoryIndexResponse : DefaultResponse
    {
        [JsonProperty("count")]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("categories")]
        public List<Category> Categories
        {
            get;
            set;
        }

        public class Category
        {
            [JsonProperty("id")]
            public int Id
            {
                get;
                set;
            }

            [JsonProperty("slug")]
            public string Slug
            {
                get;
                set;
            }

            [JsonProperty("title")]
            public string Title
            {
                get;
                set;
            }

            [JsonProperty("description")]
            public string Description
            {
                get;
                set;
            }

            [JsonProperty("parent")]
            public int Parent
            {
                get;
                set;
            }

            [JsonProperty("post_count")]
            public int PostCount
            {
                get;
                set;
            }
        }
    }
}
