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

        protected AtlasBoxSystem(params int[] componentIds)
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
            var updatedEntities = new List<IEntity>();
            if (Box.HasValue)
            {
                foreach (var entity in entityManager.GetEntities(ComponentIds).ToList())
                {
                    var pos = entity.GetComponent<AtlasPositionComponent>(ComponentTypes.AtlasPosition)
                                .Position;

                    if (Box.Value.Contains(pos) && !updatedEntities.Contains(entity))
                    {
                        UpdateEntity(entityManager, entity, game);
                        updatedEntities.Add(entity);
                    }
                }
            }
        }
    }
}
