using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.Crawler.Api.Entity
{
    public class Post
    {
        public Uri Address
        {
            get;
            set;
        }

        public List<Content> Contents
        {
            get;
            set;
        }

        public List<Image> Images
        {
            get;
            set;
        }

        public class Content
        {
            public string Description { get; set; }
        }

        public class Title : Content
        {
            public string Text { get; set; }
        }

        public class Body : Content
        {
            public string Text { get; set; }
        }

        public class Image : Content
        {
            private bool isOpen = false;
            public bool IsOpen
            {
                get
                {
                    return !isOpen;
                }
                set
                {
                    isOpen = value;
                }
            }

            public string Url { get; set; }
        }
    }
}
