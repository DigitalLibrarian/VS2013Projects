using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;
using Tiles.Math;
using Tiles.Random;

namespace Tiles.Splatter
{
    public interface ISplatterFascade
    {
        void Register(Vector3 worldPos, IMaterial material);
        void Track(Vector3 worldPos, Vector3 dir);
        void ArterySquirt(Vector3 pos, IMaterial blood, int squirtDistance = 3);
    }

    public class SplatterFascade : ISplatterFascade
    {
        private IRandom Random { get; set; }
        private IAtlas Atlas { get; set; }
        public SplatterFascade(IRandom random, IAtlas atlas)
        {
            Random = random;
            Atlas = atlas;
        }

        public void Register(Vector3 worldPos, IMaterial material)
        {
            var tile = Atlas.GetTileAtPos(worldPos);

            if (!TryTake(tile, material))
            {
                var compassVectors = CompassVectors.GetAll().ToList();
                while (compassVectors.Count() > 0)
                {
                    var index = Random.NextIndex(compassVectors);
                    var pos = worldPos + compassVectors.ElementAt(index);
                    tile = Atlas.GetTileAtPos(pos);
                    if (TryTake(tile, material))
                    {
                        break;
                    }
                    else
                    {
                        compassVectors.RemoveAt(index);
                    }
                }
            }
        }

        public void Track(Vector3 worldPos, Vector3 dir)
        {
            /* Dwarves walking on tiles with blood (smear) will spread it to the next tile, 
             * where it will form a spattering of blood, then a smear. That is why blood areas 
             * tends to grow quickly on above ground when heavy traffic is there. */

            var tile = Atlas.GetTileAtPos(worldPos);
            if (tile.SplatterAmount != SplatterAmount.Max) return;

            var newTile = Atlas.GetTileAtPos(worldPos + dir);
            if(newTile.SplatterAmount == SplatterAmount.Max) return;

            tile.SplatterAmount--;
            newTile.SplatterAmount++;

            newTile.SplatterMaterial = tile.SplatterMaterial;
        }

        private bool TryTake(ITile tile, IMaterial material)
        {
            if (tile.SplatterAmount == SplatterAmount.Max) return false;
            
            //TODO - figure out how to check for material identity
            tile.SplatterMaterial = material;

            if (tile.SplatterAmount == SplatterAmount.Some)
                tile.SplatterAmount = SplatterAmount.Max;
            else
                tile.SplatterAmount = SplatterAmount.Some;


            return true;
        }

        public void ArterySquirt(Vector3 pos, IMaterial blood, int squirtDistance = 3)
        {
            var dir = Random.NextElement(CompassVectors.GetAll());
            for (int i = 0; i < squirtDistance-1; i++)
            {
                pos += dir;
                Register(pos, blood);
                var tile = Atlas.GetTileAtPos(pos);
                if (!tile.IsTerrainPassable) break;
            }
        }
    }
}
