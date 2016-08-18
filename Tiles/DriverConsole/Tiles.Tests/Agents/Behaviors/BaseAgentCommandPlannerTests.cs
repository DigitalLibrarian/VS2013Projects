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
using Tiles.Agents.Behaviors;
using Tiles.Random;
using Tiles.Agents.Combat;

namespace Tiles.Tests.Agents.Behaviors
{
    [TestClass]
    public class BaseAgentCommandPlannerTests
    {
        // class is used via inheritance
        class TestPlanner : BaseAgentCommandPlanner
        {
            public TestPlanner(IRandom random, IAgentCommandFactory commandFactory, ICombatMoveDiscoverer moveDisco) 
                : base(random, commandFactory, moveDisco) { }

            public override IEnumerable<IAgentCommand> PlanBehavior(IGame game, IAgent agent)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IAgentCommand> GetNewNothingCommand(IAgent agent) { return Nothing(agent); }
            public IEnumerable<IAgentCommand> GetNewWanderCommand(IAgent agent) { return Wander(agent); }
            public IEnumerable<IAgentCommand> GetNewDeadCommand(IAgent agent) { return Dead(agent); }
            public IEnumerable<IAgentCommand> GetNewSeekCommand(IAgent agent, Vector3 pos) { return Seek(agent, pos); }
            public IEnumerable<ICombatMove> GetAttackMoves(IAgent agent, IAgent target) { return AttackMoves(agent, target); }
            public Vector3? RunFindNearbyPos(Vector3 center, Predicate<Vector3> finderPred, int halfBoxSize) { return FindNearbyPos(center, finderPred, halfBoxSize); }

        }

        Mock<IRandom> RandomMock { get; set; }
        Mock<IAgentCommandFactory> CommandFactoryMock { get; set; }
        Mock<ICombatMoveDiscoverer> MoveDiscoMock { get; set; }
        TestPlanner Planner { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            CommandFactoryMock = new Mock<IAgentCommandFactory>();
            MoveDiscoMock = new Mock<ICombatMoveDiscoverer>();
            Planner = new TestPlanner(RandomMock.Object, CommandFactoryMock.Object, MoveDiscoMock.Object);
        }
        
        [TestMethod]
        public void Wander()
        {
            var agentMock = new Mock<IAgent>();
            var dir = new Vector3(1, 1, 1);
            RandomMock.Setup(x => x.NextElement(It.IsAny<ICollection<Vector3>>())).Returns(dir);

            var commandMock = new Mock<IEnumerable<IAgentCommand>>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, dir)).Returns(commandMock.Object);

            var command = Planner.GetNewWanderCommand(agentMock.Object);

            Assert.AreEqual(commandMock.Object, command);

            RandomMock.Verify(
                x => x.NextElement<Vector3>(
                    It.Is<ICollection<Vector3>>(c => 
                        c.Count() == CompassVectors.GetAll().Count()
                        && c.All(v => CompassVectors.GetAll().Contains(v))
                )), Times.Once());

            CommandFactoryMock.Verify(x => x.MoveDirection(agentMock.Object, dir), Times.Once());
        }
        
        [TestMethod]
        public void Nothing()
        {
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IEnumerable<IAgentCommand>>();

            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(commandMock.Object);

            var command = Planner.GetNewNothingCommand(agentMock.Object);

            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once());
            Assert.AreSame(commandMock.Object, command);

        }

        [TestMethod]
        public void Dead()
        {
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IEnumerable<IAgentCommand>>();

            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(commandMock.Object);

            var command = Planner.GetNewDeadCommand(agentMock.Object);

            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once());
            Assert.AreSame(commandMock.Object, command);
        }

        [TestMethod]
        public void Seek_MoveFound()
        {
            var agentMock = new Mock<IAgent>();
            var agentPos = new Vector3(1, 1, 1);
            var targetPos = new Vector3(3, 3, 1);
            var goodMove = new Vector3(1, 1, 0);

            agentMock.Setup(x => x.Pos).Returns(agentPos);
            agentMock.Setup(x => x.CanMove(goodMove)).Returns(true);
            
            var commandMock = new Mock<IEnumerable<IAgentCommand>>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, goodMove)).Returns(commandMock.Object);

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);

            Assert.AreSame(commandMock.Object, command);

            agentMock.Verify(x => x.CanMove(It.Is<Vector3>(v => v.X != goodMove.X || v.Y != goodMove.Y || v.Z != goodMove.Z)), Times.Never());
            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector2>>()), Times.Never());

            CommandFactoryMock.Verify(x => x.MoveDirection(agentMock.Object, goodMove), Times.Once());
        }
        
        [TestMethod]
        public void Seek_NoMoveFound()
        {
            var agentMock = new Mock<IAgent>();
            var agentPos = new Vector3(1, 1, 1);
            var targetPos = new Vector3(3, 3, 3);

            var wanderDir = new Vector3(1, 1, 0);
            RandomMock.Setup(x => x.NextElement(It.IsAny<ICollection<Vector3>>())).Returns(wanderDir);

            agentMock.Setup(x => x.Pos).Returns(agentPos);
            agentMock.Setup(x => x.CanMove(It.IsAny<Vector3>())).Returns(false);
            
            var commandMock = new Mock<IEnumerable<IAgentCommand>>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, wanderDir)).Returns(commandMock.Object);

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);

            Assert.AreSame(commandMock.Object, command);

            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector3>>()), Times.Once());

            foreach (var compassDir in CompassVectors.GetAll())
            {
                agentMock.Verify(x => x.CanMove(compassDir), Times.Once());
            }
            CommandFactoryMock.Verify(x => x.MoveDirection(agentMock.Object, wanderDir), Times.Once());
        }

        [TestMethod]
        public void AttackMoves()
        {
            var agentMock = new Mock<IAgent>();
            var targetMock = new Mock<IAgent>();

            var mockedResult = new List<ICombatMove>();
            MoveDiscoMock.Setup(x => x.GetPossibleMoves(agentMock.Object, targetMock.Object)).Returns(mockedResult);

            var result = Planner.GetAttackMoves(agentMock.Object, targetMock.Object);

            Assert.AreSame(mockedResult, result);

            MoveDiscoMock.Verify(x => x.GetPossibleMoves(agentMock.Object, targetMock.Object), Times.Once());
        }

        [TestMethod]
        public void FindNearbyPos_Miss()
        {
            var center = new Vector3(20, 20, 20);
            var halfBoxSize = 1;

            var testedVectors = new List<Vector3>();
            Predicate<Vector3> finderPred = new Predicate<Vector3>((v) =>
            {
                testedVectors.Add(v);
                return false;
            });

            var result = Planner.RunFindNearbyPos(center, finderPred, halfBoxSize);

            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(9, testedVectors.Count());

            // TOOD - assert tested vectors
        }

        [TestMethod]
        public void FindNearbyPos_Hit()
        {
            var center = new Vector3(20, 20, 20);
            var halfBoxSize = 1;

            var winner = new Vector3(20, 20, 20);
            Predicate<Vector3> finderPred = new Predicate<Vector3>((v) => v.X == winner.X && v.Y == winner.Y);
            var result = Planner.RunFindNearbyPos(center, finderPred, halfBoxSize);

            Assert.IsTrue(result.HasValue);
            Asserter.AreEqual(winner, result.Value);
        }
    }
}
