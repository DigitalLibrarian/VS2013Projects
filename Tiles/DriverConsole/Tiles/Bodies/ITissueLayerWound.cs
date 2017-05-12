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
        Fraction EffectFraction { get; }
        Fraction DentFraction { get; }
        Fraction CutFraction { get; }

        double WoundArea { get; }
        double WoundAreaRatio { get; }
        double PenetrationRatio { get; }

        bool IsPulped();
    }
}
