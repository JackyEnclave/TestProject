using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Windows;
using System.Threading;

namespace CaloryCalculator
{
    class Parser
    {
        private HtmlDocument _htmlDocument = new HtmlDocument();
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
            using (FileStream _fs = new FileStream(@"C:\Workroom\dishes.json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(List<Dish>));
                List<Dish> dishes = new List<Dish>();
                for (int i = 1; i < _lastPage; i++)
                {
                    _htmlDocument.LoadHtml(_webClient.DownloadString($"http://www.calorizator.ru/product/all?page={i}"));
                    HtmlNodeCollection nameNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-title active')]");
                    HtmlNodeCollection protNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-protein-value')]");
                    HtmlNodeCollection fatNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-fat-value')]");
                    HtmlNodeCollection carbohydNodes = _htmlDocument.DocumentNode.SelectNodes(@".//td[contains(@class, 'views-field-field-carbohydrate-value')]");

                    if (nameNodes.Count != protNodes.Count || protNodes.Count != fatNodes.Count || fatNodes.Count != carbohydNodes.Count)
                    {
                        MessageBox.Show("Неверно распарсилась страница, проверьте подключение к интернету и попробуйте снова", "Что-то пошло не так... :-(((", MessageBoxButton.OK, MessageBoxImage.Information);
                        Thread.Sleep(1000000);
                    }

                    for (int k = 0; k < nameNodes.Count; k++)
                    {
                        Dish dish = new Dish
                        {
                            Name = nameNodes[k].InnerText.Trim(),
                            Prots = Convert.ToDouble(0 + protNodes[k].InnerText.Replace(".", ",").Trim()),
                            Fats = Convert.ToDouble(0 + fatNodes[k].InnerText.Replace(".", ",").Trim()),
                            Carbohyds = Convert.ToDouble(0 + carbohydNodes[k].InnerText.Replace(".", ",").Trim()),
                        };
                        dish.Calories = Calculator.CalculateCalories(dish);
                        dishes.Add(dish);
                        Console.WriteLine($"Добавлено: {dish.Name}\nБелки: {dish.Prots}\nЖиры: {dish.Fats}\nУглеводы: {dish.Carbohyds}\nКалории: {dish.Calories}\n");
                    }
                }
                contractJsonSerializer.WriteObject(_fs, dishes);
            }
        }
    }
}
