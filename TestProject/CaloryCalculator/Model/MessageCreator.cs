using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    class MessageCreator
    {
        internal static string GetMessage(Acc acc, double? sum)
        {
            double? limit = Calculator.CalculateCaloryLimit(acc);
            string message = string.Empty;
            if (acc.Gender == Acc.Genders.MAN)
            {
                if (acc.Target == Acc.Targets.WEIGHTLOSING)
                {
                    if (limit * 0.9 > sum)
                        message = "Можно поесть еще, жирнее ты,\nвозможно, не станешь...но это не точно";
                    else if (limit * 0.9 <= sum && limit >= sum)
                        message = "Вот! Вот так норм, больше сегодня не ешь!\nПопей водички и иди спать";
                    else if (limit < sum && limit * 1.2 > sum)
                        message = "Таким количеством еды, которое ты сожрал\nсегодня, можно прокормить несколько собак\nили небольшого коня, например";
                    else if (limit * 1.2 <= sum)
                        message = "Вот это жирдяй! Килограмм пятьсот,\nне меньше!";
                }
                else if (acc.Target == Acc.Targets.WEIGHTSAVING)
                {
                    if (limit * 0.9 > sum)
                        message = "Надо поесть! Иначе будешь дрыщом";
                    else if (limit * 0.9 <= sum && limit * 1.2 >= sum)
                        message = "Вот! Вот так норм, больше сегодня не ешь!\nПопей водички и иди спать";
                    else if (limit * 1.2 < sum)
                        message = "Вот это жирдяй! Килограмм пятьсот,\nне меньше!";
                }
                else if (acc.Target == Acc.Targets.WEIGHTGAINING)
                {
                    if (limit > sum)
                        message = "Надо поесть еще, здоровяк!";
                    else if (limit <= sum)
                        message = "Уже нормально, но еще немного белка\nсделают тебя еще мощнее";
                }
            }
            else if (acc.Gender == Acc.Genders.WOMAN)
            {
                if (acc.Target == Acc.Targets.WEIGHTLOSING)
                {
                    if (limit * 0.9> sum)
                        message = "Можно поесть еще, жирнее ты,\nвозможно, не станешь...но это не точно";
                    else if (limit * 0.9 <= sum && limit >= sum)
                        message = "Вот! Вот так норм, больше сегодня не ешь!\nПопей водички и иди спать";
                    else if (limit < sum && limit * 1.2 > sum)
                        message = "Таким количеством еды, которое ты сожралa\nсегодня, можно прокормить несколько собак\nили небольшого коня, например";
                    else if (limit * 1.2 <= sum)
                        message = "Кто здесь самая толстая девочка?!\nДа-да, ты!";
                }
                else if (acc.Target == Acc.Targets.WEIGHTSAVING)
                {
                    if (limit * 0.9 > sum)
                        message = "Надо поесть! Иначе будешь дистрофичкой";
                    else if (limit * 0.9 <= sum && limit * 1.2 >= sum)
                        message = "Вот! Вот так норм, больше сегодня не ешь!\nПопей водички и иди спать";
                    else if (limit * 1.2 < sum)
                        message = "Кто здесь самая толстая девочка?!\nДа-да, ты!";
                }
                else if (acc.Target == Acc.Targets.WEIGHTGAINING)
                {
                    if (limit > sum)
                        message = "Надо поесть еще! Будешь большой и сильной";
                    else if (limit <= sum)
                        message = "Уже нормально, но еще немного белка\nсделают тебя еще мощнее";
                }
            }
            return message;
        }
    }
}
