using GlycanGUI.Algorithm;
using GlycanGUI.Algorithm.CurveFitting;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Builder.NGlycans;
using GlycanQuant.Engine.Quant;
using GlycanQuant.Engine.Quant.XIC;
using GlycanQuant.Engine.Search;
using GlycanQuant.Engine.Search.Envelope;
using GlycanQuant.Engine.Search.NGlycans;
using GlycanQuant.Engine.Search.Select;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Process.PeakPicking;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuantApp
{
    public class QuantEngine
    {
        private readonly object resultLock = new object();

        public void Run(string path, Counter counter, ISpectrumReader spectrumReader, NormalizerEngine normalizer)
        {
            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<IGlycanPeak> glycans = builder.Build();

            spectrumReader.Init(path);

            IResultSelect resultSelect = new ResultMaxSelect(SearchingParameters.Access.Threshold,
                SearchingParameters.Access.PeakCutoff);
            List<IResult> final = new List<IResult>();

            int start = spectrumReader.GetFirstScan();
            int end = spectrumReader.GetLastScan();
            Parallel.For(start, end, (i) =>
            {
                if (spectrumReader.GetMSnOrder(i) < 2)
                {
                    IResultFactory factory = new NGlycanResultFactory();
                    EnvelopeProcess envelopeProcess = new EnvelopeProcess(
                        SearchingParameters.Access.Tolerance,
                        SearchingParameters.Access.ToleranceBy);
                    MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);
                    IProcess spectrumProcess = new LocalNeighborPicking();
                    ISpectrumSearch spectrumSearch = new NGlycanSpectrumSearch(glycans,
                        spectrumProcess, envelopeProcess, monoisotopicSearcher,
                        SearchingParameters.Access.MaxCharage, SearchingParameters.Access.Cutoff);

                    ISpectrum spectrum = spectrumReader.GetSpectrum(i);
                    List<IResult> results = spectrumSearch.Search(spectrum);
                    lock (resultLock)
                    {
                        resultSelect.Add(results);
                        final.AddRange(results);
                    }
                }
                counter.Add(end - start);
            });

            //original
            string outputPath = Path.Combine(Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + "_one_quant.csv");
            List<string> outputString = new List<string>();
            foreach (IResult present in final.OrderBy(r => r.GetRetention()))
            {
                int scan = present.GetScan();
                double rt = present.GetRetention();
                double index = Math.Round(normalizer.Normalize(rt), 2);

                TIQ3XIC xicer = new TIQ3XIC(spectrumReader);
                double area = xicer.OneArea(present);

                List<string> output;
                if (!normalizer.Initialized())
                {
                    output = new List<string>()
                        {
                            scan.ToString(), rt.ToString(),
                            present.Glycan().GetGlycan().Name(),
                            present.GetMZ().ToString(),
                            area.ToString(),
                        };
                }
                else
                {
                    output = new List<string>()
                        {
                            scan.ToString(), rt.ToString(),
                            index > 0? index.ToString():"0",
                            present.Glycan().GetGlycan().Name(),
                            present.GetMZ().ToString(),
                            area.ToString(),
                        };
                }
                outputString.Add(string.Join(",", output));
            }

            using (FileStream ostrm = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    if (normalizer.Initialized())
                        writer.WriteLine("scan,time,glycan,mz,area");
                    else
                        writer.WriteLine("scan,time,GUI,glycan,mz,area");
                    foreach (string output in outputString)
                    {
                        writer.WriteLine(output);
                    }
                    writer.Flush();
                }
            }



            // merged
            string outputPathMerge = Path.Combine(Path.GetDirectoryName(path),
               Path.GetFileNameWithoutExtension(path) + "_quant.csv");
            List<string> outputStringMerge = new List<string>();
            Dictionary<string, List<SelectResult>> resultContainer = resultSelect.Produce();

            foreach (string name in resultContainer.Keys)
            {
                List<SelectResult> selectResults = resultContainer[name];
                foreach (SelectResult select in selectResults)
                {
                    IResult present = select.Present;
                    int scan = present.GetScan();
                    double rt = present.GetRetention();
                    double index = Math.Round(normalizer.Normalize(rt), 2);

                    IXIC xicer = new TIQ3XIC(spectrumReader);
                    double area = xicer.Area(select);

                    List<string> output;
                    if (!normalizer.Initialized())
                    {
                        output = new List<string>()
                        {
                            scan.ToString(), rt.ToString(),
                            name,
                            present.GetMZ().ToString(),
                            area.ToString(),
                        };
                    }
                    else
                    {
                        output = new List<string>()
                        {
                            scan.ToString(), rt.ToString(),
                            index.ToString(),
                            name,
                            present.GetMZ().ToString(),
                            area.ToString(),
                        };
                    }
                    outputStringMerge.Add(string.Join(",", output));
                }
            }

            using (FileStream ostrm = new FileStream(outputPathMerge, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    if (normalizer.Initialized())
                        writer.WriteLine("scan,time,glycan,mz,area");
                    else
                        writer.WriteLine("scan,time,GUI,glycan,mz,area");
                    foreach (string output in outputStringMerge)
                    {
                        writer.WriteLine(output);
                    }
                    writer.Flush();
                }
            }
        }
    }
}
