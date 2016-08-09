using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Agents.Behaviors;
using Tiles.Agents;
using Tiles.Random;
using Tiles.Tests.Assertions;
using Tiles.Math;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class QueueAgentCommandPlannerTests
    {
        Mock<IRandom> RandomMock { get; set;}
        QueueAgentCommandPlanner Planner { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            Planner = new QueueAgentCommandPlanner(RandomMock.Object);
        }

        [TestMethod]
        public void PlanBehavior_ClassInvariants()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var command = Planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void PlanBehavior_Queue()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock1 = new Mock<AgentCommand>();
            var commandMock2 = new Mock<AgentCommand>();

            var queueProducer = Planner as IAgentCommandQueueProducer;
            queueProducer.Enqueue(commandMock1.Object);
            queueProducer.Enqueue(commandMock2.Object);

            Assert.AreSame(commandMock1.Object, Planner.PlanBehavior(gameMock.Object, agentMock.Object));
            Assert.AreSame(commandMock2.Object, Planner.PlanBehavior(gameMock.Object, agentMock.Object));

            var command = Planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreNotSame(commandMock1.Object, command);
            Assert.AreNotSame(commandMock2.Object, command);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }


    }
}