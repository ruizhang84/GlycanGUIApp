using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlycanQuant.Engine.Search
{
    public class SelectResult
    {
        public IResult Present { get; set; }
        public List<IResult> Results { get; set; }
        public SelectResult(IResult present, List<IResult> results)
        {
            Present = present;
            Results = results;
        }
    }

    public interface IResultSelect
    {
        IResult Select(List<IResult> results);
        void Add(List<IResult> results);
        Dictionary<string, List<SelectResult>> Produce();
    }
}
