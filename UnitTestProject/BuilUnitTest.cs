using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
