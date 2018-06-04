using Gardener.Crawler.Api.Config;
using Gardener.Crawler.Api.Entity;
using Gardener.Crawler.Api.Interface;
using Gardener.Crawler.Api.Rule;
using Gardener.Crawler.Api.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gardener.Crawler.Api.Api
{
    abstract class BaseApi : IApi
    {
        protected IRuleConfig ruleConfig = null;

        HttpUtil httpUtil = null;

        public BaseApi(IRuleConfig ruleConfig)
        {
            this.ruleConfig = ruleConfig;
            this.httpUtil = new HttpUtil();
        }

        protected Uri GetProxy()
        {
            return null;
        }

        protected async Task<string> GetHTML(string address, IPage page)
        {
            string html = string.Empty;

            if (Uri.TryCreate(address, UriKind.Absolute, out Uri _address))
            {
                if (page.UseProxy)
                {
                    var proxy = GetProxy();
                    html = await httpUtil.Do(_address, proxy);
                }
                else
                {
                    html = await httpUtil.Do(_address);
                }
            }

            return html;
        }

        public abstract Task<List<Category>> GetCategoriesAsync();

        public abstract Task<List<Gallery>> GetGalleriesAsync(Category category, int pageIndex = 1);

        public abstract Task<Post> GetPost(Gallery gallery);
    }
}
