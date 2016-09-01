using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tiles.Bodies;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class TissueTests
    {
        [TestMethod]
        public void TotalThickness_HasLayers()
        {
            var layerMock1 = new Mock<ITissueLayer>();
            layerMock1.Setup(x => x.Thickness).Returns(1);
            var layerMock2 = new Mock<ITissueLayer>();
            layerMock2.Setup(x => x.Thickness).Returns(2);
            var layerMock3 = new Mock<ITissueLayer>();
            layerMock3.Setup(x => x.Thickness).Returns(3);

            var tissue = new Tissue(new List<ITissueLayer> { layerMock1.Object, layerMock2.Object, layerMock3.Object });

            Assert.AreEqual(6, tissue.TotalThickness);
        }

        [TestMethod]
        public void TotalThickness_NoLayers()
        {
            var tissue = new Tissue(new List<ITissueLayer>());

            Assert.AreEqual(0, tissue.TotalThickness);
        }
    }
}
