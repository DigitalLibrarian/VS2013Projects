using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Items.Outfits;
using Tiles.Math;

namespace Tiles.Tests.Agents
{
    [TestClass]
    public class PlayerTests
    {
        Vector3 StartPos { get; set; }
        Mock<IAtlas> AtlasMock { get; set; }
        Mock<ISprite> SpriteMock { get; set; }
        Mock<IBody> BodyMock { get; set; }
        Mock<IInventory> InventoryMock { get; set; }
        Mock<IOutfit> OutfitMock { get; set; }
        Mock<IAgentCommandQueue> CommandQueueMock { get; set; }

        Player Player { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            StartPos = Vector3.Zero;
            AtlasMock = new Mock<IAtlas>();
            SpriteMock = new Mock<ISprite>();
            BodyMock = new Mock<IBody>();
            InventoryMock = new Mock<IInventory>();
            OutfitMock = new Mock<IOutfit>();
            CommandQueueMock= new Mock<IAgentCommandQueue>();

            Player = new Player(AtlasMock.Object, SpriteMock.Object, StartPos, BodyMock.Object, InventoryMock.Object, OutfitMock.Object, CommandQueueMock.Object);
        }

        [TestMethod]
        public void Agent()
        {
            Assert.AreSame(Player, Player.Agent);
        }

        [TestMethod]
        public void IsPlayer()
        {
            Assert.IsTrue(Player.IsPlayer);
            Assert.IsTrue(Player.Agent.IsPlayer);
        }
        
        [TestMethod]
        public void HasCommands()
        {
            CommandQueueMock.Setup(x => x.Any()).Returns(false);
            Assert.IsFalse(Player.HasCommands);
            CommandQueueMock.Setup(x => x.Any()).Returns(true);
            Assert.IsTrue(Player.HasCommands);
        }


        [TestMethod]
        public void EnqueueCommand()
        {
            Assert.IsNull(Player.LastCommand);

            var commandMock = new Mock<IAgentCommand>();

            Player.EnqueueCommand(commandMock.Object);

            CommandQueueMock.Verify(x => x.Enqueue(commandMock.Object));
            Assert.AreSame(commandMock.Object, Player.LastCommand);
        }
    }
}
