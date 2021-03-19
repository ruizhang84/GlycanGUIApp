using GlycanQuant.Model.Algorithm;
using GlycanQuant.Model;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanQuant.Model.Builder;

namespace GlycanQuant.Model.Search.NGlycans
{
    public class NGlycanSpectrumSearch : ISpectrumSearch
    {
        double tol;
        ISearch<IPeak> searcher;

        public NGlycanSpectrumSearch(double tol, ISearch<IPeak> searcher)
        {
            this.tol = tol;
            this.searcher = searcher;
        }


        public List<IPeak> Envelope(double mz, int charge)
        {
            List<IPeak> matches = searcher.Search(mz);
            return matches;
        }

        public double Score(List<IPeak> cluster, IGlycanPeak glycan)
        {
            throw new NotImplementedException();
        }

        public List<IResult> Search(ISpectrum spectrum)
        {
            throw new NotImplementedException();
        }

        public void SetTolerance(double tol)
        {
            this.tol = tol;
        }

        public double Target(IGlycanPeak glycan)
        {
            return glycan.HighestPeak();
        }

        public double Tolerance()
        {
            return tol;
        }
    }
}
