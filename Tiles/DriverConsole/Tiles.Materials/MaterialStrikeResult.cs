using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class MaterialStrikeResult
    {
        public StressMode StressMode { get; set; }

        public double ContactArea { get; set; }
        public double ContactAreaRatio { get; set; }
        public double Momentum { get; set; }
        public double Stress { get; set; }

        public double PenetrationRatio { get; set; }
        public double PreDentRatio { get; set; }

        public StressResult StressResult { get; set; }
        public double ResultMomentum { get; set; }
        public bool IsDefeated { get; set; }
        public bool IsBluntCrack { get; set; }

        public int ImplementMaxPenetration { get; set; }
        public int ImplementRemainingPenetration { get; set; }
        public bool ImplementWasSmall { get; set; }
        public double ImplementSize { get; set; }

        public double ImplementContactArea { get; set; }

    }
}
