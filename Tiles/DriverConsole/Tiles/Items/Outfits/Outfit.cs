using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
    public class Outfit : IOutfit
    {
        public int NumLayers { get; set; }
        IList<IOutfitLayer> Layers { get; set; }
        IOutfitLayer WeaponLayer { get; set; }
        IBody Body { get; set; }


        public Outfit(IBody body, int numLayers = 1)
        {
            Body = body;
            NumLayers = numLayers;
            Allocate();
        }

        void Allocate()
        {
            Layers = new List<IOutfitLayer>(NumLayers);
            for (int i = 0; i < NumLayers; i++)
            {
                Layers.Add(new OutfitLayer<ArmorSlot>(Body,
                    item => item.IsArmor,
                    part => part.ArmorSlot,
                    item => item.Armor.ArmorClass.RequiredSlots));
            }

            WeaponLayer = new OutfitLayer<WeaponSlot>(Body,
                item => item.IsWeapon,
                part => part.WeaponSlot,
                item => item.Weapon.WeaponClass.RequiredSlots);
        }


        public IEnumerable<IItem> GetItems()
        {
            return Layers.SelectMany<IOutfitLayer, IItem>(x => x.GetItems()).Concat(WeaponLayer.GetItems());
        }

        public IEnumerable<IItem> GetItems(int layer)
        {
            return Layers[layer].GetItems();
        }

        public IEnumerable<IItem> GetItems(IBodyPart part)
        {
            return Layers.SelectMany<IOutfitLayer, IItem>(x => x.GetItems(part)).Concat(WeaponLayer.GetItems(part));
        }

        public IEnumerable<IOutfitLayer> GetLayers()
        {
            return Layers;
        }

        public bool CanWear(IItem item, int layer)
        {
            return Layers[layer].CanEquip(item);
        }

        public bool Wear(IItem item, int layer)
        {
            return Wear(item, layer);
        }

        public bool IsWorn(IItem item)
        {
            return Layers.Any(x => x.IsEquipped(item));
        }

        public void TakeOff(IItem item)
        {
            foreach (var layer in Layers)
            {
                layer.Unequip(item);
            }
        }

        public bool CanWear(IItem Item)
        {
            return Layers.FirstOrDefault(x => x.CanEquip(Item)) != null;
        }


        public bool Wear(IItem item)
        {
            if (!CanWear(item)) return false;
            return Layers.First(x => x.CanEquip(item)).Equip(item);
        }


        public bool CanWield(IItem item)
        {
            return WeaponLayer.CanEquip(item);
        }

        public bool Wield(IItem item)
        {
            return WeaponLayer.Equip(item);
        }

        public bool IsWielded(IItem item)
        {
            return WeaponLayer.IsEquipped(item);
        }

        public void Unwield(IItem item)
        {
            WeaponLayer.Unequip(item);
        }

        public IItem GetWeaponItem(IBodyPart bodyPart)
        {
            var partItems = WeaponLayer.GetItems(bodyPart);
            return partItems.SingleOrDefault();
        }
    }
}
