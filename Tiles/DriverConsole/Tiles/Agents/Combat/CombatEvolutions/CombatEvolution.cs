using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public abstract class CombatEvolution : ICombatEvolution
    {
        protected IActionReporter Reporter { get; private set; }
        IAgentReaper Reaper { get; set; }
        public CombatEvolution(IActionReporter reporter, IAgentReaper reaper)
        {
            Reporter = reporter;
            Reaper = reaper;
        }

        protected abstract bool Should(ICombatMoveContext session);
        protected abstract void Run(ICombatMoveContext session);

        public bool Evolve(ICombatMoveContext session)
        {
            if (!Should(session)) return false;
            Run(session);
            return true;
        }

        #region Reaping
        protected void HandleDeath(IAgent attacker, IAgent defender, ICombatMove move)
        {
            Reaper.Reap(defender);
        }

        protected void HandleShedPart(IAgent attacker, IAgent defender, ICombatMove move, IBodyPart shedPart)
        {
            Reaper.Reap(defender, shedPart);
        }
        #endregion
    }    
}
