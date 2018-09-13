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
        FileStream _fs = new FileStream("dishes.json", FileMode.OpenOrCreate);
        WebClient webClient = new WebClient();

        public void ParseData()
        {
            webClient.Encoding = Encoding.UTF8;
            _htmlDocument.LoadHtml(webClient.DownloadString($"http://www.calorizator.ru/product/all"));
            for (int i = 1; i < int.Parse(_htmlDocument.DocumentNode.SelectSingleNode("//li [@class='pager-last']").InnerText); i++)
            {
                _htmlDocument.LoadHtml(webClient.DownloadString($"http://www.calorizator.ru/product/all?page={i}"));
                HtmlNodeCollection htmlNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-title active')]");
                Console.WriteLine($"HTML страницы {i} успешно получен");
                List<Dish> dishes = new List<Dish>();
                DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(List<Dish>));
                Console.WriteLine($"Json страницы {i} успешно создан");
                foreach (var node in htmlNodes)
                {
                    Dish dish = new Dish { Name = node.InnerText.Trim() };
                    dishes.Add(dish);
                    //Console.WriteLine($"Добавлено: {dish.Name}");
                }
                contractJsonSerializer.WriteObject(_fs, dishes);
            }
        }
    }
}
