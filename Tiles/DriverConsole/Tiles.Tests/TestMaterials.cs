using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Tests
{
    public static class TestMaterials
    {
        public static readonly IMaterial Skin = new Material("skin", "skin")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            SolidDensity = 1000
        };

        public static readonly IMaterial Muscle = new Material("muscle", "muscle")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 50000,

            ShearYield = 20000,
            ShearFracture = 20000,
            ShearStrainAtYield = 50000,

            SolidDensity = 1060
        };

        public static readonly IMaterial Bone = new Material("bone", "bone")
        {
            ImpactYield = 200000,
            ImpactFracture = 20000,
            ImpactStrainAtYield = 100,

            ShearYield = 115000,
            ShearFracture = 130000,
            ShearStrainAtYield = 100,

            SolidDensity = 500
        };

        public static readonly IMaterial Steel = new Material("steel", "steel")
        {
            ImpactYield = 1505000,
            ImpactFracture = 2520000,
            ImpactStrainAtYield = 940,

            ShearYield = 430000,
            ShearFracture = 720000,
            ShearStrainAtYield = 215,

            SolidDensity = 7850
        };

        public static readonly IMaterial Silver = new Material("silver", "silver")
        {
            ImpactYield = 350000,
            ImpactFracture = 595000,
            ImpactStrainAtYield = 350,

            ShearYield = 100000,
            ShearFracture = 170000,
            ShearStrainAtYield = 333,

            SolidDensity = 10490
        };

        public static readonly IMaterial Adamantine = new Material("adamantine", "adamantine")
        {
            ImpactYield = 5000000,
            ImpactFracture = 5000000,
            ImpactStrainAtYield = 0,

            ShearYield = 5000000,
            ShearFracture = 5000000,
            ShearStrainAtYield = 0,

            SolidDensity = 200
        };

        public static readonly IMaterial Wood = new Material("wood", "wood")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 1000,

            ShearYield = 40000,
            ShearFracture = 40000,
            ShearStrainAtYield = 1000,

            SolidDensity = 500
        };

        public static readonly IMaterial Feather = new Material("feather", "feather")
        {
            ImpactYield = 10000,
            ImpactFracture = 10000,
            ImpactStrainAtYield = 100000,

            ShearYield = 60000,
            ShearFracture = 120000,
            ShearStrainAtYield = 100000,

            SolidDensity = 100
        };

        public static readonly IMaterial Copper = new Material("copper", "copper")
        {
            ImpactYield = 245000,
            ImpactFracture = 770000,
            ImpactStrainAtYield = 175,

            ShearYield = 70000,
            ShearFracture = 220000,
            ShearStrainAtYield = 145,

            SolidDensity = 100
        };
    }
}
