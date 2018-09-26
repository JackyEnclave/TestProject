using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ViewModel()
        {
            FileInfo fi = new FileInfo(@"D:\Program Files\dishes.json");
            if (!fi.Exists || fi.Length == 0)
            {
                Parser parser = new Parser();
                parser.ParseData();
            }

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Dish>));
            using (FileStream fs = new FileStream(@"D:\Program Files\dishes.json", FileMode.Open))
            {
                List<Dish> dishes = (List<Dish>)jsonFormatter.ReadObject(fs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
