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
    public class XICPeak
    {
        public XICPeak(double rt, double intensity)
        {
            RTime = rt;
            Intensity = intensity;
        }
        public double RTime { get; set; }
        public double Intensity { get; set; }
    }

    public class PeakXIC : IXIC
    {
        IAreaCalculator calculator;
        ISpectrumReader spectrumReader;
        double tol;
        ToleranceBy by;
        double mzRange = 1.0;
        double rtTolerance = 1.0;
        double ratio = 0.5;


        public PeakXIC(IAreaCalculator calculator,
            ISpectrumReader spectrumReader, double tol, ToleranceBy by)
        {
            this.calculator = calculator;
            this.spectrumReader = spectrumReader;
            this.tol = tol;
            this.by = by;
        }
        public double Area(IResult glycan)
        {
            List<XICPeak> peaks = Find(glycan);

            // local max
            int maxIndex = 0;
            double maxValue = 0;
            for(int i = 0; i < peaks.Count; i++)
            {
                if (peaks[i].Intensity > maxValue)
                {
                    maxValue = peaks[i].Intensity;
                    maxIndex = i;
                }
            }


            double cutoff = ratio * maxValue;
            // left and right bound
            int left = maxIndex;
            for (int i = maxIndex; i >= 0; i--)
            {
                if (peaks[i].Intensity > cutoff)
                    left = i;
            }
            int right = maxIndex;
            for(int i = maxIndex; i < peaks.Count; i++)
            {
                if (peaks[i].Intensity > cutoff)
                    right = i;
            }
            while (left > 0)
            {
                if (peaks[left].Intensity > peaks[left - 1].Intensity)
                    left--;
            }
            while (right < peaks.Count - 1)
            {
                if (peaks[right].Intensity > peaks[right + 1].Intensity)
                    right++;
            }

            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            for(int i = left; i <= right; i++)
            {
                X.Add(peaks[i].RTime);
                Y.Add(peaks[i].Intensity);
            }

            return calculator.Area(X, Y);
        }

        private bool UpdateIntensity(int index, List<IPeak> peaks,
            double mz, ref double intensity)
        {
            IPeak peak = peaks[j];
            if (Math.Abs(mz - peak.GetMZ()) > mzRange)
                return false;
            intensity += peak.GetIntensity();

            return true;
        }

        private bool UpdateXIC(double mz, int scan, double rt, 
            ref List<XICPeak> results)
        {
            double rtTemp = spectrumReader.GetRetentionTime(scan);
            if (Math.Abs(rt - rtTemp) > rtTolerance)
                return false;

            List<IPeak> peaks = spectrumReader.GetSpectrum(scan).GetPeaks();
            double intensity = 0;

            int index = BinarySearch.Search(peaks, mz, tol, by);
            if (index < 0)
                return false;

            for (int j = index; j > 0; j--)
            {
                bool updated = UpdateIntensity(j, peaks, mz, ref intensity);
                if (!updated) break;
            }

            for (int j = index + 1; j < peaks.Count; j++)
            {
                bool updated = UpdateIntensity(j, peaks, mz, ref intensity);
                if (!updated) break;
            }

            results.Add(new XICPeak(rtTemp, intensity));
            return false;
        }

        public List<XICPeak> Find(IResult glycan)
        {
            double mz = glycan.GetMZ();
            int scan = glycan.GetScan();
            double rt = spectrumReader.GetRetentionTime(scan);

            List<XICPeak> results = new List<XICPeak>();
            for (int i = scan; i > 0; i--)
            {
                bool updated = UpdateXIC(mz, i, rt, ref results);
                if (!updated) break;
            }

            results.Reverse();

            for (int i = scan + 1; i < spectrumReader.GetLastScan(); i++)
            {
                bool updated = UpdateXIC(mz, i, rt, ref results);
                if (!updated) break;
            }
            return results;
        }
    }
}
