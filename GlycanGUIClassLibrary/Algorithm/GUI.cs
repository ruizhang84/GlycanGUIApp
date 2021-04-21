using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUI.Algorithm
{
    public class GUI
    {
        public int Unit { get; set; }
        public int Scan { get; set; }
        public IPeak Peak { get; set; }

        public GUI(int unit, int scan, IPeak peak)
        {
            Unit = unit;
            Scan = scan;
            Peak = peak;
        }
    }
}
