using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
    struct OutfitBinding<TSlot>
    {
        public IBodyPart Part;
        public TSlot Slot;
        public IItem Item;
    }
}
