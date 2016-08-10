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
            public TestPlanner(IRandom random, IAgentCommandFactory commandFactory, IAttackMoveFactory moveFactory) 
                : base(random, commandFactory, moveFactory) { }

            public override IAgentCommand PlanBehavior(IGame game, IAgent agent)
            {
                throw new NotImplementedException();
            }

            public IAgentCommand GetNewNothingCommand(IAgent agent) { return Nothing(agent); }
            public IAgentCommand GetNewWanderCommand(IAgent agent) { return Wander(agent); }
            public IAgentCommand GetNewDeadCommand(IAgent agent) { return Dead(agent); }
            public IAgentCommand GetNewSeekCommand(IAgent agent, Vector2 pos) { return Seek(agent, pos); }
            public IEnumerable<IAttackMove> GetAttackMoves(IAgent agent, IAgent target) { return AttackMoves(agent, target); }
            public Vector2? RunFindNearbyPos(Vector2 center, Predicate<Vector2> finderPred, int halfBoxSize) { return FindNearbyPos(center, finderPred, halfBoxSize); }

        }

        Mock<IRandom> RandomMock { get; set; }
        Mock<IAgentCommandFactory> CommandFactoryMock { get; set; }
        Mock<IAttackMoveFactory> MoveFactoryMock { get; set; }
        TestPlanner Planner { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            CommandFactoryMock = new Mock<IAgentCommandFactory>();
            MoveFactoryMock = new Mock<IAttackMoveFactory>();
            Planner = new TestPlanner(RandomMock.Object, CommandFactoryMock.Object, MoveFactoryMock.Object);
        }

        [TestMethod]
        public void Wander()
        {
            var agentMock = new Mock<IAgent>();
            var dir = new Vector2(1, 1);
            RandomMock.Setup(x => x.NextElement(It.IsAny<ICollection<Vector2>>())).Returns(dir);

            var commandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, dir)).Returns(commandMock.Object);

            var command = Planner.GetNewWanderCommand(agentMock.Object);

            Assert.AreEqual(commandMock.Object, command);

            RandomMock.Verify(x => x.NextElement(It.Is<ICollection<Vector2>>(c => 
                c.Count() == CompassVectors.GetAll().Count()
                && c.Select(e => CompassVectors.GetAll().Contains(e)).All(b => b)
                )), Times.Once());

            CommandFactoryMock.Verify(x => x.MoveDirection(agentMock.Object, dir), Times.Once());
        }

        [TestMethod]
        public void Nothing()
        {
            var agentMock = new Mock<IAgent>();
            var commandMock = new Mock<IAgentCommand>();

            CommandFactoryMock.Setup(x => x.Nothing(agentMock.Object)).Returns(commandMock.Object);

            var command = Planner.GetNewNothingCommand(agentMock.Object);

            CommandFactoryMock.Verify(x => x.Nothing(agentMock.Object), Times.Once());
            Assert.AreSame(commandMock.Object, command);

        }

        [TestMethod]
        public void Dead()
        {
            Nothing(); // same case for now
        }

        [TestMethod]
        public void Seek_MoveFound()
        {
            var agentMock = new Mock<IAgent>();
            var agentPos = new Vector2(1, 1);
            var targetPos = new Vector2(3, 3);
            var goodMove = new Vector2(1, 1);

            agentMock.Setup(x => x.Pos).Returns(agentPos);
            agentMock.Setup(x => x.CanMove(goodMove)).Returns(true);

            var commandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, goodMove)).Returns(commandMock.Object);

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);

            Assert.AreSame(commandMock.Object, command);

            agentMock.Verify(x => x.CanMove(It.Is<Vector2>(v => v.X != goodMove.X || v.Y != goodMove.Y)), Times.Never());
            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector2>>()), Times.Never());

            CommandFactoryMock.Verify(x => x.MoveDirection(agentMock.Object, goodMove), Times.Once());
        }
        
        [TestMethod]
        public void Seek_NoMoveFound()
        {
            var agentMock = new Mock<IAgent>();
            var agentPos = new Vector2(1, 1);
            var targetPos = new Vector2(3, 3);

            var wanderDir = new Vector2(1, 1);
            RandomMock.Setup(x => x.NextElement(It.IsAny<ICollection<Vector2>>())).Returns(wanderDir);

            agentMock.Setup(x => x.Pos).Returns(agentPos);
            agentMock.Setup(x => x.CanMove(It.IsAny<Vector2>())).Returns(false);

            var commandMock = new Mock<IAgentCommand>();
            CommandFactoryMock.Setup(x => x.MoveDirection(agentMock.Object, wanderDir)).Returns(commandMock.Object);

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);

            Assert.AreSame(commandMock.Object, command);

            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector2>>()), Times.Once());

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

            var mockedResult = new List<IAttackMove>();
            MoveFactoryMock.Setup(x => x.GetPossibleMoves(agentMock.Object, targetMock.Object)).Returns(mockedResult);

            var result = Planner.GetAttackMoves(agentMock.Object, targetMock.Object);

            Assert.AreSame(mockedResult, result);

            MoveFactoryMock.Verify(x => x.GetPossibleMoves(agentMock.Object, targetMock.Object), Times.Once());
        }


        [TestMethod]
        public void FindNearbyPos_Miss()
        {
            var center = new Vector2(20, 20);
            var halfBoxSize = 1;

            var testedVectors = new List<Vector2>();
            Predicate<Vector2> finderPred = new Predicate<Vector2>((v) =>
            {
                testedVectors.Add(v);
                return false;
            });

            var result = Planner.RunFindNearbyPos(center, finderPred, halfBoxSize);

            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(9, testedVectors.Count());
            Assert.IsTrue(testedVectors.Contains(new Vector2(19, 19)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(19, 20)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(19, 21)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(20, 19)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(20, 20)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(20, 21)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(21, 19)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(21, 20)));
            Assert.IsTrue(testedVectors.Contains(new Vector2(21, 21)));
        }

        [TestMethod]
        public void FindNearbyPos_Hit()
        {
            var center = new Vector2(20, 20);
            var halfBoxSize = 1;

            var winner = new Vector2(20, 20);
            Predicate<Vector2> finderPred = new Predicate<Vector2>((v) => v.X == winner.X && v.Y == winner.Y);
            var result = Planner.RunFindNearbyPos(center, finderPred, halfBoxSize);

            Assert.IsTrue(result.HasValue);
            Asserter.AreEqual(winner, result.Value);
        }
    }
}
