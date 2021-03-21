using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search
{
    public interface ISpectrumSearch
    {
        List<IResult> Search(ISpectrum spectrum);
        void SetTolerance(double tol);
        void SetToleranceBy(ToleranceBy by);
        void SetMaxCharge(int charge);
        void SetCutoff(int score);
    }
}
