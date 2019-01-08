using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.EntityComponents;
using Tiles.Random;
using Tiles.Splatter;

namespace Tiles.EntitySystems
{
    public class AutonomicBodySystem : AtlasBoxSystem
    {
        private static readonly int HardThreshold = 250;
        private ISplatterFascade Splatter { get; set; }
        private IRandom Random { get; set; }
        private IActionReporter ActionReporter { get; set; }
        private IAgentReaper Reaper { get; set; }

        public AutonomicBodySystem(IRandom random, ISplatterFascade splatter, IActionReporter actionReporter, IAgentReaper reaper)
            : base(ComponentTypes.Body)
        {
            Random = random;
            Splatter = splatter;
            ActionReporter = actionReporter;
            Reaper = reaper;
        }

        protected override void UpdateEntity(Ecs.IEntityManager entityManager, Ecs.IEntity entity, IGame game)
        {
            var body = entity.GetComponent<BodyComponent>(ComponentTypes.Body).Body;
            var totalBleeding = body.TotalBleeding;
            var ratio = (double)totalBleeding / (body.Blood.Denominator * 0.05d);

            if (totalBleeding > HardThreshold || Random.NextDouble() < ratio)
            {
                var pos = entity.GetComponent<AtlasPositionComponent>(ComponentTypes.AtlasPosition).Position;
                Splatter.Register(pos, body.Class.BloodMaterial);
            }


            if (entity.HasComponent(ComponentTypes.Agent))
            {
                var agent = entity.GetComponent<AgentComponent>(ComponentTypes.Agent).Agent;
                var isDead = body.IsDead;
                body.Blood.Numerator = System.Math.Max(0, body.Blood.Numerator - totalBleeding);
                if (!isDead && body.IsDead)
                {
                    ActionReporter.ReportBledOut(agent);
                    Reaper.Reap(agent);
                }
            }
        }
    }
}
