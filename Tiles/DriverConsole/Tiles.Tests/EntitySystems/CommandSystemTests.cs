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
using Tiles.Math;

namespace Tiles.Tests.EntitySystems
{
    [TestClass]
    public class CommandSystemTests
    {
        CommandSystem System { get; set; }

        Mock<IAgent> AgentMock { get; set; }
        Mock<IAgentBehavior> BehaviorMock { get; set; }

        CommandComponent CommandComponent { get; set; }
        AgentComponent AgentComponent { get; set; }
        AtlasPositionComponent AtlasPositionComponent { get; set; }

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

            CommandComponent = new CommandComponent(BehaviorMock.Object);
            AgentComponent = new AgentComponent(AgentMock.Object);

            var atlasPos = new Vector3(1, 2, 3);
            AtlasPositionComponent = new AtlasPositionComponent
            {
                PositionFunc = () => atlasPos
            };

            EntityManagerMock = new Mock<IEntityManager>();
            Entities = new List<IEntity>();
            EntityManagerMock.Setup(x => x.GetEntities(It.IsAny<IEnumerable<int>>()))
                .Returns(Entities);

            System.SetBox(new Box3(atlasPos, atlasPos));
        }

        [TestMethod]
        public void ComponentIds()
        {
            Assert.IsTrue(new int[]{
                ComponentTypes.Agent,
                ComponentTypes.Command,
                ComponentTypes.AtlasPosition
            }.SequenceEqual(System.ComponentIds));
        }

        [TestMethod]
        public void UpdateEntity()
        {
            var entityMock = new Mock<IEntity>();

            entityMock.Setup(x => x.GetComponent<AgentComponent>(ComponentTypes.Agent))
                .Returns(AgentComponent);

            entityMock.Setup(x => x.GetComponent<CommandComponent>(ComponentTypes.Command))
                .Returns(CommandComponent);

            entityMock.Setup(x => x.GetComponent<AtlasPositionComponent>(ComponentTypes.AtlasPosition))
                .Returns(AtlasPositionComponent);

            Entities.Add(entityMock.Object);

            System.Update(EntityManagerMock.Object, GameMock.Object);

            BehaviorMock.Verify(x => x.Update(GameMock.Object, AgentMock.Object), Times.Once());

        }
    }
}
