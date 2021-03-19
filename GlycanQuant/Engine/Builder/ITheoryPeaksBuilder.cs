using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Builder
{
    public interface ITheoryPeaksBuilder
    {
        List<IGlycanPeak> Build();
    }
}
