using GlycanQuant.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.NGlycans
{
    public class NGlycan : IGlycan
    {
        public const double kHexNAc = 203.0794;
        public const double kHex = 162.0528;
        public const double kFuc = 146.0579;
        public const double kNeuAc = 291.0954;
        public const double kNeuGc = 307.0903;

        public const double kPermHexNAc = 245.1263;
        public const double kPermHex = 204.0998;
        public const double kPermFuc = 174.0892;
        public const double kPermNeuAc = 361.1737;  //N-acetyl-neuraminic acid
        public const double kPermNeuGc = 391.1842;  //N-glycolyl-neuraminic acid

        Dictionary<Monosaccharide, int> composition;
        Compound formula;
        string name = "";
        bool permethylated = true;

        public NGlycan(bool permethylated=true, int HexNAc=2, int Hex=3, 
            int Fuc=0, int NeuAc=0, int NeuGc = 0)
        {
            composition = new Dictionary<Monosaccharide, int>() 
            { 
                { Monosaccharide.HexNAc, HexNAc }, 
                { Monosaccharide.Hex, Hex },
                { Monosaccharide.Fuc, Fuc },
                { Monosaccharide.NeuAc, NeuAc },
                { Monosaccharide.NeuGc, NeuGc }
            };

            this.permethylated = permethylated;
            name = String.Join("-", composition.Select(i => i.Value));
            Dictionary<Element, int>  formulaComposition = new Dictionary<Element, int>();

            foreach(var sugar in composition.Keys)
            {
                Dictionary<Element, int> tempCompose = NMonosaccharideCreator.Get.SubCompositions(
                    sugar, permethylated);
                foreach (Element elm in tempCompose.Keys)
                {
                    if (formulaComposition.ContainsKey(elm))
                    {
                        formulaComposition[elm] = 0;
                    }
                    formulaComposition[elm] = tempCompose[elm] * composition[sugar];
                }
            }

            formula = new Compound(formulaComposition);
        }

        public Compound Formula()
        {
            return formula;
        }

        public Dictionary<Monosaccharide, int> Composition()
        {
            return composition;
        }

        public double Mass()
        {
            return formula.Mass;
        }

        public string Name()
        {
            return name;
        }

        public void SetComposition(Dictionary<Monosaccharide, int> composition)
        {
            this.composition = composition;
        }

        public void SetFormula(Compound formula)
        {
            this.formula = formula;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
