using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Liquids
{
    public class LiquidsManager
    {
        ISite Site { get; set; }

        List<Vector3> LiquidTileIndices { get; set; }

        public LiquidsManager(ISite site)
        {
            Site = site;
            LiquidTileIndices = new List<Vector3>();
        }

        /// <summary>
        /// Sets the liquid depth value, while keeping the known indicies synchronized.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="depth"></param>
        void SetLiquidAt(Vector3 index, int depth, List<Vector3> known)
        {
            var tile = Site.GetTileAtSitePos(index);

            if (depth == 0)
            {
                if (tile.LiquidDepth > 0)
                {
                    known.Remove(tile.Index);
                }
            }
            else
            {
                if (tile.LiquidDepth == 0)
                {
                    known.Add(tile.Index);
                }
            }
            tile.LiquidDepth = depth;
        }

        public void Update(int ticks)
        {
            var down = new Vector3(0, 0, -1);
            foreach (var liquidIndex in LiquidTileIndices.ToArray())
            {
                var sourceTile = Site.GetTileAtSitePos(liquidIndex);

                // can go down?
                var takerTile = Line(Site.Box, liquidIndex, down)
                    .Select(i => Site.GetTileAtSitePos(i))
                    .FirstOrDefault(tile => CanTakeLiquid(tile) >= sourceTile.LiquidDepth);
                if (takerTile == null)
                {
                    // can go over?

                    if (takerTile == null)
                    {
                        // can teleport somewhere adjacent but equal or lower?

                    }
                }
            }
        }

        int CanTakeLiquid(ITile tile)
        {
            return 7 - tile.LiquidDepth;
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
    }
}
