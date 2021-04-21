using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant
{
    public interface IAreaCalculator
    {
        double Area(List<double> X, List<double> Y);
    }
}
