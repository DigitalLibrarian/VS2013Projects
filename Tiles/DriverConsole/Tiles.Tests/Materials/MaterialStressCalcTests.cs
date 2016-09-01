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
    }
}
