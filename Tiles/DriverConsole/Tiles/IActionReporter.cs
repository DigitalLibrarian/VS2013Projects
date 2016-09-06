using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles
{
    public interface IActionReporter
    {
        void ReportGrabStartBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportGrabMiss(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportGrabReleaseBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportMeleeItemStrikeBodyPart(ICombatMoveContext session, IVerb verb, IItem item, IBodyPart bodyPart, bool targetPartWasShed);
        void ReportMeleeStrikeBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart bodyPart, bool targetPartWasShed);


        void ReportDeath(Agents.IAgent agent);
    }
}
