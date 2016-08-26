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

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            CommandFactoryMock = new Mock<IAgentCommandFactory>();
            MoveDiscoMock = new Mock<ICombatMoveDiscoverer>();
            PosFinderMock = new Mock<IPositionFinder>();

            Planner = new DefaultAgentCommandPlanner(RandomMock.Object, CommandFactoryMock.Object, MoveDiscoMock.Object, PosFinderMock.Object);
        }

        [TestMethod]
        public void AgentIsDead()
        {
            var gameMock = new Mock<IGame>();
            var agentMock = new Mock<IAgent>();
            agentMock.Setup(x => x.IsDead).Returns(true);

            var nothingCommandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(new IAgentCommand[] {nothingCommandMock.Object});

            var result = Planner.PlanBehavior(gameMock.Object, agentMock.Object);

            Assert.AreEqual(1, result.Count());
            Assert.AreSame(nothingCommandMock.Object, result.ElementAt(0));
        }

        [Ignore]
        [TestMethod]
        public void CannotFindTarget()
        {

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
    }
}