using System;
using System.Collections.Generic;
using System.Text;
using Gardener.Crawler.Api.Rule;

namespace Gardener.Crawler.Api.Entity
{
    public class Category
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public PageType PageType { get; set; }
    }
}
