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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void SizeMustBePositive()
        {
            new Body(new Mock<IBodyClass>().Object, new List<IBodyPart>(), 0);
        }

        [TestMethod]
        public void IsDead_NoParts()
        {
            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { }, 10);
            Assert.IsTrue(body.IsDead);
        }

        [TestMethod]
        public void IsDead_PrinciplePartIsEffectivelyPulped()
        {
            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.IsEffectivelyPulped).Returns(true);
            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { partMock.Object }, 10);
            Assert.IsTrue(body.IsDead);

            partMock.Setup(x => x.IsEffectivelyPulped).Returns(false);
            Assert.IsFalse(body.IsDead);
        }

        [TestMethod]
        public void IsDead_NoBlood()
        {
            var partMock = new Mock<IBodyPart>();
            partMock.Setup(x => x.IsEffectivelyPulped).Returns(false);
            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { partMock.Object }, 10);
            body.BloodFraction.Numerator = 0;
            Assert.IsTrue(body.IsDead);

            body.BloodFraction.Numerator = 1;
            Assert.IsFalse(body.IsDead);
        }

        [TestMethod]
        public void WrestlingProperties()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var parts = new List<IBodyPart> { partMock1.Object, partMock2.Object };

            var body = new Body(new Mock<IBodyClass>().Object, parts, 10);
            Assert.AreEqual(10, body.Size);

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
        public void Amputate_ChildPart()
        {
            var partMock1 = new Mock<IBodyPart>();
            var partMock2 = new Mock<IBodyPart>();
            var partMock3 = new Mock<IBodyPart>();

            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object }, 1);

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

            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object, partMock4.Object, partMock5.Object }, 1);

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

            var body = new Body(new Mock<IBodyClass>().Object, new List<IBodyPart> { partMock1.Object, partMock2.Object, partMock3.Object }, 1);

            body.Amputate(unknownPartMock.Object);

            Assert.AreEqual(3, body.Parts.Count());
            Assert.AreSame(partMock1.Object, body.Parts[0]);
            Assert.AreSame(partMock2.Object, body.Parts[1]);
            Assert.AreSame(partMock3.Object, body.Parts[2]);
        }
    }
}
