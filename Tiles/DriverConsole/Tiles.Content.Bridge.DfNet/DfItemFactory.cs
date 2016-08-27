using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfItemFactory : IDfItemFactory
    {
        IDfItemBuilderFactory BuilderFactory { get; set; }
        IDfCombatMoveFactory CombatMoveFactory { get; set; }

        IDfObjectStore Store { get; set; }

        public DfItemFactory(IDfObjectStore store, 
            IDfItemBuilderFactory builderFactory,
            IDfCombatMoveFactory combatMoveFactory)
        {
            Store = store;
            BuilderFactory = builderFactory;
            CombatMoveFactory = combatMoveFactory;
        }

        void AddTypeSpecificSlots(string type, IDfItemBuilder b)
        {
            switch (type)
            {
                case DfTags.ITEM_WEAPON:
                    b.AddSlotRequirement(WeaponSlot.Main);
                    break;
                case DfTags.ITEM_SHOES:
                    b.AddSlotRequirement(ArmorSlot.LeftFoot);
                    b.AddSlotRequirement(ArmorSlot.RightFoot);
                    break;
                case DfTags.ITEM_ARMOR:
                    b.AddSlotRequirement(ArmorSlot.Torso);
                    break;
                case DfTags.ITEM_GLOVES:
                    b.AddSlotRequirement(ArmorSlot.LeftHand);
                    b.AddSlotRequirement(ArmorSlot.RightHand);
                    break;
                case DfTags.ITEM_HELM:
                    b.AddSlotRequirement(ArmorSlot.Head);
                    break;
                case DfTags.ITEM_PANTS:
                    b.AddSlotRequirement(ArmorSlot.LeftLeg);
                    b.AddSlotRequirement(ArmorSlot.RightLeg);
                    break;
            }
        }

        public Item Create(string type, string itemName, Material material)
        {
            var df = Store.Get(type, itemName);
            var tags = df.Tags.ToList();
            var b = BuilderFactory.Create();

            AddTypeSpecificSlots(type, b);

            for (int i = 0; i < tags.Count(); i++)
            {
                var tag = tags[i];
                switch (tag.Name)
                {
                    case DfTags.MiscTags.NAME:
                        b.SetName(tag.GetParam(0), tag.GetParam(1));
                        break;
                    case DfTags.MiscTags.LAYER:
                        b.SetArmorLayer(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.TILE:
                        HandleTileTag(tag, b);
                        break;
                }
            }

            foreach (var attackTag in tags.Where(t => t.Name.Equals(DfTags.MiscTags.ATTACK)))
            {
                var index = tags.IndexOf(attackTag);
                var nextIndex = tags.FindIndex(index + 1, t => t.Name.Equals(DfTags.MiscTags.ATTACK));
                if (nextIndex == -1)
                {
                    nextIndex = tags.Count();
                }
                var attackTags = tags.GetRange(index, nextIndex - index);

                var attackDf = new DfObject(attackTags);
                var move = CombatMoveFactory.Create(attackDf);

                b.AddCombatMove(move);
            }

            b.SetMaterial(material);
            return b.Build();
        }

        private void HandleTileTag(DfTag tag, IDfItemBuilder b)
        {
            var p = tag.GetParam(0);

            int result = 0;
            if (int.TryParse(p, out result))
            {

            }
            else if (p.Count() == 3)
            {
                result = (int)p[1];
            }
            else
            {
                throw new NotImplementedException();
            }
            b.SetSymbol(result);

        }
    }
}
