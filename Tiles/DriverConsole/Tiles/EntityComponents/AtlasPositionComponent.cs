using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Math;

namespace Tiles.EntityComponents
{
    public class AtlasPositionComponent : IComponent
    {
        public int Id { get { return ComponentTypes.AtlasPosition; } }

        public Func<Vector3> PositionFunc { get; set; }
        public Vector3 Position { get { return PositionFunc(); } }
    }
}
