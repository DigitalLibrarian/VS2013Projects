using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Tests.Agents
{
    [TestClass]
    public class AgentReaperTests
    {
        Mock<IAtlas> AtlasMock { get; set; }
        Mock<IActionReporter> ReporterMock { get; set; }
        Mock<IItemFactory> ItemFactoryMock { get; set; }

        AgentReaper Reaper { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            AtlasMock = new Mock<IAtlas>();
            ReporterMock = new Mock<IActionReporter>();
            ItemFactoryMock = new Mock<IItemFactory>();

            Reaper = new AgentReaper(AtlasMock.Object, ReporterMock.Object, ItemFactoryMock.Object);
        }

        [TestMethod]
        public void ReapAgent()
        {
            var invItems = new List<IItem>
            {
                new Mock<IItem>().Object,
                new Mock<IItem>().Object,
                new Mock<IItem>().Object,
            };
            var wornItems = new List<IItem>()
            {
                new Mock<IItem>().Object,
                new Mock<IItem>().Object,
                new Mock<IItem>().Object,
            };
            var inventoryMock = new Mock<IInventory>();
            inventoryMock.Setup(x => x.GetItems()).Returns(invItems);
            inventoryMock.Setup(x => x.GetWorn()).Returns(wornItems);

            var agentMock = new Mock<IAgent>();
            agentMock.Setup(x => x.Inventory).Returns(inventoryMock.Object);

            var agentGrasperPartMock = new Mock<IBodyPart>();
            var otherGraspeePartMock = new Mock<IBodyPart>();
            agentGrasperPartMock.Setup(x => x.IsGrasping).Returns(true);
            agentGrasperPartMock.Setup(x => x.Grasped).Returns(otherGraspeePartMock.Object);

            var otherGrasperPartMock = new Mock<IBodyPart>();
            var agentGraspeePartMock = new Mock<IBodyPart>();
            agentGraspeePartMock.Setup(x => x.IsBeingGrasped).Returns(true);
            agentGraspeePartMock.Setup(x => x.GraspedBy).Returns(otherGrasperPartMock.Object);

            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart> { agentGraspeePartMock.Object, agentGrasperPartMock.Object });
            agentMock.Setup(x => x.Body).Returns(bodyMock.Object);

            var agentPos = new Vector3(10, 10, 10);
            agentMock.Setup(x => x.Pos).Returns(agentPos);
            var tileItems = new List<IItem>();
            var tileMock = new Mock<ITile>();
            tileMock.Setup(x => x.Items).Returns(tileItems);
            AtlasMock.Setup(x => x.GetTileAtPos(agentPos)).Returns(tileMock.Object);

            var result = Reaper.Reap(agentMock.Object);
            Assert.AreEqual(invItems.Count() + wornItems.Count() + 1, result.Count());
            Assert.AreEqual(invItems.Count() + wornItems.Count() + 1, tileItems.Count());
                        
            foreach (var item in invItems)
            {
                inventoryMock.Verify(x => x.RemoveItem(item), Times.Once());
                Assert.IsTrue(tileItems.Contains(item));
            }

            foreach (var item in wornItems)
            {
                inventoryMock.Verify(x => x.RemoveItem(item), Times.Once());
                Assert.IsTrue(tileItems.Contains(item));
            }

            ReporterMock.Verify(x => x.ReportDeath(agentMock.Object), Times.Once());
            ItemFactoryMock.Verify(x => x.CreateCorpse(agentMock.Object), Times.Once());

            agentGrasperPartMock.Verify(x => x.StopGrasp(otherGraspeePartMock.Object), Times.Once());
            otherGrasperPartMock.Verify(x => x.StopGrasp(agentGraspeePartMock.Object), Times.Once());
        }

        [TestMethod]
        public void ReapAgentBodyPart()
        {
            var agentGrasperPartMock = new Mock<IBodyPart>();
            var otherGraspeePartMock = new Mock<IBodyPart>();
            agentGrasperPartMock.Setup(x => x.IsGrasping).Returns(true);
            agentGrasperPartMock.Setup(x => x.Grasped).Returns(otherGraspeePartMock.Object);

            var otherGrasperPartMock = new Mock<IBodyPart>();
            var agentGraspeePartMock = new Mock<IBodyPart>();
            agentGraspeePartMock.Setup(x => x.IsBeingGrasped).Returns(true);
            agentGraspeePartMock.Setup(x => x.GraspedBy).Returns(otherGrasperPartMock.Object);

            var agentMock = new Mock<IAgent>();
            var bodyMock = new Mock<IBody>();
            bodyMock.Setup(x => x.Parts).Returns(new List<IBodyPart> { agentGrasperPartMock.Object, agentGraspeePartMock.Object });
            agentMock.Setup( x => x.Body).Returns(bodyMock.Object);

            var agentPos = new Vector3(10, 10, 10);
            agentMock.Setup(x => x.Pos).Returns(agentPos);
            var tileItems = new List<IItem>();
            var tileMock = new Mock<ITile>();
            tileMock.Setup(x => x.Items).Returns(tileItems);
            AtlasMock.Setup(x => x.GetTileAtPos(agentPos)).Returns(tileMock.Object);

            var weaponItemMock = new Mock<IItem>();
            var armorItemMock = new Mock<IItem>();
            
            var inventoryMock = new Mock<IInventory>();
            agentMock.Setup(x => x.Inventory).Returns(inventoryMock.Object);
            agentGrasperPartMock.Setup(x => x.Weapon).Returns(weaponItemMock.Object);
            agentGrasperPartMock.Setup(x => x.Armor).Returns(armorItemMock.Object);
            inventoryMock.Setup(x => x.GetWorn(weaponItemMock.Object)).Returns(weaponItemMock.Object);
            inventoryMock.Setup(x => x.GetWorn(armorItemMock.Object)).Returns(armorItemMock.Object);

            var shedLimbItemMock = new Mock<IItem>();
            ItemFactoryMock.Setup(x => x.CreateShedLimb(agentMock.Object, agentGrasperPartMock.Object)).Returns(shedLimbItemMock.Object);


            var result = Reaper.Reap(agentMock.Object, agentGrasperPartMock.Object);

            Assert.AreEqual(3, result.Count());
            Assert.AreSame(weaponItemMock.Object, result.ElementAt(0));
            Assert.AreSame(armorItemMock.Object, result.ElementAt(1));
            Assert.AreSame(shedLimbItemMock.Object, result.ElementAt(2));

            agentGrasperPartMock.Verify(x => x.StopGrasp(otherGraspeePartMock.Object), Times.Once());
            otherGrasperPartMock.Verify(x => x.StopGrasp(agentGraspeePartMock.Object), Times.Never());

            inventoryMock.Verify(x => x.GetItems(), Times.Never());
            inventoryMock.Verify(x => x.GetWorn(), Times.Never());

            inventoryMock.Verify(x => x.RemoveItem(weaponItemMock.Object), Times.Once());
            inventoryMock.Verify(x => x.RemoveItem(armorItemMock.Object), Times.Once());

            ItemFactoryMock.Verify(x => x.CreateShedLimb(agentMock.Object, agentGrasperPartMock.Object), Times.Once());
        }
    }
}
