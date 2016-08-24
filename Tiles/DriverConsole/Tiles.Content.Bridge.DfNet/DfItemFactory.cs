using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfMaterialFactory
    {
        Material CreateTissue(string tissueTemplate);
    }


    public class DfItemFactory
    {
        IDfItemBuilderFactory BuilderFactory { get; set; }
        IDfObjectStore Store { get; set; }
        public string Type { get; set; }

        public Item Create(string itemName, Material material)
        {
            var df = Store.Get(Type, itemName);
            var tags = df.Tags.ToList();
            var b = BuilderFactory.Create();

            for (int i = 0; i < tags.Count(); i++)
            {
                var tag = tags[i];
                switch (tag.Name)
                {
                    case DfTags.MiscTags.NAME:
                        b.SetName(tag.GetParam(0), tag.GetParam(1));
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

                var move = new CombatMove
                {
                    Verb = new Verb
                    {
                        SecondPerson = attackTag.GetParam(3),
                        ThirdPerson = attackTag.GetParam(4),
                        IsTransitive = false
                    },
                    ContactArea = int.Parse(attackTag.GetParam(1)),
                    MaxPenetration = int.Parse(attackTag.GetParam(2)),
                    VelocityMultiplier = int.Parse(attackTag.GetParam(6))
                };
                foreach (var subTag in tags.GetRange(index, nextIndex - index))
                {
                    switch (subTag.Name)
                    {
                        case DfTags.MiscTags.ATTACK_PREPARE_AND_RECOVER:
                            move.PrepTime = int.Parse(subTag.GetParam(0));
                            move.RecoveryTime = int.Parse(subTag.GetParam(1));
                            break;
                    }
                }
                b.AddCombatMove(move);
            }

            b.SetMaterial(material);
            return b.Build();
        }
    }

    public interface IDfItemBuilderFactory
    {
        IDfItemBuilder Create();
    }



    public interface IDfItemBuilder
    {
        void SetMaterial(Material material);
        void AddCombatMove(CombatMove move);

        void SetName(string singular, string plural);

        Item Build();
    }


    public class DfItemBuilder : IDfItemBuilder
    {
        Item Item { get; set; }
        public void SetMaterial(Material material)
        {
            Item.Material = material;
        }

        public void AddCombatMove(CombatMove move)
        {
            Item.Weapon.Moves.Add(move);
        }

        public void SetName(string singular, string plural)
        {
            Item.NameSingular = singular;
            Item.NamePlural = plural;
        }

        public Item Build()
        {
            return Item;
        }
    }
}
