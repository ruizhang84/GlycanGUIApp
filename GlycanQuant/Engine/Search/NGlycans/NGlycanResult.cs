using GlycanQuant.Model;
using GlycanQuant.Model.Builder;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Search.NGlycans
{
    public class NGlycanResult : IResult
    {
        IGlycanPeak glycan;
        double score;
        List<IPeak> peaks;

        public NGlycanResult(IGlycanPeak glycan, double score, List<IPeak> peaks)
        {
            this.glycan = glycan;
            this.score = score;
            this.peaks = peaks;
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

        public void SetGlycan(IGlycanPeak glycan)
        {
            this.glycan = glycan;
        }

        public void SetMatches(List<IPeak> peaks)
        {
            this.peaks = peaks;
        }

        public void SetScore(double score)
        {
            this.score = score;
        }
    }
}
