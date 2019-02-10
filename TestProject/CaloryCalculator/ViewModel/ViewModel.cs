using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Linq;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> Dishes { get; }
        private readonly List<string> names = new List<string>();
        private readonly List <string> todayMeal = new List<string>();
        private readonly List <Dish> todayDishesList = new List<Dish>();
        private static double Quantity { get; set; }

        public ViewModel()
        {
            if (!Parser.CheckOrCreateDirectory())
                Parser.ParseData();

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(@"C:\Users\Public\Calorizzation\dishes.json", FileMode.Open))
            {
                Dishes = (List<Dish>)jsonFormatter.ReadObject(fs);
                DishesList = names;
            }

            foreach (var dish in Dishes)
            {
                names.Add(dish.Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        
        public List<string> DishesList
        {
            get => names;
            set => OnPropertyChanged(nameof(DishesList));
        }

        private string dishInfo;
        public string DishInfo
        {
            get => dishInfo;
            set => OnPropertyChanged(nameof(DishInfo));
        }

        public List<string> TodayMeal
        {
            get => todayMeal.ToList();
            set => OnPropertyChanged(nameof(TodayMeal));
        }

        private double _dishQuantity;
        public double DishQuantity
        {
            get => _dishQuantity;
            set => Quantity = value;
        }

        private string _selectedObject;
        public string SelectedObject
        {
            get => _selectedObject;
            set
            {
                Dish currDish = new Dish();
                currDish = Dishes.FirstOrDefault(x => x.Name == value);
                dishInfo = $"{currDish.Name}\nБелки: {currDish.Prots}\nЖиры: {currDish.Fats}\nУглеводы: {currDish.Carbohyds}\nКалории: {currDish.Calories}";
                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                currDish.Quantity = Quantity;
                if (todayDishesList.Contains(currDish))
                {
                    var dish = todayDishesList.FirstOrDefault(x => x.Name == currDish.Name);
                    todayDishesList[todayDishesList.IndexOf(dish)].Quantity += Quantity;
                    todayMeal[todayMeal.IndexOf(dish.Name)] =
                        $"{dish.Name} ({Quantity} гр./{currDish.Calories * currDish.Quantity / 100} ккал)";
                }
                todayDishesList.Add(currDish);
                todayMeal.Add($"{currDish.Name} ({Quantity} гр./{currDish.Calories * currDish.Quantity / 100} ккал)");
                TodayMeal = todayMeal;
                DishInfo = dishInfo;
            }
        }
    }
}
