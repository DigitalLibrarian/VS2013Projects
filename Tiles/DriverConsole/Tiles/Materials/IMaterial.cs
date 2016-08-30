using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterial
    {
        string Name { get; }
        string Adjective { get; }

        int ImpactYield { get; }
        int ImpactFracture { get; }
        int ImpactStrainAtYield { get; }

        int ShearYield { get; }
        int ShearFracture { get; }
        int ShearStrainAtYield { get; }
    }
}
