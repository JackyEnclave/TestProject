using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Linq;
using System.Windows.Input;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> Dishes { get; set; }
        private readonly List<Dish> todayDishesList = new List<Dish>();

        public ViewModel()
        {
            if (!Parser.CheckOrCreateDirectory())
                SendRequestToRefresh();
            DeserealizeJson(this);
        }

        private RelayCommand buttonRefresh;
        public RelayCommand RefreshButtonClick
        {
            get
            {
            return buttonRefresh ??
                (buttonRefresh = new RelayCommand(obj =>
                {
                    SendRequestToRefresh();
                    DeserealizeJson(this);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private List<string> names;
        public List<string> DishesList
        {
            get => names;
            set => OnPropertyChanged(nameof(DishesList));
        }

        private string caloriesSum = "0 ккал";
        public string CaloriesSum
        {
            get => caloriesSum;
            set => OnPropertyChanged(nameof(CaloriesSum));
        }

        private string dishInfo;
        public string DishInfo
        {
            get => dishInfo;
            set => OnPropertyChanged(nameof(DishInfo));
        }

        private List<string> todayMeal;
        public List<string> TodayMeal
        {
            get => todayMeal;
            set => OnPropertyChanged(nameof(TodayMeal));
        }
        public static double? DishQuantity { get; set; }

        public string SelectedObject
        {
            set
            {
                Dish currDish = Dishes.FirstOrDefault(x => x.Name == value);
                DishInfo = dishInfo = Parser.CreateDishInfo(currDish);

                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                
                TodayMeal = todayMeal = Parser.CreateDishesList(currDish, DishQuantity, todayDishesList);
                CaloriesSum = caloriesSum = Parser.CalculateSum(todayDishesList);
                Parser.SerializeToJson("todaydishes", todayDishesList);

                DishQuantity = null;
            }
        }

        private static void SendRequestToRefresh()
        {
            View.Refresh refresh = new View.Refresh();
            refresh.Show();
            Parser.ParseData();
            refresh.Close();
            refresh = null;
        }

        private static void DeserealizeJson(ViewModel vm)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(@"C:\Users\Public\Calorizzation\dishes.json", FileMode.Open))
            {
                vm.Dishes = (List<Dish>)jsonFormatter.ReadObject(fs);
            }
            vm.names = new List<string>();
            foreach (var dish in vm.Dishes)
            {
                vm.names.Add(dish.Name);
            }
            vm.DishesList = vm.names;
        }
    }
}
