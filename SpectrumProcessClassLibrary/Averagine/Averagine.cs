using PrecursorIonClassLibrary.Brain;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Averagine
{
    public enum AveragineType
    {
        Peptide,
        GlycoPeptide,
        Glycan,
        PermethylatedGlycan
    }

    public class Averagine
    {
        public AveragineType Type { get; set; }
        public Averagine(AveragineType type = AveragineType.GlycoPeptide)
        {
            Type = type;
        }

        public Compound Fit(double mass)
        {
            Dictionary<Element, int> compos = new Dictionary<Element, int>();

            // compute scale
            double total = 0;
            foreach(var item in Composition())
            {
                Element e = item.Key;
                double num = item.Value;
                total += e.Mass[e.Atomic] * num;
            }

            double scale = mass / total;

            // compute additional H
            total = 0;
            foreach (var item in Composition())
            {
                Element e = item.Key;
                int num = (int) Math.Floor(item.Value * scale);
                compos[e] = num;
                total += e.Mass[e.Atomic] * num;
            }
            int addit = (int) Math.Floor(mass - total);
            foreach(var item in compos)
            {
                if (item.Key.Atomic == 1)
                {
                    compos[item.Key] = item.Value + addit;
                    break;
                }
            }

            return new Compound(compos);
        }


        public Dictionary<Element, double> Composition()
        {
            return Composition(Type);
        }

        public Dictionary<Element, double> Composition(AveragineType type)
        {
            switch(type)
            {
                case AveragineType.Peptide:
                    return Peptide;
                case AveragineType.GlycoPeptide:
                    return Glycopeptide;
                case AveragineType.Glycan:
                    return Glycan;
                case AveragineType.PermethylatedGlycan:
                    return PermethylatedGlycan;
                default:
                    break;
            }
            return Peptide;
        }

        public Dictionary<Element, double> Peptide
            = new Dictionary<Element, double>()
            {
                {new C(), 4.9384 },
                {new H(), 7.7583 },
                {new N(), 1.3577 },
                {new O(), 1.4773 },
                {new S(), 0.0417 }
            };
        public Dictionary<Element, double> Glycopeptide
            = new Dictionary<Element, double>()
            {
                {new C(), 10.93 },
                {new H(), 15.75 },
                {new N(), 1.6577 },
                {new O(), 6.4773 },
                {new S(), 0.02054 }
            };
        public Dictionary<Element, double> Glycan
            = new Dictionary<Element, double>()
            {
                {new C(), 7.0 },
                {new H(), 11.8333 },
                {new N(), 0.5 },
                {new O(), 5.16666 }
            };
        public Dictionary<Element, double> PermethylatedGlycan
            = new Dictionary<Element, double>()
            {
                {new C(), 12.0 },
                {new H(), 21.8333 },
                {new N(), 0.5 },
                {new O(), 5.16666 }
            };

    }
}
