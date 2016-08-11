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
        Mock<IAgentCommandFactory> CommandFactoryMock { get; set; }
        QueueAgentCommandPlanner Planner { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            CommandFactoryMock = new Mock<IAgentCommandFactory>();
            Planner = new QueueAgentCommandPlanner(RandomMock.Object, CommandFactoryMock.Object);
        }

        [TestMethod]
        public void PlanBehavior_ClassInvariants()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();

            var commandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(commandMock.Object);
            var command = Planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreSame(commandMock.Object, command);
            
            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once());
        }

        [TestMethod]
        public void PlanBehavior_Queue()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock1 = new Mock<IAgentCommand>();
            var commandMock2 = new Mock<IAgentCommand>();

            var queueProducer = Planner as IAgentCommandQueue;
            queueProducer.Enqueue(commandMock1.Object);
            queueProducer.Enqueue(commandMock2.Object);

            Assert.AreSame(commandMock1.Object, Planner.PlanBehavior(gameMock.Object, agentMock.Object));
            Assert.AreSame(commandMock2.Object, Planner.PlanBehavior(gameMock.Object, agentMock.Object));


            var fallBackCommandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Never());
            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(fallBackCommandMock.Object);

            var command = Planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreNotSame(commandMock1.Object, command);
            Assert.AreNotSame(commandMock2.Object, command);
            Assert.AreSame(fallBackCommandMock.Object, command);
            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once());
        }
    }
}