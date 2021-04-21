using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycanQuant.Spectrum.Process.Refinement
{
    public class SNRFilter : IProcess
    {
        public IProcess peakPicking;
        double ratio;

        public SNRFilter(IProcess peakPicking, double ratio = 3)
        {
            this.peakPicking = peakPicking;
            this.ratio = ratio;
        }


        public ISpectrum Process(ISpectrum spectrum)
        {
            ISpectrum filtered = peakPicking.Process(spectrum);
            List<IPeak> peaks = new List<IPeak>();
            double minIntensity = filtered.GetPeaks().Min(p => p.GetIntensity());
            filtered.SetPeaks(
                peaks.Where(p => p.GetIntensity() > ratio * minIntensity).ToList());
            return filtered;
        }
    }
}
