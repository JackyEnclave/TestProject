﻿using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<Dish> _dishes;
        public List<string> names = new List<string>();

        public ViewModel()
        {
            if (!Directory.Exists(@"C:\Users\Public\Calorizzation"))
                Directory.CreateDirectory(@"C:\Users\Public\Calorizzation");
            FileInfo fi = new FileInfo(@"C:\Users\Public\Calorizzation\dishes.json");

            if (!fi.Exists || fi.Length == 0)
            {
                Parser parser = new Parser();
                MessageBox.Show("Это может занять пару минут", "Идет обновление базы данных", MessageBoxButton.OK, MessageBoxImage.Information);
                parser.ParseData();
            }
            
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
