using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Bodies.Injuries;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.Tests.Bodies.Injuries
{
    [TestClass]
    public class InjuryReportCalcTests
    {
        Mock<IInjuryFactory> InjuryFactoryMock { get; set; }
        Mock<ILayeredMaterialStrikeResultBuilder> BuilderMock { get; set; }

        InjuryReportCalc Calc { get; set; }

        Mock<IBody> TargetBodyMock { get; set; }
        Mock<IBodyPart> TargetPartMock { get; set; }
        Mock<ITissue> TargetPartTissueMock { get; set; }
        Mock<IMaterial> StrikerMaterialMock { get; set; }
        Mock<ILayeredMaterialStrikeResult> StrikeResultMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            BuilderMock = new Mock<ILayeredMaterialStrikeResultBuilder>();

            Calc = new InjuryReportCalc(InjuryFactoryMock.Object, BuilderMock.Object);

            TargetBodyMock = new Mock<IBody>();
            TargetPartMock = new Mock<IBodyPart>();
            TargetPartTissueMock = new Mock<ITissue>();
            TargetPartTissueMock.Setup(x => x.TissueLayers).Returns(new List<ITissueLayer>());
            TargetPartMock.Setup(x => x.Tissue).Returns(TargetPartTissueMock.Object);
            StrikerMaterialMock = new Mock<IMaterial>();

            StrikeResultMock = new Mock<ILayeredMaterialStrikeResult>();
            BuilderMock.Setup(x => x.Build())
                .Returns(StrikeResultMock.Object);
        }

        [TestMethod]
        public void MaterialStrike_ZeroCase()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Never());
            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(), 
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Never());

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d => d.Count() == 0)), Times.Once());
        }

        [TestMethod]
        public void MaterialStrike_ArmorLayer()
        {
            // TODO - this test does not nail down the ordering of the armor items in the input
            var armorItem1MaterialMock = new Mock<IMaterial>();
            var armorItem1ClassMock = new Mock<IItemClass>();
            armorItem1ClassMock.Setup(x => x.Material).Returns(armorItem1MaterialMock.Object);
            var armorItem1Mock = new Mock<IItem>();
            armorItem1Mock.Setup(x => x.Class).Returns(armorItem1ClassMock.Object);

            var armorItem2MaterialMock = new Mock<IMaterial>();
            var armorItem2ClassMock = new Mock<IItemClass>();
            armorItem2ClassMock.Setup(x => x.Material).Returns(armorItem2MaterialMock.Object);
            var armorItem2Mock = new Mock<IItem>();
            armorItem2Mock.Setup(x => x.Class).Returns(armorItem2ClassMock.Object);

            var armorItems = new List<IItem>() { armorItem1Mock.Object, armorItem2Mock.Object };

            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(armorItem1MaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.AddLayer(armorItem2MaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Exactly(2));
            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Never());

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d => d.Count() == 0)), Times.Once());
        }

        [TestMethod]
        public void MaterialStrike_NonCosmeticTissueLayer()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var tissueLayer1ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer1ClassMock.Setup(x => x.IsCosmetic).Returns(false);
            var tissueLayer1Thickness = 12d;
            var tissueLayer1Volume = 120d;
            var tissueLayer1MaterialMock = new Mock<IMaterial>();
            var tissueLayer1Mock = new Mock<ITissueLayer>();
            tissueLayer1Mock.Setup(x => x.Material).Returns(tissueLayer1MaterialMock.Object);
            tissueLayer1Mock.Setup(x => x.Volume).Returns(tissueLayer1Volume);
            tissueLayer1Mock.Setup(x => x.Thickness).Returns(tissueLayer1Thickness);
            tissueLayer1Mock.Setup(x => x.Class).Returns(tissueLayer1ClassMock.Object);
            tissueLayer1Mock.Setup(x => x.IsPulped).Returns(false);

            var tissueLayer2ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer2ClassMock.Setup(x => x.IsCosmetic).Returns(false);
            var tissueLayer2Thickness = 12d;
            var tissueLayer2Volume = 120d;
            var tissueLayer2MaterialMock = new Mock<IMaterial>();
            var tissueLayer2Mock = new Mock<ITissueLayer>();
            tissueLayer2Mock.Setup(x => x.Material).Returns(tissueLayer2MaterialMock.Object);
            tissueLayer2Mock.Setup(x => x.Volume).Returns(tissueLayer2Volume);
            tissueLayer2Mock.Setup(x => x.Thickness).Returns(tissueLayer2Thickness);
            tissueLayer2Mock.Setup(x => x.Class).Returns(tissueLayer2ClassMock.Object);
            tissueLayer2Mock.Setup(x => x.IsPulped).Returns(false);

            var tissueLayers = new List<ITissueLayer>{ tissueLayer1Mock.Object, tissueLayer2Mock.Object };
            TargetPartTissueMock.Setup(x => x.TissueLayers)
                .Returns(tissueLayers);

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var layerOrder = new List<IMaterial>();
            BuilderMock.Setup(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>())).Callback((IMaterial mat, double thick, double vol, object tag) =>
                {
                    layerOrder.Add(mat);
                });

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Never());
            BuilderMock.Verify(x => x.AddLayer(
                tissueLayer1MaterialMock.Object,
                tissueLayer1Thickness,
                tissueLayer1Volume,
                tissueLayer1Mock.Object), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(
                tissueLayer2MaterialMock.Object,
                tissueLayer2Thickness,
                tissueLayer2Volume,
                tissueLayer2Mock.Object), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Exactly(2));

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d =>
                    d.Count() == 2
                    && d[tissueLayer1Mock.Object] == TargetPartMock.Object
                    && d[tissueLayer2Mock.Object] == TargetPartMock.Object)
                    ), Times.Once());

            Assert.AreEqual(layerOrder.Count(), 2);
            Assert.AreSame(tissueLayer2MaterialMock.Object, layerOrder[0]);
            Assert.AreSame(tissueLayer1MaterialMock.Object, layerOrder[1]);
        }

        [TestMethod]
        public void MaterialStrike_CosmeticTissueLayer()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var tissueLayerClassMock = new Mock<ITissueLayerClass>();
            tissueLayerClassMock.Setup(x => x.IsCosmetic).Returns(true);

            var tissueLayerThickness = 12d;
            var tissueLayerVolume = 120d;
            var tissueLayerMaterialMock = new Mock<IMaterial>();
            var tissueLayerMock = new Mock<ITissueLayer>();
            tissueLayerMock.Setup(x => x.Material).Returns(tissueLayerMaterialMock.Object);
            tissueLayerMock.Setup(x => x.Volume).Returns(tissueLayerVolume);
            tissueLayerMock.Setup(x => x.Thickness).Returns(tissueLayerThickness);
            tissueLayerMock.Setup(x => x.Class).Returns(tissueLayerClassMock.Object);
            var tissueLayers = new List<ITissueLayer> { tissueLayerMock.Object };
            TargetPartTissueMock.Setup(x => x.TissueLayers)
                .Returns(tissueLayers);

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Never());
            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Never());

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d => d.Count() == 0)), Times.Once());
        }

        [TestMethod]
        public void MaterialStrike_InternalNonCosmeticTissueLayer()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var tissueLayer1ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer1ClassMock.Setup(x => x.IsCosmetic).Returns(false);

            var tissueLayer1Thickness = 12d;
            var tissueLayer1Volume = 120d;
            var tissueLayer1MaterialMock = new Mock<IMaterial>();
            var tissueLayer1Mock = new Mock<ITissueLayer>();
            tissueLayer1Mock.Setup(x => x.Material).Returns(tissueLayer1MaterialMock.Object);
            tissueLayer1Mock.Setup(x => x.Volume).Returns(tissueLayer1Volume);
            tissueLayer1Mock.Setup(x => x.Thickness).Returns(tissueLayer1Thickness);
            tissueLayer1Mock.Setup(x => x.Class).Returns(tissueLayer1ClassMock.Object);

            var tissueLayer2ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer2ClassMock.Setup(x => x.IsCosmetic).Returns(false);

            var tissueLayer2Thickness = 10d;
            var tissueLayer2Volume = 121d;
            var tissueLayer2MaterialMock = new Mock<IMaterial>();
            var tissueLayer2Mock = new Mock<ITissueLayer>();
            tissueLayer2Mock.Setup(x => x.Material).Returns(tissueLayer2MaterialMock.Object);
            tissueLayer2Mock.Setup(x => x.Volume).Returns(tissueLayer2Volume);
            tissueLayer2Mock.Setup(x => x.Thickness).Returns(tissueLayer2Thickness);
            tissueLayer2Mock.Setup(x => x.Class).Returns(tissueLayer2ClassMock.Object);

            var tissueLayers = new List<ITissueLayer> { tissueLayer1Mock.Object, tissueLayer2Mock.Object };
            var tissueMock = new Mock<ITissue>();
            tissueMock.Setup(x => x.TissueLayers)
                .Returns(tissueLayers);

            var internalPartMock = new Mock<IBodyPart>();
            internalPartMock.Setup(x => x.Tissue).Returns(tissueMock.Object);

            TargetBodyMock.Setup(x => x.GetInternalParts(TargetPartMock.Object))
                .Returns(new List<IBodyPart> { internalPartMock.Object });

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var layerOrder = new List<IMaterial>();
            BuilderMock.Setup(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>())).Callback((IMaterial mat, double thick, double vol, object tag) =>
                {
                    layerOrder.Add(mat);
                });

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Never());
            BuilderMock.Verify(x => x.AddLayer(
                tissueLayer1MaterialMock.Object,
                tissueLayer1Thickness,
                tissueLayer1Volume,
                tissueLayer1Mock.Object), Times.Once());
            BuilderMock.Verify(x => x.AddLayer(
                tissueLayer2MaterialMock.Object,
                tissueLayer2Thickness,
                tissueLayer2Volume,
                tissueLayer2Mock.Object), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Exactly(2));

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d =>
                    d.Count() == 2
                    && d[tissueLayer1Mock.Object] == internalPartMock.Object
                    && d[tissueLayer2Mock.Object] == internalPartMock.Object)
                    ), Times.Once());

            Assert.AreEqual(layerOrder.Count(), 2);
            Assert.AreSame(tissueLayer2MaterialMock.Object, layerOrder[0]);
            Assert.AreSame(tissueLayer1MaterialMock.Object, layerOrder[1]);
        }

        [TestMethod]
        public void MaterialStrike_InternalCosmeticTissueLayer()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.None;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            TargetPartMock.Setup(x => x.ContactArea)
                .Returns(partContactArea);

            var tissueLayer1ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer1ClassMock.Setup(x => x.IsCosmetic).Returns(true);

            var tissueLayer1Thickness = 12d;
            var tissueLayer1Volume = 120d;
            var tissueLayer1MaterialMock = new Mock<IMaterial>();
            var tissueLayer1Mock = new Mock<ITissueLayer>();
            tissueLayer1Mock.Setup(x => x.Material).Returns(tissueLayer1MaterialMock.Object);
            tissueLayer1Mock.Setup(x => x.Volume).Returns(tissueLayer1Volume);
            tissueLayer1Mock.Setup(x => x.Thickness).Returns(tissueLayer1Thickness);
            tissueLayer1Mock.Setup(x => x.Class).Returns(tissueLayer1ClassMock.Object);

            var tissueLayer2ClassMock = new Mock<ITissueLayerClass>();
            tissueLayer2ClassMock.Setup(x => x.IsCosmetic).Returns(true);

            var tissueLayer2Thickness = 10d;
            var tissueLayer2Volume = 121d;
            var tissueLayer2MaterialMock = new Mock<IMaterial>();
            var tissueLayer2Mock = new Mock<ITissueLayer>();
            tissueLayer2Mock.Setup(x => x.Material).Returns(tissueLayer2MaterialMock.Object);
            tissueLayer2Mock.Setup(x => x.Volume).Returns(tissueLayer2Volume);
            tissueLayer2Mock.Setup(x => x.Thickness).Returns(tissueLayer2Thickness);
            tissueLayer2Mock.Setup(x => x.Class).Returns(tissueLayer2ClassMock.Object);

            var tissueLayers = new List<ITissueLayer> { tissueLayer1Mock.Object, tissueLayer2Mock.Object };
            var tissueMock = new Mock<ITissue>();
            tissueMock.Setup(x => x.TissueLayers)
                .Returns(tissueLayers);

            var internalPartMock = new Mock<IBodyPart>();
            internalPartMock.Setup(x => x.Tissue).Returns(tissueMock.Object);

            TargetBodyMock.Setup(x => x.GetInternalParts(TargetPartMock.Object))
                .Returns(new List<IBodyPart> { internalPartMock.Object });

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

           
            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                TargetBodyMock.Object,
                TargetPartMock.Object,
                StrikerMaterialMock.Object,
                sharpness,
                false);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.AddLayer(It.IsAny<IMaterial>()), Times.Never());
            BuilderMock.Verify(x => x.AddLayer(
                It.IsAny<IMaterial>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<object>()), Times.Never());

            BuilderMock.Verify(x => x.Build(), Times.Once());

            InjuryFactoryMock.Verify(x => x.Create(
                TargetBodyMock.Object,
                TargetPartMock.Object,
                contactArea, maxPen, StrikeResultMock.Object,
                It.Is<Dictionary<ITissueLayer, IBodyPart>>(d => d.Count() == 0)), Times.Once());
        }
    }
}
