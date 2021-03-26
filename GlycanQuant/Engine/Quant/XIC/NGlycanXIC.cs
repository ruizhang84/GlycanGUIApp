﻿using GlycanQuant.Engine.Search;
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

        public List<XICPeak> Find(IResult glycan)
        {
            List<XICPeak> results = new List<XICPeak>();

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
                results.Add(new XICPeak(spectrumReader.GetRetentionTime(i),
                    temp.GetMZ()));
            }

            results.Reverse();
            results.Add(new XICPeak(spectrumReader.GetRetentionTime(scan), mz));

            for (int i = scan + 1; i < spectrumReader.GetLastScan(); i++)
            {
                if (spectrumReader.GetMSnOrder(i) != 1)
                    continue;
                ISpectrum spectrum = spectrumReader.GetSpectrum(i);
                IResult temp = spectrumSearcher.Search(spectrum, glycan.Glycan(), mz, charge);
                if (temp == null)
                    break;
                results.Add(new XICPeak(spectrumReader.GetRetentionTime(i),
                    temp.GetMZ()));
            }
            return results;
        }

        public double Area(IResult glycan) 
        {
            List<XICPeak> results = Find(glycan);
            List<double> rt = results.Select(p => p.RTime).ToList();
            List<double> Y = results.Select(p => p.Intensity).ToList();
            return calculator.Area(rt, Y);
        }
    }
}
