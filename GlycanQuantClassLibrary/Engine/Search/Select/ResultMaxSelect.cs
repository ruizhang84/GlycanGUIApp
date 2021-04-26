using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Builder;
using GlycanQuantClassLibrary.Engine.Search.Select;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search.Select
{
    public class ResultMaxSelect : IResultSelect
    {
        ConcurrentDictionary<string, List<IResult>> resultMap 
            = new ConcurrentDictionary<string, List<IResult>>();

        private int pricison = 2;
        private ResultSplit splitor;

        public ResultMaxSelect(double threshold=0.5, double cutoff=0.5)
        {
            splitor = new ResultSplit(threshold, cutoff);
        }
        public void SetThreshold(double threshold)
        {
            splitor.SetThreshold(threshold);
        }

        public void SetCutoff(double cutoff)
        {
            splitor.SetCutoff(cutoff);
        }

        public void Add(List<IResult> results)
        {
            foreach (IResult r in results)
            {
                string name = r.Glycan().GetGlycan().Name();
                if (!resultMap.ContainsKey(name))
                {
                    resultMap[name] = new List<IResult>();
                }
                resultMap[name].Add(r);
            }
        }

        public Dictionary<string, List<SelectResult>> Produce()
        {
            Dictionary<string, List<SelectResult>> filtered 
                = new Dictionary<string, List<SelectResult>>();

            foreach (string glycanName in resultMap.Keys)
            {
                Dictionary<double, List<IResult>> resultGroup =
                    resultMap[glycanName].GroupBy(r => Math.Round(r.GetMZ(), pricison))
                    .ToDictionary(g => g.Key, g => g.OrderBy(r => r.GetScan()).ToList());
                filtered[glycanName] = new List<SelectResult>();

                foreach (double mz in resultGroup.Keys)
                {
                    splitor.Split(resultGroup[mz], filtered[glycanName]);
                }
            }

            return filtered;
        }

        public IResult Select(List<IResult> results)
        {
            IResult best = null;
            double bestArea = 0;
            foreach(IResult r in results)
            {
                double area = r.Matches().Select(p => p.GetIntensity()).Sum();
                if (area > bestArea)
                {
                    bestArea = area;
                    best = r;
                }
            }
            return best;
        }
    }
}
