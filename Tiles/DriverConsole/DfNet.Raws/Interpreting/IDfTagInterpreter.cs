using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public interface IDfTagInterpreter
    {
        string TagName { get; }

        void Run(IDfObjectStore store, IDfObjectContext context, DfTag tag, IList<DfTag> tags);
    }
}
