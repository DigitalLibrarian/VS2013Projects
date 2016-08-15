using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Random
{
    public class RandomWrapper : IRandom
    {
        System.Random Inner { get; set; }

        public RandomWrapper(System.Random random)
        {
            Inner = random;
        }

        public int NextIndex<T>(ICollection<T> list)
        {
            return Next(list.Count);
        }

        public T NextElement<T>(ICollection<T> list)
        {
            return list.ElementAt(NextIndex(list));
        }

        public double NextDouble()
        {
            return Inner.NextDouble();
        }

        public int Next(int max)
        {
            return Next(0, max);
        }

        public int Next(int min, int max)
        {
            return Inner.Next(min, max);
        }


        public Vector2 Next(Vector2 max)
        {
            return new Vector2(Next(0, max.X), Next(0, max.Y));
        }

        public Vector3 Next(Vector3 max)
        {
            return new Vector3(Next(0, max.X), Next(0, max.Y), Next(0, max.Z));
        }

        public Vector2 NextInBox(Box2 box)
        {
            return new Vector2(Next(box.Min.X, box.Max.X), Next(box.Min.Y, box.Max.Y));
        }

        public Vector3 NextInBox(Box3 box)
        {
            return new Vector3(
                Next(box.Min.X, box.Max.X), 
                Next(box.Min.Y, box.Max.Y),
                Next(box.Min.Z, box.Max.Z) 
                );
        }
    }
}
