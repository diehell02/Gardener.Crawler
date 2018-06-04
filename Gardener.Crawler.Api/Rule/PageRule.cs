using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.Crawler.Api.Rule
{
    class PageRule : IRuleConfig
    {
        Dictionary<PageType, IPage> dictionary = new Dictionary<PageType, IPage>();

        public void AddRule(PageType pageType, IPage page)
        {
            if(!dictionary.ContainsKey(pageType))
            {
                dictionary.Add(pageType, page);
            }
        }

        public IPage GetRule(PageType pageType)
        {
            IPage page;

            dictionary.TryGetValue(pageType, out page);

            return page;
        }
    }
}
