using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrecursorIonClassLibrary.Averagine;
using SpectrumData;

namespace PrecursorIonClassLibrary.Averagine
{
    public class LocalMaximaPicking
    {
        private double ppm = 5;

        public LocalMaximaPicking(double ppm=5)
        {
            this.ppm = ppm;
        }

        public void SetTolerance(double ppm)
        {
            this.ppm = ppm; 
        }

        private List<int> WindowMaxamIndexes(List<IPeak> peaks)
        {
            HashSet<int> indexes = new HashSet<int>();
            LinkedList<int> indexer = new LinkedList<int>();

            // init
            int idx = 0, maxIdx = 0;
            for (; idx < peaks.Count; idx++)
            {
                if (Calculator.To.ComputePPM(peaks[0].GetMZ(), peaks[idx].GetMZ()) >= ppm)
                    break;

                while (indexer.Count > 0 
                    && peaks[indexer.Last.Value].GetIntensity() < peaks[idx].GetIntensity())
                {
                    indexer.RemoveLast();
                }
                indexer.AddLast(idx);
                if (peaks[idx].GetIntensity() > peaks[maxIdx].GetIntensity())
                    maxIdx = idx;
            }
            indexes.Add(maxIdx);

            //sliding window
            for (; idx < peaks.Count; idx++)
            {
                while (indexer.Count > 0 
                    && Calculator.To.ComputePPM(peaks[indexer.First.Value].GetMZ(), peaks[idx].GetMZ()) > ppm)
                {
                    indexer.RemoveFirst();
                }

                while (indexer.Count > 0 
                    && peaks[indexer.Last.Value].GetIntensity() <= peaks[idx].GetIntensity())
                {
                    indexer.RemoveLast();
                }
                indexer.AddLast(idx);
                indexes.Add(indexer.First.Value);
            }
            if (indexes.Count == 0)
                return indexes.ToList();
            return indexes.OrderBy(i => peaks[i].GetMZ()).ToList();
        }

        private List<IPeak> LocalMaxam(List<IPeak> peaks)
        {
            List<IPeak> select = new List<IPeak>();
            List<int> maxIndexes = WindowMaxamIndexes(peaks);

            foreach (int index in maxIndexes)
            {
                if (index > 0 && 
                    peaks[index - 1].GetIntensity() > peaks[index].GetIntensity())
                    continue;
                else if (index < peaks.Count - 1 
                    && peaks[index + 1].GetIntensity() > peaks[index].GetIntensity())
                    continue;
                select.Add(peaks[index]);
            }
            return select;
        }

        public List<IPeak> Process(List<IPeak> peaks)
        {
            return LocalMaxam(peaks);

        }
    }
}
