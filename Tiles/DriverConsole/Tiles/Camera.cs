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
        Vector3 Pos { get; set; }

        void Move(Vector3 delta);
    }

    public class Camera : ICamera
    {
        public Vector3 Pos { get; set; }

        public Camera(Vector3 pos)
        {
            Pos = pos;
        }

        public void Move(Vector3 delta)
        {
            Pos += delta;
        }
    }
}
