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

        [TestMethod]
        public void Amputate_ChildPart()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var partMock3 = new Mock<IBodyPart>();

            var body = new Body(new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object });

            body.Amputate(partMock2.Object);

            Assert.AreEqual(2, body.Parts.Count());
            Assert.AreSame(partMock1.Object, body.Parts[0]);
            Assert.AreSame(partMock3.Object, body.Parts[1]);
        }

        [TestMethod]
        public void Amputate_ParentPart()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var partMock3 = new Mock<IBodyPart>();
            var partMock4 = new Mock<IBodyPart>();
            var partMock5 = new Mock<IBodyPart>();

            partMock3.Setup(x => x.Parent).Returns(partMock2.Object);
            partMock4.Setup(x => x.Parent).Returns(partMock3.Object);
            partMock5.Setup(x => x.Parent).Returns(partMock4.Object);

            var body = new Body(new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object, partMock4.Object, partMock5.Object });

            body.Amputate(partMock2.Object);

            Assert.AreEqual(1, body.Parts.Count());
            Assert.AreSame(partMock1.Object, body.Parts[0]);
        }

        [TestMethod]
        public void Amputate_UnknownPart()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var partMock3 = new Mock<IBodyPart>();

            var unknownPartMock = new Mock<IBodyPart>();

            var body = new Body(new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object });

            body.Amputate(unknownPartMock.Object);

            Assert.AreEqual(3, body.Parts.Count());
            Assert.AreSame(partMock1.Object, body.Parts[0]);
            Assert.AreSame(partMock2.Object, body.Parts[1]);
            Assert.AreSame(partMock3.Object, body.Parts[2]);
        }
    }
}
