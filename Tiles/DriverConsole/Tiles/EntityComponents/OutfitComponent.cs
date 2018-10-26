using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Items.Outfits;

namespace Tiles.EntityComponents
{
    public class OutfitComponent : IComponent
    {
        public int Id { get { return ComponentTypes.Outfit; } }
        public IOutfit Outfit { get; set; }
    }
}
