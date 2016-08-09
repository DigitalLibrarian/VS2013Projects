using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
    public interface IOutfit
    {
        IEnumerable<IItem> GetItems();
        IEnumerable<IItem> GetItems(int layer);
        IEnumerable<IItem> GetItems(IBodyPart part);

        int NumLayers { get; }
        IEnumerable<IOutfitLayer> GetLayers();

        bool CanWear(IItem Item);
        bool Wear(IItem item);

        bool IsWorn(IItem item);
        void TakeOff(IItem item);
                
        bool CanWield(IItem item);
        bool Wield(IItem item);

        bool IsWielded(IItem item);
        void Unwield(IItem item);

        IItem GetWeaponItem(IBodyPart bodyPart);
    }
}
