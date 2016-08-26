using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles
{
    public class PositionFinder : IPositionFinder
    {
        public Vector3? FindNearbyPos(Vector3 centerWorldPos3d, Predicate<Vector3> finderPred, int halfBoxSize)
        {
            for (int i = 0; i <= halfBoxSize; i++)
            {
                var centerWorldPos = new Vector2(
                    centerWorldPos3d.X, centerWorldPos3d.Y
                    );
                var halfSize = new Vector2(i, i);
                var box = new Box2(centerWorldPos - halfSize, centerWorldPos + halfSize);
                for (int x = box.Min.X; x <= box.Max.X; x++)
                {
                    for (int y = box.Min.Y; y <= box.Max.Y; y++)
                    {
                        if (x == box.Min.X || x == box.Max.X || y == box.Min.Y || y == box.Max.Y)
                        {
                            var worldPos = new Vector3(x, y, centerWorldPos3d.Z);
                            if (finderPred(worldPos))
                            {
                                return worldPos;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
