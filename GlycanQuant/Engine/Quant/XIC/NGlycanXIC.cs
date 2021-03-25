using GlycanQuant.Engine.Search;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant.XIC
{
    public class NGlycanXIC : IXIC
    {
        IAreaCalculator calculator;
        ISpectrumReader spectrumReader;
        ISpectrumSearch spectrumSearcher;

        public NGlycanXIC(IAreaCalculator calculator, 
            ISpectrumReader spectrumReader, ISpectrumSearch spectrumSearcher)
        {
            this.calculator = calculator;
            this.spectrumReader = spectrumReader;
            this.spectrumSearcher = spectrumSearcher;
        }

        public List<IResult> Find(IResult glycan)
        {
            List<IResult> results = new List<IResult>();

            int scan = glycan.GetScan();
            double mz = glycan.GetMZ();
            int charge = glycan.GetCharge();

            for (int i = scan-1; i > 0; i--)
            {
                if (spectrumReader.GetMSnOrder(i) != 1)
                    continue;
                ISpectrum spectrum = spectrumReader.GetSpectrum(i);
                IResult temp = spectrumSearcher.Search(spectrum, glycan.Glycan(), mz, charge);
                if (temp == null)
                    break;
                results.Add(temp);
            }

            results.Reverse();
            results.Add(glycan);

            for (int i = scan + 1; i < spectrumReader.GetLastScan(); i++)
            {
                if (spectrumReader.GetMSnOrder(i) != 1)
                    continue;
                ISpectrum spectrum = spectrumReader.GetSpectrum(i);
                IResult temp = spectrumSearcher.Search(spectrum, glycan.Glycan(), mz, charge);
                if (temp == null)
                    break;
                results.Add(temp);
            }
            return results;
        }

        private double  PeakArea(List<IPeak> peaks)
        {
            List<double> X = peaks.Select(p => p.GetMZ()).ToList();
            List<double> Y = peaks.Select(p => p.GetIntensity()).ToList();
            return calculator.Area(X, Y);
        }

        public double Area(IResult glycan) 
        {
            List<IResult> results = Find(glycan);
            List<double> rt = results.Select(p => spectrumReader.GetRetentionTime(p.GetScan())).ToList();
            List<double> Y = results.Select(p => PeakArea(p.Matches())).ToList();
            return calculator.Area(rt, Y);
        }
    }
}
