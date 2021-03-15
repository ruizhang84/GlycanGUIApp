using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PrecursorIonClassLibrary.Charges
{
    public class PattersonFourierCombine : Patterson, ICharger
    {
        AirPLS airPLS = new AirPLS();
        protected readonly double precison = 0.005;

        public override int Charge(List<IPeak> peaks, double lower, double upper)
        {
            // default to charge 2
            int charge = 2;

            // patterson
            Dictionary<int, double> patterson = new Dictionary<int, double>();
            for (int i = 1; i <= maxCharge; i++)
            {
                double delta = 1.0 / i;
                double val = RountineFunc(delta, peaks, lower, upper);
                delta = 1.0 / (i - 1.0 / 3);
                val = Math.Max(val, RountineFunc(delta, peaks, lower, upper));
                delta = 1.0 / (i + 1.0 / 3);
                val = Math.Max(val, RountineFunc(delta, peaks, lower, upper));
                patterson[i] = val;
            }

            // Fourier
            Dictionary<int, double> fourier = new Dictionary<int, double>();

            // take points from user-selected area
            var selected = peaks.Where(p => p.GetMZ() >= lower && p.GetMZ() <= upper);
            if (selected.ToList().Count < 2)
            {
                double bestVal = 0;
                foreach(int i in patterson.Keys)
                {
                    if(bestVal < patterson[i])
                    {
                        bestVal = patterson[i];
                        charge = i;
                    }
                }
                return charge;
            }
            List<double> X = selected.Select(p => p.GetMZ()).ToList();
            List<double> Y = selected.Select(p => p.GetIntensity()).ToList();

            // baseline corrects
            double[,] yArray = new double[1, Y.Count];
            for (int i = 0; i < Y.Count; i++)
            {
                yArray[0, i] = Y[i];
            }
            double[,] z = airPLS.Correction(yArray);
            for (int i = 0; i < Y.Count; i++)
            {
                Y[i] = z[0, i];
            }

            // linearized space by cubic spline
            List<double> x = new List<double>();
            for (double i = X.Min(); i <= X.Max(); i += precison)
            {
                x.Add(i);
            }
            List<double> y = Interpolation.Spline(x, X, Y);

            // pads the data with zeros to the next power of 2
            int size = (int)Math.Pow(2.0, Math.Ceiling(Math.Log(x.Count, 2))) - x.Count;
            for (int i = 0; i < size; i++)
            {
                y.Add(0.0);
            }

            //FFT to charge map
            double[] magnitue = FFT.Transform(y.Select(p => new Complex(Math.Max(0, p), 0)).ToArray())
                .Select(p => p.Magnitude).ToArray();

            for (int i = 1; i <= Math.Min(maxCharge, magnitue.Count() - 1); i++)
            {
                fourier[i] = magnitue[i];
            }
             
            // combine
            double best = 0;
            foreach(int i in fourier.Keys)
            {
                if (patterson.ContainsKey(i))
                {
                    double value = fourier[i] * patterson[i];
                    if (value > best)
                    {
                        best = value;
                        charge = i;
                    }
                }
            }

            return charge;
        }

    }
}
