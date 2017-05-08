using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials.Tests
{
    [TestClass]
    public class SingleLayerStrikeTesterTests
    {
        Mock<IMaterialStrikeResultBuilder> BuilderMock { get; set; }

        SingleLayerStrikeTester Tester { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            BuilderMock = new Mock<IMaterialStrikeResultBuilder>();

            Tester = new SingleLayerStrikeTester(BuilderMock.Object);
        }

        [TestMethod]
        public void Build_BasicFascade()
        {
            var stressMode = StressMode.Edge;
            var strikerMatMock = new Mock<IMaterial>();
            double strikerSharpness = 2d, strikerContactArea = 3d;
            double momentum = 4d, penetrationLeft = 5d;
            var strickenMatMock = new Mock<IMaterial>();
            double strickenThickness = 6d, strickenVolume = 7d, strickenContactArea = 8d;

            var builderResult = new MaterialStrikeResult();
            BuilderMock.Setup(x => x.Build())
                .Returns(builderResult);

            // act
            var result = Tester.StrikeTest(
                stressMode,
                strikerMatMock.Object, strikerSharpness, strikerContactArea,
                momentum, penetrationLeft,
                strickenMatMock.Object, strickenThickness, strickenVolume, strickenContactArea);

            Assert.AreSame(builderResult, result);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(strikerMatMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(strikerSharpness), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenMaterial(strickenMatMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikeMomentum(momentum), Times.Once());
            BuilderMock.Verify(x => x.SetLayerVolume(strickenVolume), Times.Once());
            BuilderMock.Verify(x => x.SetLayerThickness(strickenThickness), Times.Once());
            BuilderMock.Verify(x => x.SetRemainingPenetration(penetrationLeft), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(strikerContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(strickenContactArea), Times.Once());
            BuilderMock.Verify(x => x.Build(), Times.Once());
        }
    }
}
