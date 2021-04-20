using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search
{
    public interface IResult
    {
        double Score();
        void SetScore(double score);
        List<IPeak> Matches();
        void SetMatches(List<IPeak> peaks);
        IGlycanPeak Glycan();
        void SetGlycan(IGlycanPeak glycan);
        int GetScan();
        void SetScan(int scan);
        double GetRetention();
        void SetRetention(double retention);
        double GetMZ();
        void SetMZ(double mz);
        int GetCharge();
        void SetCharge(int charge);
    }
}
