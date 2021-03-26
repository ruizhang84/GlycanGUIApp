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

            List<int> delta = new List<int>() { -2, -1, 1, 2 };
            List<double> topArea = new List<double>();
            for (int i = Math.Max(spectrumReader.GetFirstScan(), scan - 2);
                i < Math.Min(scan + 3, spectrumReader.GetLastScan()); i++)
            {
                if (spectrumReader.GetMSnOrder(i) != 1)
                    continue;
                ISpectrum spetrum = spectrumReader.GetSpectrum(i);
                List<IPeak> top = new List<IPeak>();

                int index = BinarySearch.Search(spetrum.GetPeaks(), mz, tol, by);
                if (index < 0)
                    continue;
                top.Add(spetrum.GetPeaks()[index]);

                foreach (int j in delta)
                {
                    index = BinarySearch.Search(spetrum.GetPeaks(), mz + 1.0 / charge * j, tol, by);
                    if (index < 0)
                        continue;
                    top.Add(spetrum.GetPeaks()[index]);
                }
                topArea.Add(top.OrderBy(p => p.GetIntensity()).
                    Take(3).Select(p => p.GetIntensity()).Sum());
            }

            return topArea.OrderBy(a => a).Take(3).Sum();
        }
    }
}
