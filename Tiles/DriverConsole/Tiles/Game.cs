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
using Tiles.Items;
using Tiles.Ecs;
using Tiles.EntitySystems;
using Tiles.Materials;
using Tiles.Bodies.Injuries;
using Tiles.Bodies.Wounds;
using Tiles.Splatter;

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
        public IEntityManager EntityManager { get; private set; }
        public ISplatterFascade Splatter { get; private set; }
        
        public ITile CameraTile { get { return Atlas.GetTileAtPos(Camera.Pos); } }
        public long DesiredFrameLength { get; set; }

        List<AtlasBoxSystem> Systems { get; set; }

        public Game(
            IEntityManager entityManager,
            IAtlas atlas, IPlayer player, ICamera camera, IActionLog log, IRandom random)
        {
            EntityManager = entityManager;
            Atlas = atlas;
            Player = player;
            Camera = camera;
            ActionLog = log;
            Random = random;

            var injuryCalc = new InjuryReportCalc(Random, new InjuryFactory(Random));
            var reporter = new ActionReporter(log);
            var reaper = new AgentReaper(EntityManager, Atlas, reporter, new ItemFactory());
            var woundFactory = new BodyPartWoundFactory();
            Splatter = new SplatterFascade(Random, Atlas);
            var evolutions = new List<ICombatEvolution>{
                new CombatEvolution_MartialArtsStrike(Atlas, injuryCalc, reporter, reaper, woundFactory, Splatter),
                new CombatEvolution_StartHold(reporter, reaper),
                new CombatEvolution_ReleaseHold(reporter, reaper),
                new CombatEvolution_BreakHold(reporter, reaper)
            };
            AttackConductor = new AttackConductor(evolutions);
            
            Atlas.GetTileAtPos(player.Agent.Pos).SetAgent(player.Agent);

            Systems = new List<AtlasBoxSystem>
            {
                new AutonomicSystem(Random, Splatter, reporter, reaper),
                new CommandSystem(),
                new LiquidsSystem(Random)
            };
        }

        public void UpdateBox(Box3 box)
        {
            foreach (var system in Systems)
            {
                system.SetBox(box);
                system.Update(EntityManager, this);
            }
        }
    }
}
