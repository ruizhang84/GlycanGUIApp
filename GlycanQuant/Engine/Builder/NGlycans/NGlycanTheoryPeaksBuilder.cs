using GlycanQuant.Model.Builder.NGlycans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Model.Builder.NGlycans
{
    public class NGlycanTheoryPeaksBuilder : ITheoryPeaksBuilder
    {
        int HexNAc;
        int Hex;
        int Fuc;
        int NeuAc;
        int NeuGc;
        bool complex = true;
        bool hybrid = false;
        bool highMannose = false;
        bool permethylated = true;

        public NGlycanTheoryPeaksBuilder(bool permethylated = true, 
            int HexNAc=12, int Hex=12, int Fuc=5, int NeuAc=4, int NeuGc=0, 
            bool complex=true, bool hybrid=false, bool highMannose=false)
        {
            this.HexNAc = HexNAc;
            this.Hex = Hex;
            this.Fuc = Fuc;
            this.NeuAc = NeuAc;
            this.NeuGc = NeuGc;
            this.permethylated = permethylated;
            this.complex = complex;
            this.hybrid = hybrid;
            this.highMannose = highMannose;
        }

        public void SetPermethylated(bool permethylated)
        {
            this.permethylated = permethylated;
        }

        public void SetBuildType(bool complex, bool hybrid, bool highMannose)
        {
            this.complex = complex;
            this.hybrid = hybrid;
            this.highMannose = highMannose;
        }

        public List<IGlycanPeak> Build()
        {
            Dictionary<string, IGlycanPeak> res = new Dictionary<string, IGlycanPeak>();
            if (complex)
                Add(ref res, Build(Complex));
            if (hybrid)
                Add(ref res, Build(Hybrid));
            if (highMannose)
                Add(ref res, Build(HighMannose));

            return res.Select(r => r.Value).ToList();
        }

        private void Add(ref Dictionary<string, IGlycanPeak> res, 
            Dictionary<string, NGlycanPeak> found)
        {
            foreach(string key in found.Keys)
            {
                res[key] = found[key];
            }
        }

        private Dictionary<string, NGlycanPeak> Build(Func<int, List<NGlycanPeak>> f)
        {
            Dictionary<string, NGlycanPeak> res = new Dictionary<string, NGlycanPeak>();

            int level = 0;
            while (true)
            {
                List<NGlycanPeak> built = f(level++);
                if (built.Count == 0)
                    break;
                foreach(NGlycanPeak pk in built)
                {
                    res[pk.GetGlycan().Name()] = pk;
                }
            }
            return res;
        }

        private List<NGlycanPeak> Complex(int level = 0)
        {
            List<NGlycanPeak> res = new List<NGlycanPeak>();
            int hexNAc = 2 + 4 * level;
            int hex = 3 + 4 * level;

            //HexNAc
            for (int a = 0; a <= 4; a++)
            {
                // Hex
                for (int b = 0; b <= a; b++)
                {
                    //Fuc
                    for (int c = 0; c <= Math.Min(Fuc, a); c++)
                    {
                        // NeuAc 
                        for(int d = 0; d <= Math.Min(NeuAc, b); d++)
                        {
                            if (Valid(hexNAc + a, hex + b, c, d, 0))
                            {
                                NGlycanPeak temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b, c, d, 0);
                                res.Add(temp);
                                if (Valid(hexNAc + a, hex + b, c + 1, d, 0))
                                {
                                    temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b, c + 1, d, 0);
                                    res.Add(temp);
                                }
                            }
                            
                        }

                        // NeuGc
                        for (int e = 0; e <= Math.Min(NeuGc, b); e++)
                        {
                            if (Valid(hexNAc + a, hex + b, c, 0, e))
                            {
                                NGlycanPeak temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b, c, 0, e);
                                res.Add(temp);
                                if (Valid(hexNAc + a, hex + b, c + 1, 0, e))
                                {
                                    temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b, c + 1, 0, e);
                                    res.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        private List<NGlycanPeak> Hybrid(int level = 0)
        {
            List<NGlycanPeak> res = new List<NGlycanPeak>();
            int hexNAc = 2 + 2 * level;
            int hex = 3 + 2 * level;

            //HexNAc
            for (int a = 0; a <= 2; a++)
            {
                // Hex
                for (int b = 0; b <= a; b++)
                {
                    // mannose
                    for (int man = 0; man <= Hex - hex; man++)
                    {
                        //Fuc
                        for (int c = 0; c <= Math.Min(Fuc, a); c++)
                        {
                            // NeuAc 
                            for (int d = 0; d <= Math.Min(NeuAc, b); d++)
                            {

                                if (Valid(hexNAc + a, hex + b + man, c, d, 0))
                                {
                                    NGlycanPeak temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b + man, c, d, 0);
                                    res.Add(temp);
                                    if (Valid(hexNAc + a, hex + b + man, c + 1, d, 0))
                                    {
                                        temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b + man, c + 1, d, 0);
                                        res.Add(temp);
                                    }
                                }

                            }

                            // NeuGc
                            for (int e = 0; e <= Math.Min(NeuGc, b); e++)
                            {
                                if (Valid(hexNAc + a, hex + b + man, c, 0, e))
                                {
                                    NGlycanPeak temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b + man, c, 0, e);
                                    res.Add(temp);
                                    if (Valid(hexNAc + a, hex + b + man, c + 1, 0, e))
                                    {
                                        temp = new NGlycanPeak(permethylated, hexNAc + a, hex + b + man, c + 1, 0, e);
                                        res.Add(temp);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return res;
        }

        private List<NGlycanPeak> HighMannose(int level = 0)
        {
            List<NGlycanPeak> res = new List<NGlycanPeak>();
            int hexNAc = 2 + 2 * level;
            int hex = 3 + 2 * level;

            // mannose
            for (int man = 0; man <= Hex - hex; man++)
            {
                NGlycanPeak temp = new NGlycanPeak(permethylated, hexNAc, hex + man, 0, 0, 0);
                res.Add(temp);
            }
               
            return res;
        }

        private bool Valid(int HexNAc, int Hex, int Fuc, int NeuAc, int NeuGc)
        {
            return HexNAc <= this.HexNAc && Hex <= this.Hex && 
                Fuc <= this.Fuc && NeuAc <= this.NeuAc && NeuGc <= this.NeuGc;
        }


    }
}
