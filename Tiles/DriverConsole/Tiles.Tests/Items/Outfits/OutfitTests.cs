using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;

using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Bodies;

namespace Tiles.Tests.Items.Outfits
{
    [TestClass]
    public class OutfitTests
    {
        int NumArmorLayers { get; set; }
        Mock<IBody> BodyMock { get; set; }
        Mock<IOutfitLayerFactory> LayerFactoryMock { get; set; }

        Mock<IBodyPart> WeaponPartMock1 { get; set; }

        List<Mock<IOutfitLayer>> ArmorLayerMocks { get; set; }
        Mock<IOutfitLayer> WeaponLayerMock { get; set; }

        Outfit Outfit { get; set; }


        [TestInitialize]
        public void Initialize()
        {
            NumArmorLayers = 10;
            BodyMock = new Mock<IBody>();

            WeaponPartMock1 = new Mock<IBodyPart>();

            BodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart> { WeaponPartMock1.Object });

            ArmorLayerMocks = new List<Mock<IOutfitLayer>>();
            LayerFactoryMock = new Mock<IOutfitLayerFactory>();
            LayerFactoryMock.Setup( x=> x.Create<ArmorSlot>(
                BodyMock.Object,
                It.IsAny<Predicate<IItem>>(),
                It.IsAny<Func<IBodyPart, ArmorSlot>>(),
                It.IsAny<Func<IItem, IEnumerable<ArmorSlot>>>()
                )).Returns(
                (IBody body,Predicate<IItem> pred, Func<IBodyPart, ArmorSlot> partSlot, Func<IItem, IEnumerable<ArmorSlot>> itemReq) => {
                    var layerMock = new Mock<IOutfitLayer>();
                    ArmorLayerMocks.Add(layerMock);

                    TestArmorPred(pred);
                    TestArmorPartSlot(partSlot);
                    TestArmorRequiredSlots(itemReq);

                    return layerMock.Object;
                });

            LayerFactoryMock.Setup(x => x.Create<WeaponSlot>(
                BodyMock.Object,
                It.IsAny<Predicate<IItem>>(),
                It.IsAny<Func<IBodyPart, WeaponSlot>>(),
                It.IsAny<Func<IItem, IEnumerable<WeaponSlot>>>()
                )).Returns(
                (IBody body, Predicate<IItem> pred, Func<IBodyPart, WeaponSlot> partSlot, Func<IItem, IEnumerable<WeaponSlot>> itemReq) =>
                {
                    WeaponLayerMock = new Mock<IOutfitLayer>();

                    TestWeaponPred(pred);
                    TestWeaponPartSlot(partSlot);
                    TestWeaponRequiredSlots(itemReq);
                    return WeaponLayerMock.Object;
                });

            Outfit = new Outfit(BodyMock.Object, LayerFactoryMock.Object, NumArmorLayers);
        }

        private void TestArmorRequiredSlots(Func<IItem, IEnumerable<ArmorSlot>> itemReq)
        {
            var itemMock = new Mock<IItem>();
            var armorClassMock = new Mock<IArmorClass>();

            var slots = new List<ArmorSlot>();
            itemMock.Setup(x => x.ArmorClass).Returns(armorClassMock.Object);
            armorClassMock.Setup(x => x.RequiredSlots).Returns(slots);

            Assert.AreSame(slots, itemReq(itemMock.Object));

        }

