using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search.Select
{
    public class ResultMaxSelect : IResultSelect
    {
        Dictionary<string, List<IResult>> resultMap 
            = new Dictionary<string, List<IResult>>();

        private double timeTol = 3;
        private int pricison = 2;

        public ResultMaxSelect(double timeTol=3)
        {
            this.timeTol = timeTol;
        }
        public void SetRetentionTolerance(double tol)
        {
            timeTol = tol;
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

                foreach(double mz in resultGroup.Keys)
                {
                    List<IResult> collect = new List<IResult>();
                    filtered[glycanName] = new List<SelectResult>();

                    foreach (IResult r in resultGroup[mz])
                    {
                        int scan = r.GetScan();
                        if (collect.Count == 0)
                        {
                            collect.Add(r);
                        }
                        else
                        {
                            // check scan continue, check mz same
                            double retention = r.GetRetention();
                            if (collect.Last().GetRetention() + timeTol < retention)
                            {
                                IResult present = Select(collect);
                                filtered[glycanName].Add(new SelectResult(present, collect));
                                collect = new List<IResult>();
                            }
                            collect.Add(r);
                        }
                    }

                    if (collect.Count > 0)
                    {
                        IResult present = Select(collect);
                        filtered[glycanName].Add(new SelectResult(present, collect));
                    }
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
