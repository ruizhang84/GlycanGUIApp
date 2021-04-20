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

        private double tol = 0.1;
        private ToleranceBy by = ToleranceBy.Dalton;
        private double timeTol = 3;

        public ResultMaxSelect(double tol = 0.1, 
            ToleranceBy by=ToleranceBy.Dalton, double timeTol=3)
        {
            this.tol = tol;
            this.by = by;
            this.timeTol = timeTol;
        }

        public void SetTol(double tol)
        {
            this.tol = tol;
        }
        
        public void SetToleranceBy(ToleranceBy by)
        {
            this.by = by;
        }

        public void SetRetentionTolerance(double tol)
        {
            timeTol = tol;
        }

        bool Differ(double curr, double target)
        {
            if (by == ToleranceBy.PPM)
            {
                if (Math.Abs(curr - target) / target * 1000000.0 <= tol) return false;
            }
            else
            {
                if (Math.Abs(curr - target) <= tol) return false;
            }

            return true;
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
                List<IResult> result = resultMap[glycanName]
                    .OrderBy(r => r.GetScan()).ToList();
                List<IResult> collect = new List<IResult>();
                filtered[glycanName] = new List<SelectResult>();

                foreach (IResult r in result)
                {
                    int scan = r.GetScan();
                    if (collect.Count == 0)
                    {
                        collect.Add(r);
                    }
                    else
                    {
                        // check scan continue, check mz same
                        double mz = r.GetMZ();
                        double retention = r.GetRetention();
                        if (collect.Last().GetRetention() + timeTol < retention ||
                            Differ(collect.Last().GetMZ(), mz))
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
