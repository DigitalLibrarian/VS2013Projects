using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfAgentBuilderFactory
    {
        IDfAgentBuilder Create();
    }
}
