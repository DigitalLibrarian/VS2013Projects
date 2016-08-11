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
        public void Update_QueueSpillOver()
        {
            long timeSlice = 100;
            var gameMock = new Mock<IGame>();
            gameMock.Setup(x => x.DesiredFrameLength).Returns(timeSlice);
            var agentMock = new Mock<IAgent>();

            // Update numbers are labeled
            var commandMock1 = new Mock<IAgentCommand>();
            commandMock1.Setup(x => x.RequiredTime).Returns(50); // 1
            var commandMock2 = new Mock<IAgentCommand>();
            commandMock2.Setup(x => x.RequiredTime).Returns(251); // 1, 2, 3, 4
            var commandMock3 = new Mock<IAgentCommand>();
            commandMock3.Setup(x => x.RequiredTime).Returns(99); // 4, 5

            IAgentCommand currentCommand = null;
            Mock<IAgentCommand> nextCommandMock = commandMock1;

            PlannerMock.Setup(x => x.PlanBehavior(gameMock.Object, agentMock.Object)).Returns(
                (IGame game, IAgent agent) =>
                {
                    return nextCommandMock.Object;
                }
                );

            ContextMock.Setup(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>())).Callback(
                (IGame game, IAgentCommand command) =>
                {
                    currentCommand = command;
                });
            ContextMock.Setup(x => x.HasCommand).Returns(() => currentCommand != null);

            long pump1Time = timeSlice;
            long pump2Time = pump1Time - commandMock1.Object.RequiredTime;
            long pump3Time = timeSlice;
            long pump4Time = timeSlice;
            long pump5Time = timeSlice;
            long pump6Time = timeSlice - 1;

            var execs = new [] 
            {
                new { 
                    ExpectedTime = pump1Time,
                    FinishCommand = true,
                    NextCommandMock = commandMock2,
                    TimeUsed = commandMock1.Object.RequiredTime
                },
                new { 
                    ExpectedTime = pump2Time,
                    FinishCommand = false,
                    NextCommandMock = (Mock<IAgentCommand>) null,
                    TimeUsed = 50L
                },
                new { 
                    ExpectedTime = pump3Time,
                    FinishCommand = false,
                    NextCommandMock = (Mock<IAgentCommand>) null,
                    TimeUsed = timeSlice
                },
                new { 
                    ExpectedTime = pump4Time,
                    FinishCommand = false,
                    NextCommandMock = (Mock<IAgentCommand>) null,
                    TimeUsed = timeSlice
                },
                new { 
                    ExpectedTime = pump5Time,
                    FinishCommand = true,
                    NextCommandMock = commandMock3,
                    TimeUsed = 1L
                },
                new { 
                    ExpectedTime = pump6Time,
                    FinishCommand = false,
                    NextCommandMock = (Mock<IAgentCommand>) null,
                    TimeUsed = 99L
                }
            };
            int execIndex = 0;

            ContextMock.Setup(x => x.Execute(gameMock.Object, agentMock.Object, It.IsAny<long>())).Returns(
                (IGame game, IAgent agent, long t) =>
                {
                    var exec = execs[execIndex++];
                    Assert.AreEqual(exec.ExpectedTime, t);
                    if (exec.FinishCommand)
                    {
                        currentCommand = null;
                    }
                    nextCommandMock = exec.NextCommandMock;
                    return exec.TimeUsed;
                }
            );

            // First update should finish command #1 and start command #2
            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Exactly(2));
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock1.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock2.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock3.Object), Times.Never());

            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, timeSlice), Times.Once());

            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[0].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[1].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, It.IsAny<long>()), Times.Exactly(2));

            // Second update should contin=ue on command #2
            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Exactly(2));
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock1.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock2.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock3.Object), Times.Never());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>()), Times.Exactly(2));


            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[0].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[1].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[2].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, It.IsAny<long>()), Times.Exactly(3));

            // Third update continues
            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Exactly(2));
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock1.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock2.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock3.Object), Times.Never());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>()), Times.Exactly(2));

            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[0].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[1].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[2].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[3].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, It.IsAny<long>()), Times.Exactly(4));

            // Forth update continues
            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Exactly(3));
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock1.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock2.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock3.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>()), Times.Exactly(3));

            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[0].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[1].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[2].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[3].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[4].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, execs[5].ExpectedTime), Times.AtLeast(1));
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, It.IsAny<long>()), Times.Exactly(6));
        }


        [TestMethod]
        public void Update_QueueFits()
        {
            long timeSlice = 100;
            var gameMock = new Mock<IGame>();
            gameMock.Setup(x => x.DesiredFrameLength).Returns(timeSlice);
            var agentMock = new Mock<IAgent>();

            var commandMock1 = new Mock<IAgentCommand>();
            commandMock1.Setup(x => x.RequiredTime).Returns(51);
            var commandMock2 = new Mock<IAgentCommand>();
            commandMock2.Setup(x => x.RequiredTime).Returns(49);

            IAgentCommand currentCommand = null;
            Mock<IAgentCommand> nextCommandMock = commandMock1;

            PlannerMock.Setup(x => x.PlanBehavior(gameMock.Object, agentMock.Object)).Returns(
                (IGame game, IAgent agent) => {
                    return nextCommandMock.Object;
                    }
                );

            ContextMock.Setup(x => x.StartNewCommand(gameMock.Object, It.IsAny<IAgentCommand>())).Callback(
                (IGame game, IAgentCommand command) =>
                {
                    currentCommand = command;
                });
            ContextMock.Setup(x => x.HasCommand).Returns(() => currentCommand != null);

            long pump1Time = timeSlice;
            long pump2Time = timeSlice - commandMock1.Object.RequiredTime;


            ContextMock.Setup(x => x.Execute(gameMock.Object, agentMock.Object, pump1Time)).Returns(
                (IGame game, IAgent agent, long t) =>
                {
                    currentCommand = null;
                    nextCommandMock = commandMock2;
                    return commandMock1.Object.RequiredTime;
                });
            ContextMock.Setup(x => x.Execute(gameMock.Object, agentMock.Object, pump2Time)).Returns(
                (IGame game, IAgent agent, long t) =>
                {
                    currentCommand = null;
                    return commandMock2.Object.RequiredTime;
                });
            
            Behavior.Update(gameMock.Object, agentMock.Object);

            PlannerMock.Verify(x => x.PlanBehavior(gameMock.Object, agentMock.Object), Times.Exactly(2));
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock1.Object), Times.Once());
            ContextMock.Verify(x => x.StartNewCommand(gameMock.Object, commandMock2.Object), Times.Once());

            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, pump1Time), Times.Once());
            ContextMock.Verify(x => x.Execute(gameMock.Object, agentMock.Object, pump2Time), Times.Once());
        }

    }
}
