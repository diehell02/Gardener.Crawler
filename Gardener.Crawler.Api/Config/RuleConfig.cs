using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Gardener.Crawler.Api.Rule;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Gardener.Crawler.Api.Config
{
    class RuleConfig : IRuleConfig
    {
        Dictionary<PageType, IPage> dictionary = new Dictionary<PageType, IPage>();

        public void AddRule(PageType pageType, IPage page)
        {
            if (!dictionary.ContainsKey(pageType))
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

        public static IRuleConfig GetPageRule(Stream stream)
        {
            IRuleConfig pageRule = null;

            using (JsonReader jsonReader = new JsonTextReader(new StreamReader(stream)))
            {
                JObject jObject = JObject.Load(jsonReader);

                var jTokens = jObject["Pages"]?.AsJEnumerable();

                if (jTokens != null)
                {
                    var _pageRule = new RuleConfig();

                    foreach (var jToken in jTokens)
                    {
                        PageType pageType;

                        if (!Enum.TryParse(jToken?.Value<string>("PageType"), out pageType))
                        {
                            continue;
                        }

                        Page page = new Page();

                        if(Boolean.TryParse(jToken?.Value<string>("UseProxy"), out bool useProxy))
                        {
                            page.UseProxy = useProxy;
                        }

                        var jRules = jToken["Rules"].AsJEnumerable();

                        foreach (var jRule in jRules)
                        {
                            Rule.Rule rule = new Rule.Rule()
                            {
                                Name = jRule?.Value<string>("Name"),
                                XPath = jRule?.Value<string>("XPath"),
                                Fun = (Rule.Rule.RuleFun)Enum.Parse(typeof(Rule.Rule.RuleFun), jRule?.Value<string>("Fun")),
                                Param = jRule?.Value<string>("Param"),
                            };
                            page.Add(rule);
                        }

                        _pageRule.AddRule(pageType, page);
                    }

                    pageRule = _pageRule;
                }
            }

            return pageRule;
        }
    }
}
