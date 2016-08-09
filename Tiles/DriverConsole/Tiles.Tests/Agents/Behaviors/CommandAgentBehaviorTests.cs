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
        Mock<IAgentCommandInterpreter> InterpreterMock { get; set; }
        CommandAgentBehavior Behavior { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            PlannerMock = new Mock<IAgentCommandPlanner>();
            InterpreterMock = new Mock<IAgentCommandInterpreter>();
            Behavior = new CommandAgentBehavior(PlannerMock.Object, InterpreterMock.Object);
        }

        [TestMethod]
        public void Update()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<AgentCommand>();

            PlannerMock.Setup(x => x.PlanBehavior(gameMock.Object, agentMock.Object)).Returns(commandMock.Object);

            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Once());
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object));
        }

    }
}
