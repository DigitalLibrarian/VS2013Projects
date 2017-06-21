using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Materials
{
    public static class StockMaterials
    {
        public static readonly IMaterial Skin = new Material("skin", "skin")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            CompressiveYield = 10000,
            CompressiveFracture = 10000,
            CompressiveStrainAtYield = 50000,

            TensileYield = 10000,
            TensileFracture = 10000,
            TensileStrainAtYield = 50000,

            TorsionYield = 10000,
            TorsionFracture = 10000,
            TorsionStrainAtYield = 50000,

            BendingYield = 10000,
            BendingFracture = 10000,
            BendingStrainAtYield = 50000,

            SolidDensity = 1000,
            SharpnessMultiplier = 0
        };

        public static readonly IMaterial Muscle = new Material("muscle", "muscle")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            CompressiveYield = 10000,
            CompressiveFracture = 10000,
            CompressiveStrainAtYield = 50000,

            TensileYield = 10000,
            TensileFracture = 10000,
            TensileStrainAtYield = 50000,

            TorsionYield = 10000,
            TorsionFracture = 10000,
            TorsionStrainAtYield = 50000,

            BendingYield = 10000,
            BendingFracture = 10000,
            BendingStrainAtYield = 50000,

            SolidDensity = 1060,
            SharpnessMultiplier = 0
        };

        public static readonly IMaterial Bone = new Material("bone", "bone")
        {
            ImpactYield = 200000,
            ImpactFracture = 200000,
            ImpactStrainAtYield = 100,

            ShearYield = 115000,
            ShearFracture = 130000,
            ShearStrainAtYield = 100,

            CompressiveYield = 20000,
            CompressiveFracture = 20000,
            CompressiveStrainAtYield = 100,

            TensileYield = 115000,
            TensileFracture = 130000,
            TensileStrainAtYield = 100,

            TorsionYield = 11500,
            TorsionFracture = 130000,
            TorsionStrainAtYield = 100,

            BendingYield = 115000,
            BendingFracture = 130000,
            BendingStrainAtYield = 100,

            SolidDensity = 500,
            SharpnessMultiplier = 0.1d
        };

        public static readonly IMaterial Nail = new Material("nail", "nail")
        {
            ImpactYield = 200000,
            ImpactFracture = 200000,
            ImpactStrainAtYield = 5000,

            ShearYield = 115000,
            ShearFracture = 130000,
            ShearStrainAtYield = 5000,

            CompressiveYield = 200000,
            CompressiveFracture = 200000,
            CompressiveStrainAtYield = 5000,

            TensileYield = 115000,
            TensileFracture = 130000,
            TensileStrainAtYield = 5000,

            TorsionYield = 115000,
            TorsionFracture = 130000,
            TorsionStrainAtYield = 5000,

            BendingYield = 115000,
            BendingFracture = 130000,
            BendingStrainAtYield = 5000,

            SolidDensity = 500,
            SharpnessMultiplier = 0.1d
        };

        public static readonly IMaterial Steel = new Material("steel", "steel")
        {
            ImpactYield = 1505000,
            ImpactFracture = 2520000,
            ImpactStrainAtYield = 940,

            ShearYield = 430000,
            ShearFracture = 720000,
            ShearStrainAtYield = 215,

            CompressiveYield = 1505000,
            CompressiveFracture = 2520000,
            CompressiveStrainAtYield = 940,

            TensileYield = 430000,
            TensileFracture = 720000,
            TensileStrainAtYield = 225,

            TorsionYield = 430000,
            TorsionFracture = 720000,
            TorsionStrainAtYield = 215,

            BendingYield = 430000,
            BendingFracture = 720000,
            BendingStrainAtYield = 215,

            SolidDensity = 7850,
            SharpnessMultiplier = 1d
        };

        public static readonly IMaterial Silver = new Material("silver", "silver")
        {
            ImpactYield = 350000,
            ImpactFracture = 595000,
            ImpactStrainAtYield = 350,

            ShearYield = 100000,
            ShearFracture = 170000,
            ShearStrainAtYield = 333,

            CompressiveYield = 350000,
            CompressiveFracture = 595000,
            CompressiveStrainAtYield = 350,

            TensileYield = 100000,
            TensileFracture = 170000,
            TensileStrainAtYield = 120,

            TorsionYield = 100000,
            TorsionFracture = 170000,
            TorsionStrainAtYield = 333,

            BendingYield = 100000,
            BendingFracture = 170000,
            BendingStrainAtYield = 120,

            SolidDensity = 10490,
            SharpnessMultiplier = 1d
        };

        public static readonly IMaterial Adamantine = new Material("adamantine", "adamantine")
        {
            ImpactYield = 5000000,
            ImpactFracture = 5000000,
            ImpactStrainAtYield = 0,

            ShearYield = 5000000,
            ShearFracture = 5000000,
            ShearStrainAtYield = 0,

            CompressiveYield = 5000000,
            CompressiveFracture = 5000000,
            CompressiveStrainAtYield = 0,

            TensileYield = 5000000,
            TensileFracture = 5000000,
            TensileStrainAtYield = 0,

            TorsionYield = 5000000,
            TorsionFracture = 5000000,
            TorsionStrainAtYield = 0,

            BendingYield = 5000000,
            BendingFracture = 5000000,
            BendingStrainAtYield = 0,

            SolidDensity = 200,
            SharpnessMultiplier = 10d
        };

        public static readonly IMaterial Wood = new Material("wood", "wood")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 1000,

            ShearYield = 40000,
            ShearFracture = 40000,
            ShearStrainAtYield = 1000,

            CompressiveYield = 10000,
            CompressiveFracture = 10000,
            CompressiveStrainAtYield = 1000,

            TensileYield = 10000,
            TensileFracture = 10000,
            TensileStrainAtYield = 1000,

            TorsionYield = 10000,
            TorsionFracture = 10000,
            TorsionStrainAtYield = 1000,

            BendingYield = 10000,
            BendingFracture = 10000,
            BendingStrainAtYield = 1000,

            SolidDensity = 500,
            SharpnessMultiplier = 0.1d
        };

        public static readonly IMaterial Feather = new Material("feather", "feather")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 100000,

            ShearYield = 60000,
            ShearFracture = 120000,
            ShearStrainAtYield = 100000,

            CompressiveYield = 10000,
            CompressiveFracture = 10000,
            CompressiveStrainAtYield = 100000,

            TensileYield = 10000,
            TensileFracture = 10000,
            TensileStrainAtYield = 100000,

            TorsionYield = 10000,
            TorsionFracture = 10000,
            TorsionStrainAtYield = 100000,

            BendingYield = 10000,
            BendingFracture = 10000,
            BendingStrainAtYield = 100000,

            SolidDensity = 100,
            SharpnessMultiplier = 0.1d
        };

        public static readonly IMaterial Copper = new Material("copper", "copper")
        {
            ImpactYield = 245000,
            ImpactFracture = 770000,
            ImpactStrainAtYield = 175,

            ShearYield = 70000,
            ShearFracture = 220000,
            ShearStrainAtYield = 145,

            CompressiveYield = 245000,
            CompressiveFracture = 770000,
            CompressiveStrainAtYield = 175,

            TensileYield = 70000,
            TensileFracture = 220000,
            TensileStrainAtYield = 58,

            TorsionYield = 70000,
            TorsionFracture = 220000,
            TorsionStrainAtYield = 145,

            BendingYield = 70000,
            BendingFracture = 220000,
            BendingStrainAtYield = 58,

            SolidDensity = 100,
            SharpnessMultiplier = 1d
        };


        public static readonly IMaterial Iron = new Material("iron", "iron")
        {
            ImpactYield = 542500,
            ImpactFracture = 1085000,
            ImpactStrainAtYield = 319,

            ShearYield = 155000,
            ShearFracture = 310000,
            ShearStrainAtYield = 189,

            CompressiveYield = 542500,
            CompressiveFracture = 1085000,
            CompressiveStrainAtYield = 319,

            TensileYield = 155000,
            TensileFracture = 310000,
            TensileStrainAtYield = 73,

            TorsionYield = 155000,
            TorsionFracture = 310000,
            TorsionStrainAtYield = 189,

            BendingYield = 155000,
            BendingFracture = 310000,
            BendingStrainAtYield = 73,

            SolidDensity = 7850,
            SharpnessMultiplier = 1d
        };

        public static readonly IMaterial Bronze = new Material("bronze", "bronze")
        {
            ImpactYield = 602000,
            ImpactFracture = 843500,
            ImpactStrainAtYield = 547,

            ShearYield = 172000,
            ShearFracture = 241000,
            ShearStrainAtYield = 156,

            CompressiveYield = 602000,
            CompressiveFracture = 843500,
            CompressiveStrainAtYield = 547,

            TensileYield = 172000,
            TensileFracture = 241000,
            TensileStrainAtYield = 156,

            TorsionYield = 172000,
            TorsionFracture = 241000,
            TorsionStrainAtYield = 156,

            BendingYield = 172000,
            BendingFracture = 241000,
            BendingStrainAtYield = 156,

            SolidDensity = 8250,
            SharpnessMultiplier = 1d
        };

    }
}
