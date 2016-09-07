using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Tests.Materials
{
    [TestClass]
    public class MaterialStressCalcTests
    {
        //[TestMethod]
        //public void StressLayer_Zero()
        //{
        //    int yield = 10;
        //    int fracture = 20;
        //    int strainAtYield = 30;

        //    double momentum = 0;

        //    double deform;

        //    var result = MaterialStressCalc.StressLayer(
        //        momentum, 1, 1,
        //        yield, fracture, strainAtYield,
        //        out deform);

        //    Assert.AreEqual(StressResult.Elastic, result);
        //    Assert.AreEqual(0, deform);
        //}

        //[TestMethod]
        //public void StressLayer_AdamantineShortSwordonSkin()
        //{
        //    int yield = 20000;
        //    int fracture = 20000;
        //    int strainAtYield = 50000;
        //    int thickness = 1071;
        //    int contactArea = 20000;

        //    double momentum = 28.20;

        //    double deform;

        //    var result = MaterialStressCalc.StressLayer(
        //        momentum, thickness, contactArea,
        //        yield, fracture, strainAtYield,
        //        out deform);

        //    Assert.AreEqual(StressResult.Fracture, result);
        //    Assert.AreEqual(1d, deform);
        //}

        //[TestMethod]
        //public void EdgedStress_AdamantineShortSwordOnSkin()
        //{
        //    var adam = TestMaterials.Adamantine;
        //    var skin = TestMaterials.Skin;
            
        //    int thickness = 1071;
        //    int contactArea = 20000;
        //    double momentum = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, adam, skin);

        //    var result = PerformEdgedTest(momentum, contactArea, thickness, adam, skin);
        //    Assert.IsTrue(result);

        //    result = PerformEdgedTest(momentum - 0.01d, contactArea, thickness, adam, skin);
        //    Assert.IsFalse(result);

        //    result = PerformEdgedTest(momentum + 0.01d, contactArea, thickness, adam, skin);
        //    Assert.IsTrue(result);
        //}

        //[TestMethod]
        //public void BluntStress_SilverMaceOnBone()
        //{
        //    var silver = TestMaterials.Silver;
        //    var bone = TestMaterials.Bone;

        //    int size = 800;
        //    double mom = 39.4424;
        //    int thickness = 26315;
        //    int contactArea = 20;

        //    var result = PerformBluntTest(size, mom, contactArea, thickness, silver, bone);
        //    Assert.IsTrue(result);

        //    result = PerformBluntTest(size, 2771, contactArea, thickness, silver, bone);
        //    Assert.IsFalse(result);

        //    result = PerformBluntTest(size, 2772, contactArea, thickness, silver, bone);
        //    Assert.IsTrue(result);
        //}

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_CopperOnSteel()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Copper, TestMaterials.Steel, 113.37);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnCopper()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Steel, TestMaterials.Copper, 5.65);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnBone()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Steel, TestMaterials.Bone, 5.39);

            AssertMatOnMat_GetEdgeThreshold(
                20, TestMaterials.Steel, TestMaterials.Bone, 5.8);

            AssertMatOnMat_GetEdgeThreshold(
                20000, TestMaterials.Steel, TestMaterials.Bone, 438.7);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_AdamantineOnSkin()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Adamantine, TestMaterials.Skin, 0);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnSkin()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Steel, TestMaterials.Skin, 0.89);
        }
        
        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_CopperOnSkin()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Copper, TestMaterials.Skin, 4.53);
        }

        [TestMethod]
        public void RelativeEdgeMats()
        {
            //Adamantine	Steel	Iron	Bronze, Bismuth Bronze	Copper	Silver

            var adamResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Adamantine, TestMaterials.Bone);
            var steelResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Steel, TestMaterials.Bone);
            var ironResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Iron, TestMaterials.Bone);
            var bronzeResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Bronze, TestMaterials.Bone);
            var copperResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Copper, TestMaterials.Bone);
            var silverResult = MaterialStressCalc.GetEdgedBreakThreshold(20, TestMaterials.Silver, TestMaterials.Bone);

            Assert.IsTrue(adamResult > steelResult);
            Assert.IsTrue(steelResult > ironResult);
            Assert.IsTrue(ironResult > bronzeResult);
            Assert.IsTrue(bronzeResult > copperResult);
            Assert.IsTrue(copperResult > silverResult);
        }

        [TestMethod]
        public void TestMaterials_GetBluntTheshold()
        {
            AssertMatOnMat_GetBluntThreshold(
                1, TestMaterials.Skin,
                0.0);

            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Skin,
                0.0);

            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Skin,
                4.8);
        }

        [TestMethod]
        public void TestMaterials_GetBluntTheshold_Silver()
        {
            AssertMatOnMat_GetBluntThreshold(
                0, TestMaterials.Silver,
                0.0);

            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Silver,
                0.4);

            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Silver,
                403.2);
        }


        [TestMethod]
        public void TestMaterials_GetBluntTheshold_Bone()
        {
            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Bone,
                0.09);

            AssertMatOnMat_GetBluntThreshold(
                200, TestMaterials.Bone,
                0.96);

            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Bone,
                96);
        }
        
        [TestMethod]
        public void TestMaterials_GetBluntTheshold_Copper()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Copper,
                621.59);
        }

        [TestMethod]
        public void TestMaterials_GetBluntTheshold_Iron()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Iron,
                781.2);
        }


        //bool PerformEdgedTest(double momentum, int contactArea, int thickness, IMaterial weaponMat, IMaterial layerMat)
        //{
        //    return MaterialStressCalc.EdgedStress(momentum, contactArea, thickness,
        //        weaponMat.ShearYield, weaponMat.ShearFracture, weaponMat.ShearStrainAtYield,
        //        layerMat.ShearYield, layerMat.ShearFracture, layerMat.ShearStrainAtYield);
        //}
        //bool PerformBluntTest(
        //    int weaponSize,
        //    double momentum, int contactArea, int thickness, IMaterial weaponMat, IMaterial layerMat)
        //{
        //    return MaterialStressCalc.BluntStress(momentum, contactArea, thickness,
        //        weaponMat.ImpactYield, weaponMat.ImpactFracture, weaponMat.ImpactStrainAtYield,
        //        layerMat.ImpactYield, layerMat.ImpactFracture, layerMat.ImpactStrainAtYield);
        //}



        void AssertMatOnMat_GetEdgeThreshold(int contactArea, IMaterial strikerMat, IMaterial strickenMat, double expectedThreshold)
        {
            var thresh = MaterialStressCalc.GetEdgedBreakThreshold(
                contactArea, strikerMat, strickenMat);

            AssertRoughly(
                expectedThreshold,
                thresh,
                "edged threshold");
        }

        void AssertRoughly(double expected, double actual, string message)
        {
            Assert.AreEqual(
                (int)(expected * 100) / 100d,
                (int)(actual * 100) / 100d,
                message);
        }
        public void AssertMatOnMat_GetBluntThreshold(int contactArea, IMaterial strickenMat, double expected)
        {
            var thresh = MaterialStressCalc.GetBluntBreakThreshold(contactArea, strickenMat);
            AssertRoughly(
                expected,
                thresh,
                "blunt threshold");
        }
    }
}
