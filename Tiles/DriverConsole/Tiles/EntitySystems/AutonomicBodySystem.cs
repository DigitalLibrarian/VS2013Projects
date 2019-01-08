using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.EntityComponents;
using Tiles.Random;
using Tiles.Splatter;

namespace Tiles.EntitySystems
{
    public class AutonomicBodySystem : AtlasBoxSystem
    {
        private static readonly int HardThreshold = 150;
        private ISplatterFascade Splatter { get; set; }
        private IRandom Random { get; set; }

        public AutonomicBodySystem(IRandom random, ISplatterFascade splatter)
            : base(ComponentTypes.Body)
        {
            Random = random;
            Splatter = splatter;
        }

        protected override void UpdateEntity(Ecs.IEntityManager entityManager, Ecs.IEntity entity, IGame game)
        {
            var body = entity.GetComponent<BodyComponent>(ComponentTypes.Body).Body;
            var totalBleeding = body.TotalBleeding;
            var ratio = (double)totalBleeding / (body.Blood.Denominator * 0.01d);

            if (totalBleeding > HardThreshold || Random.NextDouble() < ratio)
            {
                var pos = entity.GetComponent<AtlasPositionComponent>(ComponentTypes.AtlasPosition).Position;
                Splatter.Register(pos, body.Class.BloodMaterial);
            }
        }
    }
}
