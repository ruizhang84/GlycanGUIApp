using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUI.Algorithm.GUISequencer
{
    public class DynamicProgrammingSequencer : IGUISequencer
    {
        private int offset; // GU stat at 2
        private int upto;   // GU up to 12

        public DynamicProgrammingSequencer(int offset=2, int upto=12)
        {
            this.offset = offset;
            this.upto = upto;
        }

        private int BackTracking(int idx, int maxIdx,
            List<List<GUI>> guis, double[,] dp, ref List<GUI> results)
        {
            
            if (dp[idx-1, maxIdx] == dp[idx, maxIdx])
            {
                    return maxIdx;
            }

            for(int i=0; i<guis[idx-1].Count; i++)
            {
                GUI g = guis[idx-1][i];
                for(int j=0; j<=maxIdx; j++)
                {
                    if(dp[idx-1, j] + g.Peak.GetIntensity() == dp[idx, maxIdx])
                    {
                        results.Add(g);
                        return j;
                    }
                }
            }
            return -1;
        }

        List<GUI> IGUISequencer.Choose(List<List<GUI>> guis)
        {
            List<GUI> results = new List<GUI>();
            if (guis.Count == 0)
                return results;

            double[,] dp = new double[guis.Count+1, upto - offset + 1];

            // init
            for (int i = 0; i < dp.GetLength(1); i++)
            {
                dp[0, i] = 0;
            }

            // extend
            for(int i = 1; i < guis.Count+1; i++)
            {
                for(int j = 0; j < dp.GetLength(1); j++)
                {
                    dp[i,j] = dp[i-1, j];   
                }

                foreach(GUI gui in guis[i - 1])
                {
                    double best = 0;
                    for (int j = 0; j < gui.Unit-offset+1; j++)
                    {
                        best = Math.Max(best, dp[i - 1, j]);
                    }
                    dp[i, gui.Unit-offset] = Math.Max(dp[i, gui.Unit - offset], 
                        best + gui.Peak.GetIntensity());
                }
            }

            // best
            double max = 0;
            int maxIdx = 0;
            for (int i = 0; i < dp.GetLength(1); i++)
            {
                if (max < dp[guis.Count, i])
                {
                    max = dp[guis.Count, i];
                    maxIdx = i;
                }
            }

            // trace back
            for (int i = dp.GetLength(0)-1; i>0; i--)
            {
                maxIdx = BackTracking(i, maxIdx, guis, dp, ref results);
            }

            return results;
        }
    }
}
