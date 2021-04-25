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
        private double timeTol = 1;

        public ResultSplit(double timeTol, double cutoff)
        {
            this.timeTol = timeTol;
            this.cutoff = cutoff;
        }

        public void SetRetentionTolerance(double timeTol)
        {
            this.timeTol = timeTol;
        }

        public void SetCutoff(double cutoff)
        {
            this.cutoff = cutoff;
        }


        public void Split(List<IResult> results, List<SelectResult> selected)
        {
            int index = 1;
            int end = results.Count - 1;
            int head = index + 1;

            //if (results.First().Glycan().GetGlycan().Name() == "5-6-1-2-0")
            //    Console.WriteLine("here");

            // local maximum

            List<int> localMax = new List<int>();
            double maxValue = cutoff * results.Select(r => r.Matches().Sum(p => p.GetIntensity())).Max();
            while (index < end)
            {
                if (results[index - 1].Matches().Sum(p=> p.GetIntensity()) < 
                    results[index].Matches().Sum(p => p.GetIntensity()))
                {
                    head = index + 1;
                }

                while (head < end
                    && results[head].Matches().Sum(p => p.GetIntensity())
                    == results[index].Matches().Sum(p => p.GetIntensity()))
                {
                    head++;
                }

                if (results[head].Matches().Sum(p => p.GetIntensity())
                    < results[index].Matches().Sum(p => p.GetIntensity()))
                {
                    if (results[index].Matches().Sum(p => p.GetIntensity()) > maxValue)
                        localMax.Add(index);
                    index = head;
                }
                index++;
            }

            localMax = localMax.
                OrderByDescending(i => results[i].Matches().Sum(p => p.GetIntensity())).ToList();

            // cutoff
            HashSet<int> visited = new HashSet<int>();
            foreach(int local in localMax)
            {
                if (visited.Contains(local))
                    continue;
                List<IResult> collector = new List<IResult>();

                double value = cutoff * results[local].Matches().Sum(r => r.GetIntensity());
                int idx = local;

                // left below cutoff
                while (idx >=0)
                {
                    double intensity = results[idx].Matches().Sum(r => r.GetIntensity());
                    if (intensity < value)
                        break;
                    visited.Add(idx);
                    collector.Add(results[idx]);
                    idx--;
                }
                // left local minum
                while (idx > 0)
                {
                    double intensity = results[idx].Matches().Sum(r => r.GetIntensity());
                    double nextIntensity = results[idx-1].Matches().Sum(r => r.GetIntensity());
                    if (nextIntensity > intensity)
                        break;
                    visited.Add(idx);
                    collector.Add(results[idx]);
                    idx--;
                }

                idx = local + 1;

                // right below cutoff
                while (idx < results.Count)
                {
                    double intensity = results[idx].Matches().Sum(r => r.GetIntensity());
                    if (intensity < value)
                        break;
                    visited.Add(idx);
                    collector.Add(results[idx]);
                    idx++;
                }
                // right local minum
                while (idx < results.Count-1)
                {
                    double intensity = results[idx].Matches().Sum(r => r.GetIntensity());
                    double nextIntensity = results[idx + 1].Matches().Sum(r => r.GetIntensity());
                    if (nextIntensity > intensity)
                        break;
                    visited.Add(idx);
                    collector.Add(results[idx]);
                    idx++;
                }

                selected.Add(new SelectResult(results[local], collector));
            }

        }
    }
}
