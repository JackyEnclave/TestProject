using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CaloryCalculator
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
            CreateHtml(CreateLink(1));
            HtmlNodeCollection htmlNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-title active')]");
            List<Dish> dishes = new List<Dish>();
            DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(List<Dish>));
            foreach (var node in htmlNodes)
            {
                Dish dish = new Dish { Name = node.InnerText.Trim()};
                dishes.Add(dish);
            }
            using (FileStream fs = new FileStream("dishes.json", FileMode.OpenOrCreate))
            {
                contractJsonSerializer.WriteObject(fs, dishes);
            }
        }
    }
}
