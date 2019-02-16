using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CaloryCalculator
{
    class ViewModelUsersParameters
    {
        public static Acc Acc { get; set; } = new Acc();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public static List<string> gens = new List<string>() { "Мужской", "Женский" };
        public static List<string> Gens
        {
            get => gens;
        }

        private string _genderComboBoxSel;
        public string GenderComboBoxSel
        {
            get => _genderComboBoxSel;
            set
            {
                if (value == "Мужской")
                    Acc.Gender = Acc.Genders.MAN;
                else if (value == "Женский")
                    Acc.Gender = Acc.Genders.WOMAN;
                else
                    Acc.Gender = Acc.Genders.UNKNOWN;
            }
        }

        public static List<string> targets = new List<string>() { "Похудение", "Сохранение веса", "Массонабор" };
        public static List<string> Targets
        {
            get => targets;
        }

        private string _targetComboBoxSel;
        public string TargetComboBoxSel
        {
            get => _targetComboBoxSel;
            set
            {
                if (value == "Похудение")
                    Acc.Target = Acc.Targets.WEIGHTLOSING;
                else if (value == "Сохранение веса")
                    Acc.Target = Acc.Targets.WEIGHTSAVING;
                else if (value == "Массонабор")
                    Acc.Target = Acc.Targets.WEIGHTGAINING;
                else
                    Acc.Target = Acc.Targets.UNKNOWN;
            }
        }
    }
}
