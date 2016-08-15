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

        static public Vector3 FromDirection(CompassDirection dir)
        {
            var v = new Vector3(0, 0, 0);
            switch (dir)
            {
                case CompassDirection.North: v = North; break;
                case CompassDirection.East: v = East; break;
                case CompassDirection.West: v = West; break;
                case CompassDirection.South: v = South; break;

                case CompassDirection.NorthEast: v = NorthEast; break;
                case CompassDirection.NorthWest: v = NorthWest; break;
                case CompassDirection.SouthEast: v = SouthEast; break;
                case CompassDirection.SouthWest: v = SouthWest; break;
            }

            return v;
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
    }
}
