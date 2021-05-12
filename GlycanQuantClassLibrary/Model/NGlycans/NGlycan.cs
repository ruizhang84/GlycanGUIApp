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
        public const double kCarbon = 12.0;
        public const double kNitrogen = 14.003074;
        public const double kOxygen = 15.99491463;
        public const double kHydrogen = 1.007825;
        // methyl
        public const double kMethyl = kCarbon + kHydrogen * 3;
        // 15 + 31
        public const double kNonReduced = kMethyl * 2 + kOxygen;
        // 15 + 47
        public const double kReduced = kMethyl * 3 + kHydrogen + kOxygen;

        Dictionary<Monosaccharide, int> composition;
        Compound formula;
        string name = "";
        bool permethylated = true;
        bool reduced = true;

        public NGlycan(bool permethylated=true, int HexNAc=2, int Hex=3, 
            int Fuc=0, int NeuAc=0, int NeuGc = 0, bool reduced=true)
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
            this.reduced = reduced;
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
                    formulaComposition[elm] += tempCompose[elm] * composition[sugar];
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
            if(permethylated)
            {
                if (reduced)
                {
                    return formula.Mass + kReduced;
                }
                else
                {
                    return formula.Mass + kNonReduced;
                }
            }
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
