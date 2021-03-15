using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    public class Fourier : ICharger
    {
        protected readonly int maxCharge = 10;
        protected readonly double precison = 0.005;
        protected AirPLS airPLS = new AirPLS();

        public int Charge(List<IPeak> peaks, double lower, double upper)
        {
            // default to charge 2
            int charge = 2;
            if (peaks.Count < 2)
                return charge;
            
            // take points from user-selected area
            var selected = peaks.Where(p => p.GetMZ() >= lower && p.GetMZ() <= upper);
            List<double> X = selected.Select(p => p.GetMZ()).ToList();
            List<double> Y = selected.Select(p => p.GetIntensity()).ToList();

            // baseline corrects by airPLS
            double[,] yArray = new double[1, Y.Count];
            for(int i = 0; i < Y.Count; i++)
            {
                yArray[0, i] = Y[i];
            }
            double[,] z = airPLS.Correction(yArray);
            for(int i = 0; i < Y.Count; i++)
            {
                Y[i] = z[0, i];
            }

            // linearized space by cubic spline
            List<double> x = new List<double>();
            for(double i = X.Min(); i <= X.Max(); i += precison)
            {
                x.Add(i);
            }
            List<double> y = Interpolation.Spline(x, X, Y);

            // pads the data with zeros to the next power of 2
            int size = (int) Math.Pow(2.0, Math.Ceiling(Math.Log(x.Count, 2))) - x.Count;
            for(int i = 0; i < size; i++)
            {
                y.Add(0.0);
            }

            //FFT to charge map
            double[] magnitue = FFT.Transform(y.Select(p => new Complex(Math.Max(0, p), 0)).ToArray())
                .Select(p => p.Magnitude).ToArray();

            double best = 0;
            for(int i = 1; i <= Math.Min(maxCharge, magnitue.Count()-1); i++)
            {
                if (magnitue[i] > best)
                {
                    best = magnitue[i];
                    charge = i;
                }
            }

            return charge;
        }
    }
}
