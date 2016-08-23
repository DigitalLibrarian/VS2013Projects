using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public interface IDfObjectPipeline
    {
        DfObject Run(string creatureName);
        DfObject Run(string creatureName, string caste = null);
    }
}
