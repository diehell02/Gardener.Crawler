using System;
using System.Collections.Generic;
using System.Text;
using Gardener.Crawler.Api.Entity;
using HtmlAgilityPack;

namespace Gardener.Crawler.Api.Api
{
    class ApiRule
    {
        private static Dictionary<CategoryPageType, ApiPageRule> dictionary;

        static ApiRule()
        {
            dictionary = new Dictionary<CategoryPageType, ApiPageRule>()
            {
                {
                    CategoryPageType.BCY_TopPost, new ApiPageRule() { Dictionary = new Dictionary<string, Rule>()
                    {
                        {
                            "Gallery", new Rule()
                            {
                                Name = "Gallery",
                                XPath = "//li[@class='l-work-thumbnail']",
                                RuleFun = RuleFun.Nodes,
                            }
                        },
                        {
                            "Gallery.Address", new Rule()
                            {
                                Name = "Gallery.Address",
                                XPath = "./div[@class='work-thumbnail js-img-error work-thumbnail--top work-thumbnail__topList']/div[@class='work-thumbnail__topBd']/a",
                                RuleFun = RuleFun.Attr,
                                Param = "href"
                            }
                        },
                        {
                            "Gallery.Title", new Rule()
                            {
                                Name = "Gallery.Title",
                                XPath = "./div[@class='work-thumbnail js-img-error work-thumbnail--top work-thumbnail__topList']/div[@class='work-thumbnail__topBd']/a",
                                RuleFun = RuleFun.Attr,
                                Param = "title"
                            }
                        },
                        {
                            "Gallery.Picture", new Rule()
                            {
                                Name = "Gallery.Picture",
                                XPath = "./div[@class='work-thumbnail js-img-error work-thumbnail--top work-thumbnail__topList']/div[@class='work-thumbnail__topBd']/a/img",
                                RuleFun = RuleFun.Attr,
                                Param = "src"
                            }
                        },
                    }}
                },
                {
                    CategoryPageType.BCY_Discover, new ApiPageRule() { Dictionary = new Dictionary<string, Rule>()
                    {
                        {
                            "Gallery", new Rule()
                            {
                                Name = "Gallery",
                                XPath = "//li[@class='l-left disc_one ovf posr boxsize']",
                                RuleFun = RuleFun.Nodes,
                            }
                        },
                        {
                            "Gallery.Address", new Rule()
                            {
                                Name = "Gallery.Address",
                                XPath = "./a",
                                RuleFun = RuleFun.Attr,
                                Param = "href"
                            }
                        },
                        {
                            "Gallery.Title", new Rule()
                            {
                                Name = "Gallery.Title",
                                XPath = "./a",
                                RuleFun = RuleFun.Attr,
                                Param = "title"
                            }
                        },
                        {
                            "Gallery.Picture", new Rule()
                            {
                                Name = "Gallery.Picture",
                                XPath = "./a/div/div/img[@class='index-thumb db']",
                                RuleFun = RuleFun.Attr,
                                Param = "src"
                            }
                        },
                    }}
                },
                {
                    CategoryPageType.BCY_AllWork, new ApiPageRule() { Dictionary = new Dictionary<string, Rule>()
                    {
                        {
                            "Gallery", new Rule()
                            {
                                Name = "Gallery",
                                XPath = "//ul[@id='imageCards']/li[@class='_box']",
                                RuleFun = RuleFun.Nodes,
                            }
                        },
                        {
                            "Gallery.Address", new Rule()
                            {
                                Name = "Gallery.Address",
                                XPath = "./a",
                                RuleFun = RuleFun.Attr,
                                Param = "href"
                            }
                        },
                        {
                            "Gallery.Title", new Rule()
                            {
                                Name = "Gallery.Title",
                                XPath = "./div[@class='work-thumbnail__bd']/a",
                                RuleFun = RuleFun.Attr,
                                Param = "title"
                            }
                        },
                        {
                            "Gallery.Picture", new Rule()
                            {
                                Name = "Gallery.Picture",
                                XPath = "./a/div[@class='posr']/img",
                                RuleFun = RuleFun.Attr,
                                Param = "src"
                            }
                        },
                    }}
                },
                {
                    CategoryPageType.BCY_AllPre, new ApiPageRule() { Dictionary = new Dictionary<string, Rule>()
                    {
                        {
                            "Gallery", new Rule()
                            {
                                Name = "Gallery",
                                XPath = "//ul[@class='grid__inner gallery24 gallery--5']/li[@class='span1']/div[@class='_box imageCard p10 work-thumbnail js-img-error work-thumbnail--second']",
                                RuleFun = RuleFun.Nodes,
                            }
                        },
                        {
                            "Gallery.Address", new Rule()
                            {
                                Name = "Gallery.Address",
                                XPath = "./div[@class='work-thumbnail__bd']/a",
                                RuleFun = RuleFun.Attr,
                                Param = "href"
                            }
                        },
                        {
                            "Gallery.Title", new Rule()
                            {
                                Name = "Gallery.Title",
                                XPath = "./div[@class='work-thumbnail__bd']/a",
                                RuleFun = RuleFun.Attr,
                                Param = "title"
                            }
                        },
                        {
                            "Gallery.Picture", new Rule()
                            {
                                Name = "Gallery.Picture",
                                XPath = "./div[@class='work-thumbnail__bd']/a/img",
                                RuleFun = RuleFun.Attr,
                                Param = "src"
                            }
                        },
                    }}
                },
                {
                    CategoryPageType.BCY_Default, new ApiPageRule() { Dictionary = new Dictionary<string, Rule>()
                    {
                        {
                            "Post.Title", new Rule()
                            {
                                Name = "Post.Title",
                                XPath = "//h1[@class='js-post-title']",
                                RuleFun = RuleFun.Text,
                            }
                        },
                        {
                            "Post.Content", new Rule()
                            {
                                Name = "Post.Content",
                                XPath = "//div[@class='post__content js-content-img-wrap js-fullimg js-maincontent mb0 pl0 pr0 l-left w650']",
                                RuleFun = RuleFun.Text,
                            }
                        },
                        {
                            "Post.Image", new Rule()
                            {
                                Name = "Post.Image",
                                XPath = "//img[@class='detail_std detail_clickable']",
                                RuleFun = RuleFun.Nodes,
                            }
                        },
                        {
                            "Post.Image.Url", new Rule()
                            {
                                Name = "Post.Imgs.Url",
                                XPath = ".",
                                RuleFun = RuleFun.Attr,
                                Param = "src"
                            }
                        },
                    }}
                }
            };
        }

