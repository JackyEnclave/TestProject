using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    class Calculator
    {
        public delegate double returnValueDelegate(Dish dish);
        public static double CalculateCalories (Dish dish) => 4 * dish.Prots + 9 * dish.Fats + 4 * dish.Carbohyds;

        public static double? CalculateCaloryLimit(Acc acc)
        {
            double? baseManLimit = 9.99 * acc.Weight + 6.25 * acc.Height - 4.92 * acc.Age + 5;
            double? baseWomanLimit = 9.99 * acc.Weight + 6.25 * acc.Height - 4.92 * acc.Age - 161;
            double coef = 1;

            if (acc.Target == Acc.Targets.WEIGHTGAINING)
                coef = 1.2;
            else if (acc.Target == Acc.Targets.WEIGHTLOSING)
                coef = 0.8;

            if (acc.Gender == Acc.Genders.MAN)
                return baseManLimit * coef;
            else if (acc.Gender == Acc.Genders.WOMAN)
                return baseWomanLimit * coef;
            else return null;
        }

        public static double CalculateFinishSum(List<Dish> dishesList, returnValueDelegate getParam)
        {
            double sum = 0;
            dishesList.ForEach(x => sum += getParam(x) * x.Quantity / 100);
            return sum;
        }
        public static double returnCalories(Dish dish) => dish.Calories;
        public static double returnProts(Dish dish) => dish.Prots;
        public static double returnFats(Dish dish) => dish.Fats;
        public static double returnCarbs(Dish dish) => dish.Carbohyds;
        public static string CreateUserInformation(Acc acc)
        {
            return $"{acc.Name}\n" +
                $"{acc.Height} см, {acc.Weight} кг, {acc.Age} лет\n" +
                $"пол: {Utils.getGenderName(acc)}, цель: {Utils.getTargetName(acc)}\n" +
                $"Лимит калорий: {CalculateCaloryLimit(acc)} ккал";
        }
    }
}
