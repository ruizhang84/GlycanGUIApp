using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GlycanQuant.Engine.Util
{
    public abstract class Element
    {
        public int Atomic { get; set; }
        public string Name { get; set; } // C, H, O, N, S, etc.
        public List<int> Isotope { get; set; } //e.g. C12 C13
        public SortedDictionary<int, double> Abundance { get; set; }
        public SortedDictionary<int, double> Mass { get; set; }
        public double MonoMass { get; set; }
        public abstract List<Complex> Root();
    }

    public class H : Element
    {
        public H()
        {
            Atomic = 1;
            Name = "Hydrogen";
            Isotope = new List<int>() { 1, 2 };
            Abundance = new SortedDictionary<int, double>() { { 1, 0.999885 }, { 2, 0.000115 } };
            Mass = new SortedDictionary<int, double>() { { 1, 1.00782503207 }, { 2, 2.0141017778} };
            MonoMass = 1.00782503207;
        }

        public override List<Complex> Root()
        {
            return new List<Complex> { -Abundance[1] / Abundance[2] };
        }
    }

    public class O : Element
    {
        public O()
        {
            Atomic = 16;
            Name = "Oxygen";
            Isotope = new List<int>() { 16, 17, 18 };
            Abundance = new SortedDictionary<int, double>() { { 16, 0.99757 }, { 17, 0.00038 }, { 18, 0.00205 } };
            Mass = new SortedDictionary<int, double>() { { 16, 15.99491461956 }, { 17, 16.9991317 }, { 18, 17.999161 } };
            MonoMass = 15.99491461956;
        }

        public override List<Complex> Root()
        {
            return EquationSolver.Root(Abundance[18], Abundance[17], Abundance[16]);
        }
    }


    public class C : Element
    {
        public C()
        {
            Atomic = 12;
            Name = "Carbon";
            Isotope = new List<int>() { 12, 13 };
            Abundance = new SortedDictionary<int, double>() { { 12, 0.9893 }, { 13, 0.0107 } };
            Mass = new SortedDictionary<int, double>() { { 12, 12.0 }, { 13, 13.0033548378 } };
            MonoMass = 12.0;
        }

        public override List<Complex> Root()
        {
            return new List<Complex> { -Abundance[12] / Abundance[13] };
        }
    }


    public class N : Element
    {
        public N()
        {
            Atomic = 14;
            Name = "Nitrogen";
            Isotope = new List<int>() { 14, 15 };
            Abundance = new SortedDictionary<int, double>() { { 14, 0.99636 }, { 15, 0.00364 } };
            Mass = new SortedDictionary<int, double>() { { 14, 14.0030740048 }, { 15, 15.0001088982 } };
            MonoMass = 14.0030740048;
        }

        public override List<Complex> Root()
        {
            return new List<Complex> { -Abundance[14] / Abundance[15] };
        }
    }

    public class S : Element
    {
        public S()
        {
            Atomic = 32;
            Name = "Sulfur";
            Isotope = new List<int>() { 32, 33, 34, 36 };
            Abundance = new SortedDictionary<int, double>()
                { { 32, 0.9499 }, { 33, 0.0075 }, { 34, 0.0425 }, { 36, 0.0001 } };
            Mass = new SortedDictionary<int, double>()
                { { 32, 31.972071 }, { 33, 32.97145876 }, { 34, 33.9678669 }, { 36, 35.96708076 } };
            MonoMass = 31.972071;
        }
        public override List<Complex> Root()
        {
            return EquationSolver.Root(Abundance[36], 0.0, Abundance[34], Abundance[33], Abundance[32]);
        }
    }


    public class Compound
    {
        public List<Element> Elements { get; set; }
        public Dictionary<Element, int> Composition { get; set; }
        public double Mass { get; set; }

        public Compound(Dictionary<Element, int> compos)
        {
            Elements = compos.Select(c => c.Key).OrderBy(c => c.Atomic).ToList();
            Composition = compos;
            Mass = 0;
            foreach(Element elem in Composition.Keys)
            {
                Mass += elem.MonoMass * Composition[elem];
            }
        }
    }
}
