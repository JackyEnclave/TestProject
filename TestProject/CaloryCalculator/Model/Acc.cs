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
        private string _name;
        private int? _age;
        private int? _weight;
        private int? _height;
        public string Name { get => _name; set => _name = value; }
        public int? Age { get => _age; set => _age = value; }
        public int? Weight { get => _weight; set => _weight = value; }
        public int? Height { get => _height; set => _height = value; }

        public static readonly string AccPath = $@"{Directory.GetCurrentDirectory()}\acc.json";
    }
}
