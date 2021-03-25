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

            for(int i = 0; i < x.Count; i++)
            {
                if (i == 0 || i == x.Count - 1)
                    sums += y[i];
                else
                    sums += 2 * y[i];
            }
            return (x[x.Count-1] - x[0]) / x.Count * sums;

        }

    }
}
