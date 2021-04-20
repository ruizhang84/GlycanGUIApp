using SpectrumData;
using SpectrumData.Spectrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Spectrum.Process.PeakPicking
{
    public class LcalNeighborPickingForDrawing : LocalNeighborPicking
    {
        protected override List<IPeak> InsertPeaks(ISpectrum spectrum)
        {
            List<IPeak> peaks = new List<IPeak>();
            double mz = spectrum.GetPeaks().First().GetMZ();
            peaks.Add(new GeneralPeak(mz - precision, 0));
            foreach (IPeak peak in spectrum.GetPeaks())
            {
                if (peak.GetMZ() - mz > precision)
                {
                    double middle = (mz + peak.GetMZ()) / 2;
                    peaks.Add(new GeneralPeak(middle, 0));
                    while (middle + precision < peak.GetMZ())
                    {
                        middle += precision;
                        peaks.Add(new GeneralPeak(middle, 0));
                    }
                }
                peaks.Add(peak);
                mz = peak.GetMZ();
            }
            peaks.Add(new GeneralPeak(mz + precision, 0));
            return peaks;
        }
    }
}
