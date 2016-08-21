using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public interface IContextApplicator
    {
        void Apply(IDfObjectStore store, IDfObjectContext context);
    }
}
