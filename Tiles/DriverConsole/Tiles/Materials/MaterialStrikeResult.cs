using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterialStrikeResult
    {
        StressMode StressMode { get; }

        double Momentum { get; }
        double MomentumThreshold { get; }

        bool BreaksThrough { get; }
    }

    public class MaterialStrikeResult : IMaterialStrikeResult
    {
        public StressMode StressMode { get; set; }

        public double Momentum { get; set; }
        public double MomentumThreshold { get; set; }

        public bool BreaksThrough { get { return Momentum >= MomentumThreshold; } }
    }
}
