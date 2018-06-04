using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Gardener.Crawler.Api.Entity;
using HtmlAgilityPack;
using Gardener.Crawler.Api.Interface;
using Gardener.Crawler.Api.Util;
using Gardener.Crawler.Api.Config;
using Gardener.Crawler.Api.Rule;

namespace Gardener.Crawler.Api.Api
{
    class BCY : BaseApi, IApi
    {
        const string scheme = "https";
        const string apiHost = "bcy.net";

        string cookie = string.Empty;

        HttpUtil httpUtil = null;

        public BCY(IRuleConfig ruleConfig) : base(ruleConfig)
        {
            //cookie = "lang_set=zh; " +
            //    "UM_distinctid=15dde4e71e555-0d8b5ff65bd62c-791238-1fa400-15dde4e71e6904; " +
            //    "acw_tc=AQAAAH9bHU2cSwwAh5QpeORR3Tl+HdPW; " +
            //    "PHPSESSID=ig4cumhk9pfehocjq252mu7dv2; " +
            //    "LOGGED_USER=Q8G5jV2ZlOcDiOYQntQTyA%3D%3D%3A2JzZVgiv4NXE5z1zq5zkJA%3D%3D; " +
            //    "CNZZDATA1257708097=1756281233-1502672690-%7C1511333691; " +
            //    "Hm_lvt_330d168f9714e3aa16c5661e62c00232=1511332578; " +
            //    "Hm_lpvt_330d168f9714e3aa16c5661e62c00232=1511334642; " +
            //    "mobile_set=no";

            httpUtil.SetCookie(cookie);
        }

        public override async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = null;

            await Task.Run(() =>
            {
                categories = new List<Category>()
                {
                    new Category()
                    {
                        Title = "排行榜",
                        Link = string.Format("{0}://{1}/coser/toppost100", scheme, apiHost),
                        PageType = PageType.BCY_TopPost
                    },
                    new Category()
                    {
                        Title = "最新正片",
                        Link = string.Format("{0}://{1}/coser/allwork", scheme, apiHost),
                        PageType = PageType.BCY_AllWork
                    },
                    new Category()
                    {
                        Title = "推荐作品",
                        Link = string.Format("{0}://{1}/coser", scheme, apiHost),
                        PageType = PageType.BCY_Discover
                    },
                    new Category()
                    {
                        Title = "最新预告",
                        Link = string.Format("{0}://{1}/coser/allpre", scheme, apiHost),
                        PageType = PageType.BCY_AllPre
                    },
                };
            });

            return categories;
        }

        public override async Task<List<Gallery>> GetGalleriesAsync(Category category, int pageIndex = 1)
        {
            List<Gallery> galleries = null;
            string _address = string.Empty;
            if (pageIndex == 1)
            {
                _address = category.Link;
            }
            else
            {
                _address = string.Format("{0}?&p={1}", category.Link, pageIndex);
            }

            string html = string.Empty;
            IPage page = ruleConfig.GetRule(category.PageType);

            html = await GetHTML(_address, page);

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            galleries = GetGalleryNodes(page, html);

            return galleries;
        }

        private List<Gallery> GetGalleryNodes(IPage page, string html)
        {
            List<Gallery> galleries = null;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var galleryNodes = page.GetNodes(doc.DocumentNode, "Gallery");
                if (galleryNodes != null)
                {
                    galleries = new List<Gallery>();

                    foreach (var node in galleryNodes)
                    {
                        Gallery gallery = new Gallery();
                        HtmlNode _node = null;

                        _node = page.GetSingleNode(node, "Gallery.Address");
                        if (_node != null)
                        {
                            string _address = page.GetValue(_node, "Gallery.Address");

                            if(_address.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                            {
                                gallery.Address = _address;
                            }
                            else
                            {
                                gallery.Address = string.Format("{0}://{1}{2}", scheme, apiHost, _address);
                            }                            

                            gallery.Title = page.GetValue(_node, "Gallery.Title");
                        }

                        _node = page.GetSingleNode(node, "Gallery.Picture");
                        if (_node != null)
                        {
                            gallery.Picture = page.GetValue(_node, "Gallery.Picture");
                        }

                        galleries.Add(gallery);
                    }
                }
            }
            catch
            {

            }            

            return galleries;
        }

        public override async Task<Post> GetPost(Gallery gallery)
        {
            Post post = null;

            string html = string.Empty;
            IPage page = ruleConfig.GetRule(PageType.BCY_Images);

            html = await GetHTML(gallery.Address, page);

            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            post = new Post();            

            post.Contents = new List<Post.Content>();

            var title = page.GetSingleNodeValue(doc.DocumentNode, "Title");
            if (!string.IsNullOrEmpty(title))
            {
                foreach (char rInvalidChar in System.IO.Path.GetInvalidPathChars())
                {
                    title = title.Replace(rInvalidChar.ToString(), string.Empty);
                }
                string errChar = "\\/:*?";
                foreach (char rInvalidChar in errChar)
                {
                    title = title.Replace(rInvalidChar.ToString(), string.Empty);
                }

                post.Contents.Add(new Post.Title() { Text = title.Replace("\n", "").Trim() });
            }

            StringBuilder stringBuilder = new StringBuilder();

            var role = page.GetSingleNodeValue(doc.DocumentNode, "Role");
            if (!string.IsNullOrEmpty(role))
            {
                stringBuilder.Append(WebUtility.HtmlDecode(role.Replace("\n", "").Trim())).Append("\n");
            }

            var staff = page.GetSingleNodeValue(doc.DocumentNode, "Staff");
            if (!string.IsNullOrEmpty(staff))
            {
                stringBuilder.Append(WebUtility.HtmlDecode(staff.Replace("\n", "").Trim())).Append("\n");
            }

            var contentNode = page.GetSingleNode(doc.DocumentNode, "Content");
            if (contentNode != null)
            {
                foreach(var child in contentNode.ChildNodes)
                {
                    if(child.NodeType == HtmlNodeType.Text)
                    {
                        stringBuilder.Append(child.InnerText);
                    }
                    else if(child.Name == "br")
                    {
                        stringBuilder.Append("\n");
                    }
                    else if (child.Name == "a")
                    {
                        stringBuilder.Append(child.InnerText);
                    }
                    else if (child.Name == "img")
                    {
                        break;
                    }
                }
            }

            post.Contents.Add(new Post.Body() { Text = stringBuilder.ToString() });

            var imgNodes = page.GetNodes(doc.DocumentNode, "Image");
            if (imgNodes != null)
            {
                Regex regex = new Regex(@"((http|https)://)(([a-zA-Z0-9\._-]+)/)+(w\d+)");
                post.Images = new List<Post.Image>();

                foreach (var node in imgNodes)
                {
                    string url = node.Attributes["src"].Value;

                    // 删除尾部限定大小                    
                    if (regex.IsMatch(url))
                    {
                        url = url.Substring(0, url.LastIndexOf('/'));
                    }

                    Post.Image image = new Post.Image() { Url = url };

                    post.Contents.Add(image);
                    post.Images.Add(image);
                }
            }

            return post;
        }
    }
}
