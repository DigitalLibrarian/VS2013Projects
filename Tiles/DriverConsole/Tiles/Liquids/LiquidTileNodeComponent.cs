using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.EntityComponents;

namespace Tiles.Liquids
{
    public class LiquidTileNodeComponent : IComponent
    {
        public int Id
        {
            get { return ComponentTypes.LiquidTileNode; }
        }

        public ISite Site { get; set; }
        public ITile Tile { get; set; }
    }
}
