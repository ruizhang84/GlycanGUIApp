using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrecursorIonClassLibrary.Charges
{
    // patterson routine
    public class Patterson : ICharger
    {
        protected readonly int maxCharge = 10;

        protected double GetIntensity(double target, double[] mz, double[] f)
        {
            if (target < mz[0] || target > mz[mz.Length -1])
                return 0;
            int start = 0, end = mz.Length - 1;
            while (start+1 < end)
            {
                int mid = (end - start) / 2 + start;
                if (mz[mid] == target)
                {
                    return f[mid];
                }
                else if (mz[mid] < target)
                {
                    start = mid;
                }
                else
                {
                    end = mid;
                }
            }
            return Interpolation.Linear(target, mz[start], f[start], mz[end], f[end]);
        }

        protected double RountineFunc(double delta, 
            List<IPeak> peaks, double lower, double upper, double precise = 0.005)
        {
            double sum = 0.0;
            var selected = peaks.Where(p => p.GetMZ() >= lower && p.GetMZ() <= upper);
            double[] mz = selected.Select(p => p.GetMZ()).ToArray();
            double[] f = selected.Select(p => p.GetIntensity()).ToArray();

            // no peaks
            if (mz.Length == 0)
                return sum;

            for(double m = lower; m <= upper; m += precise)
            {
                double left = m - delta / 2;
                double right = m + delta / 2;

                sum += GetIntensity(left, mz, f) * GetIntensity(right, mz, f);
            }
            return sum;
        }

        public virtual int Charge(List<IPeak> peaks, double lower, double upper)
        {
            // default to charge 2
            int charge = 2;
            double maxVal = 0;

            for(int i = 1; i <= maxCharge; i++)
            {
                double delta = 1.0 / i;
                double val = RountineFunc(delta, peaks, lower, upper);
                if (val > maxVal)
                {
                    maxVal = val;
                    charge = i;
                }

                delta = 1.0 / (i - 1.0 / 3);
                val = RountineFunc(delta, peaks, lower, upper);
                if (val > maxVal)
                {
                    maxVal = val;
                    charge = i;
                }

                delta = 1.0 / (i + 1.0 / 3);
                val = RountineFunc(delta, peaks, lower, upper);
                if (val > maxVal)
                {
                    maxVal = val;
                    charge = i;
                }
            }
            return charge;
        }


    }
}
