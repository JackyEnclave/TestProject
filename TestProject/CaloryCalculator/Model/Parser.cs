using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Windows;
using System.Threading;

namespace CaloryCalculator
{
    class Parser
    {
        public static void ParseData()
        {
            WebClient webClient = new WebClient {Encoding = Encoding.UTF8};

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(webClient.DownloadString($"http://www.calorizator.ru/product/all")); //забираем начальную страницу

            int.TryParse(htmlDocument.DocumentNode.SelectSingleNode("//li [@class='pager-last']").InnerText, out int lastPage); //получаем количество страниц

            List<Dish> dishes = new List<Dish>();

            for (int i = 1; i < 3; i++)
            {
                try
                {
                    htmlDocument.LoadHtml(webClient.DownloadString($"http://www.calorizator.ru/product/all?page={i}"));
                }
                catch
                {
                    MessageBox.Show("Проверьте подключение к интернету и попробуйте снова", "Что-то пошло не так... :-(((", MessageBoxButton.OK, MessageBoxImage.Information);
                    Thread.Sleep(1000000);
                }
                HtmlNodeCollection nameNodes = FindHtmlNodes(htmlDocument, @".//td[contains(@class, 'views-field-title active')]");
                HtmlNodeCollection protNodes = FindHtmlNodes(htmlDocument, @".//td[contains(@class, 'views-field-field-protein-value')]");
                HtmlNodeCollection fatNodes = FindHtmlNodes(htmlDocument, @".//td[contains(@class, 'views-field-field-fat-value')]");
                HtmlNodeCollection carbohydNodes = FindHtmlNodes(htmlDocument, @".//td[contains(@class, 'views-field-field-carbohydrate-value')]");

                if (nameNodes.Count != protNodes.Count || protNodes.Count != fatNodes.Count || fatNodes.Count != carbohydNodes.Count)
                {
                    MessageBox.Show("Неверно распарсилась страница, проверьте подключение к интернету и попробуйте снова", "Что-то пошло не так... :-(((", MessageBoxButton.OK, MessageBoxImage.Information);
                    Thread.Sleep(1000000);
                    continue;
                }

                dishes.AddRange(GetDishesList(nameNodes, protNodes, fatNodes, carbohydNodes));
            }
            Utils.SerializeToJson("dishes", dishes);
        }

        /// <summary>
        /// Инкапсуляция поиска узлов
        /// </summary>
        private static HtmlNodeCollection FindHtmlNodes(HtmlDocument htmlDocument, string nodePattern) => 
            htmlDocument.DocumentNode.SelectNodes(nodePattern);

        /// <summary>
        /// Получение списка продуктов
        /// </summary>
        private static List<Dish> GetDishesList (HtmlNodeCollection names, HtmlNodeCollection prots, HtmlNodeCollection fats, HtmlNodeCollection carbohyds)
        {
            List<Dish> dishes = new List<Dish>();
            for (int k = 0; k < names.Count; k++)
            {
                Dish dish = new Dish
                {
                    Name = names[k].InnerText.Trim(),
                    Prots = Convert.ToDouble(0 + prots[k].InnerText.Replace(".", ",").Trim()),
                    Fats = Convert.ToDouble(0 + fats[k].InnerText.Replace(".", ",").Trim()),
                    Carbohyds = Convert.ToDouble(0 + carbohyds[k].InnerText.Replace(".", ",").Trim()),
                };
                dish.Calories = Calculator.CalculateCalories(dish);
                dishes.Add(dish);
                Console.WriteLine($"Добавлено: {dish.Name}\nБелки: {dish.Prots}\nЖиры: {dish.Fats}\nУглеводы: {dish.Carbohyds}\nКалории: {dish.Calories}\n");
            }
            return dishes;
        }
    }
}
