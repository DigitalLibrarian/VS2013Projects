using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.EntitySystems;
using Tiles.Math;

namespace Tiles.Liquids
{
    public class LiquidsSystem : AtlasBoxSystem
    {
        public LiquidsSystem()
            : base(ComponentTypes.LiquidTileNode, 
                    ComponentTypes.AtlasPosition)
        {
        }

        IEnumerable<Vector3> Line(Box3 box, Vector3 index, Vector3 delta)
        {
            if (delta.Length() > 0)
            {
                while (box.Contains(index))
                {
                    yield return index;
                    index += delta;
                }
            }
        }

        protected override void UpdateEntity(Ecs.IEntityManager entityManager, Ecs.IEntity entity, IGame game)
        {
            var ltc = entity.GetComponent<LiquidTileNodeComponent>();
            var site = ltc.Site;
            var tile = ltc.Tile;

            if (tile.LiquidDepth == 0)
            {
                entityManager.DeleteEntity(entity.Id);
                return;
            }

            var index = site.Box.Min + tile.Index;
            var down = new Vector3(0, 0, -1);
            var nextPos = index + down;
            var takerTile = game.Atlas.GetTileAtPos(nextPos, false);
            if (takerTile != null)
            {
                if (takerTile.IsTerrainPassable && !takerTile.IsLiquidFull)
                {
                    var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                    FlowInto(entityManager, entity, ltc, takerSite, takerTile);
                }
            }
        }

        void FlowInto(IEntityManager entityManager, IEntity entity, LiquidTileNodeComponent l, ISite nextSite, ITile nextTile)
        {
            var room = 7 - nextTile.LiquidDepth;
            var amount = l.Tile.LiquidDepth;
            if (room >= amount)
            {
                l.Tile.LiquidDepth = 0;
                if (nextTile.LiquidDepth == 0)
                {
                    // can take all and there is no entity.  re-use existing entity
                    l.Site = nextSite;
                    l.Tile = nextTile;
                }
                else
                {
                    // can take all and there is an entity.  give liquid and delete this entity
                    var lte = entityManager.CreateEntity();
                    var ltc = new LiquidTileNodeComponent
                    {
                        Site = nextSite,
                        Tile = nextTile
                    };
                    lte.AddComponent(ltc);

                    // Always start with ltc, so we can just change the tile pointer when the 
                    // water moves in the simple case
                    lte.AddComponent(new AtlasPositionComponent
                    {
                        PositionFunc = () => ltc.Site.Box.Min + ltc.Tile.Index
                    });

                    entityManager.DeleteEntity(entity.Id);
                }

                nextTile.LiquidDepth += amount;
            }
            else
            {
                // only partial room
                l.Tile.LiquidDepth = l.Tile.LiquidDepth - room;
                nextTile.LiquidDepth = nextTile.LiquidDepth + room;
            }
        }
    }
}
