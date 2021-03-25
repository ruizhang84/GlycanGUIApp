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
using SpectrumData;
using SpectrumData.Reader;
using NUnit.Framework;
using GlycanQuant.Engine.Quant.XIC;
using GlycanQuant.Engine.Quant;

namespace UnitTestProject
{
    public class AreaUnitTest
    {
        [Test]
        public void AreaTest()
        {
            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<IGlycanPeak> glycans = builder.Build();

            ISpectrumReader spectrumReader = new ThermoRawSpectrumReader();
            spectrumReader.Init(@"C:\Users\iruiz\Downloads\Serum_dextrinspiked_C18_10162018_1.raw");
            int scan = 3474;
            ISpectrum spectrum = spectrumReader.GetSpectrum(scan);


            IResultFactory factory = new NGlycanResultFactory();
            EnvelopeProcess envelopeProcess = new EnvelopeProcess(10, ToleranceBy.PPM);
            MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);
            IProcess spectrumProcess = new LocalNeighborPicking();
            ISpectrumSearch spectrumSearch = new NGlycanSpectrumSearch(glycans,
                spectrumProcess, envelopeProcess, monoisotopicSearcher);

            List<IResult> results = spectrumSearch.Search(spectrum);

            IAreaCalculator areaCalculator = new TrapezoidalRule();
            IXIC xicer = new NGlycanXIC(areaCalculator, spectrumReader, spectrumSearch);
            foreach (IResult r in results)
            {
                Console.WriteLine(r.Glycan().GetGlycan().Name());

                double area = xicer.Area(r);
                Console.WriteLine(area);


                break;
            }

            Assert.Pass();
        }

    }
}
