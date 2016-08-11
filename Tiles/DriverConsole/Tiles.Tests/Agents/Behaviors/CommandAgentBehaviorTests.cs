using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Agents.Behaviors;
using Tiles.Agents;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class CommandAgentBehaviorTests
    {
        Mock<IAgentCommandPlanner> PlannerMock { get; set; }
        Mock<IAgentCommandExecutionContext> ContextMock { get; set; }
        CommandAgentBehavior Behavior { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            PlannerMock = new Mock<IAgentCommandPlanner>();
            ContextMock = new Mock<IAgentCommandExecutionContext>();
            Behavior = new CommandAgentBehavior(PlannerMock.Object, ContextMock.Object);
        }

        [TestMethod]
        public void Update_Planning()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            long timeSlice = 10;
            gameMock.Setup(x => x.DesiredFrameLength).Returns(timeSlice);

            ContextMock.Setup(x => x.HasCommand).Returns(false);
            PlannerMock.Setup(x => x.PlanBehavior(gameMock.Object, agentMock.Object)).Returns(commandMock.Object);

            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Once());

            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock.Object), Times.Once());
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, timeSlice), Times.Once());
        }

        [TestMethod]
        public void Update_Planned()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            long timeSlice = 10;
            gameMock.Setup(x => x.DesiredFrameLength).Returns(timeSlice);

            ContextMock.Setup(x => x.HasCommand).Returns(true);
            PlannerMock.Setup(x => x.PlanBehavior(gameMock.Object, agentMock.Object)).Returns(commandMock.Object);

            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Never());

            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>()), Times.Never());
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, timeSlice), Times.Once());

        }
    }
}
