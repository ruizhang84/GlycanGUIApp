using GlycanGUIClassLibrary.Algorithm;
using GlycanGUIClassLibrary.Algorithm.CurveFitting;
using GlycanGUIClassLibrary.Algorithm.GUIFinder;
using GlycanGUIClassLibrary.Algorithm.GUISequencer;
using GlycanQuant.Engine.Quant;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Process.PeakPicking;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuantApp
{
    public class NormalizerEngine
    {
        public ICurveFitting Fitter { get; set; }
        public double PPM { get; set; } = 5;
        private readonly object resultLock = new object();

        private bool initialized = false;
        public List<double> Retention { get; set; } = new List<double>();
        public List<double> Guis { get; set; } = new List<double>();

        protected List<GUI> GuiPoints = new List<GUI>();
        IAreaCalculator areaCalculator = new TrapezoidalRule();

        public double Area(ISpectrumReader reader)
        {
            if (!initialized) return 0;

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

        public void Run(ISpectrumReader reader, Counter counter)
        {
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
                    lock (resultLock)
                    {
                        pointMaps[i] = finder.FindGlucoseUnits(spectrum);
                    }
                }
                counter.Add(end - start);
            });
            List<List<GUI>> points =
                pointMaps.OrderBy(p => p.Key).Select(p => p.Value).ToList();

            IGUISequencer sequencer = new DynamicProgrammingSequencer();
            GuiPoints = sequencer.Choose(points);

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
            initialized = true;
        }

        public double Normalize(double time)
        {
            if (!initialized)
                return -1;
            return Fitter.GlucoseUnit(time);
        }

    }
}
