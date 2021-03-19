using GlycanQuant.Model;
using GlycanQuant.Model.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Search
{
    public interface ISpectrumSearch
    {
        double Tolerance();
        void SetTolerance(double tol);
        List<IResult> Search(ISpectrum spectrum);
        double Target(IGlycanPeak glycan);
        List<IPeak> Envelope(double mz, int charge);
        double Score(List<IPeak> cluster, IGlycanPeak glycan);
    }
}
