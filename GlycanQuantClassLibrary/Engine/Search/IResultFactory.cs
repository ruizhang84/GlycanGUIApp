using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search
{
    public interface IResultFactory
    {
        IResult Produce(IGlycanPeak glycan, double score, List<IPeak> peaks);
    }
}
