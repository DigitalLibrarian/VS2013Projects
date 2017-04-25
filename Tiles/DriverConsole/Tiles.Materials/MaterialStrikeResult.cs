using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterialStrikeResult
    {
        StressMode StressMode { get; set; }
        StressResult StressResult { get; }

        double ContactArea { get; }
        double ContactAreaRatio { get; }

        double Stress { get; }
        double WoundArea { get; }
        double RemainingPenetration { get; }

        double Momentum { get; }
        double ResultMomentum { get; }

        bool IsDefeated { get; }

        double ShearDentCost { get; }
        double ShearCutCost { get; }
        double ShearCutThroughCost { get; }
        double ImpactDentCost { get; }
        double ImpactInitiateFractureCost { get; }
        double ImpactCompleteFractureCost { get; }

        double LayerThickness { get; }
        double Sharpness { get; }
    }

    public class MaterialStrikeResult : IMaterialStrikeResult
    {
        public StressMode StressMode { get; set; }

        public double ContactArea { get; set; }
        public double ContactAreaRatio { get; set; }
        public double Momentum { get; set; }
        public double Stress { get; set; }

        public StressResult StressResult { get; set; }
        public double ResultMomentum { get; set; }
        public bool IsDefeated { get; set; }
        public double ShearDentCost { get; set; }
        public double ShearCutCost { get; set; }
        public double ShearCutThroughCost { get; set; }
        public double ImpactDentCost { get; set; }
        public double ImpactInitiateFractureCost { get; set; }
        public double ImpactCompleteFractureCost { get; set; }

        public double WoundArea { get; set; }
        public double RemainingPenetration { get; set; }

        public double LayerThickness { get; set; }

        public double Sharpness { get; set; }
    }
}
