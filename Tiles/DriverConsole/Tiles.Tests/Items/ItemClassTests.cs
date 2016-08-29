using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tiles.Materials;
using Tiles.Items;

namespace Tiles.Tests.Items
{
    [TestClass]
    public class ItemClassTests
    {
        [TestMethod]
        public void WeaponConstructor()
        {
            string name = "name";
            var spriteMock = new Mock<ISprite>();
            var materialMock = new Mock<IMaterial>();
            var weaponClassMock = new Mock<IWeaponClass>();

            var itemClass = new ItemClass(name, spriteMock.Object, materialMock.Object, weaponClassMock.Object);

            Assert.AreSame(name, itemClass.Name);
            Assert.AreSame(spriteMock.Object, itemClass.Sprite);
            Assert.AreSame(materialMock.Object, itemClass.Material);
            Assert.AreSame(weaponClassMock.Object, itemClass.WeaponClass);
            Assert.IsNull(itemClass.ArmorClass);
        }
        
        [TestMethod]
        public void ArmorConstructor()
        {
            string name = "name";
            var spriteMock = new Mock<ISprite>();
            var materialMock = new Mock<IMaterial>();
            var armorClassMock = new Mock<IArmorClass>();

            var itemClass = new ItemClass(name, spriteMock.Object, materialMock.Object, armorClassMock.Object);

            Assert.AreSame(name, itemClass.Name);
            Assert.AreSame(spriteMock.Object, itemClass.Sprite);
            Assert.AreSame(materialMock.Object, itemClass.Material);
            Assert.AreSame(armorClassMock.Object, itemClass.ArmorClass);
            Assert.IsNull(itemClass.WeaponClass);
        }

        [TestMethod]
        public void FullConstructor()
        {
            string name = "name";
            var spriteMock = new Mock<ISprite>();
            var materialMock = new Mock<IMaterial>();
            var weaponClassMock = new Mock<IWeaponClass>();
            var armorClassMock = new Mock<IArmorClass>();

            var itemClass = new ItemClass(name, spriteMock.Object, materialMock.Object, weaponClassMock.Object, armorClassMock.Object);

            Assert.AreSame(name, itemClass.Name);
            Assert.AreSame(spriteMock.Object, itemClass.Sprite);
            Assert.AreSame(materialMock.Object, itemClass.Material);
            Assert.AreSame(weaponClassMock.Object, itemClass.WeaponClass);
            Assert.AreSame(armorClassMock.Object, itemClass.ArmorClass);

        }
    }
}
