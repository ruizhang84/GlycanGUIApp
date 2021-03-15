using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumData
{
    public interface ISpectrumReader
    {
        void Init(string fileName);
        int GetFirstScan();
        int GetLastScan();
        int GetMSnOrder(int scanNum);
        double GetRetentionTime(int scanNum);
        ISpectrum GetSpectrum(int scanNum);
        double GetPrecursorMass(int scanNum, int msOrder);
    }
}
