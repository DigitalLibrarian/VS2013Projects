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
    public class AgentCommandQueueTests
    {
        AgentCommandQueue Queue { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Queue = new AgentCommandQueue();
        }

        [TestMethod]
        public void Any()
        {
            Assert.IsFalse(Queue.Any());
            var commandMock = new Mock<IAgentCommand>();
            Queue.Enqueue(commandMock.Object);
            Assert.IsTrue(Queue.Any());
        }

        [TestMethod]
        public void Enqueue()
        {
            var commandMock1 = new Mock<IAgentCommand>();
            var commandMock2 = new Mock<IAgentCommand>();
            var commandMock3 = new Mock<IAgentCommand>();

            Queue.Enqueue(commandMock1.Object);
            Queue.Enqueue(commandMock2.Object);
            Queue.Enqueue(commandMock3.Object);
        }

        [TestMethod]
        public void Next()
        {
            var commandMock1 = new Mock<IAgentCommand>();
            var commandMock2 = new Mock<IAgentCommand>();

            Queue.Enqueue(commandMock1.Object);
            Queue.Enqueue(commandMock2.Object);

            Assert.AreSame(commandMock1.Object, Queue.Next());
            Assert.AreSame(commandMock2.Object, Queue.Next());


            InvalidOperationException caughtException = null;
            try
            {
                Queue.Next();
            }
            catch (InvalidOperationException e)
            {
                caughtException = e;
            }
            finally
            {
                Assert.IsNotNull(caughtException);
            }
        }
    }
}