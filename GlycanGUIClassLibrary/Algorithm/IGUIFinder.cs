using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUI.Algorithm
{
    public interface IGUIFinder
    {
        double GetTolerance();
        void SetTolerance(double ppm);
        List<GUI> FindGlucoseUnits(ISpectrum spectrum);
    }
}
