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

        Mock<IBody> BodyMock { get; set; }
        Mock<IBodyPart> PartMock { get; set; }
        Mock<IMaterial> StrikerMaterialMock { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            InjuryFactoryMock = new Mock<IInjuryFactory>();
            BuilderMock = new Mock<ILayeredMaterialStrikeResultBuilder>();

            Calc = new InjuryReportCalc(InjuryFactoryMock.Object, BuilderMock.Object);

            BodyMock = new Mock<IBody>();
            PartMock = new Mock<IBodyPart>();
            var tissueMock = new Mock<ITissue>();
            var tissueLayers = new List<ITissueLayer>();
            tissueMock.Setup(x => x.TissueLayers).Returns(tissueLayers);
            PartMock.Setup(x => x.Tissue).Returns(tissueMock.Object);
            StrikerMaterialMock = new Mock<IMaterial>();
        }

        [TestMethod]
        public void MaterialStrike_NonLayerBuilderParams()
        {
            var armorItems = new List<IItem>();
            var stressMode = StressMode.Other;
            double momentum = 1d, contactArea = 2d, sharpness = 4d;
            double partContactArea = 5d;
            var maxPen = 3;

            PartMock.Setup(x => x.GetContactArea())
                .Returns(partContactArea);

            var bpInjuries = new List<IBodyPartInjury>();
            InjuryFactoryMock.Setup(x => x.Create(
                PartMock.Object,
                contactArea, maxPen, It.IsAny<ILayeredMaterialStrikeResult>(),
                It.IsAny<Dictionary<ITissueLayer, IBodyPart>>()
                )).Returns(bpInjuries);

            var result = Calc.CalculateMaterialStrike(
                armorItems,
                stressMode,
                momentum,
                contactArea,
                maxPen,
                BodyMock.Object,
                PartMock.Object,
                StrikerMaterialMock.Object,
                sharpness);

            Assert.IsNotNull(result);
            Assert.AreSame(bpInjuries, result.BodyPartInjuries);

            BuilderMock.Verify(x => x.Clear(), Times.Once());
            BuilderMock.Verify(x => x.SetStressMode(stressMode), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerContactArea(contactArea), Times.Once());
            BuilderMock.Verify(x => x.SetStrickenContactArea(partContactArea), Times.Once());
            BuilderMock.Verify(x => x.SetMaxPenetration(maxPen), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerMaterial(StrikerMaterialMock.Object), Times.Once());
            BuilderMock.Verify(x => x.SetStrikerSharpness(sharpness), Times.Once());

            BuilderMock.Verify(x => x.Build());
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_ZeroCase()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_ArmorLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_NonCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_CosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_InternalNonCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void MaterialStrike_InternalCosmeticTissueLayer()
        {
            throw new NotImplementedException();
        }
    }
}
