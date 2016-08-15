using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class BodyTests
    {
        [TestMethod]
        public void TakeDamage_NoPartOutOfHealth()
        {
            var targetHealthMock = new Mock<HealthVector>();
            targetHealthMock.Setup(x => x.OutOfHealth).Returns(false);
            var targetBodyPartMock = new Mock<IBodyPart>();
            targetBodyPartMock.Setup(x => x.Health).Returns(targetHealthMock.Object);
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
        public void TakeDamage_OutOfHealth_NoAmputate()
        {
            var targetHealthMock = new Mock<HealthVector>();
            targetHealthMock.Setup(x => x.OutOfHealth).Returns(true);
            var targetBodyPartMock = new Mock<IBodyPart>();
            targetBodyPartMock.Setup(x => x.Health).Returns(targetHealthMock.Object);
            targetBodyPartMock.Setup(x => x.CanAmputate).Returns(false);

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
            targetBodyPartMock.Setup(x => x.CanAmputate).Returns(true);

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
    }
}
