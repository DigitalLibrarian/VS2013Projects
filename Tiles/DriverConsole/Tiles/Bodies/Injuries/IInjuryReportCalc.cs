using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.Bodies.Injuries
{
    public interface IInjuryReportCalc
    {
        IInjuryReport CalculateMaterialStrike(
            IEnumerable<IItem> armorItems, StressMode stressMode,
            double momentum, double contactArea, double maxPenetration,
            IBody targetBody, IBodyPart targetPart, IMaterial strikerMat, double sharpness, bool implementWasSmall);
    }
}
