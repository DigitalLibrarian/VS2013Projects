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
        IEnumerable<IAgentCommand> Nothing(IAgent agent);
        IEnumerable<IAgentCommand> MeleeAttack(IAgent agent, IAgent target, ICombatMove attackMove);
        IEnumerable<IAgentCommand> WieldWeapon(IAgent agent, IItem item);
        IEnumerable<IAgentCommand> WearArmor(IAgent agent, IItem item);
        IEnumerable<IAgentCommand> PickUpItemsOnAgentTile(IAgent agent);
        IEnumerable<IAgentCommand> MoveDirection(IAgent agent, Vector3 direction);

        IEnumerable<IAgentCommand> UnwieldWeapon(IAgent agent, IItem item);
        IEnumerable<IAgentCommand> TakeOffArmor(IAgent agent, IItem item);
        IEnumerable<IAgentCommand> DropInventoryItem(IAgent agent, IItem item);
    }
}
