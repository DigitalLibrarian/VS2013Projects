using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Materials.Tests
{
    [TestClass]
    public class LayeredMaterialStrikeResultBuilderTests
    {
        Mock<ISingleLayerStrikeTester> LayerTesterMock { get; set; }
        Queue<MaterialStrikeResult> LayerResultQueue { get; set; }

        LayeredMaterialStrikeResultBuilder Builder { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            LayerResultQueue = new Queue<MaterialStrikeResult>();
            LayerTesterMock = new Mock<ISingleLayerStrikeTester>();
            LayerTesterMock.Setup(x => x.StrikeTest(
                It.IsAny<StressMode>(),
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>()))
                .Returns(() => LayerResultQueue.Dequeue());

            Builder = new LayeredMaterialStrikeResultBuilder(LayerTesterMock.Object);
        }

        private void AssertTotalLayerTests(int expected)
        {
            LayerTesterMock.Verify(x => x.StrikeTest(
                It.IsAny<StressMode>(),
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<double>()), Times.Exactly(expected));
        }

        private void SetupLayerResponse(params MaterialStrikeResult[] layerResults)
        {
            foreach(var layerResult in layerResults)
            {
                LayerResultQueue.Enqueue(layerResult);
            }
        }

        [TestMethod]
        public void SingleLayer_Edge_Defeated()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = 100d;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            var strickenMatMock = new Mock<IMaterial>();
            double strickenContactArea = 1.3d,
                strickenThickness = 1.4d,
                strickenVolume = 1.5d;

            object tag = new object();
           
            var layerResult = new MaterialStrikeResult { IsDefeated = true };
            SetupLayerResponse(layerResult);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(
                strickenMatMock.Object,
                strickenThickness,
                strickenVolume, 
                tag);

            var result = Builder.Build();

            Assert.AreEqual(strickenThickness, result.Penetration);
            Assert.AreEqual(1, result.LayerResults.Count());
            Assert.AreSame(layerResult, result.LayerResults.First());
            Assert.AreEqual(1, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(tag));
            Assert.AreEqual(layerResult, result.TaggedResults[tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                strickenMatMock.Object,
                strickenThickness,
                strickenVolume,
                strickenContactArea), Times.Once());

            AssertTotalLayerTests(1);
        }

        [TestMethod]
        public void SingleLayer_Edge_Undefeated_BluntRetry()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = 100d;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            var strickenMatMock = new Mock<IMaterial>();
            double strickenContactArea = 1.3d,
                strickenThickness = 1.4d,
                strickenVolume = 1.5d;

            object tag = new object();

            var layerResult0 = new MaterialStrikeResult 
            { 
                IsDefeated = false,
                StressResult = StressResult.None,
                ResultMomentum =  momentum
            };

            var layerResult1 = new MaterialStrikeResult
            {
                IsDefeated = false,
                StressResult = StressResult.Impact_Dent
            };
            SetupLayerResponse(layerResult0, layerResult1);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(
                strickenMatMock.Object,
                strickenThickness,
                strickenVolume,
                tag);

            var result = Builder.Build();

            Assert.AreEqual(0d, result.Penetration);
            Assert.AreEqual(1, result.LayerResults.Count());
            Assert.AreSame(layerResult1, result.LayerResults.First());
            Assert.AreEqual(1, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(tag));
            Assert.AreEqual(layerResult1, result.TaggedResults[tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                strickenMatMock.Object,
                strickenThickness,
                strickenVolume,
                strickenContactArea), Times.Once());

            LayerTesterMock.Verify(x => x.StrikeTest(
                StressMode.Blunt,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                strickenMatMock.Object,
                strickenThickness,
                strickenVolume,
                strickenContactArea), Times.Once());

            AssertTotalLayerTests(2);
            
        }

        [Ignore]
        [TestMethod]
        public void Edge_PenetrationCap_BluntConversion()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_CompleteFracture_TempEdgedConversion()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_UseLastLayerMaterial() 
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void EndsWithNone()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void EndsWhenOutOfMomentum()
        {
            throw new NotImplementedException();
        }
    }
}
