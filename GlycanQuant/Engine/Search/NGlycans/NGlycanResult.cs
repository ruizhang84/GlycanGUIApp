using GlycanQuant.Engine.Search;
using GlycanQuant.Engine;
using GlycanQuant.Engine.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search.NGlycans
{
    public class NGlycanResult : IResult
    {
        IGlycanPeak glycan;
        double score;
        List<IPeak> peaks;
        int scan = 0;
        double mz = 0;
        int charge = 0;

        public NGlycanResult(IGlycanPeak glycan, double score, List<IPeak> peaks)
        {
            this.glycan = glycan;
            this.score = score;
            this.peaks = peaks;
        }

        public int GetCharge()
        {
            return charge;
        }

        public double GetMZ()
        {
            return mz;
        }

        public int GetScan()
        {
            return scan;
        }

        public IGlycanPeak Glycan()
        {
            return glycan;
        }

        public List<IPeak> Matches()
        {
            return peaks;
        }

        public double Score()
        {
            return score;
        }

        public void SetCharge(int charge)
        {
            this.charge = charge;
        }

        public void SetGlycan(IGlycanPeak glycan)
        {
            this.glycan = glycan;
        }

        public void SetMatches(List<IPeak> peaks)
        {
            this.peaks = peaks;
        }

        public void SetMZ(double mz)
        {
            this.mz = mz;
        }

        public void SetScan(int scan)
        {
            this.scan = scan;
        }

        public void SetScore(double score)
        {
            this.score = score;
        }
    }
}
