using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    class Dish
    {
        private string _name;
        private double _prots;
        private double _fats;
        private double _carbohyds;
        private double _calories;
        public string Name { get => _name; set => _name = value; }
        public double Prots { get => _prots; set => _prots = value; }
        public double Fats { get => _fats; set => _fats = value; }
        public double Carbohyds { get => _carbohyds; set => _carbohyds = value; }
        public double Calories { get => _calories; set => _calories = value; }
    }
}
