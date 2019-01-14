using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Agents.CommandInterpreters
{
    public class AgentCommandTypeInterpreter_AttackMeleeRecovery : AgentCommandTypeInterpreter_None
    {
        public override AgentCommandType CommandType
        {
            get
            {
                return AgentCommandType.AttackMeleeRecovery;
            }
        }
    }
}
