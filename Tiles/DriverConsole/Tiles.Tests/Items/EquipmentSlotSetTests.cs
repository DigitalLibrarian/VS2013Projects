using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Tests.Assertions;

namespace Tiles.Tests.Items
{
    [TestClass]
    public class EquipmentSlotSetTests
    {
        [TestMethod]
        public void EmptyBody()
        {
            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart>());

            var slotSet = new EquipmentSlotSet(bodyMock.Object);

            foreach (var weaponSlotObj in Enum.GetValues(typeof(WeaponSlot)))
            {
                var weaponSlot = (WeaponSlot)weaponSlotObj;
                Assert.IsFalse(slotSet.HasSlot(weaponSlot));
                Assert.IsFalse(slotSet.IsSlotFull(weaponSlot));
                Assert.IsNull(slotSet.Get(weaponSlot));

                Asserter.AssertException(() => slotSet.Empty(weaponSlot));
                Asserter.AssertException(() => slotSet.Fill(weaponSlot, new Mock<IWeapon>().Object));
            }

            foreach (var armorSlotObj in Enum.GetValues(typeof(ArmorSlot)))
            {
                var armorSlot = (ArmorSlot)armorSlotObj;
                Assert.IsFalse(slotSet.HasSlot(armorSlot));
                Assert.IsFalse(slotSet.IsSlotFull(armorSlot));
                Assert.IsNull(slotSet.Get(armorSlot));

                Asserter.AssertException(() => slotSet.Empty(armorSlot));
                Asserter.AssertException(() => slotSet.Fill(armorSlot, new Mock<IArmor>().Object));
            }

        }
        [TestMethod]
        public void BodyWithSlots()
        {
            var bodyMock = new Mock<IBody>();
            var partMock1 = new Mock<IBodyPart>();
            partMock1.Setup(x => x.WeaponSlot).Returns(WeaponSlot.Main);
            partMock1.Setup(x => x.ArmorSlot).Returns(ArmorSlot.RightHand);

            var partMock2 = new Mock<IBodyPart>();
            partMock1.Setup(x => x.WeaponSlot).Returns(WeaponSlot.None);
            partMock1.Setup(x => x.ArmorSlot).Returns(ArmorSlot.RightArm);

            var partMock3 = new Mock<IBodyPart>();
            partMock3.Setup(x => x.WeaponSlot).Returns(WeaponSlot.None);
            partMock3.Setup(x => x.ArmorSlot).Returns(ArmorSlot.Torso);

            var partMock4 = new Mock<IBodyPart>();
            partMock4.Setup(x => x.WeaponSlot).Returns(WeaponSlot.None);
            partMock4.Setup(x => x.ArmorSlot).Returns(ArmorSlot.None);

            var parts = new List<IBodyPart>{
                partMock1.Object, partMock2.Object, partMock3.Object, partMock4.Object
            };
            bodyMock.Setup(x => x.Parts).Returns(parts);

            var slotSet = new EquipmentSlotSet(bodyMock.Object);

            foreach (var part in parts)
            {
                Assert.AreEqual(part.WeaponSlot != WeaponSlot.None, slotSet.HasSlot(part.WeaponSlot));
                Assert.AreEqual(part.ArmorSlot != ArmorSlot.None, slotSet.HasSlot(part.ArmorSlot));
            }
        }
    }
}
