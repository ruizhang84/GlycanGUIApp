using GlycanQuant.Model.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.NGlycans
{

    public class NMonosaccharideCreator
    {
        protected static readonly Lazy<NMonosaccharideCreator>
            lazy = new Lazy<NMonosaccharideCreator>(() => new NMonosaccharideCreator());
        public static NMonosaccharideCreator Get { get { return lazy.Value; } }

        protected NMonosaccharideCreator(){}

        public Compound Compounds(Monosaccharide sugar)
        {

            Dictionary<Element, int> composition = Compositions(sugar);
            return new Compound(composition);
        }

        public Dictionary<Element, int> Compositions(Monosaccharide sugar)
        {
            // GlcNAc C8H15NO6, Man/Gal C6H12O6, Fuc C6H12O5,  NeuAc C11H19NO9, NeuGc C11H19NO10
            switch (sugar)
            {
                case Monosaccharide.HexNAc:
                    return  new Dictionary<Element, int>()
                    { { new C(), 8}, { new H(), 15}, { new N(), 1}, { new O(), 6} };
                case Monosaccharide.Hex:
                    return new Dictionary<Element, int>()
                    { {new C(), 6}, {new H(), 12}, {new O(), 6} };
                case Monosaccharide.Fuc:
                    return new Dictionary<Element, int>()
                    { {new C(), 6}, {new H(), 12}, {new O(), 5} };
                case Monosaccharide.NeuAc:
                    return new Dictionary<Element, int>()
                    { {new C(), 11}, {new H(), 19}, {new N(), 1}, {new O(), 9} };
                case Monosaccharide.NeuGc:
                    return new Dictionary<Element, int>()
                    { { new C(), 11}, { new H(), 19}, { new N(), 1}, { new O(), 10} };
            }

            return new Dictionary<Element, int>();
        }

        // composition in a glycan
        public Dictionary<Element, int> SubCompositions(Monosaccharide sugar, bool permethylated)
        {
            // GlcNAc C8H15NO6, Man/Gal C6H12O6, Fuc C6H12O5,  NeuAc C11H19NO9, NeuGc C11H19NO10
            if (permethylated)
            {
                switch (sugar)
                {
                    case Monosaccharide.HexNAc:
                        return new Dictionary<Element, int>()
                    { { new C(), 11}, { new H(), 19}, { new N(), 1}, { new O(), 5} };
                    case Monosaccharide.Hex:
                        return new Dictionary<Element, int>()
                    { {new C(), 9}, {new H(), 16}, {new O(), 5} };
                    case Monosaccharide.Fuc:
                        return new Dictionary<Element, int>()
                    { {new C(), 8}, {new H(), 14}, {new O(), 4} };
                    case Monosaccharide.NeuAc:
                        return new Dictionary<Element, int>()
                    { {new C(), 16}, {new H(), 27}, {new N(), 1}, {new O(), 8} };
                    case Monosaccharide.NeuGc:
                        return new Dictionary<Element, int>()
                    { { new C(), 17}, { new H(), 29}, { new N(), 1}, { new O(), 9} };
                }
            }
            else
            {
                switch (sugar)
                {
                    case Monosaccharide.HexNAc:
                        return new Dictionary<Element, int>()
                    { { new C(), 8}, { new H(), 13}, { new N(), 1}, { new O(), 5} };
                    case Monosaccharide.Hex:
                        return new Dictionary<Element, int>()
                    { {new C(), 6}, {new H(), 10}, {new O(), 5} };
                    case Monosaccharide.Fuc:
                        return new Dictionary<Element, int>()
                    { {new C(), 6}, {new H(), 10}, {new O(), 4} };
                    case Monosaccharide.NeuAc:
                        return new Dictionary<Element, int>()
                    { {new C(), 11}, {new H(), 17}, {new N(), 1}, {new O(), 8} };
                    case Monosaccharide.NeuGc:
                        return new Dictionary<Element, int>()
                    { { new C(), 11}, { new H(), 17}, { new N(), 1}, { new O(), 9} };
                }
            }

            

            return new Dictionary<Element, int>();
        }

    }
}
