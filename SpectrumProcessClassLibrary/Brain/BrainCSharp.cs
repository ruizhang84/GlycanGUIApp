using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace PrecursorIonClassLibrary.Brain
{
    public class BrainCSharp
    {
        private double PsiFunc(Compound compound, int order)
        {
            Complex psi = 0;
            foreach(var item in compound.Composition)
            {
                int num = item.Value;
                Element element = item.Key;
                foreach(Complex root in element.Root())
                {
                    psi += num * Complex.Pow(root, -order);
                }
            }
            return psi.Real;
        }

        private double IteractionCoeff(List<double> coeff, List<double> polynom)
        {
            int j = coeff.Count;
            double qi = 0;
            for(int l = 0; l < j; l++)
            {
                qi += coeff[j - 1 - l] * polynom[l];
            }
            return -qi / j;
        }

        public List<double> Run(Compound compound, int order)
        {
            List<double> coeff = new List<double>();
            double q0 = 1.0;
            foreach(var item in compound.Composition)
            {
                int num = item.Value;
                Element element = item.Key;
                q0 *= Math.Pow(element.Abundance[element.Abundance.Keys.Min()], num);
            }
            coeff.Add(q0);


            double qi = q0;
            List<double> polynom = new List<double>();
            for(int i = 1; i < order; i++)
            {
                double psi = PsiFunc(compound, i);
                polynom.Add(psi);

                qi = IteractionCoeff(coeff, polynom);
                coeff.Add(qi);
            }
            return coeff;
        }

       

    }
}
