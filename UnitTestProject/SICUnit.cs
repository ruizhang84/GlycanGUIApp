using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Builder.NGlycans;
using NUnit.Framework;


namespace UnitTestProject
{
    public class SICUnit
    {
        [Test]
        public void GetSICParam()
        {
            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<IGlycanPeak> glycans = builder.Build();

            HashSet<double> mzList = new HashSet<double>();
            foreach(IGlycanPeak g in glycans)
            {
                if (!mzList.Contains(g.HighestPeak()))
                    mzList.Add(g.HighestPeak());
            }


            using (StreamWriter file = new(@"C:\Users\iruiz\Downloads\SICParam.csv"))
            {
                file.WriteLine("MZ,MZToleranceDa");
                foreach(double mz in mzList.OrderBy(p => p).ToList())
                {
                    file.WriteLine(mz.ToString() + ",0.5");
                }
            }


        }
    }
}
