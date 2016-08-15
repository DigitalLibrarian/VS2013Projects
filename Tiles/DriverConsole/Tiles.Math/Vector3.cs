using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public struct Vector3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3(int x, int y, int z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                System.Math.Min(v1.X, v2.X),
                System.Math.Min(v1.Y, v2.Y),
                System.Math.Min(v1.Z, v2.Z)
                );
        }

        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                System.Math.Max(v1.X, v2.X),
                System.Math.Max(v1.Y, v2.Y),
                System.Math.Max(v1.Z, v2.Z)
                );
        }
        public static Vector3 operator +(Vector3 c1, Vector3 c2)
        {
            return new Vector3(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static Vector3 operator -(Vector3 c1, Vector3 c2)
        {
            return new Vector3(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }
        public static Vector3 operator *(Vector3 c1, double d)
        {
            return new Vector3((int)(c1.X * d), (int)(c1.Y * d), (int) (c1.Z * d));
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vector3 Zero { get { return new Vector3(0, 0, 0); } }
        public override string ToString()
        {
            return string.Format("{0}X={1},Y={2},Z={3}{4}", '{', X, Y, Z, '}');
        }
    }
}
