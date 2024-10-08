﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    public class BranchAndBoundItemModel
    {
        public double Weight { get; set; }
        public double Value { get; set; }
        public double Ratio { get; set; }
        public string Name { get; set; }
        public double IsSelected { get; set; }
        public bool Locked { get; set; }

        public string ToString()
        {
            return Name + "\t|" + Math.Round(IsSelected, 3) + "\t|" + Math.Round(Weight, 3) + "\t|" + Math.Round(Value, 3) + "\t|"+Locked;
        }
    }
}
