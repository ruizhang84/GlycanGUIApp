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
        private double cutoff = 0.3;
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

        double Area(IResult result)
        {
            return result.Matches().Sum(p => p.GetIntensity());
        }

        public void Picking(List<IResult> results, 
            List<SelectResult> selected, HashSet<int> visited)
        {
            // local maximum
            int local = 0;
            double bestIntensity = 0;
            for(int i = 0; i < results.Count; i++)
            {
                IResult result = results[i];
                double intensity = Area(result);
                if (intensity > bestIntensity)
                {
                    bestIntensity = intensity;
                    local = i;
                }
            }

            // cutoff
            List<IResult> collector = new List<IResult>();
            double value = cutoff * results[local].Matches().Sum(r => r.GetIntensity());
            int idx = local;

            // left below cutoff
            while (idx >= 0)
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
                double nextIntensity = results[idx - 1].Matches().Sum(r => r.GetIntensity());
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
            while (idx < results.Count - 1)
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

        public void Split(List<IResult> results, List<SelectResult> selected)
        {
            HashSet<int> visited = new HashSet<int>();
            Picking(results, selected, visited);

            // start
            List<IResult> start = new List<IResult>();
            for(int i = 0; i < results.Count; i++)
            {
                if (visited.Contains(i))
                    break;
                start.Add(results[i]);
            }

            // end
            List<IResult> end = new List<IResult>();
            for (int i = visited.Max() + 1; i < results.Count; i++)
            {
                end.Add(results[i]);
            }

            if (start.Count > 0)
                Split(start, selected);
            if (end.Count > 0)
                Split(end, selected);

        }
    }
}
