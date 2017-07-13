using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials.Tests
{
    [TestClass]
    public class MaterialTests
    {
        Material M1 { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            int seq = 1;
            M1 = new Material("name", "adjective")
            {
                ImpactYield = seq++,
                ImpactFracture = seq++,
                ImpactStrainAtYield = seq++,

                ShearYield = seq++,
                ShearFracture = seq++,
                ShearStrainAtYield = seq++,

                CompressiveYield = seq++,
                CompressiveFracture = seq++,
                CompressiveStrainAtYield = seq++,

                TensileYield = seq++,
                TensileFracture = seq++,
                TensileStrainAtYield = seq++,

                TorsionYield = seq++,
                TorsionFracture = seq++,
                TorsionStrainAtYield = seq++,

                BendingYield = seq++,
                BendingFracture = seq++,
                BendingStrainAtYield = seq++,

                SolidDensity = seq++,
                SharpnessMultiplier = seq++
            };
        }

        [TestMethod]
        public void GetModeProperties_Blunt()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Blunt, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.ImpactYield, yield);
            Assert.AreEqual(M1.ImpactFracture, fracture);
            Assert.AreEqual(M1.ImpactStrainAtYield, strainAtYield);
        }

        [TestMethod]
        public void GetModeProperties_Edge()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Edge, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.ShearYield, yield);
            Assert.AreEqual(M1.ShearFracture, fracture);
            Assert.AreEqual(M1.ShearStrainAtYield, strainAtYield);
        }

        [TestMethod]
        public void GetModeProperties_Compressive()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Compressive, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.CompressiveYield, yield);
            Assert.AreEqual(M1.CompressiveFracture, fracture);
            Assert.AreEqual(M1.CompressiveStrainAtYield, strainAtYield);
        }

        [TestMethod]
        public void GetModeProperties_Tensile()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Tensile, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.TensileYield, yield);
            Assert.AreEqual(M1.TensileFracture, fracture);
            Assert.AreEqual(M1.TensileStrainAtYield, strainAtYield);
        }

        [TestMethod]
        public void GetModeProperties_Torsion()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Torsion, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.TorsionYield, yield);
            Assert.AreEqual(M1.TorsionFracture, fracture);
            Assert.AreEqual(M1.TorsionStrainAtYield, strainAtYield);
        }

        [TestMethod]
        public void GetModeProperties_Bending()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.Bending, out yield, out fracture, out strainAtYield);

            Assert.AreEqual(M1.BendingYield, yield);
            Assert.AreEqual(M1.BendingFracture, fracture);
            Assert.AreEqual(M1.BendingStrainAtYield, strainAtYield);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetModeProperties_None()
        {
            int yield, fracture, strainAtYield;

            M1.GetModeProperties(StressMode.None, out yield, out fracture, out strainAtYield);
        }

        [TestMethod]
        public void IsSoft_Edge()
        {
            M1.ShearStrainAtYield = 49999;
            Assert.IsFalse(M1.IsSoft(StressMode.Edge));

            M1.ShearStrainAtYield = 50000;
            Assert.IsTrue(M1.IsSoft(StressMode.Edge));
        }

        [TestMethod]
        public void IsSoft_NotEdged()
        {
            var enumValues = Enum.GetValues(typeof(StressMode))
                .AsQueryable()
                .Cast<StressMode>()
                .Where(x => x != StressMode.Edge);

            M1.ImpactStrainAtYield = 49999;

            foreach (var enumValue in enumValues)
            {
                Assert.IsFalse(M1.IsSoft(enumValue));
            }

            M1.ImpactStrainAtYield = 50000;

            foreach (var enumValue in enumValues)
            {
                Assert.IsTrue(M1.IsSoft(enumValue));
            }
        }
    }
}
