using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents
{
    public interface IAgentCommandFactory
    {
        IAgentCommand Nothing(IAgent agent);
        IAgentCommand MeleeAttack(IAgent agent, IAgent target, IAttackMove attackMove);
        IAgentCommand WieldWeapon(IAgent agent, IItem item, IWeapon weapon);
        IAgentCommand WearArmor(IAgent agent, IItem item, IArmor armor);
        IAgentCommand PickUpItemsOnAgentTile(IAgent agent);
        IAgentCommand MoveDirection(IAgent agent, Vector2 direction);

        IAgentCommand UnwieldWeapon(IAgent agent, IItem item, IWeapon weapon);
        IAgentCommand TakeOffArmor(IAgent agent, IItem item, IArmor armor);
        IAgentCommand DropInventoryItem(IAgent agent, IItem item);

    }
}
