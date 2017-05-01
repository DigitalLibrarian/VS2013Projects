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
        public void ContactArea_StrikerIsSmaller()
        {
            double sharpness = 1d, momentum = 1d, thickness = 1d, volume = 1d, remainingPen = 10d;
            double strickenContactArea = 10d;
            double strikerContactArea = 9d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            Builder.SetStressMode(StressMode.None);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerMaterial(strikerMaterialMock.Object);
            Builder.SetStrickenContactArea(strickenContactArea);
            Builder.SetStrickenMaterial(strickenMaterialMock.Object);
            Builder.SetStrikerSharpness(sharpness);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerThickness(thickness);
            Builder.SetLayerVolume(volume);
            Builder.SetRemainingPenetration(remainingPen);

            var result = Builder.Build();

            Assert.AreEqual(981, (int) (result.ContactAreaRatio * 1000));
            Assert.AreEqual(981, (int) (result.ContactArea * 100));
        }

        [TestMethod]
        public void ContactArea_StrickenIsSmaller()
        {
            double sharpness = 5000d, momentum = 1d, thickness = 1d, volume = 1d, remainingPen = 10d;
            double strickenContactArea = 9d;
            double strikerContactArea = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            Builder.SetStressMode(StressMode.None);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerMaterial(strikerMaterialMock.Object);
            Builder.SetStrickenContactArea(strickenContactArea);
            Builder.SetStrickenMaterial(strickenMaterialMock.Object);
            Builder.SetStrikerSharpness(sharpness);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerThickness(thickness);
            Builder.SetLayerVolume(volume);
            Builder.SetRemainingPenetration(remainingPen);

            var result = Builder.Build();

            Assert.AreEqual(1000, (int)(result.ContactAreaRatio * 1000));
            Assert.AreEqual((int)(strickenContactArea - 1d), (int) result.ContactArea);
        }

        [TestMethod]
        public void ContactArea_SameSizeStriker()
        {
            double contactArea = 10d, sharpness = 5000d, momentum = 1d, thickness = 1d, volume = 1d, remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            Builder.SetStressMode(StressMode.None);
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

            Assert.AreEqual(1d, result.ContactAreaRatio);
            Assert.AreEqual(contactArea - 1d, result.ContactArea);
        }

        [TestMethod]
        public void Edged_None_NoStrainResist()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 5000d, momentum = 1d;
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
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
            Assert.AreEqual(momentum, result.ResultMomentum);
        }
        
        [TestMethod]
        public void Edged_Undefeated_StrainAtYield_To_ResultMomentum()
        {
            // Stricken strain at yield -> result momentum
            var d = new Dictionary<int, double>
            {
                { 25000, 5d },          // absorb 50%
                { 50000, 10d },         // no contribution
                { 100000, 20d },          // yield
            };

            foreach (var pair in d)
            {
                int strainAtYield = pair.Key;
                double expectedResultMomentum = pair.Value;

                var stressMode = StressMode.Edge;
                double contactArea = 10d, sharpness = 5000d, momentum = 10d;
                double thickness = 1d, volume = 1d;
                var remainingPen = 10d;

                var strikerMaterialMock = new Mock<IMaterial>();
                var strickenMaterialMock = new Mock<IMaterial>();

                var expectedStress = 10d;
                var episilon = 0.0001d;
                StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                    .Returns(expectedStress + episilon);

                int strickenYield = 1, strickenFracture = 1;
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
                Assert.AreEqual(StressResult.None, result.StressResult);
                Assert.IsFalse(result.IsDefeated);
                Assert.AreEqual(expectedResultMomentum, result.ResultMomentum, episilon);
            }
        }

        [TestMethod]
        public void Edged_Defeated_Sharpness_To_ResultMomentum()
        {
            // Weapon Sharpness -> result momentum
            var d = new Dictionary<double, double>
            {
                { 2500d, 8.00006d },        // dull
                { 5000d, 9.00003d },        // median 
                { 7500d, 9.33335d },        // sharp
                { 10000d, 9.500015d },      // double sharp
            };

            foreach (var pair in d)
            {
                double sharpness = pair.Key;
                double expectedResultMomentum = pair.Value;

                var stressMode = StressMode.Edge;
                double contactArea = 10d, momentum = 10d;
                double thickness = 1d, volume = 1d;
                var remainingPen = 10d;

                var strikerMaterialMock = new Mock<IMaterial>();
                var strickenMaterialMock = new Mock<IMaterial>();

                var expectedStress = 10d;
                var episilon = 0.0001d;
                StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                    .Returns(expectedStress - episilon - episilon - episilon);

                StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                    .Returns(episilon);

                StressCalcMock.Setup(x => x.ShearCost3(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness, volume))
                    .Returns(episilon);

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
                Assert.AreEqual(StressResult.Shear_CutThrough, result.StressResult);
                Assert.IsTrue(result.IsDefeated);
                Assert.IsTrue(result.ResultMomentum > 0, "Non-Positive Result Momentum");
                Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
                Assert.IsTrue(result.ResultMomentum < momentum, "Slows down");
                Assert.AreEqual(expectedResultMomentum, result.ResultMomentum, episilon);
            }
        }

        [TestMethod]
        public void Edged_Shear_Dent_NoStrainResist()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 5000d, momentum = 1d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 1d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(episilon);

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
            Assert.AreEqual(momentum, result.ResultMomentum);
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
        }

        [TestMethod]
        public void Edged_Shear_Cut_NoStrainResist_InadequateStress()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 5000d, momentum = 10d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 10d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(episilon);

            StressCalcMock.Setup(x => x.ShearCost3(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness, volume))
                .Returns(episilon);

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
            Assert.AreEqual(StressResult.Shear_Cut, result.StressResult);
            Assert.IsTrue(result.IsDefeated);
            Assert.IsTrue(result.ResultMomentum > 0, "Non-Positive Result Momentum");
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
            Assert.IsTrue(result.ResultMomentum < momentum, "Slows down");
            Assert.AreEqual(9, (int)result.ResultMomentum);
        }
        
        [TestMethod]
        public void Edged_Shear_Cut_NoStrainResist_EnoughStress_LowWeaponContactArea()
        {
            var stressMode = StressMode.Edge;
            double strickenContactArea = 10d;
            double strikerContactArea = strickenContactArea - 1d;
            double sharpness = 5000d, momentum = 10d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 0.189999999999999d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon - episilon - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(episilon);

            StressCalcMock.Setup(x => x.ShearCost3(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness, volume))
                .Returns(episilon);

            int strickenYield = 1, strickenFracture = 1, strainAtYield = 50000;
            strickenMaterialMock.Setup(x => x.GetModeProperties(stressMode, out strickenYield, out strickenFracture, out strainAtYield));

            Builder.SetStressMode(stressMode);
            Builder.SetStrikerContactArea(strikerContactArea);
            Builder.SetStrikerMaterial(strikerMaterialMock.Object);
            Builder.SetStrickenContactArea(strickenContactArea);
            Builder.SetStrickenMaterial(strickenMaterialMock.Object);
            Builder.SetStrikerSharpness(sharpness);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetLayerThickness(thickness);
            Builder.SetLayerVolume(volume);
            Builder.SetRemainingPenetration(remainingPen);

            var result = Builder.Build();

            Assert.AreEqual((int)(10000 * expectedStress / 100), (int)(10000 * result.Stress / 100));
            Assert.AreEqual(momentum, result.Momentum);
            var expectedContactArea = strikerContactArea + (strikerContactArea * 0.09d);

            Assert.AreEqual(expectedContactArea, result.ContactArea);
            Assert.AreEqual(981, (int)(result.ContactAreaRatio*1000));
            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(StressResult.Shear_Cut, result.StressResult);
            Assert.IsTrue(result.IsDefeated);
            Assert.IsTrue(result.ResultMomentum > 0, "Non-Positive Result Momentum");
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
            Assert.IsTrue(result.ResultMomentum < momentum, "Slows down");
            Assert.AreEqual(9, (int)result.ResultMomentum);
        }

        [TestMethod]
        public void Edged_Shear_Cut_NoStrainResist_EnoughStress_EnoughContactArea_LowPenetrationRemaining()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 5000d, momentum = 10d;
            double thickness = 2d, volume = 1d;
            var remainingPen = 1d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 10d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon - episilon - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(episilon);

            StressCalcMock.Setup(x => x.ShearCost3(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness, volume))
                .Returns(episilon);

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
            Assert.AreEqual(StressResult.Shear_Cut, result.StressResult);
            Assert.IsTrue(result.IsDefeated);
            Assert.IsTrue(result.ResultMomentum > 0, "Non-Positive Result Momentum");
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
            Assert.IsTrue(result.ResultMomentum < momentum, "Slows down");
            Assert.AreEqual(8, (int)result.ResultMomentum);
        }

        [TestMethod]
        public void Edged_Shear_CutThrough_NoStrainResist()
        {
            var stressMode = StressMode.Edge;
            double contactArea = 10d, sharpness = 5000d, momentum = 10d;
            double thickness = 1d, volume = 1d;
            var remainingPen = 10d;

            var strikerMaterialMock = new Mock<IMaterial>();
            var strickenMaterialMock = new Mock<IMaterial>();

            var expectedStress = 10d;
            var episilon = 0.0001d;
            StressCalcMock.Setup(x => x.ShearCost1(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(expectedStress - episilon - episilon - episilon);

            StressCalcMock.Setup(x => x.ShearCost2(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness))
                .Returns(episilon);

            StressCalcMock.Setup(x => x.ShearCost3(strikerMaterialMock.Object, strickenMaterialMock.Object, sharpness, volume))
                .Returns(episilon);

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
            Assert.AreEqual(StressResult.Shear_CutThrough, result.StressResult);
            Assert.IsTrue(result.IsDefeated);
            Assert.IsTrue(result.ResultMomentum > 0, "Non-Positive Result Momentum");
            Assert.IsTrue(result.ResultMomentum <= momentum, "Conserve energy");
            Assert.IsTrue(result.ResultMomentum < momentum, "Slows down");
            Assert.AreEqual(9, (int)result.ResultMomentum);
        }

        [Ignore]
        [TestMethod]
        public void Blunt_None_NoStrainResist()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_Dent_NoStrainResist()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_InitiateFracture_NoStrainResist()
        {
            throw new NotImplementedException();
        }
        
        [Ignore]
        [TestMethod]
        public void Blunt_CompleteFracture_NoStrainResist()
        {
            throw new NotImplementedException();
        }

        [Ignore]
        [TestMethod]
        public void Blunt_Deflection()
        {
            throw new NotImplementedException();
        }
    }
}
