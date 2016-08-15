using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;

namespace Tiles.Tests.Agents.Combat
{
    [TestClass]
    public class AttackConductorTests
    {
        [TestMethod]
        public void Conduct()
        {
            var evoMock1 = new Mock<ICombatEvolution>();
            var evoMock2 = new Mock<ICombatEvolution>();
            var evoMock3 = new Mock<ICombatEvolution>();

            var attackerMock = new Mock<IAgent>();
            var defenderMock = new Mock<IAgent>();
            var combatMoveMock = new Mock<ICombatMove>();

            var conductor = new AttackConductor(new List<ICombatEvolution> { evoMock1.Object, evoMock2.Object, evoMock3.Object });

            conductor.Conduct(attackerMock.Object, defenderMock.Object, combatMoveMock.Object);

            evoMock1.Verify(x => x.Evolve(It.Is<ICombatMoveContext>(context =>
                context.Attacker == attackerMock.Object
                && context.Defender == defenderMock.Object
                && context.Move == combatMoveMock.Object)), Times.Once());
        }
    }
}
