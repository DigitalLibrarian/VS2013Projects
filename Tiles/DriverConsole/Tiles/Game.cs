﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Control;
using Tiles.Render;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Random;
using Tiles.Agents.Combat.CombatEvolutions;

namespace Tiles
{
    public class Game : IGame
    {
        public IAtlas Atlas { get; private set; }
        public ICamera Camera { get; private set; }
        public IActionLog ActionLog { get; private set; }
        public IPlayer Player { get; private set; }
        public IAttackConductor AttackConductor { get; private set;}
        public IRandom Random { get; private set; }

        public ITile CameraTile { get { return Atlas.GetTileAtPos(Camera.Pos); } }

        public Game(IAtlas atlas, IPlayer player, ICamera camera, IActionLog log, IRandom random)
        {
            Atlas = atlas;
            Player = player;
            Camera = camera;
            ActionLog = log;
            Random = random;


            var reporter = new ActionReporter(log);
            var damageCalc = new DamageCalc();
            var reaper = new AgentReaper(Atlas);
            var evolutions = new List<ICombatEvolution>{
                new CombatEvolution_MartialArtsStrike(reporter, damageCalc, reaper),
                new CombatEvolution_StartHold(reporter, damageCalc, reaper),
                new CombatEvolution_ReleaseHold(reporter, damageCalc, reaper),
                new CombatEvolution_BreakHold(reporter, damageCalc, reaper)
            };
            AttackConductor = new AttackConductor(evolutions);
                 

            Atlas.GetTileAtPos(player.Agent.Pos).SetAgent(player.Agent);
        }


        public long DesiredFrameLength { get; set; }
    }
}
