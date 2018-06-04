using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Gardener.Crawler.Api.Rule
{
    interface IPage
    {
        bool UseProxy
        {
            get;
        }

        HtmlNodeCollection GetNodes(HtmlNode htmlNode, string ruleName);

        HtmlNode GetSingleNode(HtmlNode htmlNode, string ruleName);

        string GetSingleNodeValue(HtmlNode htmlNode, string ruleName);

        string GetValue(HtmlNode htmlNode, string ruleName);
    }
}
