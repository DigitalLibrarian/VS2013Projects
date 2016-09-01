﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.Tests.Items
{
    [TestClass]
    public class ItemFactoryTests
    {
        [TestMethod]
        public void Create_FromItemClass() 
        {
            var classMock = new Mock<IItemClass>();
            var factory = new ItemFactory();
            var item = factory.Create(classMock.Object);
            Assert.IsNotNull(item);
            Assert.IsNotNull(item.Class);
            Assert.AreSame(classMock.Object, item.Class);
        }

        [TestMethod]
        public void CreateShedLimb()
        {
            var agentMock = new Mock<IAgent>();
            var partMock = new Mock<IBodyPart>();
            int partSize = 11;
            partMock.Setup(x => x.Size)
                .Returns(partSize);

            var agentName = "Waldo";
            agentMock.Setup(x => x.Name).Returns(agentName);
            var partName = "crusty nose";
            partMock.Setup(x => x.Name).Returns(partName);

            var factory = new ItemFactory();
            var item = factory.CreateShedLimb(agentMock.Object, partMock.Object);

            Assert.IsNotNull(item);
            Assert.IsNotNull(item.Class);
            Assert.IsTrue(item.Class.Name.Contains(agentName));
            Assert.IsTrue(item.Class.Name.Contains(partName));
            Assert.IsNotNull(item.Class.Sprite);
            Assert.IsNotNull(item.Class.WeaponClass);
            Assert.IsTrue(item.Class.WeaponClass.AttackMoveClasses.Any());
            Assert.AreEqual(partSize, item.Class.Size);
        }

        [TestMethod]
        public void CreateCorpse()
        {
            var agentMock = new Mock<IAgent>();
            var agentName = "Waldo";
            agentMock.Setup(x => x.Name).Returns(agentName);
            
            var bodyMock = new Mock<IBody>();
            int bodySize = 10;
            bodyMock.Setup(x => x.Size)
                .Returns(bodySize);
            agentMock.Setup(x => x.Body).Returns(bodyMock.Object);

            var factory = new ItemFactory();
            var item = factory.CreateCorpse(agentMock.Object);


            Assert.IsNotNull(item);
            Assert.IsNotNull(item.Class);
            Assert.IsTrue(item.Class.Name.Contains(agentName));
            Assert.IsNotNull(item.Class.Sprite);
            Assert.IsNotNull(item.Class.WeaponClass);
            Assert.IsTrue(item.Class.WeaponClass.AttackMoveClasses.Any());
            Assert.AreEqual(bodySize, item.Class.Size);
        }
    }
}
