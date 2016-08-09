using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
    public class OutfitLayerFactory : IOutfitLayerFactory
    {
        public IOutfitLayer Create<TSlot>(IBody body, Predicate<IItem> isSuitablePred, Func<IBodyPart, TSlot> partSlotFunc, Func<IItem, IEnumerable<TSlot>> itemRequiredSlotFunc)
        {
            return new OutfitLayer<TSlot>(body, isSuitablePred, partSlotFunc, itemRequiredSlotFunc);
        }
    }
}
