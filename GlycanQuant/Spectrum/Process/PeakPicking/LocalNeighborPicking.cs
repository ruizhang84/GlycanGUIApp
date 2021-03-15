using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpectrumData;
using SpectrumData.Spectrum;

namespace GlycanQuant.Spectrum.Process.PeakPicking
{
    public class LocalNeighborPicking : IProcess
    {
        protected double precision = 0.1;

        public ISpectrum Process(ISpectrum spectrum)
        {
            if (spectrum.GetPeaks().Count == 0)
                return spectrum;
            // insert pseudo peaks for large gap
            List<IPeak> peaks = new List<IPeak>();
            double mz = spectrum.GetPeaks().First().GetMZ();
            peaks.Add(new GeneralPeak(mz - precision, 0));
            foreach(IPeak peak in spectrum.GetPeaks())
            {
                if (peak.GetMZ() - mz > precision)
                {
                    double middle = (mz + peak.GetMZ()) / 2;
                    peaks.Add(new GeneralPeak(middle, 0));
                }
                peaks.Add(peak);
                mz = peak.GetMZ();
            }
            peaks.Add(new GeneralPeak(mz + precision, 0));

            List<IPeak> processedPeaks = new List<IPeak>();
           
            int index = 1;
            int end = peaks.Count - 1;
            int head = index + 1;
            while (index < end)
            {
                if (peaks[index - 1].GetIntensity() < peaks[index].GetIntensity())
                {
                    head = index + 1;
                }

                while (head < end
                    && peaks[head].GetIntensity() == peaks[index].GetIntensity())
                {
                    head++;
                }

                if (peaks[head].GetIntensity() < peaks[index].GetIntensity())
                {
                    processedPeaks.Add(peaks[index]);
                    index = head;
                }
                index++;

            }

            ISpectrum newSpectrum = spectrum.Clone();
            newSpectrum.SetPeaks(processedPeaks);
            return newSpectrum;
        }
    }
}
