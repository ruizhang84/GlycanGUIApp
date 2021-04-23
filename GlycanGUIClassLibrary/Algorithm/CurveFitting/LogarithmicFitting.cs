using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycanGUI.Algorithm.CurveFitting
{
    public class LogarithmicFitting : ICurveFitting
    {
        PolynomialFitting polynomial = new PolynomialFitting(1);

        public void Fit(List<double> rentention, List<double> guis)
        {
            List<double> X = rentention.Select(r => Math.Log(r)).ToList();
            polynomial.Fit(X, guis);
        }

        public double GlucoseUnit(double rentention)
        {
            return polynomial.GlucoseUnit(Math.Log(rentention));
        }

        public double[] Parameter()
        {
            return polynomial.Parameter();
        }
    }
}
