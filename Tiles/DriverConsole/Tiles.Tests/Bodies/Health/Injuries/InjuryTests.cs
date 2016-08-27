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

namespace Tiles.Tests.Bodies.Health.Injuries
{
    [TestClass]
    public class InjuryTests
    {
        readonly string DefaultAdjective = "MOCK ADJECTIVE";
        readonly int DefaultTtl = 10;

        Mock<IInjuryClass> ClassMock { get; set; }
        Mock<IBodyPart> PartMock { get; set; }
        Mock<ITissueLayer> LayerMock { get; set; }

        Injury Injury { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            ClassMock = new Mock<IInjuryClass>();
            ClassMock.Setup(c => c.Adjective).Returns(DefaultAdjective);
            ClassMock.Setup(c => c.UsesTtl).Returns(true);
            ClassMock.Setup(c => c.Ttl).Returns(DefaultTtl);
            PartMock = new Mock<IBodyPart>();
            LayerMock = new Mock<ITissueLayer>();

            Injury = new Injury(
                ClassMock.Object,
                PartMock.Object,
                LayerMock.Object);
        }

        [TestMethod]
        public void ClassInvariants()
        {
            Assert.AreSame(ClassMock.Object, Injury.Class);
            Assert.AreSame(PartMock.Object, Injury.BodyPart);
            Assert.AreSame(LayerMock.Object, Injury.TissueLayer);

            Assert.AreEqual(DefaultAdjective, Injury.Adjective);
            ClassMock.Verify(ic => ic.Adjective, Times.Once());

            Assert.IsFalse(Injury.IsInstantDeath);
            ClassMock.Verify(ic => ic.IsInstantDeath, Times.Once());

            Assert.IsFalse(Injury.IsPermanant);
            ClassMock.Verify(ic => ic.IsPermanant, Times.Once());

            Assert.IsFalse(Injury.CanBeHealed);
            ClassMock.Verify(ic => ic.CanBeHealed, Times.Once());

            Assert.IsFalse(Injury.CripplesBodyPart);
            ClassMock.Verify(ic => ic.CripplesBodyPart, Times.Once());

            ClassMock.Verify(ic => ic.UsesTtl, Times.Once());
            ClassMock.Verify(ic => ic.Ttl, Times.Once());

            Assert.IsFalse(Injury.IsOver);

            Assert.AreEqual(DefaultTtl, Injury.Ttl);
        }

        [TestMethod]
        public void NoTtl()
        {
            var classMock = new Mock<IInjuryClass>();
            classMock.Setup(x => x.UsesTtl).Returns(false);

            Injury = new Injury(classMock.Object, PartMock.Object, LayerMock.Object);
            Injury.Update(DefaultTtl);

            Assert.AreEqual(0, Injury.Ttl);
            Assert.IsFalse(Injury.IsOver);
        }

        [TestMethod]
        public void Ttl_OnTheNose()
        {
            Injury.Update(DefaultTtl);

            Assert.AreEqual(0, Injury.Ttl);
            Assert.IsTrue(Injury.IsOver);
        }

        [TestMethod]
        public void Ttl_LessThanOnePump()
        {
            Injury.Update(DefaultTtl*2);

            Assert.AreEqual(0, Injury.Ttl);
            Assert.IsTrue(Injury.IsOver);
        }

        [TestMethod]
        public void Ttl_MultiplePumps()
        {
            var step = 5;
            Injury.Update(step);

            Assert.AreEqual(DefaultTtl - step, Injury.Ttl);
            Assert.IsFalse(Injury.IsOver);

            Injury.Update(step);
            Assert.AreEqual(0, Injury.Ttl);
            Assert.IsTrue(Injury.IsOver);
        }

        [Ignore]
        [TestMethod]
        public void GetDisplayLabel_TissueLayer()
        {

        }

        [Ignore]
        [TestMethod]
        public void GetDisplayLabel_NoTissueLayer()
        {

        }
    }
}
