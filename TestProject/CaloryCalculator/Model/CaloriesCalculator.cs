using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    class Calculator
    {
        public double CalculateCalories (Dish dish) => 4 * dish.Prots + 9 * dish.Fats + 4 * dish.Carbohyds;
        public double CalculateCaloryLimit (Acc acc) => 9.99*acc.Weight +6.25*acc.Height-4.92*acc.Age+5;
    }
}
