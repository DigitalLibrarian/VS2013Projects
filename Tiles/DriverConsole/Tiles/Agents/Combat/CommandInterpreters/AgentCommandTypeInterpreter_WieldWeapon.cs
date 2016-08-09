﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_WieldWeapon : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.WieldWeapon; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand)
        {
            var slot = agentCommand.Weapon.WeaponClass.WeaponSlot;
            agent.EquipmentSlots.Fill(slot, agentCommand.Weapon);
            agent.Inventory.AddToWorn(agentCommand.Weapon, agentCommand.Item);
            game.ActionLog.AddLine(string.Format("The {0} starts wielding the {1}.", agent.Name, agentCommand.Item.Name));
        }
    }
}
