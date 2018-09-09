using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    [DataContract]
    class Dish
    {
        [DataMember]
        private string _name;
        [DataMember]
        private double _prots;
        [DataMember]
        private double _fats;
        [DataMember]
        private double _carbohyds;
        [DataMember]
        private double _calories;
        public string Name { get => _name; set => _name = value; }
        public double Prots { get => _prots; set => _prots = value; }
        public double Fats { get => _fats; set => _fats = value; }
        public double Carbohyds { get => _carbohyds; set => _carbohyds = value; }
        public double Calories { get => _calories; set => _calories = value; }
    }
}
