using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Items.Outfits
{
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
