using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public interface IDfObjectStore
    {
        DfObject Get(string type, string name);
        IEnumerable<DfObject> Get(string type);

        void Add(DfObject o);
    }
}