        public static ApiPageRule GetRule(CategoryPageType pageType)
        {
            if(dictionary.ContainsKey(pageType))
            {
                return dictionary[pageType];
            }

            return null;
        }

        public class ApiPageRule
        {
            public Dictionary<string, Rule> Dictionary { get; set; }

            public HtmlNodeCollection GetNodes(HtmlNode htmlNode, string ruleName)
            {
                if(this.Dictionary.ContainsKey(ruleName))
                {
                    if(Dictionary[ruleName].RuleFun == RuleFun.Nodes)
                    {
                        return htmlNode.SelectNodes(Dictionary[ruleName].XPath);
                    }                    
                }

                return null;
            }

            public HtmlNode GetSingleNode(HtmlNode htmlNode, string ruleName)
            {
                if (this.Dictionary.ContainsKey(ruleName))
                {
                    if (Dictionary[ruleName].RuleFun == RuleFun.Nodes)
                    {
                        return null;
                    }

                    return htmlNode.SelectSingleNode(Dictionary[ruleName].XPath);
                }

                return null;
            }

            public string GetValue(HtmlNode htmlNode, string ruleName)
            {
                if (this.Dictionary.ContainsKey(ruleName))
                {
                    switch (Dictionary[ruleName].RuleFun)
                    {
                        case RuleFun.Attr:
                            return htmlNode.Attributes[Dictionary[ruleName].Param].Value;
                        case RuleFun.Text:
                            return htmlNode.InnerText;
                    }
                }

                return string.Empty;
            }
        }
    }
}
