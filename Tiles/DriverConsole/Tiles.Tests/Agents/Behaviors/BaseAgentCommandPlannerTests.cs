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
            public TestPlanner(IRandom random, IAttackMoveFactory moveFactory) : base(random, moveFactory) { }

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
        Mock<IAttackMoveFactory> MoveFactoryMock { get; set; }
        TestPlanner Planner { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RandomMock = new Mock<IRandom>();
            MoveFactoryMock = new Mock<IAttackMoveFactory>();
            Planner = new TestPlanner(RandomMock.Object, MoveFactoryMock.Object);
        }

        [TestMethod]
        public void Wander()
        {
            var agentMock = new Mock<IAgent>();
            var dir = new Vector2(1, 1);
            RandomMock.Setup(x => x.NextElement(It.IsAny<ICollection<Vector2>>())).Returns(dir);

            var command = Planner.GetNewWanderCommand(agentMock.Object);

            Assert.AreEqual(AgentCommandType.Move, command.CommandType);
            Asserter.AreEqual(dir, command.Direction);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);

            RandomMock.Verify(x => x.NextElement(It.Is<ICollection<Vector2>>(c => 
                c.Count() == CompassVectors.GetAll().Count()
                && c.Select(e => CompassVectors.GetAll().Contains(e)).All(b => b)
                )), Times.Once());
        }

        [TestMethod]
        public void Nothing()
        {
            var agentMock = new Mock<IAgent>();
            var command = Planner.GetNewNothingCommand(agentMock.Object);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
        }

        [TestMethod]
        public void Dead()
        {
            var agentMock = new Mock<IAgent>();
            var command = Planner.GetNewDeadCommand(agentMock.Object);

            Assert.AreEqual(AgentCommandType.None, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(Vector2.Zero, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);
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

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);

            Assert.AreEqual(AgentCommandType.Move, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(goodMove, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);

            agentMock.Verify(x => x.CanMove(It.Is<Vector2>(v => v.X != goodMove.X || v.Y != goodMove.Y)), Times.Never());
            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector2>>()), Times.Never());
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

            var command = Planner.GetNewSeekCommand(agentMock.Object, targetPos);
            Assert.AreEqual(AgentCommandType.Move, command.CommandType);
            Asserter.AreEqual(Vector2.Zero, command.TileOffset);
            Asserter.AreEqual(wanderDir, command.Direction);
            Assert.IsNull(command.Target);
            Assert.IsNull(command.AttackMove);
            Assert.IsNull(command.Item);
            Assert.IsNull(command.Weapon);
            Assert.IsNull(command.Armor);

            RandomMock.Verify(x => x.NextElement(It.IsAny<ICollection<Vector2>>()), Times.Once());

            foreach (var compassDir in CompassVectors.GetAll())
            {
                agentMock.Verify(x => x.CanMove(compassDir), Times.Once());
            }
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
