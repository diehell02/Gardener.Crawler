using System;
using System.Collections.Generic;
using System.Text;
using Gardener.Crawler.Api.Rule;

namespace Gardener.Crawler.Api.Config
{
    interface IRuleConfig
    {
        IPage GetRule(PageType pageType);
    }
}
