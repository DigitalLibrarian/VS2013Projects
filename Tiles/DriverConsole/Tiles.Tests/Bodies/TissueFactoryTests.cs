using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Tests.Bodies
{
    [TestClass]
    public class TissueFactoryTests
    {
        [TestMethod]
        public void Create()
        {
            int totalRelThick = 10;
            
            var layerClassMock1 = new Mock<ITissueLayerClass>();
            layerClassMock1.Setup(x => x.RelativeThickness).Returns(1);

            var layerClassMock2 = new Mock<ITissueLayerClass>();
            layerClassMock2.Setup(x => x.RelativeThickness).Returns(2);

            var layerClassMock3 = new Mock<ITissueLayerClass>();
            layerClassMock3.Setup(x => x.RelativeThickness).Returns(3);

            var layerClassMock4 = new Mock<ITissueLayerClass>();
            layerClassMock4.Setup(x => x.RelativeThickness).Returns(1);
            layerClassMock4.Setup(x => x.ThickensOnStrength).Returns(true);

            var layerClassMock5 = new Mock<ITissueLayerClass>();
            layerClassMock5.Setup(x => x.RelativeThickness).Returns(1);
            layerClassMock5.Setup(x => x.ThickensOnEnergyStorage).Returns(true);

            var tissueClassMock = new Mock<ITissueClass>();
            tissueClassMock.Setup(x => x.TissueLayers).Returns(new ITissueLayerClass[]{
                layerClassMock1.Object, 
                layerClassMock2.Object, 
                layerClassMock3.Object,
                layerClassMock4.Object,
                layerClassMock5.Object
            });
            tissueClassMock.Setup(x => x.TotalRelativeThickness)
                .Returns(tissueClassMock.Object.TissueLayers.Sum(x => x.RelativeThickness));

            int bodySize = 8463;
            var factory = new TissueFactory();
            var result = factory.Create(tissueClassMock.Object, bodySize, 1250d);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.TissueLayers.Count());

            var layer1 = result.TissueLayers.ElementAt(0);
            Assert.AreEqual(54.54d, layer1.Thickness, 0.01d);
            Assert.AreEqual(1057.87d, layer1.Volume, 0.01d);

            var layer2 = result.TissueLayers.ElementAt(1);
            Assert.AreEqual(109.09d, layer2.Thickness, 0.01d);
            Assert.AreEqual(2115.75d, layer2.Volume, 0.01d);

            var layer3 = result.TissueLayers.ElementAt(2);
            Assert.AreEqual(163.64d, layer3.Thickness, 0.01d);
            Assert.AreEqual(3173.62d, layer3.Volume, 0.01d);

            var layer4 = result.TissueLayers.ElementAt(3);
            Assert.AreEqual(68.18d, layer4.Thickness, 0.01d);
            Assert.AreEqual(1322.34d, layer4.Volume, 0.01d);

            var layer5 = result.TissueLayers.ElementAt(4);
            Assert.AreEqual(109.1d, layer5.Thickness, 0.01d);
            Assert.AreEqual(2115.75d, layer5.Volume, 0.01d);
        }
    }
}
