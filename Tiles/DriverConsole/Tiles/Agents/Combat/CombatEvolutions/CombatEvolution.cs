﻿using System;
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
        protected IDamageCalc DamageCalc { get; set; }
        IAgentReaper Reaper { get; set; }
        public CombatEvolution(IActionReporter reporter, IDamageCalc damageCalc, IAgentReaper reaper)
        {
            Reporter = reporter;
            DamageCalc = damageCalc;
            Reaper = reaper;
        }

        protected abstract bool Should(ICombatMoveContext session);
        protected abstract void Run(ICombatMoveContext session);

        public bool Evolve(ICombatMoveContext session)
        {
            var move = session.Move;
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