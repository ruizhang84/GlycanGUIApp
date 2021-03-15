using GlycanQuant.Engine.Util;
using GlycanQuant.Model;
using GlycanQuant.Model.NGlycans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Builder.NGlycans
{
    public class NGlycanPeak : IGlycanPeak
    {
        IGlycan glycan;
        List<double> distrib;
        int order = 10;

        public NGlycanPeak(bool permethylated = true, int HexNAc = 2, int Hex = 3,
            int Fuc = 0, int NeuAc = 0, int NeuGc = 0)
        {
            glycan = new NGlycan(permethylated, HexNAc, Hex, Fuc, NeuAc, NeuGc);
            distrib = Brain.Run.Distribute(glycan.Formula(), order) ;
        }

        public List<double> GetDistrib()
        {
            return distrib;
        }

        public IGlycan GetGlycan()
        {
            return glycan;
        }
    }
}
