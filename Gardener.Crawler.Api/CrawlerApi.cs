using System;
using System.Collections.Generic;
using System.IO;
using Gardener.Crawler.Api.Config;
using Gardener.Crawler.Api.Interface;
using Gardener.Crawler.Api.Rule;

namespace Gardener.Crawler.Api
{
    public class CrawlerApi
    {
        private static Dictionary<ApiType, IApi> apiDic;
        private static Dictionary<ApiType, Type> ruleDic;

        static CrawlerApi()
        {
            ruleDic = new Dictionary<ApiType, Type>()
            {
                { ApiType.JDLingYu, typeof(Gardener.Crawler.Api.Api.JDLingYu) },
                { ApiType.BCY, typeof(Gardener.Crawler.Api.Api.BCY) }
            };
        }

        public static IApi GetApi(ApiType apiType, Stream stream)
        {
            if(apiDic is null)
            {
                IRuleConfig ruleConfig = RuleConfig.GetPageRule(stream);

                apiDic = new Dictionary<ApiType, IApi>();

                foreach(var rule in ruleDic)
                {
                    apiDic.Add(rule.Key, (IApi)Activator.CreateInstance(rule.Value, ruleConfig));
                }
            }

            if (apiDic.ContainsKey(apiType))
            {
                return apiDic[apiType];
            }

            return null;
        }
    }
}
