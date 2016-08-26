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
            var tissueClassMock = new Mock<ITissueClass>();

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


            var factory = new TissueFactory();
            var result = factory.Create(tissueClassMock.Object);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.TissueLayers.Count());

            var layer1 = result.TissueLayers.ElementAt(0);
            Assert.AreSame(materialMock1.Object, layer1.Material);
            Assert.AreEqual(1, layer1.RelativeThickness);

            var layer2 = result.TissueLayers.ElementAt(1);
            Assert.AreSame(materialMock1.Object, layer2.Material);
            Assert.AreEqual(2, layer2.RelativeThickness);

            var layer3 = result.TissueLayers.ElementAt(2);
            Assert.AreSame(materialMock1.Object, layer2.Material);
            Assert.AreEqual(3, layer3.RelativeThickness);
        }
    }
}
