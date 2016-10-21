using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Injuries
{
    public static class TissueLayerInjuryClasses
    {
        public static ITissueLayerInjuryClass Bruise = new TissueLayerInjuryClass
        {
            Adjective = "bruised",
            Gerund = "bruising",
            DamageType = DamageType.Bludgeon,
            Max = 0.3,
            IsLowerBound = true
        };

        public static ITissueLayerInjuryClass Fracture = new TissueLayerInjuryClass
        {
            Adjective = "fractured",
            Gerund = "fracturing",
            DamageType = DamageType.Bludgeon,
            Min = 0.3,
            IsUpperBound = true
        };

        public static ITissueLayerInjuryClass Tear = new TissueLayerInjuryClass
        {
            Adjective = "torn",
            Gerund = "tearing",
            DamageType = DamageType.Slash,
            Max = .3,
            IsLowerBound = true
        };

        public static ITissueLayerInjuryClass TearApart = new TissueLayerInjuryClass
        {
            Adjective = "torn apart",
            Gerund = "tearing apart",
            DamageType = DamageType.Slash,
            Min = .3,
            IsUpperBound = true,
        };
    }

    public class MsrTissueLayerInjuryClass : ITissueLayerInjuryClass
    {
        IBodyPart BodyPart { get; set; }
        ITissueLayer Layer { get; set; }
        IMaterialStrikeResult StrikeResult { get; set; }
        public MsrTissueLayerInjuryClass(IBodyPart bodyPart,  ITissueLayer layer, IMaterialStrikeResult strikeResult)
        {
            BodyPart = bodyPart;
            Layer = layer;
            StrikeResult = strikeResult;
        }

        public string Adjective
        {
            get { return StrikeResult.StressResult.ToString(); }
        }

        public string Gerund
        {
            get 
            {
                switch (StrikeResult.StressResult)
                {
                    case MaterialStressResult.None:
                        return "stopping at";
                    case MaterialStressResult.Impact_Dent:
                        return IsVascular() ? "bruising" : "denting";
                    case MaterialStressResult.Impact_Bypass:
                        return IsVascular() ? "bruising" : "denting";
                    case MaterialStressResult.Impact_InitiateFracture:
                        if (IsChip()) return "chipping";
                        else
                        {
                            return IsSoft() ? "tearing" : "fracturing";
                        }
                    case MaterialStressResult.Impact_CompleteFracture:
                        return IsSoft() ? "tearing apart" : "shattering";
                    case MaterialStressResult.Shear_Dent:
                        return "denting";
                    case MaterialStressResult.Shear_Cut:
                        return IsSoft() ? "tearing" : "fracturing";
                    case MaterialStressResult.Shear_CutThrough:
                        return IsSoft() ? "tearing apart" : "shattering";
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private bool IsVascular()
        {
            return Layer.Class.VascularRating > 0;
        }

        private bool IsSoft()
        {
            if (StrikeResult.StressMode == StressMode.Edge) 
            { 
                return Layer.Material.ImpactStrainAtYield >= 50000;
            }
            else
            {
                return Layer.Material.ShearStrainAtYield >= 50000;
            }
        }

        public bool IsChip()
        {
            return StrikeResult.WoundArea <= BodyPart.GetContactArea() * 0.25d;
        }

        public DamageType DamageType
        {
            get;
            set;
        }

        public bool IsInRange(double dVal)
        {
            return false;
        }
    }
}
