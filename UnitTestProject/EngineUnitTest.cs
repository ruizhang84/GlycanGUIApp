using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Builder.NGlycans;
using GlycanQuant.Engine.Search;
using GlycanQuant.Engine.Search.Envelope;
using GlycanQuant.Engine.Search.NGlycans;
using GlycanQuant.Model.Util;
using GlycanQuant.Spectrum.Charges;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Process.PeakPicking;
using NUnit.Framework;
using SpectrumData;
using SpectrumData.Reader;

namespace UnitTestProject
{
    public class EngineUnitTest
    {
        [Test]
        public void NGlycanSearch()
        {
            IResultFactory factory = new NGlycanResultFactory();
            EnvelopeProcess envelopeProcess = new EnvelopeProcess(10, ToleranceBy.PPM);
            MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);

            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<IGlycanPeak> glycans = builder.Build();

            IProcess spectrumProcess = new LocalNeighborPicking();
            ISpectrumSearch spectrumSearch = new NGlycanSpectrumSearch(glycans,
                spectrumProcess, envelopeProcess, monoisotopicSearcher);

            ISpectrumReader spectrumReader = new ThermoRawSpectrumReader();
            spectrumReader.Init(@"C:\Users\iruiz\Downloads\Serum_dextrinspiked_C18_10162018_2.raw");
            for(int scan = spectrumReader.GetFirstScan(); scan <= spectrumReader.GetLastScan(); scan++)
            {
                if (spectrumReader.GetMSnOrder(scan) != 1)
                    continue;
                ISpectrum spectrum = spectrumReader.GetSpectrum(scan);

                List<IResult> results = spectrumSearch.Search(spectrum);
                foreach(IResult r in results)
                {
                    Console.WriteLine(r.Glycan().GetGlycan().Name() 
                        + ": " + r.Glycan().GetGlycan().Mass().ToString());

                    List<double> mzList = Calculator.To.ComputeMZ(r.Glycan().HighestPeak(), 3);
                    Console.WriteLine(string.Join(",", mzList));


                    Console.WriteLine(r.Score());
                    foreach (IPeak pk in r.Matches())
                    {
                        Console.WriteLine(pk.GetMZ() + " " + pk.GetIntensity());
                    }
                    Console.WriteLine();
                }

                break;
            }
            

            Assert.Pass();
        }
    }
}
