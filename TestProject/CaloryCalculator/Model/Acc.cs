﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Model
{
    class Acc
    {
        private string _name;
        private SecureString _password;
        private int _age;
        private int _weight;
        private int _height;
        public string Name { get => _name; set => _name = value; }
        public SecureString Password { set => _password = value; }
        public int Age { get => _age; set => _age = value; }
        public int Weight { get => _weight; set => _weight = value; }
        public int Height { get => _height; set => _height = value; }

    }
}
