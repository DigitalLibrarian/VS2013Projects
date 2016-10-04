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
        /*
        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_CopperOnSteel()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Copper, TestMaterials.Steel, 74.499);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnCopper()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Steel, TestMaterials.Copper, 2.02);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnBone_PinPrick()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Steel, TestMaterials.Bone, 3.25);

        }
        
        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnBone_Mace()
        {
            AssertMatOnMat_GetEdgeThreshold(
                20, TestMaterials.Steel, TestMaterials.Bone, 3.66);
        }
            
        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnBone_SwordSlash()
        {
            AssertMatOnMat_GetEdgeThreshold(
                20000, TestMaterials.Steel, TestMaterials.Bone, 436.56);
        }

        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_SteelOnNail_SwordSlash()
        {
            AssertMatOnMat_GetEdgeThreshold(
                20000, TestMaterials.Steel, TestMaterials.Nail, 436.56);
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
                1, TestMaterials.Steel, TestMaterials.Skin, 0.56);
        }
        
        [TestMethod]
        public void TestMaterials_GetEdgeThreshold_CopperOnSkin()
        {
            AssertMatOnMat_GetEdgeThreshold(
                1, TestMaterials.Copper, TestMaterials.Skin, 3.45);
        }
        */
        [TestMethod]
        public void RelativeEdgeMats()
        {
            //Adamantine	Steel	Iron	Bronze, Bismuth Bronze	Copper	Silver
            int contactArea = 20000;
            var controlMat = TestMaterials.Skin;
            var adamResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Adamantine, controlMat);
            var steelResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Steel, controlMat);
            var ironResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Iron, controlMat);
            var bronzeResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Bronze, controlMat);
            var copperResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Copper, controlMat);
            var silverResult = MaterialStressCalc.GetEdgedBreakThreshold(contactArea, TestMaterials.Silver, controlMat);

            Assert.IsTrue(adamResult < steelResult);
            Assert.IsTrue(steelResult < ironResult);
            Assert.IsTrue(ironResult < bronzeResult);
            Assert.IsTrue(bronzeResult < copperResult);
            Assert.IsTrue(copperResult < silverResult);
        }
        /*
        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Skin_PinPrick()
        {
            AssertMatOnMat_GetBluntThreshold(
                1, TestMaterials.Skin,
                0.0);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Skin_Mace()
        {
            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Skin,
                0.0);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Skin_SwordSlash()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Skin,
                4.8);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Silver_Small()
        {
            AssertMatOnMat_GetBluntThreshold(
                0, TestMaterials.Silver,
                0.0);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Silver_Medium()
        {
            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Silver,
                0.4);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Silver_Large()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Silver,
                403.2);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Bone_SmallContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Bone,
                0.09);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Bone_MediumContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                200, TestMaterials.Bone,
                0.96);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Bone_LargeContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Bone,
                96);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Nail_SmallContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                20, TestMaterials.Nail,
                0.09);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Nail_MediumContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                200, TestMaterials.Nail,
                0.96);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Nail_LargeContactArea()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Nail,
                96);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Copper()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Copper,
                621.59);
        }

        [TestMethod]
        public void TestMaterials_GetBluntThreshold_Iron()
        {
            AssertMatOnMat_GetBluntThreshold(
                20000, TestMaterials.Iron,
                781.2);
        }

        */
        void AssertMatOnMat_GetEdgeThreshold(int contactArea, IMaterial strikerMat, IMaterial strickenMat, double expectedThreshold)
        {
            var thresh = MaterialStressCalc.GetEdgedBreakThreshold(
                contactArea, strikerMat, strickenMat);

            AssertRoughly(
                expectedThreshold,
                thresh,
                string.Format("edged threshold={0}", thresh));
        }

        void AssertRoughly(double expected, double actual, string message)
        {
            Assert.AreEqual(
                (int)(expected * 100) / 100d,
                (int)(actual * 100) / 100d,
                message);
        }
    }
}
