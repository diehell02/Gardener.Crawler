using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gardener.Crawler.Api
{
    public class GetRecentPostsResponse : DefaultResponse
    {
        [JsonProperty("count")]
        public int Count
        {
            get;
            set;
        }

        [JsonProperty("count_total")]
        public int CountTotal
        {
            get;
            set;
        }

        [JsonProperty("pages")]
        public int Pages
        {
            get;
            set;
        }

        [JsonProperty("posts")]
        public List<Post> Posts
        {
            get;
            set;
        }

        public class Post
        {
            [JsonProperty("id")]
            public int Id
            {
                get;
                set;
            }

            [JsonProperty("type")]
            public string Type
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

            [JsonProperty("url")]
            public string Url
            {
                get;
                set;
            }

            [JsonProperty("status")]
            public string Status
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

            [JsonProperty("title_plain")]
            public string TitlePlain
            {
                get;
                set;
            }

            [JsonProperty("content")]
            public string Content
            {
                get;
                set;
            }

            [JsonProperty("excerpt")]
            public string Excerpt
            {
                get;
                set;
            }

            [JsonProperty("date")]
            public DateTime Date
            {
                get;
                set;
            }

            [JsonProperty("modified")]
            public DateTime Modified
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

            [JsonProperty("tags")]
            public List<Tag> Tags
            {
                get;
                set;
            }

            public class Tag
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

                [JsonProperty("post_count")]
                public int PostCount
                {
                    get;
                    set;
                }
            }

            [JsonProperty("author")]
            public Author_ Author
            {
                get;
                set;
            }

            public class Author_
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

                [JsonProperty("name")]
                public string Name
                {
                    get;
                    set;
                }

                [JsonProperty("first_name")]
                public string FirstName
                {
                    get;
                    set;
                }

                [JsonProperty("last_name")]
                public string LastName
                {
                    get;
                    set;
                }

                [JsonProperty("nickname")]
                public string Nickname
                {
                    get;
                    set;
                }

                [JsonProperty("url")]
                public string Url
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
            }
        }
    }
}
