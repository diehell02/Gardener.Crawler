using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gardener.Crawler.Api.Entity;
using HtmlAgilityPack;
using Gardener.Crawler.Api.Util;
using Gardener.Crawler.Api.Interface;
using Gardener.Crawler.Api.Rule;
using Gardener.Crawler.Api.Config;

namespace Gardener.Crawler.Api.Api
{
    class JDLingYu : IApi
    {
        const string apiHost = "www.jdlingyu.fun";

        HttpUtil httpUtil = new HttpUtil();

        IRuleConfig ruleConfig = null;

        public JDLingYu(IRuleConfig ruleConfig)
        {

        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = null;

            //categories = new List<Category>()
            //{
            //    new Category()
            //    {
            //        Title = "首页",
            //        Link = string.Format("http://{0}/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "专题",
            //        Link = string.Format("http://{0}/%e4%b8%93%e9%a2%98/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "特点",
            //        Link = string.Format("http://{0}/%e7%89%b9%e7%82%b9/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "弄潮",
            //        Link = string.Format("http://{0}/%e5%bc%84%e6%bd%ae/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "Cosplay",
            //        Link = string.Format("http://{0}/cosplay/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "妹子图",
            //        Link = string.Format("http://{0}/mzitu/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "Hentai好物",
            //        Link = string.Format("http://{0}/hentai/", apiHost)
            //    },
            //    new Category()
            //    {
            //        Title = "二次元",
            //        Link = string.Format("http://{0}/acg/", apiHost)
            //    },
            //};

            string html = await httpUtil.Do(string.Format("http://{0}/", apiHost));

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var categoryNode = doc.DocumentNode.SelectNodes("//ul[@id='menu-nev']/li/a");
            if (categoryNode != null)
            {
                categories = new List<Category>();
                foreach (var category in categoryNode)
                {
                    string title = category.InnerText;
                    string link = category.Attributes["href"].Value;

                    if (!link.EndsWith("/"))
                    {
                        link += "/";
                    }

                    categories.Add(new Category()
                    {
                        Title = title,
                        Link = link
                    });
                }
            }

            return categories;
        }

        public async Task<List<Gallery>> GetGalleriesAsync(Category category, int page = 1)
        {
            List<Gallery> galleries = null;
            string _address = string.Empty;
            if (page == 1)
            {
                _address = category.Link;
            }
            else
            {
                _address = string.Format("{0}page/{1}/", category.Link, page);
            }

            string html = await HttpClient.Do(_address);

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            galleries = GetGalleryNodes(html);

            return galleries;
        }

        private List<Gallery> GetGalleryNodes(string html)
        {
            List<Gallery> galleries = null;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var galleryNodes = doc.DocumentNode.SelectNodes("//div[@class='pin-coat']/a");
            if (galleryNodes != null)
            {
                galleries = new List<Gallery>();

                foreach (var node in galleryNodes)
                {
                    Gallery gallery = new Gallery
                    {
                        Address = node.Attributes["href"].Value
                    };

                    HtmlNode imgNode = null;
                    foreach (var _node in node.ChildNodes)
                    {
                        if (_node.Name == "img")
                        {
                            imgNode = _node;
                            break;
                        }
                    }

                    gallery.Title = imgNode.Attributes["alt"].Value;
                    gallery.Picture = imgNode.Attributes["original"].Value;

                    galleries.Add(gallery);
                }
            }

            return galleries;
        }

        public async Task<Post> GetPost(Gallery gallery)
        {
            Post post = null;

            string html = await HttpClient.Do(gallery.Address);

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            post = new Post();

            post.Contents = new List<Post.Content>();

            var titleNode = doc.DocumentNode.SelectSingleNode("//h2[@class='main-title']");
            if (titleNode != null)
            {
                post.Contents.Add(new Post.Title() { Text = titleNode.InnerHtml });
            }

            var imgNodes = doc.DocumentNode.SelectNodes("//div[@class='main-body']/p/a");
            if (imgNodes != null)
            {                
                post.Images = new List<Post.Image>();

                foreach (var node in imgNodes)
                {
                    var image = new Post.Image() { Url = node.Attributes["href"].Value };

                    post.Contents.Add(image);
                    post.Images.Add(image);
                }
            }

            return post;
        }
    }
}
