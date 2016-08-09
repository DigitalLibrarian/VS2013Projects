using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;

namespace Tiles
{
    public interface ICamera
    {
        Vector2 Pos { get; set; }

        void Move(Vector2 delta);
    }

    public class Camera : ICamera
    {
        public Vector2 Pos { get; set; }

        public Camera(Vector2 pos)
        {
            Pos = pos;
        }

        public void Move(Vector2 delta)
        {
            Pos += delta;
        }
    }
}
