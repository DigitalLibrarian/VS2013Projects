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
        static readonly Vector3[] NeighborOffsets_Over = {
                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(-1, -1, 0),
                    
                    new Vector3(1, 0, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, -1, 0),
                };

        static readonly Vector3[] NeighborOffsets_Pressure = {
                    new Vector3(0, 0, -1),
                    new Vector3(1, 0, -1),
                    new Vector3(0, 1, -1),
                    new Vector3(-1, 0, -1),
                    new Vector3(0, -1, -1),

                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, -1, 0),
                    
                    new Vector3(1, 0, 1),
                    new Vector3(0, 1, 1),
                    new Vector3(-1, 0, 1),
                    new Vector3(0, -1, 1),
                    new Vector3(0, 0, 1),
                };

        static readonly Vector3[] NeighborOffsets = {
                    new Vector3(0, 0, -1),
                    new Vector3(1, 1, -1),
                    new Vector3(1, -1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, -1),
                    new Vector3(1, 0, -1),
                    new Vector3(0, 1, -1),
                    new Vector3(-1, 0, -1),
                    new Vector3(0, -1, -1),

                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(-1, 1, 0),
                    new Vector3(-1, -1, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, -1, 0),
                    
                    new Vector3(1, 1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(-1, 1, 1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, 0, 1),
                    new Vector3(0, 1, 1),
                    new Vector3(-1, 0, 1),
                    new Vector3(0, -1, 1),
                    new Vector3(0, 0, 1),
                };
        IRandom Random { get; set; }

        private static readonly int[] LiquidsSystemComponentTypes = { ComponentTypes.LiquidTileNode, ComponentTypes.AtlasPosition };
        public LiquidsSystem(IRandom random)
            : base(LiquidsSystemComponentTypes)
        {
            Random = random;
        }

        protected override void UpdateEntity(Ecs.IEntityManager entityManager, Ecs.IEntity entity, IGame game)
        {
            var ltc = entity.GetComponent<LiquidTileNodeComponent>(ComponentTypes.LiquidTileNode);
            var site = ltc.Site;
            var tile = ltc.Tile;

            if (tile.LiquidDepth == 0 || tile.LiquidDepth == 1 && Random.Next(5000) == 0)
            {
                entityManager.DeleteEntity(entity.Id);
                tile.LiquidDepth = 0;
                return;
            }

            if (ltc.IsSleeping) return;

            bool hit = false;
            var worldPos = site.Box.Min + tile.Index;
            var nextPos = worldPos + new Vector3(0, 0, -1);
            var takerTile = game.Atlas.GetTileAtPos(nextPos, false);
            if (takerTile != null && takerTile.IsTerrainPassable && takerTile.LiquidDepth < MaxDepth) // can fall?
            {
                var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                FlowDown(entityManager, entity, ltc, takerSite, takerTile);
                hit = true;
            }
            else if(tile.LiquidDepth > 1)
            {
                // swap two elements, to create chaos over time and avoid favoring the original order
                SwapTwoElements(Random, NeighborOffsets_Over);
                foreach (var nextOff in NeighborOffsets_Over)
                {
                    nextPos = worldPos + nextOff;
                    takerTile = game.Atlas.GetTileAtPos(nextPos, false);
                    if (takerTile != null
                        && takerTile.IsTerrainPassable
                        && takerTile.LiquidDepth < tile.LiquidDepth) // can flow sideways?
                    {
                        var takerSite = game.Atlas.GetSiteAtPos(nextPos, false);
                        FlowInto(entityManager, entity, ltc, takerSite, takerTile);
                        hit = true;
                        break;
                    }
                    else if (Random.Next(NeighborOffsets.Count()) == 0) return;
                }

                if(!hit)
                {
                    // now we need to flood fill around the surface of the body of water, that is not above our
                    // entity position.  If we find a vacant spot to flow into, we teleport/flow there.
                    hit = TryTeleFlow(game.Atlas, entityManager, entity, ltc, worldPos, worldPos);
                }
            }
            
            if (hit)
            {
                // there was a disturbance, wake up any neighbors
                WakeUpLiquids(entityManager, worldPos);
            }
            else
            {
                // put this node to sleep and we won't update it until something wakes it up
                ltc.IsSleeping = true;
            }
        }

        public static void WakeUpLiquids(IEntityManager entityManager, Vector3 worldPos)
        {
            WakeUp(entityManager.GetEntities(LiquidsSystemComponentTypes), worldPos);
        }
        static void WakeUp(IEnumerable<IEntity> everybody, Vector3 worldPos)
        {
            foreach (var off in NeighborOffsets)
            {
                var offPos = worldPos + off;
                var neighbor = everybody.SingleOrDefault(e => e.GetComponent<AtlasPositionComponent>(ComponentTypes.AtlasPosition).Position.Equals(offPos));
                if (neighbor != null)
                {
                    var ltc = neighbor.GetComponent<LiquidTileNodeComponent>(ComponentTypes.LiquidTileNode);
                    ltc.IsSleeping = false;
                }
            }
        }

        static void SwapTwoElements(IRandom random, Vector3[] array)
        {
            var i0 = random.NextIndex(array);
            var i1 = random.NextIndex(array);
            Vector3 temp = array[i0];
            array[i0] = array[i1];
            array[i1] = temp;
        }

        public static IEntity CreateLiquidsNode(IEntityManager entityManager, ISite site, ITile tile)
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
            // if there was round-off, bump up by one, so that we err on the side of sloshiness
            if ((double)diff / 2d > (double)flow) 
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

        bool TryTeleFlow(IAtlas atlas, IEntityManager entityManager, IEntity entity, LiquidTileNodeComponent l, Vector3 originalWorldPos, Vector3 worldPos, List<Vector3> visited = null)
        {
            visited = visited ?? new List<Vector3>{ worldPos };

            // flood fill
            foreach(var off in NeighborOffsets_Pressure)
            {
                var offPos = worldPos + off;
                if (offPos.Z > originalWorldPos.Z) continue;
                if (visited.Contains(offPos)) continue;

                visited.Add(offPos);
                var offTile = atlas.GetTileAtPos(offPos);
                if (offTile.LiquidDepth == MaxDepth)
                {
                    // Let us skip forward in this direction until we hit a wall
                    foreach (var testPos in Line(offPos, off, v => atlas.GetTileAtPos(v).LiquidDepth != MaxDepth))
                    {
                        visited.Add(testPos);
                    }

                    if (TryTeleFlow(atlas, entityManager, entity, l, originalWorldPos, visited.Last(), visited))
                    {
                        return true;
                    }
                }
                else if (offTile.IsTerrainPassable && offTile.LiquidDepth < MaxDepth)
                {
                    var offSite = atlas.GetSiteAtPos(offPos);
                    if (offPos.Z < originalWorldPos.Z)
                    {
                        FlowDown(entityManager, entity, l, offSite, offTile);
                        return true;
                    }
                    else if (offTile.LiquidDepth < l.Tile.LiquidDepth)
                    {
                        FlowInto(entityManager, entity, l, offSite, offTile);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        static IEnumerable<Vector3> Line(Vector3 start, Vector3 delta, Predicate<Vector3> stopCondition)
        {
            while (!stopCondition(start + delta))
            {
                yield return start += delta;
            }
        }
    }
}
