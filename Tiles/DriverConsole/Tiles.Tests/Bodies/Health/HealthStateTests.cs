using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Health;

namespace Tiles.Tests.Bodies.Health
{
    [TestClass]
    public class HealthStateTests
    {
        Mock<IBody> BodyMock { get; set; }

        HealthState HealthState { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            BodyMock = new Mock<IBody>();

            HealthState = new HealthState(BodyMock.Object);
        }

        [TestMethod]
        public void ClassInvariants()
        {
            Assert.IsFalse(HealthState.IsWounded);
            Assert.IsFalse(HealthState.IsDead);
        }

        [TestMethod]
        public void IsWounded()
        {
            var injuryMock = new Mock<IInjury>();

            HealthState.Add(injuryMock.Object);
            Assert.IsTrue(HealthState.IsWounded);
            Assert.IsFalse(HealthState.InstantDeath);
        }

        [TestMethod]
        public void Update()
        {
            var injury1Mock = new Mock<IInjury>();
            var injury2Mock = new Mock<IInjury>();

            HealthState.Add(injury1Mock.Object);
            HealthState.Add(injury2Mock.Object);

            int ticks = 9;
            HealthState.Update(ticks);

            injury1Mock.Verify(i => i.Update(ticks), Times.Once());
            injury2Mock.Verify(i => i.Update(ticks), Times.Once());
        }

        [TestMethod]
        public void InjuryCausesInstantDeath()
        {
            int ticks = 42;
            var injuryMock = new Mock<IInjury>();
            bool mockInstDeath = false;
            injuryMock.Setup(i => i.IsInstantDeath).Returns(() => mockInstDeath);

            HealthState.Add(injuryMock.Object);
            Assert.IsFalse(HealthState.IsDead);
            Assert.IsFalse(HealthState.InstantDeath);

            HealthState.Update(ticks);
            Assert.IsFalse(HealthState.IsDead);
            Assert.IsFalse(HealthState.InstantDeath);

            mockInstDeath = true;
            HealthState.Update(ticks);

            Assert.IsTrue(HealthState.IsDead);
            Assert.IsTrue(HealthState.InstantDeath);

            Assert.IsTrue(HealthState.GetInjuries().Contains(injuryMock.Object));

            // turning it back off has no effect
            mockInstDeath = false;
            HealthState.Update(ticks);

            Assert.IsTrue(HealthState.IsDead);
            Assert.IsTrue(HealthState.InstantDeath);

            Assert.IsTrue(HealthState.GetInjuries().Contains(injuryMock.Object));
        }


        [TestMethod]
        public void AddInjury()
        {
            var injuryMock = new Mock<IInjury>();
            Assert.IsFalse(HealthState.GetInjuries().Contains(injuryMock.Object));
            HealthState.Add(injuryMock.Object);
            Assert.IsTrue(HealthState.GetInjuries().Contains(injuryMock.Object));
        }

        [TestMethod]
        public void WoundsCanHaveSuddenEnd()
        {
            int ticks = 42;
            var injuryMock1 = new Mock<IInjury>();
            bool mockIsOver = false;
            injuryMock1.Setup(i => i.Update(ticks)).Callback(() =>
            {
                mockIsOver = true;
            });
            injuryMock1.Setup(i => i.IsOver).Returns(() => mockIsOver);

            HealthState.Add(injuryMock1.Object);
            Assert.IsFalse(HealthState.IsDead);
            Assert.IsFalse(HealthState.InstantDeath);
            Assert.IsFalse(mockIsOver);

            HealthState.Update(ticks);
            injuryMock1.Verify(i => i.Update(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsFalse(HealthState.GetInjuries().Contains(injuryMock1.Object));

            Assert.IsFalse(HealthState.IsDead);
            Assert.IsFalse(HealthState.InstantDeath);

            HealthState.Update(ticks);
            injuryMock1.Verify(i => i.Update(It.IsAny<int>()), Times.Exactly(1));
        }
    }
}
