using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    class Utils
    {
        /// <summary>
        /// Проверка наличия json
        /// </summary>
        internal static bool CheckOrCreateJson()
        {
            string path = $@"{Directory.GetCurrentDirectory()}\dishes.json";
            return File.Exists(path) && File.ReadAllText(path).Length != 0;
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
                todayMeal.Add(Dish.returnStringWithInfo(dish));
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
        /// Десериализация джейсона и создание листа продуктов
        /// </summary>
        internal static List<string> DeserealizeJson(string path, stringCreator function, ref List<Dish> targetDishesList)
        {
            if (!File.Exists(path) || File.ReadAllText(path).Length == 0) return null;
            List<string> names = new List<string>();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                targetDishesList = (List<Dish>)jsonFormatter.ReadObject(fs);
            }
            foreach (var dish in targetDishesList)
            {
                names.Add(function?.Invoke(dish));
            }
            return names;
        }


        internal static Acc DeserealizeJson(string path, ref Acc account)
        {
            if (!File.Exists(path) || File.ReadAllText(path).Length == 0) return null;
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Acc));
            using (FileStream fs = new FileStream(path, FileMode.Open))
                return (Acc)jsonFormatter.ReadObject(fs);
        }


        /// <summary>
        /// Сериализация в джейсон
        /// </summary>>
        internal static void SerializeToJson(string fileName, dynamic serializableObj)
        {
            if (serializableObj == null) return;
            File.Delete($@"{Directory.GetCurrentDirectory()}\{fileName}.json");
            using (FileStream _fs = new FileStream($@"{Directory.GetCurrentDirectory()}\{fileName}.json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(serializableObj.GetType());
                contractJsonSerializer.WriteObject(_fs, serializableObj); //запись в json
            }
        }
    }
}
