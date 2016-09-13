using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public enum MaterialStressResult
    {
        None,
        Dent_Shear,
        Cut_Shear,
        CutThrough_Shear,
        Dent_Impact,
        InitiateFracture_Impact,
        CompleteFracture_Impact
    }



    public interface IMaterialStrikeResult
    {
        StressMode StressMode { get; set; }

        int ContactArea { get; }

        double Momentum { get; }
        double MomentumThreshold { get; }

        double ExcessMomentum { get; }
        bool BreaksThrough { get; }

        MaterialStressResult StressResult { get; }
        double ResultMomentum { get; }
        bool IsDefeated { get; }
        double ShearDentCost { get; }
        double ShearCutCost { get; }
        double ShearCutThroughCost { get; }
        double ImpactDentCost { get; }
        double ImpactInitiateFractureCost { get; }
        double ImpactCompleteFractureCost { get; }
    }

    public class MaterialStrikeResult : IMaterialStrikeResult
    {
        public StressMode StressMode { get; set; }

        public int ContactArea { get; set; }
        public double Momentum { get; set; }
        public double MomentumThreshold { get; set; }

        public double ExcessMomentum { get { return Momentum - (MomentumThreshold); } }
        public bool BreaksThrough { get { return Momentum >= MomentumThreshold; } }

        public MaterialStressResult StressResult { get; set; }
        public double ResultMomentum { get; set; }
        public bool IsDefeated { get; set; }
        public double ShearDentCost { get; set; }
        public double ShearCutCost { get; set; }
        public double ShearCutThroughCost { get; set; }
        public double ImpactDentCost { get; set; }
        public double ImpactInitiateFractureCost { get; set; }
        public double ImpactCompleteFractureCost { get; set; }
    }
}
