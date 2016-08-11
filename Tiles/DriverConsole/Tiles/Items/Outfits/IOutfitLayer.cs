using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
    public interface IOutfitLayer
    {
        IEnumerable<IItem> GetItems();
        IEnumerable<IItem> GetItems(IBodyPart part);

        bool CanEquip(IItem item);
        bool Equip(IItem item);


        bool IsEquipped(IItem item);
        void Unequip(IItem item);

        IEnumerable<IBodyPart> FindParts(IItem item);
    }
}
