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
    delegate string stringCreator (Dish currDish);
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> _dishes = new List<Dish>();
        private List<Dish> _todayDishesList = new List<Dish>();

        public ViewModel()
        {
            if (!Parser.CheckOrCreateDirectory())
                SendRequestToRefresh();
            DishesList = _names = DeserealizeJson("dishes", Parser.returnCleanString, ref _dishes);
            TodayMeal = _todayMeal = DeserealizeJson("todaydishes", Parser.returnStringWithInfo, ref _todayDishesList);
        }

        private RelayCommand _buttonRefresh;
        public RelayCommand RefreshButtonClick
        {
            get
            {
            return _buttonRefresh ??
                (_buttonRefresh = new RelayCommand(obj =>
                {
                    SendRequestToRefresh();
                    DishesList = _names = DeserealizeJson("dishes", Parser.returnCleanString, ref _dishes);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private List<string> _names;
        public List<string> DishesList
        {
            get => _names;
            set => OnPropertyChanged(nameof(DishesList));
        }

        private string _caloriesSum = "0 ккал";
        public string CaloriesSum
        {
            get => _caloriesSum;
            set => OnPropertyChanged(nameof(CaloriesSum));
        }

        private string _dishInfo;
        public string DishInfo
        {
            get => _dishInfo;
            set => OnPropertyChanged(nameof(DishInfo));
        }

        private List<string> _todayMeal;
        public List<string> TodayMeal
        {
            get => _todayMeal;
            set => OnPropertyChanged(nameof(TodayMeal));
        }
        public static double? DishQuantity { get; set; }

        public string SelectedObject
        {
            set
            {
                Dish currDish = _dishes.FirstOrDefault(x => x.Name == value);
                DishInfo = _dishInfo = Parser.CreateDishInfo(currDish);

                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                
                TodayMeal = _todayMeal = Parser.CreateDishesList(currDish, DishQuantity, _todayDishesList);
                CaloriesSum = _caloriesSum = Parser.CalculateSum(_todayDishesList);
                Parser.SerializeToJson("todaydishes", _todayDishesList);

                DishQuantity = null;
            }
        }

        private static void SendRequestToRefresh()
        {
            View.Refresh refresh = new View.Refresh();
            refresh.Show();
            Parser.ParseData();
            refresh.Close();
        }

        private static List<string> DeserealizeJson(string fileName, stringCreator function, ref List<Dish> targetDishesList)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream($@"C:\Users\Public\Calorizzation\{fileName}.json", FileMode.Open))
            {
                targetDishesList = (List<Dish>)jsonFormatter.ReadObject(fs);
            }
            List<string> names = new List<string>();
            foreach (var dish in targetDishesList)
            {
                names.Add(function?.Invoke(dish));
            }
            return names;
        }
    }
}
