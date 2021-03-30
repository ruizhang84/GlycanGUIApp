using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUIClassLibrary.Algorithm
{
    public interface IGUIFinder
    {
        double GetTolerance();
        void SetTolerance(double ppm);
        List<GUI> FindGlucoseUnits(ISpectrum spectrum);
    }
}
