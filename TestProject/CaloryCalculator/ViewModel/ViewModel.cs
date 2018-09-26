using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> _dishes;
        public List<string> names = new List<string>();
        public List<double> prots = new List<double>();
        public List<double> carbohyds = new List<double>();
        public List<double> fats = new List<double>();
        public List<double> calories = new List<double>();

        public ViewModel()
        {
            FileInfo fi = new FileInfo(@"C:\Workroom\dishes.json");
            if (!fi.Exists || fi.Length == 0)
            {
                Parser parser = new Parser();
                parser.ParseData();
            }

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(@"C:\Workroom\dishes.json", FileMode.Open))
            {
                _dishes = (List<Dish>)jsonFormatter.ReadObject(fs);
                DishesList = names;
            }

            foreach (var dish in _dishes)
            {
                names.Add(dish.Name);
                prots.Add(dish.Prots);
                fats.Add(dish.Fats);
                carbohyds.Add(dish.Carbohyds);
                calories.Add(dish.Calories);
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
    }
}
