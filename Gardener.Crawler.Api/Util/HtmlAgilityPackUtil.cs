using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Gardener.BCY.Client.Api.Util
{
    public class HtmlAgilityPackUtil
    {
        public static IEnumerable<HtmlNode> SelectNode(HtmlNode node, string tagName, string className = "")
        {
            IEnumerable<HtmlNode> nodes;

            nodes = node.Descendants(tagName).Where(y =>
            {
                if (string.IsNullOrEmpty(className))
                {
                    return true;
                }

                var classAttr = y.Attributes["class"];
                if (classAttr != null && classAttr.Value == className)
                {
                    return true;
                }

                return false;
            });

            return nodes;
        }

        public static HtmlNode SelectSingleNode(HtmlNode node, string tagName, string className = "")
        {
            var nodes = SelectNode(node, tagName, className);
            if (nodes == null)
            {
                return null;
            }

            return nodes.First();
        }

        public static bool WhereNode(HtmlNode node, string tagName, string className = "")
        {
            if (node.Name == tagName)
            {
                if (!string.IsNullOrEmpty(className))
                {
                    var classAttr = node.Attributes["class"];
                    if (classAttr != null && classAttr.Value == className)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
