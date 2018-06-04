using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Gardener.Crawler.Api.Rule
{
    class Page : IPage
    {
        private Dictionary<string, Rule> dictionary = new Dictionary<string, Rule>();

        private bool useProxy = false;
        public bool UseProxy
        {
            get
            {
                return useProxy;
            }
            set
            {
                useProxy = value;
            }
        }

        public void Add(Rule rule)
        {
            if (!dictionary.ContainsKey(rule.Name))
            {
                dictionary.Add(rule.Name, rule);
            }
        }

        public HtmlNodeCollection GetNodes(HtmlNode htmlNode, string ruleName)
        {
            if (dictionary.ContainsKey(ruleName))
            {
                if (dictionary[ruleName].Fun == Rule.RuleFun.Nodes)
                {
                    return htmlNode.SelectNodes(dictionary[ruleName].XPath);
                }
            }

            return null;
        }

        public HtmlNode GetSingleNode(HtmlNode htmlNode, string ruleName)
        {
            if (this.dictionary.ContainsKey(ruleName))
            {
                if (dictionary[ruleName].Fun == Rule.RuleFun.Nodes)
                {
                    return null;
                }

                return htmlNode.SelectSingleNode(dictionary[ruleName].XPath);
            }

            return null;
        }

        public string GetSingleNodeValue(HtmlNode htmlNode, string ruleName)
        {
            if (this.dictionary.ContainsKey(ruleName))
            {
                if (dictionary[ruleName].Fun == Rule.RuleFun.Nodes)
                {
                    return null;
                }

                var node = htmlNode.SelectSingleNode(dictionary[ruleName].XPath);

                if(node is null)
                {
                    return null;
                }

                return GetValue(node, ruleName);
            }

            return null;
        }

        public string GetValue(HtmlNode htmlNode, string ruleName)
        {
            if (this.dictionary.ContainsKey(ruleName))
            {
                switch (dictionary[ruleName].Fun)
                {
                    case Rule.RuleFun.Attr:
                        return htmlNode.Attributes[dictionary[ruleName].Param].Value;
                    case Rule.RuleFun.Text:
                        return htmlNode.InnerText;
                }
            }

            return string.Empty;
        }
    }
}
