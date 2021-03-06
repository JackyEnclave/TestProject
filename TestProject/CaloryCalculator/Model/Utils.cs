﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    static class Utils
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
            if (todayDishesList.Any(x => x.Name == currDish.Name) && dishQuantity != null)
            {   
                var dishFromList = todayDishesList.FirstOrDefault(x => x.Name == currDish.Name);
                if (-dishQuantity >= dishFromList.Quantity)
                    todayDishesList.Remove(todayDishesList[todayDishesList.IndexOf(dishFromList)]);
                else
                    todayDishesList[todayDishesList.IndexOf(dishFromList)].Quantity += (double)dishQuantity;
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
        internal static string CreateDishInfo(Dish currDish) => $"{currDish.Name}\n-----------\nБелки: {currDish.Prots}\nЖиры: {currDish.Fats}\nУглеводы: {currDish.Carbohyds}\nКалории: {currDish.Calories}";


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


        internal static Acc DeserealizeJson(string path)
        {
            if (!File.Exists(path) || File.ReadAllText(path).Length == 0) return null;
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Acc));
            using (FileStream fs = new FileStream(path, FileMode.Open))
                return (Acc)jsonFormatter.ReadObject(fs);
        }


        /// <summary>
        /// Сериализация в джейсон
        /// </summary>
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


        /// <summary>
        /// Проверка валидности введенных данных для аккаунта
        /// </summary>
        internal static bool CheckInputData(Acc acc)
        {
            if (acc == null) return false;
            bool isCorrect = !string.IsNullOrEmpty(acc.Name) && acc.Height.HasValue &&
                             acc.Weight.HasValue && acc.Age.HasValue && acc.Gender != Acc.Genders.UNKNOWN && 
                             acc.Target != Acc.Targets.UNKNOWN;
            return 
                (isCorrect);
        }


        internal static string getGenderName (Acc acc)
        {
            if (acc.Gender == Acc.Genders.MAN)
                return "Мужской";
            else if (acc.Gender == Acc.Genders.WOMAN)
                return "Женский";
            else return null;
        }

        internal static string getTargetName (Acc acc)
        {
            if (acc.Target == Acc.Targets.WEIGHTLOSING)
                return "Похудение";
            else if (acc.Target == Acc.Targets.WEIGHTSAVING)
                return "Сохранение веса";
            else if (acc.Target == Acc.Targets.WEIGHTGAINING)
                return "Массонабор";
            else return null;
        }
    }
}
