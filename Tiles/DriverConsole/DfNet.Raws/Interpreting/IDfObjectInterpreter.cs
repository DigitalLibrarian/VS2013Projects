using System;
using System.Collections.Generic;
namespace DfNet.Raws.Interpreting
{
    public interface IDfObjectInterpreter
    {
        void Interpret(IDfObjectStore store, IDfObjectContext context, IList<DfTag> tags);
    }
}
