using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanQuant.Spectrum.Charges
{
    public interface ICharger
    {
        int Charge(List<IPeak> peaks, double lower, double upper);
    }
}
