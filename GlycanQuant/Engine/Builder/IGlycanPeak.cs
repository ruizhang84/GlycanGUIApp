using GlycanQuant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Builder
{
    interface IGlycanPeak
    {
        IGlycan GetGlycan();
        List<double> GetDistrib();
    }
}
