using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items
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

    public interface IOutfitLayer
    {
        IEnumerable<IItem> GetItems();
        IEnumerable<IItem> GetItems(IBodyPart part);

        bool CanEquip(IItem item);
        bool Equip(IItem item);


        bool IsEquipped(IItem item);
        void Unequip(IItem item);
    }

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

    struct OutfitBinding<TSlot>
    {
        public IBodyPart Part;
        public TSlot Slot;
        public IItem Item;
    }

    public class OutfitLayer<TSlot> : IOutfitLayer
    {
        IBody Body { get; set; }
        IList<OutfitBinding<TSlot>> Bindings { get; set; }

        public OutfitLayer(IBody body, Predicate<IItem> isSuitablePred, Func<IBodyPart, TSlot> partSlotFunc, Func<IItem, IEnumerable<TSlot>> itemRequiredSlotFunc)
        {
            Body = body;
            IsSuitablePred = isSuitablePred;
            PartSlotFunc = partSlotFunc;
            ItemRequiredSlotsFunc = itemRequiredSlotFunc;
            Bindings = new List<OutfitBinding<TSlot>>();
        }

        Predicate<IItem> IsSuitablePred { get; set; }
        Func<IBodyPart, TSlot> PartSlotFunc { get; set; }
        Func<IItem, IEnumerable<TSlot>> ItemRequiredSlotsFunc { get; set; }

        bool IsSuitable(IItem item)
        {
            return IsSuitablePred(item);
        }
        TSlot PartSlot(IBodyPart part)
        {
            return PartSlotFunc(part);
        }

        IEnumerable<TSlot> RequiredSlots(IItem item)
        {
            return ItemRequiredSlotsFunc(item);
        }

        public IEnumerable<IItem> GetItems()
        {
            return Bindings.Select(x => x.Item);
        }

        public IEnumerable<IItem> GetItems(IBodyPart part)
        {
            return Bindings.Where(x => x.Part == part).Select(x => x.Item);
        }

        IEnumerable<IBodyPart> FindParts(IItem item)
        {
            if (IsSuitable(item))
            {
                var reqSlots = RequiredSlots(item);
                foreach (var part in Body.Parts)
                {
                    if (reqSlots.Contains(PartSlot(part)))
                    {
                        yield return part;
                    }
                }
            }
        }

        public bool CanEquip(IItem item)
        {
            if (!IsSuitable(item)) return false;
            var requiredSlots = RequiredSlots(item).ToList();
            return !Bindings.Any(x => requiredSlots.Contains(x.Slot))   // we haven't used any required bindings
            && HaveAllRequiredSlots(item);
        }

        bool HaveAllRequiredSlots(IItem item)
        {
            if (!IsSuitable(item)) return false;

            var parts = FindParts(item).ToList();

            foreach (var slot in RequiredSlots(item))
            {
                if (!parts.Any(x => PartSlot(x).Equals(slot))) // are we missing the slot?
                {
                    return false;
                }
                else
                {
                    parts.Remove(parts.First(x => PartSlot(x).Equals(slot))); // use the slot so it can't be considered again
                }
            }
            return true;
        }

        public bool Equip(IItem item)
        {
            if (CanEquip(item))
            {
                foreach (var bodyPart in FindParts(item))
                {
                    Bindings.Add(new OutfitBinding<TSlot>
                    {
                        Part = bodyPart,
                        Slot = PartSlot(bodyPart),
                        Item = item
                    });
                }
                return true;
            }
            return false;
        }

        public bool IsEquipped(IItem item)
        {
            return Bindings.Where(x => x.Item == item).Any();
        }

        public void Unequip(IItem item)
        {
            var bindings = Bindings.Where(x => x.Item == item).ToList();
            foreach (var binding in bindings)
            {
                Bindings.Remove(binding);
            }
        }
    }
}
