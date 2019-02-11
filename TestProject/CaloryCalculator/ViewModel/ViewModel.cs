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
        private List <string> todayMeal;
        private readonly List <Dish> todayDishesList = new List<Dish>();

        public ViewModel()
        {
            if (!Parser.CheckOrCreateDirectory())
            {
                View.Refresh refresh = new View.Refresh();
                refresh.Show();
                Parser.ParseData();
                refresh.Close();
            }

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
            get => todayMeal;
            set => OnPropertyChanged(nameof(TodayMeal));
        }
        public static double DishQuantity { get; set; }

        private string _selectedObject;
        public string SelectedObject
        {
            get => _selectedObject;
            set
            {
                Dish currDish = Dishes.FirstOrDefault(x => x.Name == value);
                DishInfo = dishInfo = $"{currDish.Name}\nБелки: {currDish.Prots}\nЖиры: {currDish.Fats}\nУглеводы: {currDish.Carbohyds}\nКалории: {currDish.Calories}";

                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                
                TodayMeal = todayMeal = Parser.CreateDishesList(currDish, DishQuantity, todayDishesList);
            }
        }
    }
}
