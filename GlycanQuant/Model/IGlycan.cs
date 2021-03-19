using GlycanQuant.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model
{
    public enum Monosaccharide
    {
        HexNAc, Hex, Fuc, NeuAc, NeuGc
    }

    public interface IGlycan
    {
        Dictionary<Monosaccharide, int> Composition();
        void SetComposition(Dictionary<Monosaccharide, int> composition);
        Compound Formula();
        void SetFormula(Compound formula);
        string Name();
        void SetName(string name);
        double Mass();
    }
}
