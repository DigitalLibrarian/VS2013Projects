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

            int momentum = 0;
            int contactArea = 10;

            int deform;

            var result = MaterialStressCalc.StressLayer(
                momentum, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Elastic, result);
            Assert.AreEqual(0, deform);
        }

        [TestMethod]
        public void StressLayer_Elastic()
        {
            int thickness = 10;

            int yield = 10;
            int fracture = 20;
            int strainAtYield = 30;

            int momentum = yield-1;
            int contactArea = 10;

            int deform;

            var result = MaterialStressCalc.StressLayer(
                momentum, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Elastic, result);
            Assert.AreEqual(270, deform);

            momentum = yield;
            result = MaterialStressCalc.StressLayer(
                momentum, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Plastic, result);
            Assert.AreEqual(300, deform);
        }

        [TestMethod]
        public void StressLayer_Plastic()
        {
            int yield = 10;
            int fracture = 20;
            int strainAtYield = 30;

            int momentum = fracture - 1;
            int contactArea = 10;

            int deform;

            var result = MaterialStressCalc.StressLayer(
                momentum, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Plastic, result);
            Assert.AreEqual(570, deform);

            momentum = fracture;
            result = MaterialStressCalc.StressLayer(
                momentum, contactArea,
                yield, fracture, strainAtYield,
                out deform);

            Assert.AreEqual(StressResult.Fracture, result);
            Assert.AreEqual(600, deform);
        }
    }
}
