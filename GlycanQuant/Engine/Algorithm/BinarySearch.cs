using SpectrumData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycanQuant.Engine.Algorithm
{
    public class BinarySearch
    {
        private static int Compare(double curr, double target, double tol, ToleranceBy by)
        {
            if (by == ToleranceBy.PPM)
            {
                if (Math.Abs(curr - target) / target * 1000000.0 <= tol) return 0;
            }
            else
            {
                if (Math.Abs(curr - target) <= tol) return 0;
            }

            if (curr > target)
                return 1;
            return -1;
        }

        public static int Search(List<IPeak> data, double target, 
            double tol, ToleranceBy by)
        {
            int start = 0;
            int end = data.Count - 1;

            while (start <= end)
            {
                int mid = end + (start - end) / 2;
                int cmp = Compare(data[mid].GetMZ(), target, tol, by);
                if (cmp == 0)
                {
                    return mid;
                }
                else if (cmp > 0)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid + 1;
                }
            }

            return -1;
        }

        public static List<int> ExtendSearch(List<IPeak> data, double target,
            double tol, ToleranceBy by)
        {
            List<int> indexes = new List<int>();

            int index = Search(data, target, tol, by);
            if (index < 0) return indexes;

            int nextIndex = index;
            while (nextIndex >= 0)
            {
                if (Compare(data[nextIndex].GetMZ(), target, tol, by) == 0)
                {
                    indexes.Add(nextIndex);
                }
                else
                {
                    break;
                }
                nextIndex--;
            }
            nextIndex = index + 1;
            while (nextIndex < data.Count - 1)
            {
                if (Compare(data[nextIndex].GetMZ(), target, tol, by) == 0)
                {
                    indexes.Add(nextIndex);
                }
                else
                {
                    break;
                }
                nextIndex++;
            }
            return indexes;
        }

        public static int BestSearch(List<IPeak> data, double target,
           double tol, ToleranceBy by)
        {
            List<int> indexes = ExtendSearch(data, target, tol, by);
            if (indexes.Count == 0)
                return -1;

            double bestInt = 0;
            int bestIndex = -1;
            foreach(int index in indexes)
            {
                if (data[index].GetIntensity() > bestInt)
                {
                    bestInt = data[index].GetIntensity();
                    bestIndex = index;
                }
            }
            return bestIndex;
        }
    }
}
