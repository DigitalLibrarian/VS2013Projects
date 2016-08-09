using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max)
        {
            if (v.X < min.X) v.X = min.X;
            if (v.X > max.X) v.X = max.X;

            if (v.Y < min.Y) v.Y = min.Y;
            if (v.Y > max.Y) v.Y = max.Y;

            return v;
        }

        public static Vector2 operator +(Vector2 c1, Vector2 c2)
        {
            return new Vector2(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static Vector2 operator -(Vector2 c1, Vector2 c2)
        {
            return new Vector2(c1.X - c2.X, c1.Y - c2.Y);
        }

        public static Vector2 operator *(Vector2 c1, double d)
        {
            return new Vector2((int)(c1.X * d), (int)(c1.Y * d));
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}X={1}, Y={2}{3}", '{', X, Y, '}');
        }

        public static Vector2 Zero { get { return new Vector2(0, 0); } }

        public static double Dot(Vector2 v1, Vector2 v2)
        {
            return ((double)v1.X * (double)v2.X)
                + ((double)v1.Y * (double)v2.Y);
        }

        public double GetLength()
        {
            return System.Math.Sqrt((X * X) + (Y * Y));
        }

        public static Vector2 Min(Vector2 v1, Vector2 v2)
        {
            return new Vector2(System.Math.Min(v1.X, v2.X), System.Math.Min(v1.Y, v2.Y));
        }

        public static Vector2 Max(Vector2 v1, Vector2 v2)
        {
            return new Vector2(System.Math.Max(v1.X, v2.X), System.Math.Max(v1.Y, v2.Y));
        }
    }
}
