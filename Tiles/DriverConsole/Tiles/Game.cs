using System;
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
        public long DesiredFrameLength { get; set; }

        public Game(IAtlas atlas, IPlayer player, ICamera camera, IActionLog log, IRandom random)
        {
            Atlas = atlas;
            Player = player;
            Camera = camera;
            ActionLog = log;
            Random = random;

            var injuryFactory = new InjuryFactory();
            var injuryCalc = new InjuryCalc(
                new InjuryResultBuilderFactory(injuryFactory),
                new DamageResistorFactory());
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
        }

        public void UpdateBox(Box3 box)
        {
            var updatedAgents = new List<IAgent>();

            // TODO - limit this to a "working set" of sites
            foreach (var tile in Atlas.GetTiles(box).ToList())
            {
                if (tile.HasAgent && !updatedAgents.Contains(tile.Agent))
                {
                    var cTile = Atlas.GetTileAtPos(tile.Agent.Pos);
                    updatedAgents.Add(tile.Agent);
                    tile.Agent.Update(this);
                }
            }
        }
    }
}
