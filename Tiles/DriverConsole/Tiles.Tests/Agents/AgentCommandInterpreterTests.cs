using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tiles;
using Tiles.Items;
using Tiles.Agents;
using Tiles.Bodies;
using Moq;
using Tiles.Math;
using Tiles.Tests.Assertions;
using Tiles.Structures;
using System.Collections.Generic;
namespace Tiles.Tests.Agents
{
    [TestClass]
    public class AgentCommandInterpreterTests
    {
        [TestMethod]
        public void Dispatch()
        {
            var attackIntMock = new Mock<IAgentCommandTypeInterpreter>();
            var moveIntMock = new Mock<IAgentCommandTypeInterpreter>();
            var typeMapMock = new Mock<IDictionary<AgentCommandType, IAgentCommandTypeInterpreter>>();
            typeMapMock.Setup(x => x.ContainsKey(AgentCommandType.AttackMelee)).Returns(true);
            typeMapMock.Setup(x => x[AgentCommandType.AttackMelee]).Returns(attackIntMock.Object);
            typeMapMock.Setup(x => x.ContainsKey(AgentCommandType.Move)).Returns(true);
            typeMapMock.Setup(x => x[AgentCommandType.Move]).Returns(moveIntMock.Object);

            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();

            var interpreter = new AgentCommandInterpreter(typeMapMock.Object);

            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.CommandType).Returns(AgentCommandType.AttackMelee); ;
            interpreter.Execute(gameMock.Object, agentMock.Object, commandMock.Object);

            typeMapMock.Verify(x => x[AgentCommandType.AttackMelee], Times.Once());
            attackIntMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Once());
            moveIntMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, commandMock.Object), Times.Never());
        }

        [TestMethod]
        public void UnknownCommandType()
        {
            var typeMapMock = new Mock<IDictionary<AgentCommandType, IAgentCommandTypeInterpreter>>();
            typeMapMock.Setup(x => x.ContainsKey(AgentCommandType.None)).Returns(false);
            var interpreter = new AgentCommandInterpreter(typeMapMock.Object);
            var commandMock = new Mock<IAgentCommand>();
            commandMock.Setup(x => x.CommandType).Returns(AgentCommandType.None);

            NotImplementedException caughtException = null;
            try
            {
                interpreter.Execute(new Mock<IGame>().Object, new Mock<IAgent>().Object, commandMock.Object);
            }
            catch (NotImplementedException e)
            {
                caughtException = e;
            }
            finally
            {
                Assert.IsNotNull(caughtException);
                typeMapMock.Verify(x => x.ContainsKey(AgentCommandType.None), Times.Once());
            }
        }
    }
}

