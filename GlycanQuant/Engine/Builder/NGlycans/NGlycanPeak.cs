using GlycanQuant.Model.Util;
using GlycanQuant.Model;
using GlycanQuant.Model.NGlycans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Builder.NGlycans
{
    public class NGlycanPeak : IGlycanPeak
    {
        IGlycan glycan;
        List<double> distrib;
        int order = 10;
        double peak = 0;
        private readonly double neutron = 1.0;

        public NGlycanPeak(bool permethylated = true, int HexNAc = 2, int Hex = 3,
            int Fuc = 0, int NeuAc = 0, int NeuGc = 0)
        {
            glycan = new NGlycan(permethylated, HexNAc, Hex, Fuc, NeuAc, NeuGc);
            distrib = Brain.Run.Distribute(glycan.Formula(), order);
            int extra = distrib.IndexOf(distrib.Max());
            peak = glycan.Mass() + neutron * extra;
        }

        public List<double> GetDistrib()
        {
            return distrib;
        }

        public IGlycan GetGlycan()
        {
            return glycan;
        }

        public double HighestPeak()
        {
            return peak;
        }
    }
}
