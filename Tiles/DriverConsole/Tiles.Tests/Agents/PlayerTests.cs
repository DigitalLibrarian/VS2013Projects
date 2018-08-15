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
        Mock<IBody> BodyMock { get; set; }
        Mock<IInventory> InventoryMock { get; set; }
        Mock<IOutfit> OutfitMock { get; set; }
        Mock<IAgentCommandQueue> CommandQueueMock { get; set; }
        Mock<IAgentClass> AgentClassMock { get; set; }

        Player Player { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            StartPos = Vector3.Zero;
            AtlasMock = new Mock<IAtlas>();
            BodyMock = new Mock<IBody>();
            InventoryMock = new Mock<IInventory>();
            OutfitMock = new Mock<IOutfit>();
            CommandQueueMock= new Mock<IAgentCommandQueue>();
            AgentClassMock = new Mock<IAgentClass>();

            Player = new Player(
                AtlasMock.Object,  
                AgentClassMock.Object,
                StartPos, 
                BodyMock.Object, 
                InventoryMock.Object,
                OutfitMock.Object, 
                CommandQueueMock.Object);
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
        public void EnqueueCommands()
        {
            var commandMock1 = new Mock<IAgentCommand>();
            var commandMock2 = new Mock<IAgentCommand>();
            var commandMock3 = new Mock<IAgentCommand>();
            var commands = new List<IAgentCommand>{ commandMock1.Object, commandMock2.Object, commandMock3.Object };
            Player.EnqueueCommands(commands);

            foreach (var command in commands)
            {
                CommandQueueMock.Verify(x => x.Enqueue(command));
            }
        }
    }
}
