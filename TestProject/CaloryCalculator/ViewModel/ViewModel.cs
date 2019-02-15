using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
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
            if (!Utils.CheckOrCreateJson())
                SendRequestToRefresh();

            //десериализуем данные из джейсонов
            AllDishesNames = _allDishesNames = Utils.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
            TodayMeal = _todayMeal = Utils.DeserealizeJson(Dish.todayDishesPath, Dish.returnStringWithInfo, ref _todayDishesList);
            _account = Utils.DeserealizeJson(Acc.AccPath, ref _account);

            if (!File.Exists(Acc.AccPath) || File.ReadAllText(Acc.AccPath).Length == 0)
            {
                while (!Utils.CheckInputData(_account))
                {
                    MessageBox.Show("Нужно корректно заполнить поля. Буковки в поле 'Имя', циферки в поля 'Рост', 'Вес' и 'Возраст'", "Кто ты?", MessageBoxButton.OK, MessageBoxImage.Information);
                    View.UserParams userParams = new View.UserParams();
                    userParams.ShowDialog();
                    _account = ViewModelUsersParameters.Acc;
                }
                Utils.SerializeToJson("acc", _account);
            }

            //информация о пользователе
            UserInfo = Calculator.CreateUserInformation(_account);

            //выводим корректную начальную сумму калорий, белков, жиров и углеводов
            RefreshInfo();
        }


        /// <summary>
        /// Отсылка запроса на обновление листа продуктов
        /// </summary>
        private void SendRequestToRefresh()
        {
            View.Refresh refresh = new View.Refresh();
            refresh.Show();
            Parser.ParseData();
            refresh.Close();
        }

        private void RefreshInfo()
        {
            double protsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnProts);
            double fatsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnFats);
            double carbsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnCarbs);
            double caloriesSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnCalories);
            CaloriesSum = _caloriesSum = $"{caloriesSum} ккал";
            ProtsFatsCarbsSums = _protsFatsCarbsSums = $"БЖУ: {(int)protsSum}/{(int)fatsSum}/{(int)carbsSum}";
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
                    AllDishesNames = _allDishesNames = Utils.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
                }));
            }
        }

        private RelayCommand _buttonClean;
        public RelayCommand CleanButtonClick
        {
            get
            {
                return _buttonClean ??
                    (_buttonClean = new RelayCommand(obj =>
                    {
                        TodayMeal = _todayMeal = new List<string>();
                        _todayDishesList = new List<Dish>();
                        RefreshInfo();
                        File.Delete(Dish.todayDishesPath);
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

        private string _protsFatsCarbsSums;
        public string ProtsFatsCarbsSums
        {
            get => _protsFatsCarbsSums;
            set => OnPropertyChanged(nameof(ProtsFatsCarbsSums));
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
                DishInfo = _dishInfo = Utils.CreateDishInfo(currDish);

                View.DishQuantity dishQuantity = new View.DishQuantity();
                dishQuantity.ShowDialog();
                
                TodayMeal = _todayMeal = Utils.CreateDishesList(currDish, DishQuantity, _todayDishesList);
                RefreshInfo();

                Utils.SerializeToJson("todaydishes", _todayDishesList);

                DishQuantity = null;
            }
        }
        #endregion
    }
}
