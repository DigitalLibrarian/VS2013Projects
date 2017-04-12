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
            int oneCubicMeter = 1000; // in cm3
            var mass = Steel.GetMassForUniformVolume(oneCubicMeter);
            Assert.AreEqual(7850d, mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelSword()
        {
            int swordSizeCubicCm = 300;
            var mass = Steel.GetMassForUniformVolume(swordSizeCubicCm);

            Assert.AreEqual(2355, (int)mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelAxe()
        {
            int sizeCubicCm = 800;
            var mass = Steel.GetMassForUniformVolume(sizeCubicCm);
            Assert.AreEqual(6280, mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelSpear()
        {
            int sizeCubicCm = 400;
            var mass = Steel.GetMassForUniformVolume(sizeCubicCm);

            Assert.AreEqual(3140, (int)mass);
        }

        [TestMethod]
        public void GetMassUniformVolume_SteelMace()
        {
            int sizeCubicCm = 800;
            var mass = Steel.GetMassForUniformVolume(sizeCubicCm);

            Assert.AreEqual(6280, (int)mass);
        }
    }
}
