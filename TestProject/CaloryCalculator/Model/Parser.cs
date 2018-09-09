using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Model
{
    class Parser
    {
        public static string CreateLink(int page) => $"http://www.calorizator.ru/product/all?page={page - 1}";
    }
}
