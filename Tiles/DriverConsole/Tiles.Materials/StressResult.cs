using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public enum StressResult
    {
        None,
        Shear_Dent,
        Shear_Cut,
        Shear_CutThrough,
        Impact_Dent,
        Impact_InitiateFracture,
        Impact_CompleteFracture,

        Impact_Bypass
    }
}
