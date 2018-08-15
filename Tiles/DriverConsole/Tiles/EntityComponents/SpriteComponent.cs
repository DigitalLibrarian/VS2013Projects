using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public interface ISpriteComponent : IComponent
    {
        Sprite Sprite { get; }
    }

    public class SpriteComponent : ISpriteComponent
    {
        public int Id { get { return ComponentTypes.Sprite; } }
        public Sprite Sprite { get; set; }
    }
}