        private void TestArmorPartSlot(Func<IBodyPart, ArmorSlot> partSlot)
        {
            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.ArmorSlot).Returns(ArmorSlot.Head);
            Assert.AreEqual(ArmorSlot.Head, partSlot(partMock.Object));
        }

        void TestArmorPred(Predicate<IItem> pred)
        {
            var armorMock = new Mock<IItem>();
            armorMock.Setup(x => x.IsArmor).Returns(true);
            var notArmorMock = new Mock<IItem>();
            notArmorMock.Setup(x => x.IsArmor).Returns(false);

            Assert.IsTrue(pred(armorMock.Object));
            Assert.IsFalse(pred(notArmorMock.Object));
        }


        private void TestWeaponRequiredSlots(Func<IItem, IEnumerable<WeaponSlot>> itemReq)
        {
            var itemMock = new Mock<IItem>();
            var weaponClassMock = new Mock<IWeaponClass>();

            var slots = new List<WeaponSlot>();
            itemMock.Setup(x => x.WeaponClass).Returns(weaponClassMock.Object);
            weaponClassMock.Setup(x => x.RequiredSlots).Returns(slots);

            Assert.AreSame(slots, itemReq(itemMock.Object));

        }

        private void TestWeaponPartSlot(Func<IBodyPart, WeaponSlot> partSlot)
        {
            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.WeaponSlot).Returns(WeaponSlot.Main);
            Assert.AreEqual(WeaponSlot.Main, partSlot(partMock.Object));
        }

        void TestWeaponPred(Predicate<IItem> pred)
        {
            var armorMock = new Mock<IItem>();
            armorMock.Setup(x => x.IsWeapon).Returns(true);
            var notWeaponMock = new Mock<IItem>();
            notWeaponMock.Setup(x => x.IsWeapon).Returns(false);

            Assert.IsTrue(pred(armorMock.Object));
            Assert.IsFalse(pred(notWeaponMock.Object));
        }

        [TestMethod]
        public void Allocation()
        {
            LayerFactoryMock.Verify(x => x.Create<ArmorSlot>(
                BodyMock.Object,
                It.IsAny<Predicate<IItem>>(),
                It.IsAny<Func<IBodyPart, ArmorSlot>>(),
                It.IsAny<Func<IItem, IEnumerable<ArmorSlot>>>()), Times.Exactly(NumArmorLayers));

            LayerFactoryMock.Verify(x => x.Create<WeaponSlot>(
                BodyMock.Object,
                It.IsAny<Predicate<IItem>>(),
                It.IsAny<Func<IBodyPart, WeaponSlot>>(),
                It.IsAny<Func<IItem, IEnumerable<WeaponSlot>>>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetItems()
        {
            var created = new List<IEnumerable<IItem>>();

            int i = 0;
            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Setup(x => x.GetItems()).Returns(() => {
                    var l = Enumerable.Repeat<IItem>(new Mock<IItem>().Object, i++);
                    created.Add(l);
                    return l;
                });
            }
            WeaponLayerMock.Setup(x => x.GetItems()).Returns(() =>
            {
                var l = Enumerable.Repeat<IItem>(new Mock<IItem>().Object, i++);
                created.Add(l);
                return l;
            });

            var items = Outfit.GetItems().ToList();

            Assert.IsTrue(items.Any());

            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Verify(x => x.GetItems(), Times.Once());
            }
            WeaponLayerMock.Verify(x => x.GetItems(), Times.Once());

            foreach (var subList in created)
            {
                foreach (var item in subList)
                {
                    Assert.IsTrue(items.Contains(item));
                    items.Remove(item);
                }
            }
            Assert.IsFalse(items.Any());
        }
        [TestMethod]
        public void GetItems_BodyPart()
        {
            var created = new List<IEnumerable<IItem>>();
            var bodyPartMock = new Mock<IBodyPart>();

            int i = 0;
            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Setup(x => x.GetItems(bodyPartMock.Object)).Returns(() =>
                {
                    var l = Enumerable.Repeat<IItem>(new Mock<IItem>().Object, i++);
                    created.Add(l);
                    return l;
                });
            }
            WeaponLayerMock.Setup(x => x.GetItems(bodyPartMock.Object)).Returns(() =>
            {
                var l = Enumerable.Repeat<IItem>(new Mock<IItem>().Object, i++);
                created.Add(l);
                return l;
            });

            var items = Outfit.GetItems(bodyPartMock.Object).ToList();

            Assert.IsTrue(items.Any());

            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Verify(x => x.GetItems(bodyPartMock.Object), Times.Once());
            }
            WeaponLayerMock.Verify(x => x.GetItems(bodyPartMock.Object), Times.Once());

            foreach (var subList in created)
            {
                foreach (var item in subList)
                {
                    Assert.IsTrue(items.Contains(item));
                    items.Remove(item);
                }
            }
            Assert.IsFalse(items.Any());
        }

        [TestMethod]
        public void GetArmorLayers()
        {
            var layers = Outfit.GetArmorLayers().ToList();

            Assert.AreEqual(ArmorLayerMocks.Count(), layers.Count());

            for (int i = 0; i < layers.Count(); i++)
            {
                Assert.AreSame(ArmorLayerMocks[i].Object, layers[i]);
            }
        }

        [TestMethod]
        public void IsEquipped()
        {
            var armorMock = new Mock<IItem>();
            var weaponMock = new Mock<IItem>();

            var armorLayerIndex = 2;
            var armorLayerMock = ArmorLayerMocks[armorLayerIndex];

            Assert.IsFalse(Outfit.IsWorn(armorMock.Object));

            armorLayerMock.Setup(x => x.IsEquipped(armorMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.IsWorn(armorMock.Object));
            for (int i = 0; i < NumArmorLayers; i++)
            {
                int expectedCalls = i <= armorLayerIndex ? 2 : 1;
                ArmorLayerMocks[i].Verify(x => x.IsEquipped(armorMock.Object), Times.Exactly(expectedCalls));
            }

            Assert.IsFalse(Outfit.IsWielded(weaponMock.Object));
            WeaponLayerMock.Setup(x => x.IsEquipped(weaponMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.IsWielded(weaponMock.Object));
            WeaponLayerMock.Verify(x => x.IsEquipped(weaponMock.Object), Times.Exactly(2));
        }

        [TestMethod]
        public void CanEquip()
        {
            var armorMock = new Mock<IItem>();
            var weaponMock = new Mock<IItem>();

            var armorLayerIndex = 2;
            var armorLayerMock = ArmorLayerMocks[armorLayerIndex];

            Assert.IsFalse(Outfit.CanWear(armorMock.Object));

            armorLayerMock.Setup(x => x.CanEquip(armorMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.CanWear(armorMock.Object));
            for (int i = 0; i < NumArmorLayers; i++)
            {
                int expectedCalls = i <= armorLayerIndex ? 2 : 1;
                ArmorLayerMocks[i].Verify(x => x.CanEquip(armorMock.Object), Times.Exactly(expectedCalls));
            }

            Assert.IsFalse(Outfit.CanWield(weaponMock.Object));
            WeaponLayerMock.Setup(x => x.CanEquip(weaponMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.CanWield(weaponMock.Object));
            WeaponLayerMock.Verify(x => x.CanEquip(weaponMock.Object), Times.Exactly(2));
        }

        [TestMethod]
        public void Wearing()
        {
            var armorMock = new Mock<IItem>();
            var weaponMock = new Mock<IItem>();

            var armorLayerIndex = 2;
            var armorLayerMock = ArmorLayerMocks[armorLayerIndex];

            Assert.IsFalse(Outfit.CanWear(armorMock.Object));
            Assert.IsFalse(Outfit.Wear(armorMock.Object));

            armorLayerMock.Setup(x => x.CanEquip(armorMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.CanWear(armorMock.Object));
            int i = 0;
            for (i = 0; i < NumArmorLayers; i++)
            {
                int expectedCalls = i <= armorLayerIndex ? 3 : 2;
                ArmorLayerMocks[i].Verify(x => x.CanEquip(armorMock.Object), Times.Exactly(expectedCalls));
            }

            Assert.IsFalse(Outfit.CanWield(weaponMock.Object));
            Assert.IsFalse(Outfit.Wield(weaponMock.Object));
            WeaponLayerMock.Setup(x => x.CanEquip(weaponMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.CanWield(weaponMock.Object));
            WeaponLayerMock.Verify(x => x.CanEquip(weaponMock.Object), Times.Exactly(2));

            i = 0;
            foreach (var layerMock in ArmorLayerMocks)
            {
                int expectedCalls = i++ == armorLayerIndex ? 3 : 0;
                layerMock.Verify(x => x.Equip(It.IsAny<IItem>()), Times.Exactly(0));
            }
            WeaponLayerMock.Verify(x => x.Equip(It.IsAny<IItem>()), Times.Exactly(1));

            Assert.IsTrue(Outfit.CanWear(armorMock.Object));
            Assert.IsFalse(Outfit.Wear(armorMock.Object));

            armorLayerMock.Setup(x => x.Equip(armorMock.Object)).Returns(true);
            Assert.IsTrue(Outfit.Wear(armorMock.Object));


            Assert.IsFalse(Outfit.Wield(weaponMock.Object));
            WeaponLayerMock.Setup(x => x.Equip(weaponMock.Object)).Returns(true);

            var partMock = new Mock<IBodyPart>();
            WeaponLayerMock.Setup(x => x.FindParts(weaponMock.Object)).Returns(
                new List<IBodyPart> { partMock.Object }
                );

            Assert.IsTrue(Outfit.Wield(weaponMock.Object));
            partMock.VerifySet(x => x.Weapon = weaponMock.Object, Times.Once());
            
            for (i = 0; i < NumArmorLayers; i++)
            {
                int expectedCalls = i == armorLayerIndex ? 2 : 0;
                ArmorLayerMocks[i].Verify(x => x.Equip(armorMock.Object), Times.Exactly(expectedCalls));
            }
            WeaponLayerMock.Verify(x => x.Equip(It.IsAny<IItem>()), Times.Exactly(3));


            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Verify(x => x.Unequip(It.IsAny<IItem>()), Times.Never());
            }
            WeaponLayerMock.Verify(x => x.Unequip(It.IsAny<IItem>()), Times.Never());

            Outfit.TakeOff(armorMock.Object);

            foreach (var layerMock in ArmorLayerMocks)
            {
                layerMock.Verify(x => x.Unequip(It.IsAny<IItem>()), Times.Exactly(1));
            }

            Outfit.Unwield(weaponMock.Object);

            partMock.VerifySet(x => x.Weapon = null, Times.Once());
            WeaponLayerMock.Verify(x => x.Unequip(It.IsAny<IItem>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetWeaponItem()
        {
            var partMock = new Mock<IBodyPart>();
            var itemMock = new Mock<IItem>();
            WeaponLayerMock.Setup(x => x.GetItems(partMock.Object)).Returns( new List<IItem>{itemMock.Object});


            Assert.AreSame(itemMock.Object, Outfit.GetWeaponItem(partMock.Object));
        }
    }
}
