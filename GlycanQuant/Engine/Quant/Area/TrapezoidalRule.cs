using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Quant
{
    public class TrapezoidalRule : IAreaCalculator
    {
        public double Area(List<double> x, List<double> y)
        {

            double sums = 0;
            if (x.Count != y.Count || x.Count < 2)
                return sums;

            for(int i = 1; i < x.Count; i++)
            {
                sums += (y[i] + y[i - 1]) * (x[i] - x[i - 1]) / 2;
            }
            return sums;
        }

    }
}
