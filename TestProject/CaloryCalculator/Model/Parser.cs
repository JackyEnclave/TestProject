using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Model
{
    class Parser
    {
        private static HtmlDocument _htmlDocument = new HtmlDocument();

        public static string CreateLink(int page) => $"http://www.calorizator.ru/product/all?page={page - 1}";

        public static void CreateHtml(string link)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            _htmlDocument.LoadHtml(webClient.DownloadString(link));
        }
        public void ParseData()
        {
            CreateHtml(CreateLink(0));
            //for (int i = 0; i<65; i++)
            //{
            //    CreateLink(i);
            //}
        }
    }
}
