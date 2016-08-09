using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Random
{
    public interface IRandom
    {
        int NextIndex<T>(ICollection<T> list);
        T NextElement<T>(ICollection<T> list);

        double NextDouble();
        int Next(int max);
        int Next(int min, int max);

        Vector2 Next(Vector2 max);
        Vector2 NextInBox(Box box);
    }
}
