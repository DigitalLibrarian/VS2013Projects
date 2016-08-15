using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public struct Box2
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }
        public Vector2 Size { get { return Max - Min; } }
        public Box2(Vector2 min, Vector2 max) : this()
        {
            Min = Vector2.Min(min, max);
            Max = Vector2.Max(min, max);
        }

        public bool Contains(Vector2 p)
        {
            return Min.X <= p.X && Max.X >= p.X && Min.Y <= p.Y && Max.Y >= p.Y;
        }
    }

    public struct Box3
    {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }
        public Vector3 Size { get { return Max - Min; } }
        public Box3(Vector3 min, Vector3 max)
            : this()
        {
            Min = Vector3.Min(min, max);
            Max = Vector3.Max(min, max);
        }

        public bool Contains(Vector3 p)
        {
            return Min.X <= p.X && Max.X >= p.X && Min.Y <= p.Y && Max.Y >= p.Y && Min.Z <= p.Z && Max.Z >= p.Z;
        }
    }
}
