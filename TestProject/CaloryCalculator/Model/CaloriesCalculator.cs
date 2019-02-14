using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    class Calculator
    {
        public static double CalculateCalories (Dish dish) => 4 * dish.Prots + 9 * dish.Fats + 4 * dish.Carbohyds;
        public static double? CalculateCaloryLimit (Acc acc) => 9.99*acc.Weight +6.25*acc.Height-4.92*acc.Age+5;
        public static string CreateUserInformation (Acc acc) => 
            $"{acc.Name}\n{acc.Height} см, {acc.Weight} кг, {acc.Age} лет\nЛимит калорий: {CalculateCaloryLimit(acc)} ккал";
    }
}
