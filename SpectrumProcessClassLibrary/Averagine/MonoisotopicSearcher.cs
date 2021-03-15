using PrecursorIonClassLibrary.Brain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpectrumData;

namespace PrecursorIonClassLibrary.Averagine
{
    public class MonoisotopicScore
    {
        private double score;
        private double mz;
        private List<IPeak> monoisotopics;

        public MonoisotopicScore(double mz, double score, List<IPeak> monoisotopics)
        {
            this.score = score;
            this.mz = mz;
            this.monoisotopics = monoisotopics;
        }
        public double GetScore() { return score; }
        public double GetMZ() { return mz; }
        public List<IPeak> GetPeaks() { return monoisotopics; }

    }

    public class MonoisotopicSearcher
    {
        private Averagine averagine;
        private BrainCSharp brain;
        private double delta = 1.0033548378; // C13 - C12
        private int numCandid = 3;  // number of shift
        private int numDistrib = 10; // number of isotopic distribution to consider

        public MonoisotopicSearcher(Averagine averagine, BrainCSharp brain)
        {
            this.averagine = averagine;
            this.brain = brain;
        }

        public void Init(double delta, int numCandid, int numDistrib)
        {
            this.delta = delta;
            this.numCandid = numCandid;
            this.numDistrib = numDistrib;
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
            List<double> alignedDistr, List<IPeak> alignedPeaks)
        {
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


        public MonoisotopicScore Fit(double precursor, int charge, SortedDictionary<int, IPeak> isotopics)
        {          
            // find distribution
            double mass = Calculator.To.ComputeMass(precursor, Calculator.proton, charge);
            Compound compound = averagine.Fit(mass);

            int start = isotopics.Keys.Min();
            int end = isotopics.Keys.Max();
            List<double> distr = brain.Run(compound, numDistrib + (end-start));

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

            // find the max intensity peak
            List<int> candidIndexes = isotopics.OrderByDescending(p => p.Value.GetIntensity())
                .Select(p => p.Key).Take(numCandid).ToList();

            int maxIsotope = 0; 
            double maxCorr = 0;
            List<IPeak> maxPeaks = new List<IPeak>();
            foreach (int isotope in candidIndexes)
            {
                //align data
                List<double> alignedDistr = new List<double>();
                List<IPeak> alignedPeaks = new List<IPeak>();
                AlignData(distr, isotopics, maxDistrIndex, isotope,
                    alignedDistr, alignedPeaks);

                // compute correlation
                double corr = Score(alignedDistr, alignedPeaks.Select(p => p.GetIntensity()).ToList());
                if (corr > maxCorr)
                {
                    maxIsotope = isotope;
                    maxCorr = corr;
                    maxPeaks = alignedPeaks;
                }
            }

            // compute monoisotopic
            int pos = maxIsotope - maxDistrIndex;
            if (isotopics.ContainsKey(pos))
                return new MonoisotopicScore(isotopics[pos].GetMZ(), maxCorr, maxPeaks);
            return new MonoisotopicScore(precursor + delta / charge * pos, maxCorr, maxPeaks);
        }

        public MonoisotopicScore Search(double precursor, int charge, SortedDictionary<int, List<IPeak>> cluster)
        {
            List<SortedDictionary<int, IPeak>> clustered = Combinator(cluster);
            double maxScore = 0;
            MonoisotopicScore best = new MonoisotopicScore(precursor, 0, new List<IPeak>());
            foreach (SortedDictionary<int, IPeak> sequence in clustered)
            {
                MonoisotopicScore score = Fit(precursor, charge, sequence);
                if (score.GetScore() >= maxScore)
                {
                    maxScore = score.GetScore();
                    best = score;
                }
            }
            return best;
        }

    }
}
