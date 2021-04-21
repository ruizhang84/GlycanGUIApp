using GlycanQuant.Engine.Algorithm;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycanQuant.Engine.Search.Envelope
{
    public class EnvelopeProcess
    {
        readonly double range = 1; // 1 mz
        ISearch<IPeak> searcher;

        public EnvelopeProcess(double tol = 0.01, ToleranceBy by = ToleranceBy.Dalton)
        {
            searcher = new BucketSearch<IPeak>(by, tol);
        }

        public void Init(ISpectrum spectrum)
        {
            searcher.Init(spectrum.GetPeaks()
                .Select(p => new Point<IPeak>(p.GetMZ(), p)).ToList());
        }

        public void SetTolerance(double tol)
        {
            searcher.SetTolerance(tol);
        }

        public void SetToleranceBy(ToleranceBy by)
        {
            searcher.SetToleranceBy(by);
        }

        public List<IPeak> Search(double mz)
        {
            return searcher.Search(mz);
        }

        public SortedDictionary<int, List<IPeak>> Cluster(double mz, int charge)
        {
            //int: diff of isotope
            SortedDictionary<int, List<IPeak>> cluster = 
                new SortedDictionary<int, List<IPeak>>();
            
            double steps = 1.0 / charge;
            int index = 0;
            while (steps * index < range) // search with 1 mz
            {
                double target = mz + steps * index;
                List<IPeak> isotopics = searcher.Search(target);
                if (isotopics.Count > 0)
                    cluster[index] = isotopics;
                index++;
            }
            index = -1;
            while (steps * index > -range)
            {
                double target = mz + steps * index;
                List<IPeak> isotopics = searcher.Search(target);
                if (isotopics.Count > 0)
                    cluster[index] = isotopics;
                index--;
            }
            
            return cluster ;
        }

    }
}
