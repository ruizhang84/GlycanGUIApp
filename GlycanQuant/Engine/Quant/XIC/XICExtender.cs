using GlycanQuant.Engine.Search;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant.XIC
{
    public class XICExtender
    {
        ISpectrumReader spectrumReader;
        ISpectrumSearch spectrumSearcher;

        public XICExtender(ISpectrumReader spectrumReader, ISpectrumSearch spectrumSearcher)
        {
            this.spectrumReader = spectrumReader;
            this.spectrumSearcher = spectrumSearcher;
        }

        public List<IResult> Find(IResult glycan)
        {
            List<IResult> results = new List<IResult>();

            int scan = glycan.GetScan();
            double mz = glycan.GetMZ();
            int charge = glycan.GetCharge();

            for (int i = scan - 1; i > 0; i--)
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
    }
}
