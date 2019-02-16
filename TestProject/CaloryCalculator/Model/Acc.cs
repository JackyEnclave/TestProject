using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaloryCalculator
{
    [Serializable]
    class Acc
    {
        public enum Genders {UNKNOWN = 0, MAN = 1, WOMAN = 2};
        public enum Targets {UNKNOWN = 0, WEIGHTLOSING = 1, WEIGHTSAVING = 2, WEIGHTGAINING = 3};
        private string _name;
        private int? _age;
        private int? _weight;
        private int? _height;
        private Genders _gender;
        private Targets _target;
        public string Name { get => _name; set => _name = value; }
        public int? Age { get => _age; set => _age = value; }
        public int? Weight { get => _weight; set => _weight = value; }
        public int? Height { get => _height; set => _height = value; }
        public Genders Gender { get => _gender; set => _gender = value; }
        public Targets Target { get => _target; set => _target = value; }


        public static readonly string AccPath = $@"{Directory.GetCurrentDirectory()}\acc.json";
    }
}
