using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Bodies.Injuries;

namespace Tiles.Tests.Bodies.Injuries
{
    [TestClass]
    public class InjuryReportTests
    {
        [TestMethod]
        public void ZeroInjuries()
        {
            var report = new InjuryReport(new List<IBodyPartInjury>());

            Assert.AreEqual(0, report.BodyPartInjuries.Count());
            Assert.AreEqual(0, report.GetSeverings().Count());

            var partMock = new Mock<IBodyPart>();
            Assert.IsFalse(report.IsSever(partMock.Object));
        }

        [TestMethod]
        public void NoSeveredParts()
        {
            var injuryMocks = new List<Mock<IBodyPartInjury>>
            {
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>()
            };

            injuryMocks[0].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            injuryMocks[1].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            injuryMocks[2].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            injuryMocks[3].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);

            var report = new InjuryReport(injuryMocks.Select(x => x.Object));

            Assert.AreEqual(4, report.BodyPartInjuries.Count());
            Assert.AreEqual(0, report.GetSeverings().Count());

            Assert.IsFalse(report.IsSever(injuryMocks[0].Object.BodyPart));
            Assert.IsFalse(report.IsSever(injuryMocks[1].Object.BodyPart));
            Assert.IsFalse(report.IsSever(injuryMocks[2].Object.BodyPart));
            Assert.IsFalse(report.IsSever(injuryMocks[3].Object.BodyPart));
        }

        [TestMethod]
        public void SeveredParts()
        {
            var injuryMocks = new List<Mock<IBodyPartInjury>>
            {
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>(),
                new Mock<IBodyPartInjury>()
            };
            
            injuryMocks[0].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);

            injuryMocks[1].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            injuryMocks[1].Setup(x => x.IsSever).Returns(true);
            
            injuryMocks[2].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            
            injuryMocks[3].Setup(x => x.BodyPart).Returns(new Mock<IBodyPart>().Object);
            injuryMocks[3].Setup(x => x.IsSever).Returns(true);

            var report = new InjuryReport(injuryMocks.Select(x => x.Object));

            Assert.AreEqual(4, report.BodyPartInjuries.Count());
            Assert.AreEqual(2, report.GetSeverings().Count());

            Assert.IsFalse(report.IsSever(injuryMocks[0].Object.BodyPart));
            Assert.IsTrue(report.IsSever(injuryMocks[1].Object.BodyPart));
            Assert.IsFalse(report.IsSever(injuryMocks[2].Object.BodyPart));
            Assert.IsTrue(report.IsSever(injuryMocks[3].Object.BodyPart));
        }
    }
}
