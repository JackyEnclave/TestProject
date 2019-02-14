using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Linq;
using System.Windows.Input;

namespace CaloryCalculator
{
    delegate string stringCreator (Dish currDish);
    class ViewModel : INotifyPropertyChanged
    {
        private List<Dish> _allDishesList = new List<Dish>();
        private List<Dish> _todayDishesList = new List<Dish>();
        private Acc _account;

        public ViewModel()
        {
            if (!Parser.CheckOrCreateJson())
                SendRequestToRefresh();

            //десериализуем данные из джейсонов
            AllDishesNames = _allDishesNames = Parser.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
            TodayMeal = _todayMeal = Parser.DeserealizeJson(Dish.todayDishesPath, Dish.returnStringWithInfo, ref _todayDishesList);
            _account = Parser.DeserealizeJson(Acc.AccPath, ref _account);

            if (!File.Exists(Acc.AccPath) || File.ReadAllText(Acc.AccPath).Length == 0)
            {
                View.UserParams userParams = new View.UserParams();
                userParams.ShowDialog();
                _account = ViewModelUsersParameters.Acc;
                Parser.SerializeToJson("acc", _account);
            }

            UserInfo = Calculator.CreateUserInformation(_account);

            //выводим корректную начальную сумму калорий
            double sum = 0;
            _todayDishesList.ForEach(x => CaloriesSum = _caloriesSum = $"{sum += x.Calories * x.Quantity / 100} ккал");
        }

        #region Свойства
        private RelayCommand _buttonRefresh;
        public RelayCommand RefreshButtonClick
        {
            get
            {
            return _buttonRefresh ??
                (_buttonRefresh = new RelayCommand(obj =>
                {
                    SendRequestToRefresh();
                    AllDishesNames = _allDishesNames = Parser.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private List<string> _allDishesNames;
        public List<string> AllDishesNames
        {
            get => _allDishesNames;
            set => OnPropertyChanged(nameof(AllDishesNames));
        }

        private string _caloriesSum;
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
        public static string UserInfo { get; set; }

        public string SelectedObject
        {
            set
            {
                Dish currDish = _allDishesList.FirstOrDefault(x => x.Name == value);
                DishInfo = _dishInfo = Parser.CreateDishInfo(currDish);

                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                
                TodayMeal = _todayMeal = Parser.CreateDishesList(currDish, DishQuantity, _todayDishesList);
                CaloriesSum = _caloriesSum = Parser.CalculateSum(_todayDishesList);
                Parser.SerializeToJson("todaydishes", _todayDishesList);

                DishQuantity = null;
            }
        }
        #endregion

        /// <summary>
        /// Отсылка запроса на обновление листа продуктов
        /// </summary>
        private static void SendRequestToRefresh()
        {
            View.Refresh refresh = new View.Refresh();
            refresh.Show();
            Parser.ParseData();
            refresh.Close();
        }
    }
}
