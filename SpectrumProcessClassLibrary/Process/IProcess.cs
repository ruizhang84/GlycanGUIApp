using SpectrumData;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrecursorIonClassLibrary.Process
{
    public interface IProcess
    {
        ISpectrum Process(ISpectrum spectrum);
    }
}
