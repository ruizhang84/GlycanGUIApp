using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanGUIClassLibrary.Util;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Builder.NGlycans;
using NUnit.Framework;

namespace UnitTestProject
{
    public class BuilUnitTest
    {
        [Test]
        public void NGlycanTest()
        {
            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<IGlycanPeak> peaks = builder.Build();

            foreach(var item in peaks)
            {
                Console.WriteLine(item.GetGlycan().Name());
                Console.WriteLine(String.Join(",", item.GetDistrib().Select(d => Math.Round(d, 2).ToString())));
                Console.WriteLine(item.GetGlycan().Mass().ToString() 
                    + " " + item.HighestPeak().ToString());
                Console.WriteLine();
            }
            Assert.Pass();
        }

        [Test]
        public void MassTest()
        {
            double[] Glucose
            = { 470.2727, 674.3725, 878.4723, 1082.572, 1286.6718, 1490.7716,
            1694.8714, 1898.9711, 2103.0709, 2307.1707, 2511.2704};

            int charge = 3;
            using (FileStream ostrm = new FileStream(@"C:\Users\iruiz\Downloads\mass.csv",
                FileMode.OpenOrCreate, FileAccess.Write))
            {
                using(StreamWriter writer = new StreamWriter(ostrm))
                {
                    foreach (double mass in Glucose)
                    {
                        List<double> mzCandidates = Calculator.To.ComputeMZ(mass, charge);
                        string output = String.Join(",", mzCandidates.Select(m => Math.Round(m, 4).ToString()));
                        writer.WriteLine(output);

                    }
                    writer.Flush();
                }

            }
            Assert.Pass();
        }

    }
}
