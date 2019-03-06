using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet.IntegrationTests
{
    [TestClass]
    public class ContentParsing_ItemTests
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory MaterialsFactory { get; set; }
        IDfItemFactory ItemFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            MaterialsFactory = new DfMaterialFactory(Store, new DfMaterialBuilderFactory(), new DfColorFactory());
            ItemFactory = new DfItemFactory(Store, 
                new DfItemBuilderFactory(),
                new DfCombatMoveFactory());
        }

        [TestMethod]
        public void AdamantineShortSword()
        {
            var adamantine = MaterialsFactory.CreateInorganic("ADAMANTINE");
            Assert.IsNotNull(adamantine);

            var swordItem = ItemFactory.Create(DfTags.ITEM_WEAPON, "ITEM_WEAPON_SWORD_SHORT", adamantine);
            Assert.IsNotNull(swordItem);
            Assert.IsNotNull(swordItem.Weapon);
            Assert.IsNull(swordItem.Armor);
            Assert.AreEqual(300, swordItem.Size);
            Assert.AreSame(adamantine, swordItem.Material);

            Assert.AreNotEqual(0, swordItem.Sprite.Symbol);

            Assert.AreEqual("short sword", swordItem.NameSingular);
            Assert.AreEqual("short swords", swordItem.NamePlural);

            var weapon = swordItem.Weapon;

            Assert.IsTrue(weapon.SlotRequirements.SequenceEqual(new WeaponSlot[] { WeaponSlot.Main }));

            Assert.AreEqual(4, weapon.Moves.Count());
            var move = weapon.Moves[0];

            Assert.AreEqual(ContactType.Shear, move.ContactType);
            Assert.AreEqual(20000, move.ContactArea);
            Assert.AreEqual(4000, move.MaxPenetration);
            Assert.AreEqual(1250, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("slash", move.Verb.SecondPerson);
            Assert.AreEqual("slashes", move.Verb.ThirdPerson);

            move = weapon.Moves[1];

            Assert.AreEqual(ContactType.Shear, move.ContactType);
            Assert.AreEqual(50, move.ContactArea);
            Assert.AreEqual(2000, move.MaxPenetration);
            Assert.AreEqual(1000, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("stab", move.Verb.SecondPerson);
            Assert.AreEqual("stabs", move.Verb.ThirdPerson);

            move = weapon.Moves[2];

            Assert.AreEqual(ContactType.Impact, move.ContactType);
            Assert.AreEqual(20000, move.ContactArea);
            Assert.AreEqual(4000, move.MaxPenetration);
            Assert.AreEqual(1250, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("slap", move.Verb.SecondPerson);
            Assert.AreEqual("slaps", move.Verb.ThirdPerson);

            move = weapon.Moves[3];

            Assert.AreEqual(ContactType.Impact, move.ContactType);
            Assert.AreEqual(100, move.ContactArea);
            Assert.AreEqual(1000, move.MaxPenetration);
            Assert.AreEqual(1000, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("strike", move.Verb.SecondPerson);
            Assert.AreEqual("strikes", move.Verb.ThirdPerson);
        }

        [TestMethod]
        public void SpongeSandals()
        {
            var sponge = MaterialsFactory.CreateTissue("SPONGE_TEMPLATE");
            var item = ItemFactory.Create(DfTags.ITEM_SHOES, "ITEM_SHOES_SANDAL", sponge);
            Assert.IsNotNull(item);

            Assert.AreNotEqual(0, item.Sprite.Symbol);
            Assert.AreEqual(25, item.Size);

            Assert.AreEqual("sandal", item.NameSingular);
            Assert.AreEqual("sandals", item.NamePlural);

            Assert.IsNotNull(item.Armor);
            Assert.AreEqual("OVER", item.Armor.ArmorLayer);

            Assert.IsTrue(item.Armor.SlotRequirements.SequenceEqual(
                new ArmorSlot[] { 
                    ArmorSlot.LeftFoot,
                    ArmorSlot.RightFoot,
                }));

            Assert.AreEqual("sponge", item.Material.Adjective);
        }

        [TestMethod]
        public void LeatherLoincloth()
        {
            var leather = MaterialsFactory.CreateFromMaterialTemplate("LEATHER_TEMPLATE");
            var item = ItemFactory.Create(DfTags.ITEM_PANTS, "ITEM_PANTS_LOINCLOTH", leather);
            Assert.IsNotNull(item);

            Assert.AreNotEqual(0, item.Sprite.Symbol);
            Assert.AreEqual(10, item.Size);

            Assert.AreEqual("loincloth", item.NameSingular);
            Assert.AreEqual("loincloths", item.NamePlural);

            Assert.IsNotNull(item.Armor);
            Assert.AreEqual("UNDER", item.Armor.ArmorLayer);

            Assert.IsTrue(item.Armor.SlotRequirements.SequenceEqual(
                new ArmorSlot[] { 
                    ArmorSlot.LeftLeg,
                    ArmorSlot.RightLeg,
                }));

            Assert.AreEqual("leather", item.Material.Adjective);
        }

        [TestMethod]
        public void IronLaddle()
        {
            var iron = MaterialsFactory.CreateInorganic("IRON");
            var item = ItemFactory.Create(DfTags.ITEM_TOOL, "ITEM_TOOL_LADLE", iron);

            Assert.AreEqual((int)'/', item.Sprite.Symbol);
            Assert.AreEqual(100, item.Size);
            Assert.AreEqual(7850, item.Material.SolidDensity);
        }
    }
}
