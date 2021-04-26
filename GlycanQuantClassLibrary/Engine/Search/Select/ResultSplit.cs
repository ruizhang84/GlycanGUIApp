using GlycanQuant.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuantClassLibrary.Engine.Search.Select
{
    public class ResultSplit
    {
        private double cutoff = 0.5;
        private double threshold = 0.5;

        public ResultSplit(double threshold, double cutoff)
        {
            this.threshold = threshold;
            this.cutoff = cutoff;
        }

        public void SetThreshold(double threshold)
        {
            this.threshold = threshold;
        }

        public void SetCutoff(double cutoff)
        {
            this.cutoff = cutoff;
        }

        double Area(IResult result)
        {
            return result.Matches().Sum(p => p.GetIntensity());
        }

        double Percentile(IEnumerable<double> seq, double percentile)
        {
            var elements = seq.ToArray();
            Array.Sort(elements);
            double realIndex = percentile * (elements.Length - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < elements.Length)
                return elements[index] * (1 - frac) + elements[index + 1] * frac;
            else
                return elements[index];
        }

        List<IResult> Collect(List<IResult> results, 
            int apex, HashSet<int> visited)
        {
            List<IResult> collector = new List<IResult>();
            if (visited.Contains(apex))
                return collector;

            // cutoff
            double value = cutoff * Area(results[apex]);
            int idx = apex;

            // left below cutoff
            while (idx >= 0)
            {
                if (visited.Contains(idx))
                    break;
                double intensity = Area(results[idx]);
                if (intensity < value)
                    break;
                visited.Add(idx);
                collector.Add(results[idx]);
                idx--;
            }
            // left local minimum
            while (idx > 0)
            {
                if (visited.Contains(idx-1))
                    break;
                double intensity = Area(results[idx]);
                double nextIntensity = Area(results[idx - 1]);
                if (nextIntensity > intensity)
                    break;
                idx--;
                visited.Add(idx);
                collector.Add(results[idx]);
            }

            idx = apex + 1;

            // right below cutoff
            while (idx < results.Count)
            {
                if (visited.Contains(idx))
                    break;
                double intensity = Area(results[idx]);
                if (intensity < value)
                    break;
                visited.Add(idx);
                collector.Add(results[idx]);
                idx++;
            }
            // right local minimum
            while (idx < results.Count - 1)
            {
                if (visited.Contains(idx + 1))
                    break;
                double intensity = Area(results[idx]);
                double nextIntensity = Area(results[idx + 1]);
                if (nextIntensity > intensity)
                    break;
                idx++;
                visited.Add(idx);
                collector.Add(results[idx]);
            }
            return collector;
        }

        List<int> Picking(List<IResult> results)
        {
            List<int> localMax = new List<int>();

            int index = 1;
            int end = results.Count - 1;
            int head = index + 1;
            while (index < end)
            {
                if (Area(results[index - 1]) < Area(results[index]))
                {
                    head = index + 1;
                }

                while (head < end
                    && Area(results[head]) == Area(results[index]))
                {
                    head++;
                }

                if (Area(results[head]) < Area(results[index]))
                {
                    localMax.Add(index);
                    index = head;
                }
                index++;
            }
            return localMax;
        }

        public void Split(List<IResult> results, List<SelectResult> selected)
        {
            List<int> localMax = Picking(results).OrderByDescending(i => Area(results[i])).ToList();
            HashSet<int> visited = new HashSet<int>();

            double noise = Percentile(results.Select(p => Area(p)), threshold);
            foreach (int apex in localMax)
            {
                List<IResult> collector = Collect(results, apex, visited);
                if (Area(results[apex]) > noise)
                    selected.Add(new SelectResult(results[apex], collector));
            }
        }
    }
}
