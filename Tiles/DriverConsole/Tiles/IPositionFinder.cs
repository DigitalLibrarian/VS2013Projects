using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles
{
    public interface IPositionFinder
    {
        Vector3? FindNearbyPos(Vector3 centerWorldPos3d, Predicate<Vector3> finderPred, int halfBoxSize);
    }
}
