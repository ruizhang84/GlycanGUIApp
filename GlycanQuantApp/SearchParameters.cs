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
        public double retentionRange { get; set; } = 3;


        //file
        public List<string> MSMSFiles { get; set; } = new List<string>();

        SearchingParameters()
        {
        }

        public void Update()
        {
            Tolerance = ConfigureParameters.Access.Tolerance;
            ToleranceBy = ConfigureParameters.Access.ToleranceBy;
            retentionRange = ConfigureParameters.Access.retentionRange;
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
        public double retentionRange { get; set; } = 3;

        protected static readonly Lazy<ConfigureParameters>
            lazy = new Lazy<ConfigureParameters>(() => new ConfigureParameters());

        public static ConfigureParameters Access { get { return lazy.Value; } }

        private ConfigureParameters() { }

    }
}
