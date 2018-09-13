using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CaloryCalculator
{
    class Parser
    {
        private HtmlDocument _htmlDocument = new HtmlDocument();
        private FileStream _fs = new FileStream("dishes.json", FileMode.OpenOrCreate);
        private WebClient _webClient = new WebClient();
        private int _lastPage;

        public Parser()
        {
            _webClient.Encoding = Encoding.UTF8;
            _htmlDocument.LoadHtml(_webClient.DownloadString($"http://www.calorizator.ru/product/all"));
            _lastPage = int.Parse(_htmlDocument.DocumentNode.SelectSingleNode("//li [@class='pager-last']").InnerText);
        }

        public void ParseData()
        {
            for (int i = 1; i < _lastPage; i++)
            {
                _htmlDocument.LoadHtml(_webClient.DownloadString($"http://www.calorizator.ru/product/all?page={i}"));
                HtmlNodeCollection nameNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-title active')]");
                HtmlNodeCollection protNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-protein-value')]");
                HtmlNodeCollection fatNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-fat-value')]");
                HtmlNodeCollection carbohydNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-carbohydrate-value')]");
                Console.WriteLine($"HTML страницы {i} успешно получен");
                List<Dish> dishes = new List<Dish>();
                DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(List<Dish>));
                Console.WriteLine($"Json страницы {i} успешно создан");
                if (nameNodes.Count != protNodes.Count || protNodes.Count != fatNodes.Count || fatNodes.Count != carbohydNodes.Count)
                {
                    Console.WriteLine("Неверно распарсилась страница");
                    Console.ReadKey();
                }
                for (int k = 0; k < nameNodes.Count; k++)
                {
                    Dish dish = new Dish {
                        Name = nameNodes[k].InnerText.Trim(),
                        Prots = Convert.ToDouble(0 + protNodes[k].InnerText.Replace(".", ",").Trim()),
                        Fats = Convert.ToDouble(0 + fatNodes[k].InnerText.Replace(".", ",").Trim()),
                        Carbohyds = Convert.ToDouble(0 + carbohydNodes[k].InnerText.Replace(".", ",").Trim()),
                };
                    dish.Calories = Calculator.CalculateCalories(dish);
                    dishes.Add(dish);
                    Console.WriteLine($"Добавлено: {dish.Name}\nБелки: {dish.Prots}\nЖиры: {dish.Fats}\nУглеводы: {dish.Carbohyds}\nКалории: {dish.Calories}\n");
                }
                contractJsonSerializer.WriteObject(_fs, dishes);
            }
        }
    }
}
