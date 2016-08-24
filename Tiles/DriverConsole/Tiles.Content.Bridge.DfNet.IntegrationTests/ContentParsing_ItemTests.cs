using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            MaterialsFactory = new DfMaterialFactory(Store);
            ItemFactory = new DfItemFactory(Store, new DfItemBuilderFactory());
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

            Assert.AreEqual("short sword", swordItem.NameSingular);
            Assert.AreEqual("short swords", swordItem.NamePlural);

            var weapon = swordItem.Weapon;
            Assert.AreEqual(4, weapon.Moves.Count());
            var move = weapon.Moves[0];

            Assert.AreEqual(20000, move.ContactArea);
            Assert.AreEqual(4000, move.MaxPenetration);
            Assert.AreEqual(1250, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("slash", move.Verb.SecondPerson);
            Assert.AreEqual("slashes", move.Verb.ThirdPerson);


            move = weapon.Moves[1];

            Assert.AreEqual(50, move.ContactArea);
            Assert.AreEqual(2000, move.MaxPenetration);
            Assert.AreEqual(1000, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("stab", move.Verb.SecondPerson);
            Assert.AreEqual("stabs", move.Verb.ThirdPerson);

            move = weapon.Moves[2];

            Assert.AreEqual(20000, move.ContactArea);
            Assert.AreEqual(4000, move.MaxPenetration);
            Assert.AreEqual(1250, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("slap", move.Verb.SecondPerson);
            Assert.AreEqual("slaps", move.Verb.ThirdPerson);

            move = weapon.Moves[3];

            Assert.AreEqual(100, move.ContactArea);
            Assert.AreEqual(1000, move.MaxPenetration);
            Assert.AreEqual(1000, move.VelocityMultiplier);
            Assert.AreEqual(3, move.PrepTime);
            Assert.AreEqual(3, move.RecoveryTime);

            Assert.IsNotNull(move.Verb);
            Assert.AreEqual("strike", move.Verb.SecondPerson);
            Assert.AreEqual("strikes", move.Verb.ThirdPerson);

        }

    }
}
