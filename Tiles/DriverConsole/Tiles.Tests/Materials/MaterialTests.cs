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
    public class MaterialTests
    {
        Material Steel = new Material("steel", "steel")
        {
            ImpactYield = 1505000,
            ImpactFracture = 2520000,
            ImpactStrainAtYield = 940,

            ShearYield = 430000,
            ShearFracture = 720000,
            ShearStrainAtYield = 215,

            SolidDensity = 7850
        };

        [TestMethod]
        public void GetMassUniformVolume_UnitVolume()
        {
            int oneCubicMeter = 100; // in cm
            var mass = Steel.GetMassForUniformVolume(oneCubicMeter);
            Assert.AreEqual(7850d, mass);
        }
        [TestMethod]
        public void GetMassUniformVolume_SteelSword()
        {
            int swordSizeCubicCm = 300;
            var mass = Steel.GetMassForUniformVolume(swordSizeCubicCm);

            Assert.AreEqual(23550, (int)mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelSpear()
        {
            int sizeCubicCm = 400;
            var mass = Steel.GetMassForUniformVolume(sizeCubicCm);

            Assert.AreEqual(31400, (int)mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelMace()
        {
            int sizeCubicCm = 800;
            var mass = Steel.GetMassForUniformVolume(sizeCubicCm);

            Assert.AreEqual(62800, (int)mass);
        }
    }
}
