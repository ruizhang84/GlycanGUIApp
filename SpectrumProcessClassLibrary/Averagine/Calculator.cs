using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Averagine
{
    public class Calculator
    {
        protected static readonly Lazy<Calculator>
            lazy = new Lazy<Calculator>(() => new Calculator());
        public static Calculator To { get { return lazy.Value; } }
        protected Calculator()
        {
            ions = new List<double> { 1.0078, 14.00307 + 1.0078 * 4, 22.98977 };
        }

        public void SetChargeIons(List<double> ionMass)
        {
            ions = ionMass;
        }

        protected List<double> ions;
        public static double proton = 1.0078;
        public static double ammonium = 14.00307 + 1.0078 * 4;
        public static double potassium = 22.98977;

        public double ComputePPM(double expected, double observed)
        {
            return Math.Abs(expected - observed) / expected * 1000000.0;
        }

        public double ComputeMass(double mz, double ion, int charge)
        {
            return (mz - ion) * charge;
        }

        public double ComputeMZ(double mass, double extra, int charge)
        {
            return (mass + extra) / charge;
        }

        private void ReCurComputeMZ(double mass, int idx,
            int charge, int maxCharge, ref List<double> ans)
        {
            if (charge >= maxCharge) return;
            for (int i = idx; i < ions.Count; i++)
            {
                double mz = ComputeMZ(mass, ions[i], charge + 1);
                ans.Add(mz);

                ReCurComputeMZ(mass + ions[i], i, charge + 1, maxCharge, ref ans);
            }
        }

        public List<double> ComputeMZ(double mass, int maxCharge = 3)
        {
            List<double> mzList = new List<double>();
            ReCurComputeMZ(mass, 0, 0, maxCharge, ref mzList);
            return mzList;
        }

    }
}
