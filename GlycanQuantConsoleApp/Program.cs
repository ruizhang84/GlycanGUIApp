using GlycanGUIClassLibrary.Algorithm;
using GlycanGUIClassLibrary.Algorithm.CurveFitting;
using GlycanGUIClassLibrary.Algorithm.GUIFinder;
using GlycanGUIClassLibrary.Algorithm.GUISequencer;
using GlycanQuant.Engine.Algorithm;
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GlycanQuantConsoleApp
{
    class Program
    {
        static double Area(ISpectrumReader reader, List<GUI> GuiPoints)
        {
            IAreaCalculator areaCalculator = new TrapezoidalRule();
            List<double> X = new List<double>();
            List<double> Y = new List<double>();

            foreach (GUI g in GuiPoints.OrderBy(p => p.Scan))
            {
                double rt = reader.GetRetentionTime(g.Scan);
                double intensity = g.Peak.GetIntensity();
                X.Add(rt);
                Y.Add(intensity);
            }

            return areaCalculator.Area(X, Y);
        }
        static List<GUI> Init(ref ICurveFitting Fitter, ISpectrumReader reader)
        {
            double ppm = 5;
            object resultLock = new object();
            List<double> Retention = new List<double>();
            List<double> Guis = new List<double>();

            IGUIFinder finder = new BinarySearchFinder(ppm);
            IProcess picking = new LocalNeighborPicking();
            Dictionary<int, List<GUI>> pointMaps = new Dictionary<int, List<GUI>>();

            int start = reader.GetFirstScan();
            int end = reader.GetLastScan();
            Parallel.For(start, end, (i) =>
            {
                if (reader.GetMSnOrder(i) < 2)
                {
                    ISpectrum spectrum = picking.Process(reader.GetSpectrum(i));
                    lock (resultLock)
                    {
                        pointMaps[i] = finder.FindGlucoseUnits(spectrum);
                    }
                }
            });


            List<List<GUI>> points =
                pointMaps.OrderBy(p => p.Key).Select(p => p.Value).ToList();

            IGUISequencer sequencer = new DynamicProgrammingSequencer();
            List<GUI> GuiPoints = sequencer.Choose(points);

            Fitter = new PolynomialFitting();

            Dictionary<int, GUI> guiSelected = new Dictionary<int, GUI>();
            foreach (GUI gui in GuiPoints)
            {
                if (guiSelected.ContainsKey(gui.Unit))
                {
                    if (guiSelected[gui.Unit].Peak.GetIntensity() < gui.Peak.GetIntensity())
                    {
                        guiSelected[gui.Unit] = gui;
                    }
                }
                else
                {
                    guiSelected[gui.Unit] = gui;
                }
            }

            Retention.Clear();
            Guis.Clear();

            List<GUI> guiChoice = guiSelected.Values.OrderBy(g => g.Scan).ToList();

            foreach (GUI gui in guiChoice)
            {
                int scan = gui.Scan;
                double time = reader.GetRetentionTime(scan);
                Retention.Add(time);
                Guis.Add(gui.Unit);
            }

            Fitter.Fit(Retention, Guis);

            return GuiPoints;
        }
        static double Normalize(ICurveFitting Fitter,  double time)
        {
            return Fitter.GlucoseUnit(time);
        }

        //"D:\\Data\\raw\\C18\\231_1_C18_50minWash_03012019.raw"
        static void Main(string[] args)
        {

            string dir = @"D:\Data\raw\C18";
            string[] files = Directory.GetFiles(dir);

            foreach(string path in files)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
                builder.SetBuildType(true, false, false);
                List<IGlycanPeak> glycans = builder.Build();

                IResultFactory factory = new NGlycanResultFactory();
                EnvelopeProcess envelopeProcess = new EnvelopeProcess(10, ToleranceBy.PPM);
                MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);
                IProcess spectrumProcess = new LocalNeighborPicking();
                ISpectrumSearch spectrumSearch = new NGlycanSpectrumSearch(glycans,
                    spectrumProcess, envelopeProcess, monoisotopicSearcher);
                ISpectrumReader spectrumReader = new ThermoRawSpectrumReader();
                spectrumReader.Init(path);


                ICurveFitting Fitter = new PolynomialFitting();

                string outputPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path),
                          System.IO.Path.GetFileNameWithoutExtension(path) + "_quant.csv");

                IResultSelect resultSelect = new ResultMaxSelect(0.5,ToleranceBy.Dalton, 3);
                List<GUI> GuiPoints = Init(ref Fitter, spectrumReader);

                for (int scan = spectrumReader.GetFirstScan(); scan <= spectrumReader.GetLastScan(); scan++)
                {
                    if (spectrumReader.GetMSnOrder(scan) != 1)
                        continue;
                    ISpectrum spectrum = spectrumReader.GetSpectrum(scan);
                    List<IResult> results = spectrumSearch.Search(spectrum);
                    resultSelect.Add(results);
                }

                List<string> outputString = new List<string>();
                Dictionary<string, List<SelectResult>> resultContainer = resultSelect.Produce();
                double areaIndex = Area(spectrumReader, GuiPoints);

                foreach (string name in resultContainer.Keys)
                {
                    List<SelectResult> selectResults = resultContainer[name];
                    foreach (SelectResult select in selectResults)
                    {
                        IResult present = select.Present;
                        int scan = present.GetScan();
                        double rt = present.GetRetention();
                        double index = Math.Round(Normalize(Fitter, rt), 2);

                        IXIC xicer = new TIQ3XIC(spectrumReader);
                        double area = xicer.Area(select);

                        List<string> output = new List<string>()
                            {
                                scan.ToString(), rt.ToString(),
                                index > 0? index.ToString():"0",
                                name,
                                present.GetMZ().ToString(),
                                area.ToString(),
                                areaIndex > 0? (area/areaIndex).ToString():"0"
                            };
                        outputString.Add(string.Join(",", output));
                    }
                }

                using (FileStream ostrm = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(ostrm))
                    {
                        writer.WriteLine("scan,time,GUI,glycan,mz,area,factor");
                        //writer.WriteLine("scan,time,glycan,mz,area");
                        foreach (string output in outputString)
                        {
                            writer.WriteLine(output);
                        }
                        writer.Flush();
                    }
                }

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);

            }

        }
    }
}
