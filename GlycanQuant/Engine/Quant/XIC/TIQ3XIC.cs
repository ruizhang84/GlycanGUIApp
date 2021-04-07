using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Search;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant.XIC
{
    public class TIQ3XIC : IXIC
    {
        ISpectrumReader spectrumReader;
        double tol;
        ToleranceBy by;

        public TIQ3XIC(ISpectrumReader spectrumReader, double tol, ToleranceBy by)
        {
            this.spectrumReader = spectrumReader;
            this.tol = tol;
            this.by = by;
        }
        public double Area(IResult glycan)
        {
            // search xic
            double mz = glycan.GetMZ();
            int scan = glycan.GetScan();
            int charge = glycan.GetCharge();

           // get neighbors
            List<ISpectrum> neighbors = new List<ISpectrum>();
            int nextScan = scan;
            while (nextScan > spectrumReader.GetFirstScan())
            {
                if (spectrumReader.GetMSnOrder(nextScan) == 1)
                {
                    ISpectrum spectrum = spectrumReader.GetSpectrum(nextScan);
                    int index = BinarySearch.Search(spectrum.GetPeaks(), mz, tol, by);
                    if (index < 0) break;

                    neighbors.Add(spectrum);
                }
                nextScan--;
            }
            nextScan = scan + 1;
            while (nextScan <= spectrumReader.GetLastScan())
            {
                if (spectrumReader.GetMSnOrder(nextScan) == 1)
                {
                    ISpectrum spectrum = spectrumReader.GetSpectrum(nextScan);
                    int index = BinarySearch.Search(spectrum.GetPeaks(), mz, tol, by);
                    if (index < 0) break;

                    neighbors.Add(spectrum);
                }
                nextScan++;
            }

            // each neighbor get top 3 peaks
            List<double> topArea = new List<double>();
            List<int> delta = new List<int>() { -2, -1, 0, 1, 2 };
            foreach (ISpectrum spetrum in neighbors)
            {
                List<IPeak> top = new List<IPeak>();

                foreach (int j in delta)
                {
                    int index = BinarySearch.BestSearch(spetrum.GetPeaks(), mz + 1.0 / charge * j, tol, by);
                    if (index < 0)
                        continue;
                    top.Add(spetrum.GetPeaks()[index]);
                }
                topArea.Add(top.OrderByDescending(p => p.GetIntensity()).
                    Take(3).Select(p => p.GetIntensity()).Sum());
            }

            return topArea.OrderByDescending(a => a).Take(3).Sum();
        }
    }
}
