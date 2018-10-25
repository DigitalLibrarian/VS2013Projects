using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.EntitySystems;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.Liquids
{
    public class LiquidsSystem : AtlasBoxSystem
    {
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

        static readonly Vector3[] Neighbors = {
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

            if (tile.LiquidDepth == 0)
            {
                entityManager.DeleteEntity(entity.Id);
                return;
            }

            var worldPos = site.Box.Min + tile.Index;
            var down = new Vector3(0, 0, -1);
            var nextPos = worldPos + down;
            var takerTile = game.Atlas.GetTileAtPos(nextPos, false);
            if (takerTile != null && takerTile.IsTerrainPassable && !takerTile.IsLiquidFull)
            {
                var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                FlowDown(entityManager, entity, ltc, takerSite, takerTile);
            }
            else
            {
                int numTries = 0;
                int maxTries = 3;
                bool found = false;
                while(!found && numTries < maxTries)
                {
                    nextPos = worldPos + Random.NextElement<Vector3>(Neighbors);
                    takerTile = game.Atlas.GetTileAtPos(nextPos, false);
                    if (takerTile != null
                        && takerTile.IsTerrainPassable
                        && takerTile.LiquidDepth < tile.LiquidDepth)
                    {
                        var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                        FlowInto(entityManager, entity, ltc, takerSite, takerTile);
                        found = true;
                    }
                    numTries++;
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
                    CreateLiquidsNode(entityManager, nextSite, nextTile);
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

        void FlowInto(IEntityManager entityManager, IEntity entity, LiquidTileNodeComponent l, ISite nextSite, ITile nextTile)
        {
            var diff = l.Tile.LiquidDepth - nextTile.LiquidDepth;
            var flow = diff / 2;
            if (l.Tile.LiquidDepth > 1 && (double)diff / 2d > flow)
            {
                flow++;
            }
            if (flow > 0)
            {
                if (nextTile.LiquidDepth == 0)
                {
                    CreateLiquidsNode(entityManager, nextSite, nextTile);
                }

                l.Tile.LiquidDepth -= flow;
                nextTile.LiquidDepth += flow;

                if (l.Tile.LiquidDepth == 0)
                {
                    entityManager.DeleteEntity(entity.Id);
                }
            }
        }

    }
}
