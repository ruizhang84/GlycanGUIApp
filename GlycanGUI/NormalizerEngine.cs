using GlycanGUI.Algorithm;
using GlycanGUI.Algorithm.CurveFitting;
using GlycanGUI.Algorithm.GUIFinder;
using GlycanGUI.Algorithm.GUISequencer;
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

namespace GlycoGUIApp
{
    public class NormalizerEngine
    {
        public ICurveFitting Fitter { get; set; }
        public double PPM { get; set; } = 5;
        private readonly object resultLock = new object();

        public List<double> Retention { get; set; } = new List<double>();
        public List<double> Guis { get; set; }  = new List<double>();

        public void Run(string path, Counter counter)
        {
            ISpectrumReader reader = new ThermoRawSpectrumReader();
            reader.Init(path);

            IGUIFinder finder = new BinarySearchFinder(PPM);
            IProcess picking = new LocalNeighborPicking();
            Dictionary<int, List<GUI>> pointMaps = new Dictionary<int, List<GUI>>();

            int start = reader.GetFirstScan();
            int end = reader.GetLastScan();
            Parallel.For(start, end, (i) =>
            {
                if (reader.GetMSnOrder(i) < 2)
                {
                    ISpectrum spectrum = picking.Process(reader.GetSpectrum(i));
                    lock(resultLock)
                    {
                        pointMaps[i] = finder.FindGlucoseUnits(spectrum);
                    }
                }
                counter.Add(end - start);
            });
            List<List<GUI>> points = 
                pointMaps.OrderBy(p => p.Key).Select(p => p.Value).ToList();

            IGUISequencer sequencer = new DynamicProgrammingSequencer();
            List<GUI> guiPoints = sequencer.Choose(points);

            Fitter = new PolynomialFitting();

            Dictionary<int, GUI> guiSelected = new Dictionary<int, GUI>();
            foreach (GUI gui in guiPoints)
            {
                if (guiSelected.ContainsKey(gui.Unit))
                {
                    if(guiSelected[gui.Unit].Peak.GetIntensity() < gui.Peak.GetIntensity())
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

            List<GUI> looped = guiSelected.Values.OrderBy(g => g.Scan).ToList();
            string output = Path.Combine(Path.GetDirectoryName(path), 
                Path.GetFileNameWithoutExtension(path) + ".csv");
            using (FileStream ostrm = new FileStream(output, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(ostrm))
                {
                    writer.WriteLine("scan,time,gui,peak,intensity");
                    foreach (GUI gui in looped)
                    {
                        int scan = gui.Scan;
                        double time = reader.GetRetentionTime(scan);
                        Retention.Add(time);
                        Guis.Add(gui.Unit);
                        writer.WriteLine(scan.ToString() + "," +
                            time.ToString() + "," +
                            gui.Unit.ToString() + "," +
                            gui.Peak.GetMZ().ToString() + "," +
                            gui.Peak.GetIntensity().ToString());
                    }
                    writer.Flush();
                }
            }
            Fitter.Fit(Retention, Guis);
        }

        public double Normalize(double time)
        {
            return Fitter.GlucoseUnit(time);
        }

        public double Coefficient()
        {
            List<double> predits = new List<double>();
            foreach(double x in Retention)
            {
                predits.Add(Fitter.GlucoseUnit(x));
            }
            double mean = Guis.Average();
            double ss_total = 0;
            double ss_residue = 0;
            for(int i = 0; i < predits.Count; i++)
            {
                ss_total += (Guis[i] - mean) * (Guis[i] - mean);
                ss_residue += (Guis[i] - predits[i]) * (Guis[i] - predits[i]);
            }
            return 1.0 - ss_residue / ss_total;
        }

    }
}
