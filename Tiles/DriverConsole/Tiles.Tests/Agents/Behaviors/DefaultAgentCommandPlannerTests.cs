using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Random;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Agents.Behaviors;
using Tiles.Math;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class DefaultAgentCommandPlannerTests
    {
        Mock<IRandom> RandomMock { get; set; }
        Mock<IAgentCommandFactory> CommandFactoryMock { get; set; }
        Mock<ICombatMoveDiscoverer> MoveDiscoMock { get; set; }
        Mock<IPositionFinder> PosFinderMock { get; set; }

        DefaultAgentCommandPlanner Planner { get; set; }

        Mock<IGame> GameMock { get; set; }
        Mock<IAgent> AgentMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            CommandFactoryMock = new Mock<IAgentCommandFactory>();
            MoveDiscoMock = new Mock<ICombatMoveDiscoverer>();
            PosFinderMock = new Mock<IPositionFinder>();

            Planner = new DefaultAgentCommandPlanner(RandomMock.Object, CommandFactoryMock.Object, MoveDiscoMock.Object, PosFinderMock.Object);

            GameMock = new Mock<IGame>();
            AgentMock = new Mock<IAgent>();
        }

        [TestMethod]
        public void AgentIsDead()
        {
            AgentMock.Setup(x => x.IsDead).Returns(true);

            var nothingCommandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.Nothing(AgentMock.Object)).Returns(new IAgentCommand[] {nothingCommandMock.Object});

            var result = Planner.PlanBehavior(GameMock.Object, AgentMock.Object);

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(nothingCommandMock.Object, result.ElementAt(0));
        }

        [TestMethod]
        public void CannotFindTarget()
        {
            var agentPos = new Vector3(42, 42, 42);
            AgentMock.Setup(x => x.Pos).Returns(agentPos);

            PosFinderMock.Setup(x => x.FindNearbyPos(agentPos, It.IsAny<Predicate<Vector3>>(), It.IsAny<int>())).Returns((Vector3?)null);

            var wanderDir = new Vector3(-1, 0, 0);
            RandomMock.Setup(x => x.NextElement(It.Is<ICollection<Vector3>>(col => col.SequenceEqual(CompassVectors.GetAll().Select(v => new Vector3(v.X, v.Y, 0))))))
                .Returns(wanderDir);

            var commandMock = new Mock<IAgentCommand>();

            CommandFactoryMock.Setup(x => x.MoveDirection(AgentMock.Object, wanderDir)).Returns(new IAgentCommand[]{commandMock.Object});

            var result = Planner.PlanBehavior(GameMock.Object, AgentMock.Object);

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(commandMock.Object, result.First());
            CommandFactoryMock.Verify(x => x.MoveDirection(AgentMock.Object, wanderDir), Times.Once());
        }

        [Ignore]
        [TestMethod]
        public void NoAvailableMoves()
        {

        }

        [Ignore]
        [TestMethod]
        public void PossibleMovesFound()
        {

        }

        [Ignore]
        [TestMethod]
        public void RandomWander()
        {

        }
    }
}