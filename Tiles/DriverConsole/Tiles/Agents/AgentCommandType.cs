using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents
{
    public enum AgentCommandType
    {
        None,
        PickUpItemOnAgentTile,
        Move,
        AttackMeleePrep,
        AttackMelee,
        AttackMeleeRecovery,
        WieldWeapon,
        UnwieldWeapon,
        WearArmor,
        TakeOffArmor,
        DropInventoryItem,
        StandUp,
        LayDown
    }
}
