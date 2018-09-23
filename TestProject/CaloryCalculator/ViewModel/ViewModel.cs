using System.ComponentModel;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;

namespace CaloryCalculator
{
    class ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ViewModel()
        {
            FileInfo fi = new FileInfo(@"C:\Program Files\CaloryCalculator\dishes.json");
            Parser parser = new Parser();
            if (fi == null || fi.Length == 0)
                parser.ParseData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
