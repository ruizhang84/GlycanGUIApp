using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanGUIClassLibrary.Algorithm;
using GlycanGUIClassLibrary.Algorithm.GUISequencer;
using NUnit.Framework;
using SpectrumData.Spectrum;

namespace UnitTestProject
{
    public class GlycoUnitTest
    {
        [Test]
        public void DynamicProgrammingTest()
        {
            List<List<GUI>> guis = new List<List<GUI>>()
            {
                new List<GUI>()
                {
                    // unit, scan, peak
                    new GUI(2, 1, new GeneralPeak(1, 2)),
                    new GUI(3, 1, new GeneralPeak(1, 10)),
                    new GUI(4, 1, new GeneralPeak(1, 4)),
                },

                new List<GUI>()
                {
                    // unit, scan, peak
                    new GUI(2, 2, new GeneralPeak(1, 4)),
                    new GUI(3, 2, new GeneralPeak(1, 10)),
                    new GUI(3, 2, new GeneralPeak(1, 2)),
                },

                new List<GUI>()
                {
                    // unit, scan, peak
                    new GUI(3, 3, new GeneralPeak(1, 11)),
                    new GUI(4, 3, new GeneralPeak(1, 10)),
                    new GUI(5, 3, new GeneralPeak(1, 12)),
                },

                new List<GUI>()
                {
                    // unit, scan, peak
                    new GUI(4, 4, new GeneralPeak(1, 10)),
                    new GUI(5, 4, new GeneralPeak(1, 14)),
                    new GUI(7, 4, new GeneralPeak(1, 10)),
                },

                new List<GUI>()
                {
                    // unit, scan, peak
                    new GUI(3, 5, new GeneralPeak(1, 15)),
                    new GUI(5, 5, new GeneralPeak(1, 3)),
                    new GUI(6, 5, new GeneralPeak(1, 4)),
                },

            };

            IGUISequencer sequencer = new DynamicProgrammingSequencer();
            List<GUI> GuiPoints = sequencer.Choose(guis);
            foreach(GUI g in GuiPoints)
            {
                Console.WriteLine(g.Unit);
            }

            Assert.Pass();

        }
    }
}
