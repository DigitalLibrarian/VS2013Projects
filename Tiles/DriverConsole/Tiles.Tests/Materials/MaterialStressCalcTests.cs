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
        [TestMethod]
        public void StressLayer_Zero()
        {
            int yield = 10;
            int fracture = 20;
            int strainAtYield = 30;

            double momentum = 0;

            double deform;

            var result = MaterialStressCalc.StressLayer(
                momentum, 1, 1,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Elastic, result);
            Assert.AreEqual(0, deform);
        }

        [TestMethod]
        public void StressLayer_AdamantineShortSwordonSkin()
        {
            int yield = 20000;
            int fracture = 20000;
            int strainAtYield = 50000;
            int thickness = 1071;
            int contactArea = 20000;

            double momentum = 2820;

            double deform;

            var result = MaterialStressCalc.StressLayer(
                momentum, thickness, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Fracture, result);
            Assert.AreEqual(13165, (int)(deform*100));
        }

        [TestMethod]
        public void EdgedStress_AdamantineShortSwordOnSkin()
        {
            var adam = TestMaterials.Adamantine;
            var skin = TestMaterials.Skin;
            
            double momentum = 2820;
            int thickness = 1071;
            int contactArea = 20000;

            var result = PerformEdgedTest(momentum, contactArea, thickness, adam, skin);
            Assert.IsTrue(result);

            result = PerformEdgedTest(5.0, contactArea, thickness, adam, skin);
            Assert.IsFalse(result);

            result = PerformEdgedTest(5.05, contactArea, thickness, adam, skin);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BluntStress_SilverMaceOnBone()
        {
            var silver = TestMaterials.Silver;
            var bone = TestMaterials.Bone;

            int size = 800;
            double mom = 39442.4;
            int thickness = 26315;
            int contactArea = 20;

            var result = PerformBluntTest(size, mom, contactArea, thickness, silver, bone);
            Assert.IsTrue(result);

            result = PerformBluntTest(size, 1343, contactArea, thickness, silver, bone);
            Assert.IsFalse(result);

            result = PerformBluntTest(size, 1344, contactArea, thickness, silver, bone);
            Assert.IsTrue(result);

        }


        bool PerformEdgedTest(double momentum, int contactArea, int thickness, IMaterial weaponMat, IMaterial layerMat)
        {
            return MaterialStressCalc.EdgedStress(momentum, contactArea, thickness,
                weaponMat.ShearYield, weaponMat.ShearFracture, weaponMat.ShearStrainAtYield,
                layerMat.ShearYield, layerMat.ShearFracture, layerMat.ShearStrainAtYield);
        }
        bool PerformBluntTest(
            int weaponSize,
            double momentum, int contactArea, int thickness, IMaterial weaponMat, IMaterial layerMat)
        {
            return MaterialStressCalc.BluntStress(weaponSize, momentum, contactArea, thickness,
                weaponMat.ImpactYield, weaponMat.ImpactFracture, weaponMat.ImpactStrainAtYield,
                layerMat.ImpactYield, layerMat.ImpactFracture, layerMat.ImpactStrainAtYield);
        }
    }
}
