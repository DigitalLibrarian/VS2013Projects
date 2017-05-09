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
    class TestLayer
    {
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public IMaterial Material { get; set; }
        public object Tag { get; set; }
    }

    [TestClass]
    public class LayeredMaterialStrikeResultBuilderTests
    {
        Mock<ISingleLayerStrikeTester> LayerTesterMock { get; set; }
        Queue<MaterialStrikeResult> LayerResultQueue { get; set; }

        LayeredMaterialStrikeResultBuilder Builder { get; set; }

        TestLayer Layer0 { get; set; }
        TestLayer Layer1 { get; set; }
        TestLayer Layer2 { get; set; }
        
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

            Layer0 = new TestLayer
            {
                Material = new Mock<IMaterial>().Object,
                Thickness = 1.4d,
                Volume = 1.5d,
                Tag = new object()
            };

            Layer1 = new TestLayer
            {
                Material = new Mock<IMaterial>().Object,
                Thickness = 1.6d,
                Volume = 1.7d,
                Tag = new object()
            };

            Layer2 = new TestLayer
            {
                Material = new Mock<IMaterial>().Object,
                Thickness = 1.8d,
                Volume = 1.9d,
                Tag = new object()
            };
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

            Assert.AreEqual(0, LayerResultQueue.Count());
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

            double strickenContactArea = 1.3d;

            var layerResult = new MaterialStrikeResult { IsDefeated = true };
            SetupLayerResponse(layerResult);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(Layer0.Material, Layer0.Thickness, Layer0.Volume, Layer0.Tag);

            var result = Builder.Build();

            Assert.AreEqual(Layer0.Thickness, result.Penetration);
            Assert.AreEqual(1, result.LayerResults.Count());
            Assert.AreSame(layerResult, result.LayerResults.First());
            Assert.AreEqual(1, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer0.Tag));
            Assert.AreEqual(layerResult, result.TaggedResults[Layer0.Tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            AssertTotalLayerTests(1);
        }

        [TestMethod]
        public void SingleLayer_Edge_None_BluntRetry()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = 100d;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            double strickenContactArea = 1.3d;

            var layerResult0 = new MaterialStrikeResult 
            { 
                IsDefeated = false,
                StressResult = StressResult.None,
            };

            // this one is served back as retry result
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

            Builder.AddLayer(Layer0.Material, Layer0.Thickness, Layer0.Volume, Layer0.Tag);

            var result = Builder.Build();

            Assert.AreEqual(0d, result.Penetration);
            Assert.AreEqual(1, result.LayerResults.Count());
            Assert.AreSame(layerResult1, result.LayerResults.First());
            Assert.AreEqual(1, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer0.Tag));
            Assert.AreEqual(layerResult1, result.TaggedResults[Layer0.Tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            LayerTesterMock.Verify(x => x.StrikeTest(
                StressMode.Blunt,  // this was changed
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            AssertTotalLayerTests(2);
        }

        [TestMethod]
        public void SingleLayer_Edge_Shear_Dent()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = 100d;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            double strickenContactArea = 1.3d;

            var layerResult0 = new MaterialStrikeResult
            {
                IsDefeated = false,
                StressResult = StressResult.Shear_Dent,
            };

            SetupLayerResponse(layerResult0);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(Layer0.Material, Layer0.Thickness, Layer0.Volume, Layer0.Tag);

            var result = Builder.Build();

            Assert.AreEqual(0d, result.Penetration);
            Assert.AreEqual(1, result.LayerResults.Count());
            Assert.AreSame(layerResult0, result.LayerResults.First());
            Assert.AreEqual(1, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer0.Tag));
            Assert.AreEqual(layerResult0, result.TaggedResults[Layer0.Tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            AssertTotalLayerTests(1);
        }

        [TestMethod]
        public void Edge_Undefeated_BluntConversion()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = 100d;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            double strickenContactArea = 1.3d;

            var layerResult0 = new MaterialStrikeResult
            {
                IsDefeated = false,
                StressResult = StressResult.Shear_Dent,
                ResultMomentum = momentum
            };

            var layerResult1 = new MaterialStrikeResult
            {
                IsDefeated = false,
            };

            SetupLayerResponse(layerResult0, layerResult1);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(Layer0.Material, Layer0.Thickness, Layer0.Volume, Layer0.Tag);
            Builder.AddLayer(Layer1.Material, Layer1.Thickness, Layer1.Volume, Layer1.Tag);

            var result = Builder.Build();

            Assert.AreEqual(0d, result.Penetration);
            Assert.AreEqual(2, result.LayerResults.Count());
            Assert.AreSame(layerResult0, result.LayerResults.ElementAt(0));
            Assert.AreSame(layerResult1, result.LayerResults.ElementAt(1));
            Assert.AreEqual(2, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer0.Tag));
            Assert.AreEqual(layerResult0, result.TaggedResults[Layer0.Tag]);

            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer1.Tag));
            Assert.AreEqual(layerResult1, result.TaggedResults[Layer1.Tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            LayerTesterMock.Verify(x => x.StrikeTest(
                StressMode.Blunt,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                layerResult0.ResultMomentum,
                penetrationLeft,
                Layer1.Material,
                Layer1.Thickness,
                Layer1.Volume,
                strickenContactArea
                ), Times.Once());

            AssertTotalLayerTests(2);
        }

        [TestMethod]
        public void Edge_PenetrationCap_BluntConversion()
        {
            var stressMode = StressMode.Edge;
            double momentum = 1d, penetrationLeft = Layer0.Thickness;

            var strikerMatMock = new Mock<IMaterial>();
            double strikerContactArea = 1.1d,
                strikerSharpness = 1.2d;

            double strickenContactArea = 1.3d;

            var layerResult0 = new MaterialStrikeResult
            {
                IsDefeated = true,
                StressResult = StressResult.Shear_Cut,
                ResultMomentum = momentum
            };

            var layerResult1 = new MaterialStrikeResult
            {
                IsDefeated = false,
                StressResult = StressResult.None
            };

            SetupLayerResponse(layerResult0, layerResult1);

            Builder.SetStressMode(stressMode);
            Builder.SetMomentum(momentum);
            Builder.SetMaxPenetration(penetrationLeft);
            Builder.SetStrikerMaterial(strikerMatMock.Object);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerSharpness(strikerSharpness);

            Builder.SetStrickenContactArea(strickenContactArea);

            Builder.AddLayer(Layer0.Material, Layer0.Thickness, Layer0.Volume, Layer0.Tag);
            Builder.AddLayer(Layer1.Material, Layer1.Thickness, Layer1.Volume, Layer1.Tag);

            var result = Builder.Build();

            Assert.AreEqual(Layer0.Thickness, result.Penetration);
            Assert.AreEqual(2, result.LayerResults.Count());
            Assert.AreSame(layerResult0, result.LayerResults.ElementAt(0));
            Assert.AreSame(layerResult1, result.LayerResults.ElementAt(1));
            Assert.AreEqual(2, result.TaggedResults.Count());
            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer0.Tag));
            Assert.AreEqual(layerResult0, result.TaggedResults[Layer0.Tag]);

            Assert.IsTrue(result.TaggedResults.ContainsKey(Layer1.Tag));
            Assert.AreEqual(layerResult1, result.TaggedResults[Layer1.Tag]);

            LayerTesterMock.Verify(x => x.StrikeTest(
                stressMode,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                momentum,
                penetrationLeft,
                Layer0.Material,
                Layer0.Thickness,
                Layer0.Volume,
                strickenContactArea), Times.Once());

            LayerTesterMock.Verify(x => x.StrikeTest(
                StressMode.Blunt,
                strikerMatMock.Object,
                strikerSharpness,
                strikerContactArea,
                layerResult0.ResultMomentum,
                penetrationLeft - Layer0.Thickness,
                Layer1.Material,
                Layer1.Thickness,
                Layer1.Volume,
                strickenContactArea
                ), Times.Once());

            AssertTotalLayerTests(2);
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
