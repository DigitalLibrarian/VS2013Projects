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
        /*
         * 
         **/
        // for blunt damage
        void SetImpactYield(int impactYield);
        void SetImpactFracture(int impactFracture);
        void SetImpactStrainAtYield(int impactSay);

        /*
         * 
         * */

        // for edge damage
        void SetShearYield(int shearYield);
        void SetShearFracture(int shearFracture);
        void SetShearStrainAtYield(int shearSay);

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


        public void SetImpactYield(int impactYield)
        {
            Material.ImpactYield = impactYield;
        }

        public void SetImpactFracture(int impactFracture)
        {
            Material.ImpactFracture = impactFracture;
        }

        public void SetImpactStrainAtYield(int impactSay)
        {
            Material.ImpactStrainAtYield = impactSay;
        }


        public void SetShearYield(int shearYield)
        {
            Material.ShearYield = shearYield;
        }

        public void SetShearFracture(int shearFracture)
        {
            Material.ShearFracture = shearFracture;
        }

        public void SetShearStrainAtYield(int shearSay)
        {
            Material.ShearStrainAtYield = shearSay;
        }

        public void SetSolidDensity(int d)
        {
            Material.SolidDensity = d;
        }

        public void SetSharpnessMultiplier(double s)
        {
            Material.SharpnessMultiplier = s;
        }
    }
}
