using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public static class CompassVectors
    {
        static public Vector2 North { get { return new Vector2(0, -1); } }
        static public Vector2 East { get { return new Vector2(1, 0); } }
        static public Vector2 West { get { return new Vector2(-1, 0); } }
        static public Vector2 South { get { return new Vector2(0, 1); } }

        static public Vector2 NorthEast { get { return North + East; } }
        static public Vector2 NorthWest { get { return North + West; } }

        static public Vector2 SouthEast { get { return South + East; } }
        static public Vector2 SouthWest { get { return South + West; } }

        static public Vector2 FromDirection(CompassDirection dir)
        {
            var v = new Vector2(0, 0);
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

        static public IEnumerable<Vector2> GetAll()
        {
            return new List<Vector2> {
                North, East, West, South, NorthEast, NorthWest, SouthEast, SouthWest 
            };
        }

        static public bool IsCompassVector(Vector2 v)
        {
            return GetAll().Contains(v);
        }

        static public bool IsCompassVector(Vector3 t)
        {
            var v = new Vector2(t.X, t.Y);
            return GetAll().Contains(v);
        }
    }
}
