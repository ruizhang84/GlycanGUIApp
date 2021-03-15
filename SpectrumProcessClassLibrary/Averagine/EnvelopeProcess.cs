using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrecursorIonClassLibrary.Averagine
{
    public class EnvelopeProcess
    {
        private double delta = 1.0033548378; // C13 - C12
        private double tol = 0.02; // 0.02 Dalton;
        private double range = 1; // 1 mz

        public void Init(double delta= 1.0033548378, double tol=0.01, double range=1)
        {
            this.delta = delta;
            this.tol = tol;
            this.range = range;
        }

        private List<IPeak> BinarySearchPeaks(List<IPeak> peaks, double mz)
        {
            List<IPeak> istotopics = new List<IPeak>();
            int start = 0, end = peaks.Count - 1;
            while (start <= end)
            {
                int mid = (end - start) / 2 + start;
                if (Math.Abs(mz - peaks[mid].GetMZ()) <= tol)
                {
                    for(int i = mid; i < peaks.Count; i++)
                    {
                        if (Math.Abs(mz - peaks[i].GetMZ()) <= tol)
                        {
                            istotopics.Add(peaks[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int i = mid-1; i >= 0; i--)
                    {
                        if (Math.Abs(mz - peaks[i].GetMZ()) <= tol)
                        {
                            istotopics.Add(peaks[i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return istotopics;
                }else if (mz > peaks[mid].GetMZ())
                {
                    start = mid + 1;
                }
                else
                {
                    end = mid - 1;
                }
            }
            return istotopics;
        }

        public SortedDictionary<int, List<IPeak>> Cluster(List<IPeak> peaks, double precursor, int charge)
        {
            //int: diff of isotope
            SortedDictionary<int, List<IPeak>> cluster = 
                new SortedDictionary<int, List<IPeak>>();
            List<IPeak> searchedPeaks = peaks.Where(
                p => p.GetMZ() < precursor + range && p.GetMZ() > precursor - range).ToList();

            double steps = delta * 1.0 / charge;
            int index = 0;
            while (steps * index < range) // search with 1 mz
            {
                double target = precursor + steps * index;
                List<IPeak> isotopics = BinarySearchPeaks(searchedPeaks, target);
                if (isotopics.Count > 0)
                    cluster[index] = isotopics;
                index++;
            }
            index = -1;
            while (steps * index > -range)
            {
                double target = precursor + steps * index;
                List<IPeak> isotopics = BinarySearchPeaks(searchedPeaks, target);
                if (isotopics.Count > 0)
                    cluster[index] = isotopics;
                index--;
            }
            
            return cluster ;
        }

    }
}
