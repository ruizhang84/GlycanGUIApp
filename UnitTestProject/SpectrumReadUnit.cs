using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpectrumData;
using SpectrumData.Reader;

namespace UnitTestProject
{
    public class SpectrumReadUnit
    {
        [Test]
        public void Read()
        {
            ISpectrumReader spectrumReader = new ThermoRawSpectrumReader();
            spectrumReader.Init(@"C:\Users\iruiz\Downloads\Serum_dextrinspiked_C18_10162018_1.raw");

            int count = 0;
            for (int scan = spectrumReader.GetFirstScan(); scan <= spectrumReader.GetLastScan(); scan++)
            {
                if(spectrumReader.GetMSnOrder(scan) == 1)
                {
                    count++;
                }
            }

            Console.WriteLine(count);
            Assert.Pass();
        }
    }
}
