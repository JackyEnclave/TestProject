using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Linq;
using System;
using System.Windows.Input;

namespace CaloryCalculator
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> _dishes;
        private List<string> names = new List<string>();
        private string dishInfo = null;
        List <string> todayMeal = new List<string>();
        View.DishQuantity dishQuantity;
        public ViewModel()
        {
            if (!Parser.CheckOrCreateDirectory())
                Parser.ParseData();

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(@"C:\Users\Public\Calorizzation\dishes.json", FileMode.Open))
            {
                _dishes = (List<Dish>)jsonFormatter.ReadObject(fs);
                DishesList = names;
            }

            foreach (var dish in _dishes)
            {
                names.Add(dish.Name);
            }
        }

        private bool OK;
        private RelayCommand buttonOK;
        public RelayCommand ButtonOK
        {
            get
            {
                return buttonOK ??
                    (buttonOK = new RelayCommand(obj =>
                    {
                        OK = true;
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public List<string> DishesList
        {
            get { return names; }
            set
            {
                OnPropertyChanged(nameof(DishesList));
            }
        }

        public string DishInfo
        {
            get { return dishInfo; }
            set
            {
                OnPropertyChanged(nameof(DishInfo));
            }
        }

        public string[] TodayMeal
        {
            get { return todayMeal.ToArray(); }
            set
            {
                OnPropertyChanged(nameof(TodayMeal));
            }
        }

        private string _selectedObject;
        public string SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                Dish currDish = new Dish();

                _selectedObject = value;
                OnPropertyChanged(nameof(SelectedObject));
                dishQuantity = new View.DishQuantity();
                currDish = _dishes.FirstOrDefault(x => x.Name == _selectedObject);
                dishInfo = $"{currDish.Name}\nБелки: {currDish.Prots}\nЖиры: {currDish.Fats}\nУглеводы: {currDish.Carbohyds}\nКалории: {currDish.Calories}";
                double _dishCalory = currDish.Calories * _dishQuantity;
                todayMeal.Add($"{currDish.Name} ({_dishQuantity} гр./{_dishCalory} ккал)");
                TodayMeal = todayMeal.ToArray();
                DishInfo = dishInfo;
                dishQuantity.ShowDialog();
            }
        }

        private double _dishQuantity;
        public double DishQuantity
        {
            get { return _dishQuantity; }
            set
           {
                _dishQuantity = value;
                RaisePropertyChanged(() => DishQuantity);
            }
        }
    }
}
