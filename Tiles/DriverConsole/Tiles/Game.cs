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
using Tiles.Bodies.Health.Injuries;
using Tiles.Ecs;
using Tiles.EntitySystems;

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

            var injuryFactory = new InjuryFactory();
            /*
            var injuryCalc = new InjuryCalc(
                new InjuryResultBuilderFactory(injuryFactory),
                new DamageResistorFactory());
             * */

            var injuryCalc = new BetterInjuryCalc(injuryFactory);
            var reporter = new ActionReporter(log);
            var damageCalc = new DamageCalc();
            var reaper = new AgentReaper(Atlas, reporter, new ItemFactory());
            var evolutions = new List<ICombatEvolution>{
                new CombatEvolution_MartialArtsStrike(injuryCalc, reporter, damageCalc, reaper),
                new CombatEvolution_StartHold(reporter, damageCalc, reaper),
                new CombatEvolution_ReleaseHold(reporter, damageCalc, reaper),
                new CombatEvolution_BreakHold(reporter, damageCalc, reaper)
            };
            AttackConductor = new AttackConductor(evolutions);
            
            Atlas.GetTileAtPos(player.Agent.Pos).SetAgent(player.Agent);

            Systems = new List<AtlasBoxSystem>
            {
                new CommandSystem()
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
