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
    public class MaterialStrikeResultBuilderTests
    {
        Mock<IMaterialStressCalc> StressCalcMock { get; set; }

        MaterialStrikeResultBuilder Builder { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            StressCalcMock = new Mock<IMaterialStressCalc>();

            Builder = new MaterialStrikeResultBuilder(StressCalcMock.Object);
        }

        [Ignore]
        [TestMethod]
        public void Clear()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Edged_None()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 1d, momentum = 1d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 1d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress + episilon);

            int strickenYield = 1, strickenFracture = 1, strainAtYield = 50000;
            strickenMaterialMock.Setup(x => x.GetModeProperties(stressMode, out strickenYield, out strickenFracture, out strainAtYield));

            Builder.SetStressMode(stressMode);
            Builder.SetStrikerContactArea(contactArea);
            Builder.SetStrikerMaterial(strikerMaterialMock.Object);
            Builder.SetStrickenContactArea(contactArea);
            Builder.SetStrickenMaterial(strickenMaterialMock.Object);
            Builder.SetStrikerSharpness(sharpness);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerThickness(thickness);
            Builder.SetLayerVolume(volume);
            Builder.SetRemainingPenetration(remainingPen);

            var result = Builder.Build();

            Assert.AreEqual(expectedStress, result.Stress);
            Assert.AreEqual(momentum, result.Momentum);
            Assert.AreEqual(contactArea-1, result.ContactArea);
            Assert.AreEqual(1d, result.ContactAreaRatio);
            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(StressResult.None, result.StressResult);
            Assert.IsFalse(result.IsDefeated);
            Assert.AreEqual(0, result.ResultMomentum);
        }

        [TestMethod]
        public void Edged_ShearDent()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 1d, momentum = 1d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 1d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress + episilon);

            int strickenYield = 1, strickenFracture = 1, strainAtYield = 50000;
            strickenMaterialMock.Setup(x => x.GetModeProperties(stressMode, out strickenYield, out strickenFracture, out strainAtYield));

            Builder.SetStressMode(stressMode);
            Builder.SetStrikerContactArea(contactArea);
            Builder.SetStrikerMaterial(strikerMaterialMock.Object);
            Builder.SetStrickenContactArea(contactArea);
            Builder.SetStrickenMaterial(strickenMaterialMock.Object);
            Builder.SetStrikerSharpness(sharpness);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerThickness(thickness);
            Builder.SetLayerVolume(volume);
            Builder.SetRemainingPenetration(remainingPen);

            var result = Builder.Build();

            Assert.AreEqual(expectedStress, result.Stress);
            Assert.AreEqual(momentum, result.Momentum);
            Assert.AreEqual(contactArea - 1, result.ContactArea);
            Assert.AreEqual(1d, result.ContactAreaRatio);
            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(StressResult.Shear_Dent, result.StressResult);
            Assert.IsFalse(result.IsDefeated);
            Assert.AreEqual(0, result.ResultMomentum);
        }

        [Ignore]
        [TestMethod]
        public void Edged_PartialCut()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Edged_CompleteCut()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_Deflection()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_PartialDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_CompleteDent()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_PartialFracture()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_CompleteFracture()
        {
            throw new NotImplementedException();
        }
    }
}
