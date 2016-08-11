using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Agents.Behaviors;

using Moq;
using Tiles.Agents;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class AgentExecutionContextTests
    {
        Mock<IAgentCommandInterpreter> InterpreterMock { get; set; }

        AgentCommandExecutionContext Context { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InterpreterMock = new Mock<IAgentCommandInterpreter>();
            Context = new AgentCommandExecutionContext(InterpreterMock.Object);
        }

        [TestMethod]
        void Invariants()
        {
            Assert.IsFalse(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void StartNewCommand()
        {
            var gameMock = new Mock<IGame>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(1);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void Execute_NoCommand()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();

            var result = Context.Execute(gameMock.Object, agentMock.Object, 1);

            Assert.AreEqual(0, result);
            InterpreterMock.Verify(x => x.Execute(It.IsAny<IGame>(), It.IsAny<IAgent>(), It.IsAny<IAgentCommand>()), Times.Never());

            Assert.IsFalse(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            var commandMock1 = new Mock<IAgentCommand>();
            Context.StartNewCommand(gameMock.Object, commandMock1.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void Execute_ZeroTimeCommand()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(0);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
            
            var result = Context.Execute(gameMock.Object, agentMock.Object, 1);

            Assert.AreEqual(0, result);
            Assert.IsFalse(Context.HasCommand);
            Assert.IsTrue(Context.Executed);

            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());

            var commandMock1 = new Mock<IAgentCommand>();
            Context.StartNewCommand(gameMock.Object, commandMock1.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void Execute_FractionalTime()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(1);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            var result = Context.Execute(gameMock.Object, agentMock.Object, 10);

            Assert.AreEqual(1, result);
            Assert.IsFalse(Context.HasCommand);
            Assert.IsTrue(Context.Executed);

            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());

            var commandMock1 = new Mock<IAgentCommand>();
            Context.StartNewCommand(gameMock.Object, commandMock1.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void Execute_ExactTime()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(10);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            var result = Context.Execute(gameMock.Object, agentMock.Object, 10);

            Assert.AreEqual(10, result);
            Assert.IsFalse(Context.HasCommand);
            Assert.IsTrue(Context.Executed);

            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());

            var commandMock1 = new Mock<IAgentCommand>();
            Context.StartNewCommand(gameMock.Object, commandMock1.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void Execute_LongTime()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(33);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            long result;

            result = Context.Execute(gameMock.Object, agentMock.Object, 10);
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Never());

            Assert.AreEqual(10, result);
            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            result = Context.Execute(gameMock.Object, agentMock.Object, 10);
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Never());

            Assert.AreEqual(10, result);
            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            result = Context.Execute(gameMock.Object, agentMock.Object, 10);
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Never());

            Assert.AreEqual(10, result);
            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);

            result = Context.Execute(gameMock.Object, agentMock.Object, 10);
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());

            Assert.AreEqual(3, result);
            Assert.IsFalse(Context.HasCommand);
            Assert.IsTrue(Context.Executed);

            result = Context.Execute(gameMock.Object, agentMock.Object, 10);
            InterpreterMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());

            Assert.AreEqual(0, result);
            Assert.IsFalse(Context.HasCommand);
            Assert.IsTrue(Context.Executed);

            var commandMock1 = new Mock<IAgentCommand>();
            Context.StartNewCommand(gameMock.Object, commandMock1.Object);

            Assert.IsTrue(Context.HasCommand);
            Assert.IsFalse(Context.Executed);
        }

        [TestMethod]
        public void CommandCompletedEvent()
        {
            int eventCount = 0;
            Context.CommandComplete += new EventHandler((obj, args) => eventCount++);

            Assert.AreEqual(0, eventCount);

            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.RequiredTime).Returns(0);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);
            Assert.AreEqual(0, eventCount);

            var result = Context.Execute(gameMock.Object, agentMock.Object, 1);
            Assert.AreEqual(0, result);
            Assert.AreEqual(1, eventCount);

            commandMock.Setup(x => x.RequiredTime).Returns(3);

            Context.StartNewCommand(gameMock.Object, commandMock.Object);
            Assert.AreEqual(1, eventCount);

            result = Context.Execute(gameMock.Object, agentMock.Object, 2);
            Assert.AreEqual(2, result);
            Assert.AreEqual(1, eventCount);

            result = Context.Execute(gameMock.Object, agentMock.Object, 2);
            Assert.AreEqual(1, result);
            Assert.AreEqual(2, eventCount);
        }
    }
}
