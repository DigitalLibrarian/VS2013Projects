using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Math;

namespace Tiles.EntityComponents
{
    public interface IAtlasPositionComponent : IComponent
    {
        IAtlasPosition AtlasPosition { get; }
    }

    public class AtlasPositionComponent : IAtlasPositionComponent
    {
        public int Id { get { return ComponentTypes.AtlasPosition; } }
        public IAtlasPosition AtlasPosition { get; set; }

        public AtlasPositionComponent(IAtlasPosition atlasPos)
        {
            AtlasPosition = atlasPos;
        }
    }
    public interface IAtlasPosition
    {
        Vector3 Position { get; }

        // TODO: Add these methods.  Then you can get rid of IAtlasBoxUpdateSystem.  Observing a box is an implementation detail.

        // bool InRealityBubble();
        // bool InViewBox();
    }

    public class AtlasPosition : IAtlasPosition
    {
        Func<Vector3> PosFunc { get; set; }
        public Vector3 Position { get { return PosFunc(); } }

        public AtlasPosition(Func<Vector3> posFunc)
        {
            PosFunc = posFunc;
        }
    }
}
