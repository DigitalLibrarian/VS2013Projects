using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Bodies
{
    public interface ITissueLayerWound
    {
        Fraction DentFraction { get; }
        Fraction CutFraction { get; }

        double AreaPercent { get; }

        bool IsPulped();
    }
}
