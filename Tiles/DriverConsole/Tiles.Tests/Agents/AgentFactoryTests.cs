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
using Tiles.Ecs;
using Tiles.Math;
using Tiles.Tests.Assertions;

namespace Tiles.Tests.Agents
{
    [TestClass]
    public class AgentFactoryTests
    {
        Mock<IEntityManager> EntityManagerMock { get; set; }
        Mock<IBodyFactory> BodyFactoryMock { get; set; }

        AgentFactory Factory { get; set;}

        [TestInitialize]
        public void Initialize()
        {
            EntityManagerMock = new Mock<IEntityManager>();
            BodyFactoryMock = new Mock<IBodyFactory>();
            Factory = new AgentFactory(EntityManagerMock.Object, BodyFactoryMock.Object);

            EntityManagerMock.Setup(x => x.CreateEntity())
                .Returns(() => new Mock<IEntity>().Object);
        }

        [TestMethod]
        public void Create()
        {
            var atlasMock = new Mock<IAtlas>();
            var plannerMock = new Mock<IAgentCommandPlanner>();

            var bodyClassMock = new Mock<IBodyClass>();
            var agentClassMock = new Mock<IAgentClass>();
            agentClassMock.Setup(x => x.BodyClass).Returns(bodyClassMock.Object);

            var bodyMock = new Mock<IBody>();
            BodyFactoryMock.Setup(x => x.Create(bodyClassMock.Object)).Returns(bodyMock.Object);

            var pos = new Vector3(1, 2, 3);

            var result = Factory.Create(atlasMock.Object, agentClassMock.Object, pos, plannerMock.Object);
            Assert.IsNotNull(result);
            Asserter.AreEqual(pos, result.Pos);
            Assert.IsNotNull(result.AgentBehavior);
            Assert.IsNotNull(result.Body);
            Assert.IsNotNull(result.CommandQueue);
            Assert.IsNotNull(result.Inventory);
            Assert.AreSame(bodyMock.Object, result.Body);
        }
    }
}
