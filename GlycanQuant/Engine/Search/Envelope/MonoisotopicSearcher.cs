using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Search;
using SpectrumData;

namespace GlycanQuant.Engine.Search.Envelope
{
    public class MonoisotopicSearcher
    {
        IResultFactory factory;
        public MonoisotopicSearcher(IResultFactory factory)
        {
            this.factory = factory;
        }

        public List<SortedDictionary<int, IPeak>> Combinator(SortedDictionary<int, List<IPeak>> cluster)
        {
            List<SortedDictionary<int, IPeak>> results = new List<SortedDictionary<int, IPeak>>();
            Queue<Tuple<int, SortedDictionary<int, IPeak>>> queue = 
                new Queue<Tuple<int, SortedDictionary<int, IPeak>>>();
            int start = cluster.Keys.Min();
            int end = cluster.Keys.Max();
            foreach(IPeak peak in cluster[start])
            {
                SortedDictionary<int, IPeak> dict = new SortedDictionary<int, IPeak>();
                dict[start] = peak;
                int next = start + 1;
                while (!cluster.ContainsKey(next) && next < end)
                {
                    next++;
                }
                queue.Enqueue(Tuple.Create(next, dict));
            }

            while(queue.Count > 0)
            {
                Tuple<int, SortedDictionary<int, IPeak>> tuple = queue.Dequeue();
                int curr = tuple.Item1;
                SortedDictionary<int, IPeak> dict = tuple.Item2;
                if (curr > end)
                {
                    results.Add(dict);
                    continue;
                }

                foreach (IPeak peak in cluster[curr])
                {
                    SortedDictionary<int, IPeak> newDict =
                        new SortedDictionary<int, IPeak>(dict);

                    newDict[curr] = peak;
                    int next = curr + 1;
                    while (!cluster.ContainsKey(next) && next < end)
                    {
                        next++;
                    }
                    queue.Enqueue(Tuple.Create(next, newDict));
                }
                
                
            }
            return results;
        }

        private double Score(List<double> alignedDistr, List<double> alignedIntensity)
        {
            // compute correlation
            double distrMean = alignedDistr.Average();
            double intensityMean = alignedIntensity.Average();

            double norminator = 0.0;
            for (int i = 0; i < alignedDistr.Count; i++)
            {
                norminator += (alignedDistr[i] - distrMean) * (alignedIntensity[i] - intensityMean);
            }
            double denominator1 = 0.0;
            for (int i = 0; i < alignedDistr.Count; i++)
            {
                denominator1 += (alignedDistr[i] - distrMean) * (alignedDistr[i] - distrMean);
            }
            double denominator2 = 0.0;
            for (int i = 0; i < alignedDistr.Count; i++)
            {
                denominator2 += (alignedIntensity[i] - intensityMean)
                    * (alignedIntensity[i] - intensityMean);
            }
            return norminator / Math.Sqrt(denominator1 * denominator2);
        }

        private void AlignData(List<double> distr, SortedDictionary<int, IPeak> isotopics,
            int distrIndex, int isotopicIndex,
            out List<double> alignedDistr, out List<IPeak> alignedPeaks)
        {
            alignedDistr = new List<double>();
            alignedPeaks = new List<IPeak>();

            int alignedDistrIndex = distrIndex;
            int alignedIsotopicIndex = isotopicIndex;
            int start = isotopics.Keys.Min();
            int end = isotopics.Keys.Max();

            // align the data
            while (alignedDistrIndex >= 0 && alignedIsotopicIndex >= start)
            {
                if (isotopics.ContainsKey(alignedIsotopicIndex))
                {
                    alignedDistr.Add(distr[alignedDistrIndex]);
                    alignedPeaks.Add(isotopics[alignedIsotopicIndex]);
                }
                alignedDistrIndex--;
                alignedIsotopicIndex--;
            }

            alignedDistr.Reverse();
            alignedPeaks.Reverse();

            alignedDistrIndex = distrIndex + 1;
            alignedIsotopicIndex = isotopicIndex + 1;

            while (alignedDistrIndex < distr.Count && alignedIsotopicIndex <= end)
            {
                if (isotopics.ContainsKey(alignedIsotopicIndex))
                {
                    alignedDistr.Add(distr[alignedDistrIndex]);
                    alignedPeaks.Add(isotopics[alignedIsotopicIndex]);
                }
                alignedDistrIndex++;
                alignedIsotopicIndex++;
            }
        }

        public double Fit(List<double> distr, SortedDictionary<int, IPeak> isotopics)
        {          
            // find the max distribution
            int maxDistrIndex = 0;
            double maxProb = 0;
            for (int i = 0; i < distr.Count; i++)
            {
                if (distr[i] > maxProb)
                {
                    maxProb = distr[i];
                    maxDistrIndex = i;
                }
            }

            // find the peaks intensity and align data
            List<double> alignedDistr;
            List<IPeak> alignedPeaks;
            AlignData(distr, isotopics, maxDistrIndex, 0,
                out alignedDistr, out alignedPeaks);

            // compute correlation
            return Score(alignedDistr, alignedPeaks.Select(p => p.GetIntensity()).ToList());       
        }

        public IResult Match(IGlycanPeak glycan, SortedDictionary<int, List<IPeak>> cluster)
        {
            List<SortedDictionary<int, IPeak>> clustered = Combinator(cluster);
            List<double> distr = glycan.GetDistrib();
            double maxScore = 0;
            List<IPeak> bestPeaks = new List<IPeak>();
            foreach (SortedDictionary<int, IPeak> sequence in clustered)
            {
                double score = Fit(distr, sequence);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPeaks = sequence.Select(s => s.Value).ToList();
                }
            }
            return factory.Produce(glycan, maxScore, bestPeaks);
        }

    }
}
