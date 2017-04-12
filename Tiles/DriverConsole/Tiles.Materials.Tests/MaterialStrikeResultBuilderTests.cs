using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        MaterialStrikeResultBuilder Builder { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Builder = new MaterialStrikeResultBuilder();
        }
        /*
        [TestMethod]
        public void Edge_AdamantineShortSwordOnSkin()
        {
            var strikerMat = TestMaterials.Adamantine;
            var strickenMat = TestMaterials.Skin;

            int contactArea = 20000;
            var threshold = MaterialStressCalc
                .GetEdgedBreakThreshold(contactArea, strikerMat, strickenMat);

            double momentum = threshold;
            Builder.SetStressMode(StressMode.Edge);

            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrikerMaterial(TestMaterials.Adamantine);
            Builder.SetStrickenMaterial(TestMaterials.Skin);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(momentum, result.Momentum);
            Assert.IsTrue(result.BreaksThrough);
            Assert.AreEqual(threshold, threshold);
        }

        [TestMethod]
        public void Edge_SkinShortSwordOnAdamantine()
        {
            var strikerMat = TestMaterials.Skin;
            var strickenMat = TestMaterials.Adamantine;

            int contactArea = 20000;
            var threshold = MaterialStressCalc
                .GetEdgedBreakThreshold(contactArea, strikerMat, strickenMat);
            double momentum = threshold;
            Builder.SetStressMode(StressMode.Edge);

            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(strickenMat);

            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.IsTrue(result.BreaksThrough);
            Assert.AreEqual(threshold, threshold);
        }

        [TestMethod]
        public void Edge_WoodShortSwordOnSteel()
        {
            var strikerMat = TestMaterials.Wood;
            var strickenMat = TestMaterials.Steel;

            int contactArea = 20;

            var threshold = MaterialStressCalc
                .GetEdgedBreakThreshold(contactArea, strikerMat, strickenMat);

            double momentum = threshold;
            Builder.SetStressMode(StressMode.Edge);

            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(strickenMat);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);

            Assert.IsTrue(result.BreaksThrough);
            Assert.AreEqual(threshold, result.MomentumThreshold);
        }

        [TestMethod]
        public void Blunt_SilverMaceOnBone()
        {
            var strikerMat = TestMaterials.Silver;
            var strickenMat = TestMaterials.Bone;

            int contactArea = 20;
            var threshold = MaterialStressCalc.GetBluntBreakThreshold(contactArea, strickenMat);
            double momentum = threshold;
            Builder.SetStressMode(StressMode.Blunt);
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);
            
            Builder.SetStrikerMaterial(strikerMat);
            Builder.SetStrickenMaterial(strickenMat);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.IsTrue(result.BreaksThrough);
            Assert.AreEqual(threshold, result.MomentumThreshold);

        }

        [TestMethod]
        public void Edge_SteelOnCopper()
        {
            Builder.SetStressMode(StressMode.Edge);

            double momentum = 0;
            int contactArea = 20000;
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrickenMaterial(TestMaterials.Copper);
            Builder.SetStrikerMaterial(TestMaterials.Steel);

            var result = Builder.Build();
            Assert.IsNotNull(result);
            Assert.IsFalse(result.BreaksThrough);
        }
         * */
    }
}
