using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search.NGlycans
{
    public class NGlycanResultFactory : IResultFactory
    {
        public IResult Produce(IGlycanPeak glycan, double score, List<IPeak> peaks)
        {
            return new NGlycanResult(glycan, score, peaks);
        }
    }
}
