using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class DefaultAgentCommandInterpreter : AgentCommandInterpreter
    {
        public DefaultAgentCommandInterpreter()
            : base(
                new Dictionary<AgentCommandType, IAgentCommandTypeInterpreter>
        {
            {AgentCommandType.None, new AgentCommandTypeInterpreter_None()},
            {AgentCommandType.Move, new AgentCommandTypeInterpreter_Move()},
            {AgentCommandType.Dodge, new AgentCommandTypeInterpreter_Dodge()},
            {AgentCommandType.AttackMeleePrep, new AgentCommandTypeInterpreter_AttackMeleePrep()},
            {AgentCommandType.AttackMelee, new AgentCommandTypeInterpreter_AttackMelee()},
            {AgentCommandType.AttackMeleeRecovery, new AgentCommandTypeInterpreter_AttackMeleeRecovery()},
            {AgentCommandType.PickUpItemOnAgentTile, new AgentCommandTypeInterpreter_PickUpItemsOnAgentTile()},
            {AgentCommandType.WieldWeapon, new AgentCommandTypeInterpreter_WieldWeapon()},
            {AgentCommandType.UnwieldWeapon, new AgentCommandTypeInterpreter_UnwieldWeapon()},
            {AgentCommandType.WearArmor, new AgentCommandTypeInterpreter_WearArmor()},
            {AgentCommandType.TakeOffArmor, new AgentCommandTypeInterpreter_TakeOffArmor()},
            {AgentCommandType.DropInventoryItem, new AgentCommandTypeInterpreter_DropInventoryItem()},
            {AgentCommandType.StandUp, new AgentCommandTypeInterpreter_StandUp()},
            {AgentCommandType.LayDown, new AgentCommandTypeInterpreter_LayDown()}
        }) { }
    }
}
