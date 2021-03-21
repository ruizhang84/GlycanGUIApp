using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search
{
    public interface IResult
    {
        double Score();
        void SetScore(double score);
        List<IPeak> Matches();
        void SetMatches(List<IPeak> peaks);
        IGlycanPeak Glycan();
        void SetGlycan(IGlycanPeak glycan);
    }
}
