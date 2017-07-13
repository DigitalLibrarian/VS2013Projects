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
    public class TissueLayerTests
    {
        [TestMethod]
        public void IsPulped_Delegation()
        {
            var classMock = new Mock<ITissueLayerClass>();
            var damageMock = new Mock<DamageVector>();
            var layer = new TissueLayer(classMock.Object, 1d, 1d, damageMock.Object);

            damageMock.Setup(x => x.IsPulped()).Returns(true);
            Assert.IsTrue(layer.IsPulped());

            damageMock.Setup(x => x.IsPulped()).Returns(false);
            Assert.IsFalse(layer.IsPulped());

            damageMock.Verify(x => x.IsPulped(), Times.Exactly(2));
        }

        [TestMethod]
        public void IsVascular_Delegation()
        {
            var classMock = new Mock<ITissueLayerClass>();
            var damageMock = new Mock<DamageVector>();
            var layer = new TissueLayer(classMock.Object, 1d, 1d, damageMock.Object);

            classMock.Setup(x => x.VascularRating).Returns(0);
            Assert.IsFalse(layer.IsVascular());

            classMock.Setup(x => x.VascularRating).Returns(1);
            Assert.IsTrue(layer.IsVascular());
        }
    }
}
