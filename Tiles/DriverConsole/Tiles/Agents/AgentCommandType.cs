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
        AttackMelee,
        WieldWeapon,
        UnwieldWeapon,
        WearArmor,
        TakeOffArmor,
        DropInventoryItem
    }
}
