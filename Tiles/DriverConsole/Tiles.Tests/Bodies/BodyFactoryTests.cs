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
    public class BodyFactoryTests
    {
        Mock<ITissueFactory> TissueFactoryMock { get; set; }

        BodyFactory BodyFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            TissueFactoryMock = new Mock<ITissueFactory>();
            BodyFactory = new BodyFactory(TissueFactoryMock.Object);
        }

        [TestMethod]
        public void Create()
        {
            int bodySize = 60;
            var bodyClassMock = new Mock<IBodyClass>();
            bodyClassMock.Setup(x => x.Size).Returns(bodySize);

            var tissueClassMock1 = new Mock<ITissueClass>();
            var tissueMock1 = new Mock<ITissue>();

            var partClassMock1 = new Mock<IBodyPartClass>();
            partClassMock1.Setup(x => x.Tissue).Returns(tissueClassMock1.Object);
            partClassMock1.Setup(x => x.RelativeSize).Returns(1);

            var tissueClassMock2 = new Mock<ITissueClass>();
            var tissueMock2 = new Mock<ITissue>();

            var partClassMock2 = new Mock<IBodyPartClass>();
            partClassMock2.Setup(pc => pc.Parent).Returns(partClassMock1.Object);
            partClassMock2.Setup(x => x.Tissue).Returns(tissueClassMock2.Object);
            partClassMock2.Setup(x => x.RelativeSize).Returns(2);

            var tissueClassMock3 = new Mock<ITissueClass>();
            var tissueMock3 = new Mock<ITissue>();

            TissueFactoryMock.Setup(x => x.Create(tissueClassMock1.Object, It.IsAny<double>(), It.IsAny<double>())).Returns(tissueMock1.Object);
            TissueFactoryMock.Setup(x => x.Create(tissueClassMock2.Object, It.IsAny<double>(), It.IsAny<double>())).Returns(tissueMock2.Object);
            TissueFactoryMock.Setup(x => x.Create(tissueClassMock3.Object, It.IsAny<double>(), It.IsAny<double>())).Returns(tissueMock3.Object);

            var partClassMock3 = new Mock<IBodyPartClass>();
            partClassMock3.Setup(pc => pc.Parent).Returns(partClassMock2.Object);
            partClassMock3.Setup(x => x.Tissue).Returns(tissueClassMock3.Object);
            partClassMock3.Setup(x => x.RelativeSize).Returns(3);

            bodyClassMock.Setup(x => x.Parts).Returns(new List<IBodyPartClass>
            {
                partClassMock1.Object, 
                partClassMock2.Object, 
                partClassMock3.Object,
            });

            bodyClassMock.Setup(x => x.Attributes).Returns(new List<IAttributeClass>
            {
                new AttributeClass("STRENGTH", 1250)
            });

            bodyClassMock.Setup(x => x.TotalBodyPartRelSize)
                .Returns(() => bodyClassMock.Object.Parts.Select(x => x.RelativeSize).Sum());

            var result = BodyFactory.Create(bodyClassMock.Object);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Parts.Count());

            var part1 = result.Parts.ElementAt(0);
            var part2 = result.Parts.ElementAt(1);
            var part3 = result.Parts.ElementAt(2);

            Assert.IsNull(part1.Parent);
            Assert.AreSame(tissueMock1.Object, part1.Tissue);
            Assert.AreSame(part1, part2.Parent);

            Assert.AreSame(tissueMock2.Object, part2.Tissue);
            Assert.AreSame(part2, part3.Parent);

            Assert.AreSame(tissueMock3.Object, part3.Tissue);

            Assert.AreEqual(10, part1.Size);
            Assert.AreEqual(20, part2.Size);
            Assert.AreEqual(30, part3.Size);
        }
    }
}
