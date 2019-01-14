using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public static class CompassVectors
    {
        static public Vector3 North { get { return new Vector3(0, -1, 0); } }
        static public Vector3 East { get { return new Vector3(1, 0, 0); } }
        static public Vector3 West { get { return new Vector3(-1, 0, 0); } }
        static public Vector3 South { get { return new Vector3(0, 1, 0); } }

        static public Vector3 NorthEast { get { return North + East; } }
        static public Vector3 NorthWest { get { return North + West; } }

        static public Vector3 SouthEast { get { return South + East; } }
        static public Vector3 SouthWest { get { return South + West; } }

        static Dictionary<CompassDirection, Vector3> DirectionToVector = new Dictionary<CompassDirection, Vector3>
        {
            {CompassDirection.North, CompassVectors.North},
            {CompassDirection.South, CompassVectors.South},
            {CompassDirection.East, CompassVectors.East},
            {CompassDirection.West, CompassVectors.West},
            
            {CompassDirection.NorthEast, CompassVectors.NorthEast},
            {CompassDirection.NorthWest, CompassVectors.NorthWest},
            {CompassDirection.SouthEast, CompassVectors.SouthEast},
            {CompassDirection.SouthWest, CompassVectors.SouthWest},
        };

        static public Vector3 FromDirection(CompassDirection dir)
        {
            return DirectionToVector[dir];
        }

        static public IEnumerable<Vector3> GetAll()
        {
            return new List<Vector3> {
                North, East, West, South, NorthEast, NorthWest, SouthEast, SouthWest 
            };
        }
        
        static public bool IsCompassVector(Vector3 v)
        {
            return GetAll().Contains(v);
        }


        static public CompassDirection GetCompassDirection(Vector3 v)
        {
            foreach (var pair in DirectionToVector)
            {
                var dir = pair.Key;
                var vector = pair.Value;
                if (vector == v)
                {
                    return dir;
                }
            }
            throw new InvalidOperationException("Not given compass vector");
        }
    }
}
