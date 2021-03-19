using GlycanQuant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Builder
{
    public interface IGlycanPeak
    {
        IGlycan GetGlycan();
        List<double> GetDistrib();
        // mass of glycan corresponding to most aboundant peak
        double HighestPeak();  
    }
}
