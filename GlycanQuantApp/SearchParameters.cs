using GlycanQuant.Engine.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuantApp
{
    public class SearchingParameters
    {
        //spectrum
        public double Tolerance { get; set; } = 10;
        public ToleranceBy ToleranceBy { get; set; } = ToleranceBy.PPM;
        // retention
        public double Threshold { get; set; } = 0.5;
        public double PeakCutoff { get; set; } = 0.5;
        // parameter
        public int MaxCharage { get; set; } = 3;
        public double Cutoff { get; set; } = 0.9;
        public bool Hydrogen { get; set; } = true;
        public bool Sodium { get; set; } = false;
        public bool Ammonium { get; set; } = false;

        //file
        public List<string> MSMSFiles { get; set; } = new List<string>();

        SearchingParameters()
        {
        }

        public void Update()
        {
            Tolerance = ConfigureParameters.Access.Tolerance;
            ToleranceBy = ConfigureParameters.Access.ToleranceBy;
            Threshold = ConfigureParameters.Access.Threshold;
            PeakCutoff = ConfigureParameters.Access.PeakCutoff;
            MaxCharage = ConfigureParameters.Access.MaxCharage;
            Cutoff = ConfigureParameters.Access.Cutoff;
            Hydrogen = ConfigureParameters.Access.Hydrogen;
            Sodium = ConfigureParameters.Access.Sodium;
            Ammonium = ConfigureParameters.Access.Ammonium;
        }

        protected static readonly Lazy<SearchingParameters>
            lazy = new Lazy<SearchingParameters>(() => new SearchingParameters());

        public static SearchingParameters Access { get { return lazy.Value; } }

    }

    public class ConfigureParameters
    {
        //spectrum
        public double Tolerance { get; set; } = 10;
        public ToleranceBy ToleranceBy { get; set; } = ToleranceBy.PPM;

        // retention
        public double Threshold { get; set; } = 0.5;
        public double PeakCutoff { get; set; } = 0.5;
        // parameter
        public int MaxCharage { get; set; } = 3;
        public double Cutoff { get; set; } = 0.9;
        public bool Hydrogen { get; set; } = true;
        public bool Sodium { get; set; } = false;
        public bool Ammonium { get; set; } = false;

        protected static readonly Lazy<ConfigureParameters>
            lazy = new Lazy<ConfigureParameters>(() => new ConfigureParameters());

        public static ConfigureParameters Access { get { return lazy.Value; } }

        private ConfigureParameters() { }

    }
}
