using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Items;

namespace Tiles.Tests.Items
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        public void IsWeapon_False()
        {
            var item = new Item(new Mock<IItemClass>().Object);
            Assert.IsFalse(item.IsWeapon);
        }

        [TestMethod]
        public void IsWeapon_True()
        {
            var itemClassMock = new Mock<IItemClass>();
            itemClassMock.Setup(x => x.WeaponClass).Returns(new Mock<IWeaponClass>().Object);
            var item = new Item(itemClassMock.Object);
            Assert.IsTrue(item.IsWeapon);
        }

        [TestMethod]
        public void IsArmor_False()
        {
            var item = new Item(new Mock<IItemClass>().Object);
            Assert.IsFalse(item.IsArmor);
        }

        [TestMethod]
        public void IsArmor_True()
        {
            var itemClassMock = new Mock<IItemClass>();
            itemClassMock.Setup(x => x.ArmorClass).Returns(new Mock<IArmorClass>().Object);
            var item = new Item(itemClassMock.Object);
            Assert.IsTrue(item.IsArmor);
        }
    }
}
