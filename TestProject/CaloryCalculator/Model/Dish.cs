using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    [Serializable]
    class Dish
    {
        private string _name;
        private double _prots;
        private double _fats;
        private double _carbohyds;
        private double _calories;
        private double _quantity = 0;
        public string Name { get => _name; set => _name = value; }
        public double Prots { get => _prots; set => _prots = value; }
        public double Fats { get => _fats; set => _fats = value; }
        public double Carbohyds { get => _carbohyds; set => _carbohyds = value; }
        public double Calories { get => _calories; set => _calories = value; }
        public double Quantity { get => _quantity; set => _quantity = value; }
        public static readonly string allDishesList = $@"{Directory.GetCurrentDirectory()}\dishes.json";
        public static readonly string todayDishesPath = $@"{Directory.GetCurrentDirectory()}\todaydishes.json";
        internal static string returnCleanString(Dish currDish) => currDish.Name;
        internal static string returnStringWithInfo(Dish currDish) =>
            $"{currDish.Name} ({currDish.Quantity} гр./{currDish.Calories * currDish.Quantity / 100} ккал)";
    }
}
