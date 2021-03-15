using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpectrumData;
using SpectrumData.Spectrum;

namespace GlycanQuant.Spectrum.Process.Refinement
{
    public class WeightedAveraging : IProcess
    {
        public IProcess peakPicking;
        protected double percent = 0.3;
        readonly double precision = 0.1;
        readonly int mzRound = 5;
        readonly int peakRound = 3;

        public WeightedAveraging(IProcess peakPicking, double percent = 0.3)
        {
            this.peakPicking = peakPicking;
            this.percent = percent;
        }

        List<IPeak> NeighborPeaks(IPeak target, List<IPeak> peaks)
        {
            List<IPeak> neighbors = new List<IPeak>();
            int index = peaks.BinarySearch(target);
            int curr = index;
            while (curr >= 0)
            {
                if (peaks[curr].GetIntensity() < target.GetIntensity() * percent)
                    break;
                else if (peaks[curr].GetMZ() + precision < target.GetMZ())
                    break;
                neighbors.Add(peaks[curr]);

                if (curr > 0 && peaks[curr - 1].GetIntensity() > peaks[curr].GetIntensity())
                    break;
                curr--;
            }
            curr = index + 1;
            while (curr < peaks.Count)
            {
                if (peaks[curr].GetIntensity() > peaks[curr - 1].GetIntensity())
                    break;
                else if (peaks[curr].GetIntensity() < target.GetIntensity() * percent)
                    break;
                else if (peaks[curr].GetMZ() - precision > target.GetMZ())
                    break;
                neighbors.Add(peaks[curr]);
                curr++;
            }
            return neighbors;
        }

        IPeak Average(IPeak target, List<IPeak> peaks)
        {
            double weighted = peaks.Select(p => p.GetMZ() * p.GetIntensity()).Sum();
            double intensity_sums = peaks.Select(p => p.GetIntensity()).Sum();
            return new GeneralPeak(
                Math.Round(weighted / intensity_sums, mzRound), 
                Math.Round(target.GetIntensity(), peakRound));
        }


        public ISpectrum Process(ISpectrum spectrum)
        {
            ISpectrum centroid = peakPicking.Process(spectrum);
            List<IPeak> peaks = new List<IPeak>();
            foreach (IPeak cPeak in centroid.GetPeaks())
            {
                List<IPeak> neighbors = NeighborPeaks(cPeak, spectrum.GetPeaks());
                peaks.Add(Average(cPeak, neighbors));
            }
            centroid.SetPeaks(peaks);
            return centroid;
        }
    }
}
