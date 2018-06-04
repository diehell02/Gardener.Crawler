using System;
using System.Collections.Generic;
using System.Text;

namespace Gardener.Crawler.Api.Entity
{
    class Rule
    {
        public string Name { get; set; }

        public string XPath { get; set; }

        public RuleFun RuleFun { get; set; }

        public string Param { get; set; }        
    }

    enum RuleFun
    {
        Nodes = 0,
        Node = 1,
        Attr = 2,
        Text = 3
    }
}
