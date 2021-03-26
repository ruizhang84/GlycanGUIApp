using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Search;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant
{
    public interface IXIC
    {
        double Area(IResult glycan);
    }
}
