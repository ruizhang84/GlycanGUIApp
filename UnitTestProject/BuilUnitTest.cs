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
            TheoryPeaksBuilder builder = new TheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            List<NGlycanPeak> peaks =  builder.Build();

            foreach(var item in peaks)
            {
                Console.WriteLine(item.GetGlycan().Name());
            }

            Assert.Pass();
        }
    }
}
