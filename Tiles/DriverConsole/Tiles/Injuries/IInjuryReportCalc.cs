using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Injuries
{
    public interface IInjuryReportCalc
    {
        IInjuryReport CalculateMaterialStrike(
            ICombatMoveContext context,
            StressMode stressMode,
            double momentum, double contactArea, int maxPenetration,
            IBodyPart targetPart,
            IMaterial strikerMat,
            double sharpness
            );
    }
}
