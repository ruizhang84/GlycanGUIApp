using GlycanQuant.Engine.Algorithm;
using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Search.Envelope;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Charges;
using GlycanQuant.Model.Util;

namespace GlycanQuant.Engine.Search.NGlycans
{
    public class NGlycanSpectrumSearch : ISpectrumSearch
    {
        List<IGlycanPeak> glycans;
        EnvelopeProcess envelopeProcessor;
        MonoisotopicSearcher monoisotopicSearcher;
        IProcess spectrumProcessor;
        int maxCharge = 4;
        double cutoff = 0.9;

        public NGlycanSpectrumSearch(List<IGlycanPeak> glycans,
            IProcess spectrumProcessor, EnvelopeProcess envelopeProcessor, 
            MonoisotopicSearcher monoisotopicSearcher,
            int maxCharge = 3, double cutoff = 0.9)
        {
            this.glycans = glycans;
            this.spectrumProcessor = spectrumProcessor;
            this.envelopeProcessor = envelopeProcessor;
            this.monoisotopicSearcher = monoisotopicSearcher;
            this.maxCharge = maxCharge;
            this.cutoff = cutoff;
        }

        public List<IResult> Search(ISpectrum spectrum)
        {
            List<IResult> results = new();

            ISpectrum spec = spectrumProcessor.Process(spectrum);
            envelopeProcessor.Init(spec);

            foreach (IGlycanPeak glycan in glycans)
            {
                IResult result = null;
                double bestScore = 0;
                for (int charge = 1; charge < maxCharge; charge++)
                {
                    List<double> mzList = Calculator.To.ComputeMZ(glycan.HighestPeak(), charge);
                    foreach(double mz in mzList)
                    {
                        List<IPeak> targets = envelopeProcessor.Search(mz);
                        if (targets.Count == 0)
                            continue;

                        SortedDictionary<int, List<IPeak>> clusters = 
                            envelopeProcessor.Cluster(mz, charge);

                        IResult temp = monoisotopicSearcher.Match(glycan, clusters);
                        double score = temp.Score();
                        if (score > cutoff && score > bestScore)
                        {
                            bestScore = score;
                            result = temp;
                            result.SetMZ(mz);
                            result.SetRetention(spectrum.GetRetention());
                            result.SetCharge(charge);
                            result.SetScan(spectrum.GetScanNum());
                        }
                    }
                }
                if (result != null)
                    results.Add(result);
            }
            return results;
        }

        public IResult Search(ISpectrum spectrum, IGlycanPeak glycan ,
            double mz, int charge)
        {

            ISpectrum spec = spectrumProcessor.Process(spectrum);
            envelopeProcessor.Init(spec);

            List<IPeak> targets = envelopeProcessor.Search(mz);
            if (targets.Count == 0)
                return null;

            SortedDictionary<int, List<IPeak>> clusters =
                envelopeProcessor.Cluster(mz, charge);

            IResult result = monoisotopicSearcher.Match(glycan, clusters);
            if (result.Matches().Count == 0)
                return null;
            
            result.SetCharge(charge);
            result.SetMZ(mz);
            result.SetScan(spectrum.GetScanNum());
            return result;
        }

        public void SetTolerance(double tol)
        {
            envelopeProcessor.SetTolerance(tol);
        }

        public void SetToleranceBy(ToleranceBy by)
        {
            envelopeProcessor.SetToleranceBy(by);
        }

        public void SetMaxCharge(int charge)
        {
            maxCharge = charge;
        }

        public void SetCutoff(int score)
        {
            cutoff = score;
        }
    }
}
