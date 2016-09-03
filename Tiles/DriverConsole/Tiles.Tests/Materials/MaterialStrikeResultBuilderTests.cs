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
    public class MaterialStrikeResultBuilderTests
    {
        MaterialStrikeResultBuilder Builder { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Builder = new MaterialStrikeResultBuilder();
        }

        [TestMethod]
        public void Edge_AdamantineShortSwordOnSkin()
        {
            Builder.SetStressMode(StressMode.Edge);

            double momentum = 2820;
            int contactArea = 20000;
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrickenThickness(1071);

            Builder.SetStrikerMaterial(TestMaterials.Adamantine);
            Builder.SetStrickenMaterial(TestMaterials.Skin);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(momentum, result.Momentum);
            Assert.IsTrue(result.BreaksThrough);
            /*
            Assert.IsTrue(65.82 < result.Stress);
            Assert.IsTrue(65.83 > result.Stress);
            Assert.IsTrue(5 < result.MomentumThreshold);
            Assert.IsTrue(5.1 > result.MomentumThreshold);
            Assert.AreEqual(StressResult.Fracture, result.StressResult);
             * */
        }

        [TestMethod]
        public void Edge_SkinShortSwordOnAdamantine()
        {
            Builder.SetStressMode(StressMode.Edge);

            double momentum = 2820;
            int contactArea = 20000;
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrickenThickness(1071);

            Builder.SetStrikerMaterial(TestMaterials.Skin);
            Builder.SetStrickenMaterial(TestMaterials.Adamantine);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(StressResult.Elastic, result.StressResult);
            Assert.IsFalse(result.BreaksThrough);
            /*
            Assert.AreEqual(momentum, result.Momentum);
            Assert.AreEqual(0, result.Stress);
            Assert.IsTrue(0.00008 < result.MomentumThreshold);
            Assert.IsTrue(0.000081 > result.MomentumThreshold);
             * */
        }

        [TestMethod]
        public void Edge_WoodShortSwordOnSteel()
        {
            Builder.SetStressMode(StressMode.Edge);

            double momentum = 6;
            int contactArea = 20;
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrickenThickness(1071);

            Builder.SetStrikerMaterial(TestMaterials.Wood);
            Builder.SetStrickenMaterial(TestMaterials.Steel);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.AreEqual(StressMode.Edge, result.StressMode);
            Assert.AreEqual(StressResult.Elastic, result.StressResult);
            Assert.IsFalse(result.BreaksThrough);
            /*
            Assert.AreEqual(momentum, result.Momentum);
            Assert.IsTrue(0.04 < result.Stress);
            Assert.IsTrue(0.05 > result.Stress);
            Assert.IsTrue(1.1112 < result.MomentumThreshold);
            Assert.IsTrue(1.1113 > result.MomentumThreshold);
             * */
        }

        [TestMethod]
        public void Blunt_SilverMaceOnBone()
        {
            Builder.SetStressMode(StressMode.Blunt);

            double momentum = 39442.4;
            int contactArea = 20;
            Builder.SetStrikeMomentum(momentum);
            Builder.SetContactArea(contactArea);

            Builder.SetStrickenThickness(26315);

            Builder.SetStrikerMaterial(TestMaterials.Silver);
            Builder.SetStrickenMaterial(TestMaterials.Bone);

            var result = Builder.Build();

            Assert.IsNotNull(result);

            Assert.IsTrue(result.BreaksThrough);
            /*
            Assert.AreEqual(StressMode.Blunt, result.StressMode);
            Assert.AreEqual(StressResult.Fracture, result.StressResult);
            Assert.AreEqual(momentum, result.Momentum);
            Assert.AreEqual(0, result.Stress);
            Assert.AreEqual(1, result.MomentumThreshold);
             * */
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
    }
}
