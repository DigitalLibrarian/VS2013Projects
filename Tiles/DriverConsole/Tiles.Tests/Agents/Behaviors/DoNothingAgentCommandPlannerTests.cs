using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class DoNothingAgentCommandPlannerTests
    {
        [TestMethod]
        public void Plan()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandsMock = new Mock<IEnumerable<IAgentCommand>>();
            var commandFactoryMock = new Mock<IAgentCommandFactory>();
            commandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(commandsMock.Object);

            var planner = new DoNothingAgentCommandPlanner(commandFactoryMock.Object);
            var result = planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreSame(commandsMock.Object, result);
            commandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once);
        }
    }
}
