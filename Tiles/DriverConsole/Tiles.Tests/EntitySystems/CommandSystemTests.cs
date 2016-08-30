using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;
using Tiles.EntityComponents;
using Tiles.EntitySystems;

namespace Tiles.Tests.EntitySystems
{
    [TestClass]
    public class CommandSystemTests
    {
        CommandSystem System { get; set; }

        Mock<IAgent> AgentMock { get; set; }
        Mock<IAgentBehavior> BehaviorMock { get; set; }

        Mock<ICommandComponent> CommandComponentMock { get; set; }
        Mock<IAgentComponent> AgentComponentMock { get; set; }

        Mock<IEntityManager> EntityManagerMock { get; set; }
        List<IEntity> Entities { get; set; }

        Mock<IGame> GameMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            System = new CommandSystem();

            AgentMock = new Mock<IAgent>();
            GameMock = new Mock<IGame>();
            BehaviorMock = new Mock<IAgentBehavior>();

            CommandComponentMock = new Mock<ICommandComponent>();
            CommandComponentMock.Setup(x => x.Id).Returns(ComponentTypes.Command);
            CommandComponentMock.Setup(x => x.Behavior).Returns(BehaviorMock.Object);

            AgentComponentMock = new Mock<IAgentComponent>();
            AgentComponentMock.Setup(x => x.Id).Returns(ComponentTypes.Agent);
            AgentComponentMock.Setup(x => x.Agent).Returns(AgentMock.Object);

            EntityManagerMock = new Mock<IEntityManager>();
            Entities = new List<IEntity>();
            EntityManagerMock.Setup(x => x.GetEntities(It.IsAny<IEnumerable<int>>()))
                .Returns(Entities);
        }

        [TestMethod]
        public void UpdateEntity()
        {
            var entityMock = new Mock<IEntity>();

            entityMock.Setup(x => x.GetComponent<IAgentComponent>())
                .Returns(AgentComponentMock.Object);

            entityMock.Setup(x => x.GetComponent<ICommandComponent>())
                .Returns(CommandComponentMock.Object);

            Entities.Add(entityMock.Object);

            System.Update(EntityManagerMock.Object, GameMock.Object);

            BehaviorMock.Verify(x => x.Update(GameMock.Object, AgentMock.Object), Times.Once());

        }
    }
}
