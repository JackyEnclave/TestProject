using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;

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
            //создаем джейсон с продуктами при первом запуске
            if (!Utils.CheckOrCreateJson())
                SendRequestToRefresh();

            //десериализуем данные из джейсонов
            AllDishesNames = _allDishesNames = Utils.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
            TodayMeal = _todayMeal = Utils.DeserealizeJson(Dish.todayDishesPath, Dish.returnStringWithInfo, ref _todayDishesList);
            _account = Utils.DeserealizeJson(Acc.AccPath, ref _account);

            //создаем аккаунт при первом запуске
            if (!File.Exists(Acc.AccPath) || File.ReadAllText(Acc.AccPath).Length == 0)
                CreateAcc();

            //информация о пользователе
            UserInfo = _userInfo = Calculator.CreateUserInformation(_account);

            //выводим корректную начальную сумму калорий, белков, жиров и углеводов
            RefreshInfo();
        }


        /// <summary>
        /// Отсылка запроса на обновление листа продуктов
        /// </summary>
        private void SendRequestToRefresh()
        {
            BusyIdicator = true;
            Task.Factory.StartNew(() =>
            {
                Parser.ParseData();
            }).ContinueWith((task) =>
            {
                if (task.IsCompleted)
                {
                    AllDishesNames = _allDishesNames = Utils.DeserealizeJson(Dish.allDishesList, Dish.returnCleanString, ref _allDishesList);
                    BusyIdicator = false;
                }
            });
        }


        /// <summary>
        /// Обновление информации о калориях и БЖУ за сегодня
        /// </summary>
        private void RefreshInfo()
        {
            double protsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnProts);
            double fatsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnFats);
            double carbsSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnCarbs);
            double caloriesSum = Calculator.CalculateFinishSum(_todayDishesList, Calculator.returnCalories);
            CaloriesSum = _caloriesSum = $"{caloriesSum} ккал";
            Message = _message = MessageCreator.GetMessage(_account, caloriesSum);
            ProtsFatsCarbsSums = _protsFatsCarbsSums = $"БЖУ: {(int)protsSum}/{(int)fatsSum}/{(int)carbsSum}";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        /// <summary>
        /// Вызов окна количества продуктов
        /// </summary>
        private void CallQuantityWindow()
        {
            View.DishQuantity dishQuantity = new View.DishQuantity();
            dishQuantity.ShowDialog();
        }


        private void CreateAcc()
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


        #region Свойства
        private RelayCommand _buttonRefresh;
        public RelayCommand RefreshButtonClick
        {
            get
            {
                return _buttonRefresh ?? (_buttonRefresh = new RelayCommand(obj =>
                {
                        SendRequestToRefresh();
                }));
            }
        }

        private RelayCommand _buttonAccRefresh;
        public RelayCommand RefreshAccButtonClick
        {
            get
            {
                return _buttonAccRefresh ?? (_buttonAccRefresh = new RelayCommand(obj =>
                {
                    _account = new Acc();
                    File.Delete(Acc.AccPath);
                    CreateAcc();
                    UserInfo = _userInfo = Calculator.CreateUserInformation(_account);
                }));
            }
        }

        private RelayCommand _buttonClean;
        public RelayCommand CleanButtonClick
        {
            get
            {
                return _buttonClean ??
                    (_buttonClean = new RelayCommand(execute: obj =>
                    {
                        TodayMeal = _todayMeal = new List<string>();
                        _todayDishesList = new List<Dish>();
                        RefreshInfo();
                        File.Delete(Dish.todayDishesPath);
                    }, 
                    canExecute: obj => _todayDishesList.Count != 0));
            }
        }

        private List<string> _allDishesNames;
        public List<string> AllDishesNames
        {
            get => _allDishesNames;
            set => OnPropertyChanged(nameof(AllDishesNames));
        }

        private bool _indicator;
        public bool BusyIdicator
        {
            get => _indicator;
            set
            {
                if (value != _indicator)
                {
                    _indicator = value;
                    OnPropertyChanged(nameof(BusyIdicator));
                }
            }
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
        
        private string _message;
        public string Message
        {
            get => _message;
            set => OnPropertyChanged(nameof(Message));
        }

        private string _userInfo;
        public string UserInfo
        {
            get => _userInfo;
            set => OnPropertyChanged(nameof(UserInfo));
        }

        public static double? DishQuantity { get; set; }

        public string SelectedObject
        {
            set
            {
                Dish currDish = _allDishesList.FirstOrDefault(x => x.Name == value);
                DishInfo = _dishInfo = Utils.CreateDishInfo(currDish);
                CallQuantityWindow();
                
                TodayMeal = _todayMeal = Utils.CreateDishesList(currDish, DishQuantity, _todayDishesList);
                RefreshInfo();
                
                Utils.SerializeToJson("todaydishes", _todayDishesList);

                DishQuantity = null;
            }
        }

        public string SelectedObjectFromTodayDishes
        {
            set
            {
                Dish currDish = _allDishesList.FirstOrDefault(x => value.StartsWith(x.Name));
                DishInfo = _dishInfo = Utils.CreateDishInfo(currDish);
                CallQuantityWindow();

                TodayMeal = _todayMeal = Utils.CreateDishesList(currDish, DishQuantity, _todayDishesList);
                RefreshInfo();

                Utils.SerializeToJson("todaydishes", _todayDishesList);

                DishQuantity = null;
            }
        }

        // механизм поиска
        private string _filterObject;
        public string FilterObject
        {
            get => _filterObject; 
            set
            {
                _filterObject = value;
                _allDishesNames = new List<string>();
                foreach (var item in _allDishesList)
                {
                    if (item.Name.ToLower().Contains(value.ToLower()))
                        _allDishesNames.Add(item.Name);
                }
                AllDishesNames = _allDishesNames;
            }
        }
        #endregion
    }
}
