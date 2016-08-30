using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.Math;

namespace Tiles.EntitySystems
{
    public interface IAtlasBoxSystem : ISystem
    {
        void SetBox(Box3 box);
    }

    public abstract class AtlasBoxSystem : BaseSystem, IAtlasBoxSystem
    {
        Box3? Box { get; set; }

        public AtlasBoxSystem(params int[] componentIds)
            : base(
                componentIds
                    .ToList()
                    .Concat(new []{ComponentTypes.AtlasPosition})
                    .ToArray())
        {
            Box = null;
        }

        public void SetBox(Box3 box)
        {
            Box = box;
        }

        public override void Update(IEntityManager entityManager, IGame game)
        {
            if (Box.HasValue)
            {
                foreach (var entity in entityManager.GetEntities(ComponentIds))
                {
                    var pos = entity.GetComponent<IAtlasPositionComponent>()
                                .AtlasPosition
                                .Position;

                    if (Box.Value.Contains(pos))
                    {
                        UpdateEntity(entity, game);
                    }
                }
            }
        }
    }
}
