using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUI.Algorithm
{
    public interface ICurveFitting
    {
        void Fit(List<double> rentention, List<double> guis);
        double GlucoseUnit(double rentention);
    }
}
