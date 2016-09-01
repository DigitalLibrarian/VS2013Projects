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
            var tissueClassMock = new Mock<ITissueClass>();
            tissueClassMock.Setup(x => x.TotalRelativeThickness)
                .Returns(totalRelThick);

            var materialMock1 = new Mock<IMaterial>();
            var materialMock2 = new Mock<IMaterial>();
            
            var layerClassMock1 = new Mock<ITissueLayerClass>();
            layerClassMock1.Setup(x => x.Material).Returns(materialMock1.Object);
            layerClassMock1.Setup(x => x.RelativeThickness).Returns(1);

            var layerClassMock2 = new Mock<ITissueLayerClass>();
            layerClassMock2.Setup(x => x.Material).Returns(materialMock1.Object);
            layerClassMock2.Setup(x => x.RelativeThickness).Returns(2);

            var layerClassMock3 = new Mock<ITissueLayerClass>();
            layerClassMock3.Setup(x => x.Material).Returns(materialMock2.Object);
            layerClassMock3.Setup(x => x.RelativeThickness).Returns(3);

            tissueClassMock.Setup(x => x.TissueLayers).Returns(new ITissueLayerClass[]{
                layerClassMock1.Object, layerClassMock2.Object, layerClassMock3.Object
            });

            int bodySize = 200;
            var factory = new TissueFactory();
            var result = factory.Create(tissueClassMock.Object, bodySize);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.TissueLayers.Count());

            var layer1 = result.TissueLayers.ElementAt(0);
            Assert.AreSame(materialMock1.Object, layer1.Material);
            Assert.AreEqual(20, layer1.Thickness);

            var layer2 = result.TissueLayers.ElementAt(1);
            Assert.AreSame(materialMock1.Object, layer2.Material);
            Assert.AreEqual(40, layer2.Thickness);

            var layer3 = result.TissueLayers.ElementAt(2);
            Assert.AreSame(materialMock1.Object, layer2.Material);
            Assert.AreEqual(60, layer3.Thickness);
        }
    }
}
