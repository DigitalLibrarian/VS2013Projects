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
    public class OutfitLayerTests
    {
        enum TestSlot { None, One, Two, Three}

        Mock<IBodyPart> PartMock1 { get; set; }
        Mock<IBodyPart> PartMock2 { get; set; }
        Mock<IBodyPart> PartMock3 { get; set; }

        Mock<IBody> BodyMock { get; set; }

        Dictionary<IItem, bool> SuitableDb { get; set; }
        Dictionary<IBodyPart, TestSlot> PartSlotsDb { get; set; }
        Dictionary<IItem, IEnumerable<TestSlot>> RequiredSlotsDb { get; set; }

        OutfitLayer<TestSlot> Layer { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            PartMock1 = new Mock<IBodyPart>();
            PartMock2 = new Mock<IBodyPart>();
            PartMock3 = new Mock<IBodyPart>();

            BodyMock = new Mock<IBody>();
            BodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart>
                {
                    PartMock1.Object,
                    PartMock2.Object,
                    PartMock3.Object,
                });

            SuitableDb = new Dictionary<IItem, bool>();
            PartSlotsDb = new Dictionary<IBodyPart, TestSlot>();
            RequiredSlotsDb = new Dictionary<IItem, IEnumerable<TestSlot>>();

            Layer = new OutfitLayer<TestSlot>(BodyMock.Object,
                item => SuitableDb[item],
                part => PartSlotsDb[part],
                item => RequiredSlotsDb[item]
                );
        }
        [TestMethod]
        public void GetItems_None()
        {
            var result = Layer.GetItems();

            Assert.IsFalse(result.Any());

            foreach (var partMock in new[] { PartMock1, PartMock2, PartMock3 })
            {
                Assert.IsFalse(Layer.GetItems(partMock.Object).Any());
            }
        }

        [TestMethod]
        public void GetItems()
        {
            var itemMock1 = new Mock<IItem>();
            var itemMock2 = new Mock<IItem>();
            SuitableDb[itemMock1.Object] = true;
            SuitableDb[itemMock2.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock1.Object] = new List<TestSlot> { TestSlot.One };
            RequiredSlotsDb[itemMock2.Object] = new List<TestSlot> { TestSlot.Two };


            Assert.IsTrue(Layer.Equip(itemMock1.Object));
            Assert.IsTrue(Layer.Equip(itemMock2.Object));

            var result = Layer.GetItems();
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Contains(itemMock1.Object));
            Assert.IsTrue(result.Contains(itemMock2.Object));

            result = Layer.GetItems(PartMock1.Object);
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(itemMock1.Object));

            result = Layer.GetItems(PartMock2.Object);
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains(itemMock2.Object));

            result = Layer.GetItems(PartMock3.Object);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Equip_Cannot()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = false;

            Assert.IsFalse(Layer.Equip(itemMock.Object));
        }

        [TestMethod]
        public void IsEquipped_False()
        {
            Assert.IsFalse(Layer.IsEquipped(new Mock<IItem>().Object));
        }

        [TestMethod]
        public void IsEquipped_True()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One };

            Layer.Equip(itemMock.Object);

            Assert.IsTrue(Layer.IsEquipped(itemMock.Object));
            Layer.Unequip(itemMock.Object);
            Assert.IsFalse(Layer.IsEquipped(itemMock.Object));
        }

        
        [TestMethod]
        public void CanEquip_NotSuitable()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = false;

            Assert.IsFalse(Layer.CanEquip(itemMock.Object));
        }

        [TestMethod]
        public void CanEquip_DontHaveRequiredSlots_NoDuplicates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;
            PartSlotsDb[PartMock1.Object] = TestSlot.None;
            PartSlotsDb[PartMock2.Object] = TestSlot.None;
            PartSlotsDb[PartMock3.Object] = TestSlot.Two;
            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One, TestSlot.Two };

            var result = Layer.CanEquip(itemMock.Object);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanEquip_DontHaveRequiredSlots_Duplicates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;
            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.None;
            PartSlotsDb[PartMock3.Object] = TestSlot.Two;
            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One, TestSlot.One, TestSlot.Two };

            var result = Layer.CanEquip(itemMock.Object);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanEquip_SomeRequiredSlotsInUse_NoDuplicates()
        {

            var itemMock = new Mock<IItem>();
            var itemInUseMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;
            SuitableDb[itemInUseMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One, TestSlot.Two };
            RequiredSlotsDb[itemInUseMock.Object] = new List<TestSlot> { TestSlot.One };

            Assert.IsTrue(Layer.CanEquip(itemMock.Object));
            Assert.IsTrue(Layer.Equip(itemInUseMock.Object));
            Assert.IsFalse(Layer.CanEquip(itemMock.Object));
        }

        [TestMethod]
        public void CanEquip_SomeRequiredSlotsInUse_Duplicates()
        {

            var itemMock = new Mock<IItem>();
            var itemInUseMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;
            SuitableDb[itemInUseMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.One;
            PartSlotsDb[PartMock3.Object] = TestSlot.Two;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One, TestSlot.One, TestSlot.Two };
            RequiredSlotsDb[itemInUseMock.Object] = new List<TestSlot> { TestSlot.One };

            Assert.IsTrue(Layer.CanEquip(itemMock.Object));
            Assert.IsTrue(Layer.Equip(itemInUseMock.Object));
            Assert.IsFalse(Layer.CanEquip(itemMock.Object));
        }

        [TestMethod]
        public void FindParts_Unsuitable()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = false;

            var result = Layer.FindParts(itemMock.Object);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void FindParts_Hit_NoDuplicates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.Two, TestSlot.Three };

            var result = Layer.FindParts(itemMock.Object);

            Assert.AreEqual(2, result.Count());
            Assert.AreSame(PartMock2.Object, result.ElementAt(0));
            Assert.AreSame(PartMock3.Object, result.ElementAt(1));
        }

        [TestMethod]
        public void FindParts_Hit_Duplicates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.One;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> {TestSlot.One, TestSlot.One, TestSlot.Three };

            var result = Layer.FindParts(itemMock.Object);

            Assert.AreEqual(3, result.Count());
            Assert.AreSame(PartMock1.Object, result.ElementAt(0));
            Assert.AreSame(PartMock2.Object, result.ElementAt(1));
            Assert.AreSame(PartMock3.Object, result.ElementAt(2));
        }

        [TestMethod]
        public void FindParts_Miss()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.One;
            PartSlotsDb[PartMock3.Object] = TestSlot.One;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.Two, TestSlot.Three };

            var result = Layer.FindParts(itemMock.Object);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void HaveAllRequiredSlots_Unsuitable()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = false;

            Assert.IsFalse(Layer.HaveAllRequiredSlots(itemMock.Object));
        }

        [TestMethod]
        public void HaveAllRequiredSlots_Miss()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.One;
            PartSlotsDb[PartMock3.Object] = TestSlot.One;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.Two, TestSlot.Three };

            Assert.IsFalse(Layer.HaveAllRequiredSlots(itemMock.Object));
        }

        [TestMethod]
        public void HaveAllRequiredSlots_Hit_NoDuplicates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Three;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.Two, TestSlot.Three };

            Assert.IsTrue(Layer.HaveAllRequiredSlots(itemMock.Object));
        }

        [TestMethod]
        public void HaveAllRequiredSlots_Hit_Duplcates()
        {
            var itemMock = new Mock<IItem>();
            SuitableDb[itemMock.Object] = true;

            PartSlotsDb[PartMock1.Object] = TestSlot.One;
            PartSlotsDb[PartMock2.Object] = TestSlot.Two;
            PartSlotsDb[PartMock3.Object] = TestSlot.Two;

            RequiredSlotsDb[itemMock.Object] = new List<TestSlot> { TestSlot.One, TestSlot.Two, TestSlot.Two };

            Assert.IsTrue(Layer.HaveAllRequiredSlots(itemMock.Object));
        }

        
        [TestMethod]
        public void IsSuitable()
        {
            IItem itemPassed = null;
            Predicate<IItem> pred = new Predicate<IItem>(item =>
            {
                itemPassed = item;
                return true;
            });

            var itemMock = new Mock<IItem>();
            var layer = new OutfitLayer<TestSlot>(BodyMock.Object, pred, null, null);

            Assert.IsTrue(layer.IsSuitable(itemMock.Object));
            Assert.AreSame(itemMock.Object, itemPassed);
        }

        [TestMethod]
        public void PartSlot()
        {
            var slot = TestSlot.Three;
            IBodyPart partPassed = null;
            var func = new Func<IBodyPart, TestSlot>(part =>
            {
                partPassed = part;
                return slot;
            });

            var partMock = new Mock<IBodyPart>();
            var layer = new OutfitLayer<TestSlot>(BodyMock.Object, null, func, null);

            Assert.AreEqual(TestSlot.Three, layer.PartSlot(partMock.Object));
            Assert.AreSame(partMock.Object, partPassed);
        }


        [TestMethod]
        public void RequiredSlots()
        {
            IEnumerable<TestSlot> slots = new List<TestSlot>();
            IItem itemPassed = null;
            var func = new Func<IItem, IEnumerable<TestSlot>>(item =>
            {
                itemPassed = item;
                return slots;
            });

            var itemMock = new Mock<IItem>();
            var layer = new OutfitLayer<TestSlot>(BodyMock.Object, null, null, func);

            Assert.AreSame(slots, layer.RequiredSlots(itemMock.Object));
        }
    }
}
