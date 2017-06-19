using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfMaterialBuilder
    {
        void SetName(string name);
        void SetAdjective(string adj);

        Material Build();

        void SetImpactYield(int yield);
        void SetImpactFracture(int fracture);
        void SetImpactStrainAtYield(int strainAtYield);

        void SetShearYield(int yield);
        void SetShearFracture(int fracture);
        void SetShearStrainAtYield(int strainAtYield);

        void SetCompressiveYield(int yield);
        void SetCompressiveFracture(int fracture);
        void SetCompressiveStrainAtYield(int strainAtYield);

        void SetTensileYield(int yield);
        void SetTensileFracture(int fracture);
        void SetTensileStrainAtYield(int strainAtYield);

        void SetTorsionYield(int yield);
        void SetTorsionFracture(int fracture);
        void SetTorsionStrainAtYield(int strainAtYield);

        void SetBendingYield(int yield);
        void SetBendingFracture(int fracture);
        void SetBendingStrainAtYield(int strainAtYield);

        void SetSolidDensity(int p);

        void SetSharpnessMultiplier(double p);
    }

    public class DfMaterialBuilder : IDfMaterialBuilder
    {
        Material Material { get; set; }

        public DfMaterialBuilder()
        {
            Material = new Material();
        }

        public void SetName(string name)
        {
            Material.Name = name;
        }

        public void SetAdjective(string adj)
        {
            Material.Adjective = adj;
        }

        public Material Build()
        {
            return Material;
        }

        public void SetSolidDensity(int d)
        {
            Material.SolidDensity = d;
        }

        public void SetSharpnessMultiplier(double s)
        {
            Material.SharpnessMultiplier = s;
        }

        public void SetImpactYield(int yield)
        {
            Material.ImpactYield = yield;
        }

        public void SetImpactFracture(int fracture)
        {
            Material.ImpactFracture = fracture;
        }

        public void SetImpactStrainAtYield(int strainAtYield)
        {
            Material.ImpactStrainAtYield = strainAtYield;
        }

        public void SetShearYield(int yield)
        {
            Material.ShearYield = yield;
        }

        public void SetShearFracture(int fracture)
        {
            Material.ShearFracture = fracture;
        }

        public void SetShearStrainAtYield(int strainAtYield)
        {
            Material.ShearStrainAtYield = strainAtYield;
        }


        public void SetCompressiveYield(int yield)
        {
            Material.CompressiveYield = yield;
        }

        public void SetCompressiveFracture(int fracture)
        {
            Material.CompressiveFracture = fracture;
        }

        public void SetCompressiveStrainAtYield(int strainAtYield)
        {
            Material.CompressiveStrainAtYield = strainAtYield;
        }

        public void SetTensileYield(int yield)
        {
            Material.TensileYield = yield;
        }

        public void SetTensileFracture(int fracture)
        {
            Material.TensileFracture = fracture;
        }

        public void SetTensileStrainAtYield(int strainAtYield)
        {
            Material.TensileStrainAtYield = strainAtYield;
        }

        public void SetTorsionYield(int yield)
        {
            Material.TorsionYield = yield;
        }

        public void SetTorsionFracture(int fracture)
        {
            Material.TorsionFracture = fracture;
        }

        public void SetTorsionStrainAtYield(int strainAtYield)
        {
            Material.TorsionStrainAtYield = strainAtYield;
        }

        public void SetBendingYield(int yield)
        {
            Material.BendingYield = yield;
        }

        public void SetBendingFracture(int fracture)
        {
            Material.BendingFracture = fracture;
        }

        public void SetBendingStrainAtYield(int strainAtYield)
        {
            Material.BendingStrainAtYield = strainAtYield;
        }
    }
}
