using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Process.PeakPicking;
using NUnit.Framework;
using SpectrumData;
using SpectrumData.Spectrum;

namespace UnitTestProject
{
    
    public class PeakDetectionUnit
    {
        public static double Percentile(IEnumerable<double> seq, double percentile)
        {
            var elements = seq.ToArray();
            Array.Sort(elements);
            double realIndex = percentile * (elements.Length - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < elements.Length)
                return elements[index] * (1 - frac) + elements[index + 1] * frac;
            else
                return elements[index];
        }

        [Test]
        public void TestUnit()
        {
            // creat spectrum
            List<IPeak> peaks = new List<IPeak>();
            using (var reader = new StreamReader(
                @"C:\Users\iruiz\Downloads\GUI\compare\peaks.csv"))
            {
                int skip = 1;
                while (!reader.EndOfStream)
                {
                    if (skip-- > 0)
                    {
                        reader.ReadLine();
                        continue;
                    }
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    double time = double.Parse(values[1]);
                    double intensity = double.Parse(values[5]);
                    IPeak p = new GeneralPeak(time, intensity);
                    peaks.Add(p);
                }
                
            }

            // write peaks
            using (StreamWriter file = new(@"C:\Users\iruiz\Downloads\GUI\compare\peaks_all.csv"))
            {
                file.WriteLine("time,area");
                foreach(IPeak p in peaks)
                {
                    file.WriteLine(p.GetMZ().ToString() + "," + p.GetIntensity().ToString());
                }
            }

            // picking
            List<IPeak> processedPeaks = new List<IPeak>();

            int index = 1;
            int end = peaks.Count - 1;
            int head = index + 1;
            while (index < end)
            {
                if (peaks[index - 1].GetIntensity() < peaks[index].GetIntensity())
                {
                    head = index + 1;
                }

                while (head < end
                    && peaks[head].GetIntensity() == peaks[index].GetIntensity())
                {
                    head++;
                }

                if (peaks[head].GetIntensity() < peaks[index].GetIntensity())
                {
                    processedPeaks.Add(peaks[index]);
                    index = head;
                }
                index++;
            }

            // signal to noise
            double noise = Percentile(processedPeaks.Select(p => p.GetIntensity()), 0.5);
            //double noise = processedPeaks.Select(p => p.GetIntensity()).Average();
            processedPeaks = processedPeaks.Where(p => p.GetIntensity() > noise).ToList();

            // write peaks
            using (StreamWriter file = new(@"C:\Users\iruiz\Downloads\GUI\compare\peaks_picked.csv"))
            {
                file.WriteLine("time,area");
                foreach (IPeak p in processedPeaks)
                {
                    file.WriteLine(p.GetMZ().ToString() + "," + p.GetIntensity().ToString());
                }
            }

            Assert.Pass();
        }
    }
}
