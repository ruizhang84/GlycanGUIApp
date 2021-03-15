using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumData.Spectrum
{
    public class GeneralPeak : IPeak
    {
        protected double mz;
        protected double intensity;
        public GeneralPeak(double mz, double intensity)
        {
            this.mz = mz;
            this.intensity = intensity;
        }

        public double GetMZ()
        {
            return mz;
        }

        public double GetIntensity()
        {
            return intensity;
        }

        public void SetMZ(double mz)
        {
            this.mz = mz;
        }

        public void SetIntensity(double intensity)
        {
            this.intensity = intensity;
        }

        public int CompareTo(IPeak other)
        {
            return GetMZ().CompareTo(other.GetMZ());
        }
    }
}
