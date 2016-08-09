﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.Combat.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_None : IAgentCommandTypeInterpreter
    {
        public AgentCommandType CommandType { get { return AgentCommandType.None; } }

        public void Execute(IGame game, IAgent agent, IAgentCommand agentCommand) { }
    }
}
