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

            SerializeToJson("dishes", dishes);
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

        /// <summary>
        /// Проверка наличия директории и json
        /// </summary>
        internal static bool CheckOrCreateDirectory()
        {
            if (!Directory.Exists(@"C:\Users\Public\Calorizzation"))
                Directory.CreateDirectory(@"C:\Users\Public\Calorizzation");

            FileInfo fi = new FileInfo(@"C:\Users\Public\Calorizzation\dishes.json");

            return fi.Exists && fi.Length != 0;
        }

        /// <summary>
        /// Создание списка захомяченых продуктов
        /// </summary>
        internal static List<string> CreateDishesList(Dish currDish, double? dishQuantity, List<Dish> todayDishesList)
        {
            if (todayDishesList.Contains(currDish) && dishQuantity != null)
            {
                todayDishesList[todayDishesList.IndexOf(currDish)].Quantity += (double)dishQuantity;
            }
            else if (dishQuantity != null && dishQuantity != 0)
            {
                currDish.Quantity = (double)dishQuantity;
                todayDishesList.Add(currDish);
            }

            var todayMeal = new List<string>();
            foreach (var dish in todayDishesList)
                todayMeal.Add($"{dish.Name} ({dish.Quantity} гр./{dish.Calories * dish.Quantity / 100} ккал)");
            return todayMeal;
        }

        /// <summary>
        /// Создание информации о блюде для корректного вывода с учетом длины строки
        /// </summary>
        internal static string CreateDishInfo(Dish currDish)
        {
            string newCurrDishName = string.Empty;
            var splittingCurrDishName = currDish.Name.Split(' ');
            int stringLenght = 25; //максимальная длина строки, помещающаяся на строчку в wpf

            foreach (var name in splittingCurrDishName)
            {
                if (newCurrDishName.Length > stringLenght)
                {
                    newCurrDishName = $"{newCurrDishName}\n{name}";
                    stringLenght += stringLenght;
                }
                else
                    newCurrDishName = $"{newCurrDishName} {name}";
            }

            return $"{newCurrDishName}\n-----------\nБелки: {currDish.Prots}\nЖиры: {currDish.Fats}\nУглеводы: {currDish.Carbohyds}\nКалории: {currDish.Calories}";
        }


        /// <summary>
        /// Вычисление суммы калорий
        /// </summary>
        internal static string CalculateSum(List<Dish> todayDishesList)
        {
            double sum = 0;
            foreach (var dish in todayDishesList)
            {
                sum += dish.Quantity*dish.Calories/100;
            }
            return $"{sum} ккал";
        }

        internal static void SerializeToJson(string fileName, List<Dish> dishes)
        {
            File.Delete($@"C:\Users\Public\Calorizzation\{fileName}.json");
            using (FileStream _fs = new FileStream($@"C:\Users\Public\Calorizzation\{fileName}.json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof(List<Dish>));
                contractJsonSerializer.WriteObject(_fs, dishes); //запись в json
            }
        }
    }
}
