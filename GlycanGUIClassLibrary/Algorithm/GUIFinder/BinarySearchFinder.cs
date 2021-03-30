using GlycanGUIClassLibrary.Util;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUIClassLibrary.Algorithm.GUIFinder
{
    public class BinarySearchFinder : IGUIFinder
    {
        public BinarySearchFinder(double ppm)
        {
            tol = ppm;
        }

        public static readonly double[] Glucose
            = { 470.2727, 674.3725, 878.4723, 1082.572, 1286.6718, 1490.7716,
            1694.8714, 1898.9711, 2103.0709, 2307.1707, 2511.2704};

        protected double tol = 10.0;

        public double GetTolerance()
        {
            return tol;
        }

        public void SetTolerance(double ppm)
        {
            tol = ppm;
        }

        private int Compare(IPeak peak, double mz)
        {
            double ppm = Calculator.To.ComputePPM(mz, peak.GetMZ());
            if (ppm < tol) return 0;
            if (peak.GetMZ() > mz) return 1;
            return -1;
        }

        private int ExtendSearchPoints(List<IPeak> peaks, double mz, int index)
        {
            int best = index;
            int left = index;
            while (left > 0 && Compare(peaks[--left], mz) == 0)
            {
                if (peaks[best].GetIntensity() < peaks[left].GetIntensity())
                {
                    best = left;
                }
            }
            int right = index;
            while (right < peaks.Count - 1 && Compare(peaks[++right], mz) == 0)
            {
                if (peaks[best].GetIntensity() < peaks[right].GetIntensity())
                {
                    best = right;
                }
            }
            return best;
        }

        private int BinarySearchPoints(List<IPeak> peaks, double mz)
        {
            int start = 0;
            int end = peaks.Count - 1;

            while (start <= end)
            {
                int mid = end + (start - end) / 2;
                int cmp = Compare(peaks[mid], mz);
                if (cmp == 0)
                {
                    return ExtendSearchPoints(peaks, mz, mid);
                }
                else if (cmp > 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return -1;
        }

        public List<GUI> FindGlucoseUnits(ISpectrum spectrum)
        {
            List<GUI> units = new List<GUI>();
            List<IPeak> peaks = spectrum.GetPeaks();
            int scan = spectrum.GetScanNum();

            for (int i = 0; i < Glucose.Length; i++)
            {
                double mass = Glucose[i];
                double bestIntensity = 0;
                IPeak bestPeak = null;

                List<double> mzCandidates = Calculator.To.ComputeMZ(mass);
                foreach (double mz in mzCandidates)
                {
                    int idx = BinarySearchPoints(peaks, mz);
                    if (idx > 0 && peaks[idx].GetIntensity() > bestIntensity)
                    {
                        bestIntensity = peaks[idx].GetIntensity();
                        bestPeak = peaks[idx];
                    }
                }

                if (bestPeak is null)
                    continue;
                units.Add(new GUI(i + 2, scan, bestPeak));
            }

            return units;
        }
}
}
