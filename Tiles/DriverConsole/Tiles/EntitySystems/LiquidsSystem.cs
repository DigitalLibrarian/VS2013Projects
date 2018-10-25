using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.EntitySystems
{
    public class LiquidsSystem : AtlasBoxSystem
    {
        static readonly int MaxDepth = 7;

        IRandom Random { get; set; }
        public LiquidsSystem(IRandom random)
            : base(ComponentTypes.LiquidTileNode, 
                    ComponentTypes.AtlasPosition)
        {
            Random = random;
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

        static readonly Vector3[] NeighborOffsets = {
                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(-1, -1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, -1, 0),
                };
        protected override void UpdateEntity(Ecs.IEntityManager entityManager, Ecs.IEntity entity, IGame game)
        {
            var ltc = entity.GetComponent<LiquidTileNodeComponent>();
            var site = ltc.Site;
            var tile = ltc.Tile;

            var worldPos = site.Box.Min + tile.Index;
            var nextPos = worldPos + new Vector3(0, 0, -1);
            var takerTile = game.Atlas.GetTileAtPos(nextPos, false);
            if (takerTile != null && takerTile.IsTerrainPassable && takerTile.LiquidDepth < MaxDepth) // can fall?
            {
                var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                FlowDown(entityManager, entity, ltc, takerSite, takerTile);
            }
            else if(tile.LiquidDepth > 1)
            {
                nextPos = worldPos + Random.NextElement<Vector3>(NeighborOffsets);
                takerTile = game.Atlas.GetTileAtPos(nextPos, false);
                if (takerTile != null
                    && takerTile.IsTerrainPassable
                    && takerTile.LiquidDepth < tile.LiquidDepth) // can flow sideways?
                {
                    var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                    FlowInto(entityManager, entity, ltc, takerSite, takerTile);
                }
            }
        }

        IEntity CreateLiquidsNode(IEntityManager entityManager, ISite site, ITile tile)
        {
            var lte = entityManager.CreateEntity();
            var ltc = new LiquidTileNodeComponent
            {
                Site = site,
                Tile = tile
            };
            lte.AddComponent(ltc);

            // Always start with ltc, so we can just change the tile pointer when the 
            // water moves in the simple case
            lte.AddComponent(new AtlasPositionComponent
            {
                PositionFunc = () => ltc.Site.Box.Min + ltc.Tile.Index
            });

            return lte;
        }

        void FlowDown(IEntityManager entityManager, IEntity entity, LiquidTileNodeComponent l, ISite nextSite, ITile nextTile)
        {
            var room = MaxDepth - nextTile.LiquidDepth;
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
                    // can take all and there is an entity.  delete this entity
                    entityManager.DeleteEntity(entity.Id);
                }

                nextTile.LiquidDepth += amount; // make the hand off
            }
            else
            {
                // only partial room, so both entities remain
                l.Tile.LiquidDepth = l.Tile.LiquidDepth - room;
                nextTile.LiquidDepth = nextTile.LiquidDepth + room;
            }
        }

        void FlowInto(IEntityManager entityManager, IEntity entity, LiquidTileNodeComponent l, ISite nextSite, ITile nextTile)
        {
            var diff = l.Tile.LiquidDepth - nextTile.LiquidDepth;
            var flow = diff / 2;
            // if there was round-off, bump up by one.  
            if (l.Tile.LiquidDepth > 1 && (double)diff / 2d > flow) 
                flow++;

            if (nextTile.LiquidDepth == 0) 
                CreateLiquidsNode(entityManager, nextSite, nextTile);

            l.Tile.LiquidDepth -= flow;
            nextTile.LiquidDepth += flow;

            if (l.Tile.LiquidDepth == 0)
            {
                entityManager.DeleteEntity(entity.Id);
            }
        }
    }
}
