using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;

namespace Tiles.Tests.Agents.Combat.CombatEvolutions
{
    [TestClass]
    public class CombatEvolutionTests
    {
        class TestCombatEvolution : CombatEvolution
        {
            Predicate<ICombatMoveContext> ShouldPred { get; set; }
            Action<ICombatMoveContext> RunCallback { get; set; }
            public TestCombatEvolution(Predicate<ICombatMoveContext> shouldPred, Action<ICombatMoveContext> runCallback,
                IActionReporter reporter, IDamageCalc damageCalc, IAgentReaper reaper)
                : base(reporter, damageCalc, reaper)
            {
                ShouldPred = shouldPred;
                RunCallback = runCallback;
            }

            protected override bool Should(ICombatMoveContext session)
            {
                return ShouldPred(session);
            }

            protected override void Run(ICombatMoveContext session)
            {
                RunCallback(session);
            }

            public void CallHandleDeath(IAgent attacker, IAgent defender, ICombatMove move)
            { 
                base.HandleDeath(attacker, defender, move);
            }

            public void CallHandlShedPart(IAgent attacker, IAgent defender, ICombatMove move, IBodyPart shedPart)
            {
                base.HandleShedPart(attacker, defender, move, shedPart);
            }
        }

        Mock<IActionReporter> ReporterMock { get; set; }
        Mock<IDamageCalc> DamageCalcMock { get; set; }
        Mock<IAgentReaper> ReaperMock { get; set; }

        TestCombatEvolution Evo { get; set; }

        ICombatMoveContext ShouldContext = null;
        int TimesShouldCalled = 0;
        bool ShouldResult = false;

        ICombatMoveContext RunContext = null;
        int TimesRunCalled = 0;
        
        [TestInitialize]
        public void Initialize()
        {
            ReporterMock = new Mock<IActionReporter>();
            DamageCalcMock = new Mock<IDamageCalc>();
            ReaperMock = new Mock<IAgentReaper>();

            Evo = new TestCombatEvolution(
                (context) =>
                {
                    ShouldContext = context;
                    TimesShouldCalled++;
                    return ShouldResult;
                },
                (context) =>
                {
                    RunContext = context;
                    TimesRunCalled++;
                },
                ReporterMock.Object,
                DamageCalcMock.Object,
                ReaperMock.Object
            );
        }

        [TestMethod]
        public void Evolve_ShouldNotRun()
        {
            ShouldResult = false;

            var contextMock = new Mock<ICombatMoveContext>();

            var result = Evo.Evolve(contextMock.Object);

            Assert.IsFalse(result);
            Assert.AreEqual(1, TimesShouldCalled);
            Assert.AreSame(contextMock.Object, ShouldContext);
            Assert.AreEqual(0, TimesRunCalled);
            Assert.IsNull(RunContext);
        }

        [TestMethod]
        public void Evolve_ShouldRun()
        {
            ShouldResult = true;

            var contextMock = new Mock<ICombatMoveContext>();
            var result = Evo.Evolve(contextMock.Object);

            Assert.IsTrue(result);
            Assert.AreEqual(1, TimesShouldCalled);
            Assert.AreSame(contextMock.Object, ShouldContext);
            Assert.AreEqual(1, TimesRunCalled);
            Assert.AreSame(contextMock.Object, RunContext);
        }

        [TestMethod]
        public void HandleDeath()
        {
            var attackerMock = new Mock<IAgent>();
            var defenderMock = new Mock<IAgent>();
            var moveMock = new Mock<ICombatMove>();

            Evo.CallHandleDeath(attackerMock.Object, defenderMock.Object, moveMock.Object);

            ReaperMock.Verify(x => x.Reap(defenderMock.Object), Times.Once());
        }

        [TestMethod]
        public void HandleShedPart()
        {
            var attackerMock = new Mock<IAgent>();
            var defenderMock = new Mock<IAgent>();
            var moveMock = new Mock<ICombatMove>();
            var partMock = new Mock<IBodyPart>();

            Evo.CallHandlShedPart(attackerMock.Object, defenderMock.Object, moveMock.Object, partMock.Object);

            ReaperMock.Verify(x => x.Reap(defenderMock.Object, partMock.Object), Times.Once());
        }
    }
}
