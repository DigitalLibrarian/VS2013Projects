using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Health;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class BodyTests
    {
        [TestMethod]
        public void WrestlingProperties()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var parts = new List<IBodyPart> { partMock1.Object, partMock2.Object };

            var body = new Body(parts);

            Assert.IsFalse(body.IsGrasping);
            Assert.IsFalse(body.IsBeingGrasped);
            Assert.IsFalse(body.IsWrestling);

            partMock1.Setup(x => x.IsGrasping).Returns(true);
            Assert.IsTrue(body.IsGrasping);
            Assert.IsFalse(body.IsBeingGrasped);
            Assert.IsFalse(body.IsWrestling);

            partMock1.Setup(x => x.IsGrasping).Returns(false);
            Assert.IsFalse(body.IsGrasping);
            Assert.IsFalse(body.IsBeingGrasped);
            Assert.IsFalse(body.IsWrestling);

            partMock2.Setup(x => x.IsBeingGrasped).Returns(true);
            Assert.IsFalse(body.IsGrasping);
            Assert.IsTrue(body.IsBeingGrasped);
            Assert.IsFalse(body.IsWrestling);


            partMock2.Setup(x => x.IsBeingGrasped).Returns(false);
            Assert.IsFalse(body.IsGrasping);
            Assert.IsFalse(body.IsBeingGrasped);
            Assert.IsFalse(body.IsWrestling);

            partMock2.Setup(x => x.IsWrestling).Returns(true);
            Assert.IsFalse(body.IsGrasping);
            Assert.IsFalse(body.IsBeingGrasped);
            Assert.IsTrue(body.IsWrestling);
        }

        [TestMethod]
        public void InjurePart()
        {
            var healthMock = new Mock<IHealthState>();

            var body = new Body(
                new List<IBodyPart>(),
                healthMock.Object);

            var injuryMock1 = new Mock<IInjury>();
            var injuryMock2 = new Mock<IInjury>();

            body.AddInjuries(new IInjury[]{ 
                injuryMock1.Object,
                injuryMock2.Object
            });

            healthMock.Verify(x => x.Add(injuryMock1.Object), Times.Once());
            healthMock.Verify(x => x.Add(injuryMock2.Object), Times.Once());
            healthMock.Verify(x => x.Add(It.IsAny<IInjury>()), Times.Exactly(2));
        }

        /*
        [TestMethod]
        public void TakeDamage_NoPartOutOfHealth()
        {
            var targetHealthMock = new Mock<IHeatlh>();
            targetHealthMock.Setup(x => x.OutOfHealth).Returns(false);
            var targetBodyPartMock = new Mock<IBodyPart>();
            targetBodyPartMock.Setup(x => x.Health).Returns(targetHealthMock.Object);


            var junkPartMock = new Mock<IBodyPart>();
            var parts = new List<IBodyPart>{
                junkPartMock.Object,
                targetBodyPartMock.Object
            };

            var body = new Body(parts);

            //uint dmg = 9;
            //var result = body.DamagePart(targetBodyPartMock.Object, dmg);
            //targetHealthMock.Verify(x => x.TakeDamage(dmg), Times.Once);

            Assert.IsNull(result);
            Assert.AreEqual(2, parts.Count());
            Assert.AreSame(junkPartMock.Object, parts.ElementAt(0));
            Assert.AreSame(targetBodyPartMock.Object, parts.ElementAt(1));
        }

        [TestMethod]
        public void TakeDamage_OutOfHealth_NoAmputate()
        {
            var targetHealthMock = new Mock<HealthVector>();
            targetHealthMock.Setup(x => x.OutOfHealth).Returns(true);
            var targetBodyPartMock = new Mock<IBodyPart>();
            targetBodyPartMock.Setup(x => x.Health).Returns(targetHealthMock.Object);
            targetBodyPartMock.Setup(x => x.CanBeAmputated).Returns(false);

            var junkPartMock = new Mock<IBodyPart>();
            var parts = new List<IBodyPart>{
                junkPartMock.Object,
                targetBodyPartMock.Object
            };

            var body = new Body(parts);

            uint dmg = 9;
            var result = body.DamagePart(targetBodyPartMock.Object, dmg);
            targetHealthMock.Verify(x => x.TakeDamage(dmg), Times.Once);

            Assert.IsNull(result);
            Assert.AreEqual(2, parts.Count());
            Assert.AreSame(junkPartMock.Object, parts.ElementAt(0));
            Assert.AreSame(targetBodyPartMock.Object, parts.ElementAt(1));
        }

        [TestMethod]
        public void TakeDamage_OutOfHealth_CanAmputate()
        {
            var targetHealthMock = new Mock<HealthVector>();
            targetHealthMock.Setup(x => x.OutOfHealth).Returns(true);
            var targetBodyPartMock = new Mock<IBodyPart>();
            targetBodyPartMock.Setup(x => x.Health).Returns(targetHealthMock.Object);
            targetBodyPartMock.Setup(x => x.CanBeAmputated).Returns(true);

            var childPartMock = new Mock<IBodyPart>();
            childPartMock.Setup(x => x.Parent).Returns(targetBodyPartMock.Object);

            var junkPartMock = new Mock<IBodyPart>();
            var parts = new List<IBodyPart>{
                junkPartMock.Object,
                targetBodyPartMock.Object,
                childPartMock.Object
            };

            var body = new Body(parts);

            uint dmg = 9;
            var result = body.DamagePart(targetBodyPartMock.Object, dmg);
            targetHealthMock.Verify(x => x.TakeDamage(dmg), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreSame(targetBodyPartMock.Object, result);
            Assert.AreEqual(1, parts.Count());
            Assert.AreSame(junkPartMock.Object, parts.ElementAt(0));
        }
         * */
    }
}
