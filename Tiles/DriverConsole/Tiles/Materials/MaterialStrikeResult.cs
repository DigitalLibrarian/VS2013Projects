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

        int ContactArea { get; }

        double Momentum { get; }
        double MomentumThreshold { get; }

        double ExcessMomentum { get; }
        bool BreaksThrough { get; }
    }

    public class MaterialStrikeResult : IMaterialStrikeResult
    {
        public StressMode StressMode { get; set; }

        public int ContactArea { get; set; }
        public double Momentum { get; set; }
        public double MomentumThreshold { get; set; }

        public double ExcessMomentum { get { return Momentum - (MomentumThreshold); } }
        public bool BreaksThrough { get { return Momentum >= MomentumThreshold; } }
    }
}
