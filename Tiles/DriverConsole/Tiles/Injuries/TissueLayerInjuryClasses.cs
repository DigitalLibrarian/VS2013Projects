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
                var gerund = "";
                switch (StrikeResult.StressResult)
                {
                    case MaterialStressResult.None:
                        gerund = "stopping at";
                        break;
                    case MaterialStressResult.Impact_Dent:
                        gerund =  IsVascular() ? "bruising" : "denting";
                        break;
                    case MaterialStressResult.Impact_Bypass:
                        gerund =  IsVascular() ? "bruising" : "denting";
                        break;
                    case MaterialStressResult.Impact_InitiateFracture:
                        if (IsChip()) gerund = "chipping";
                        else
                        {
                            gerund = IsSoft() ? "tearing" : "fracturing";
                        }
                        break;
                    case MaterialStressResult.Impact_CompleteFracture:
                        gerund = IsSoft() ? "tearing apart" : "shattering";
                        break;
                    case MaterialStressResult.Shear_Dent:
                        gerund = "denting";
                        break;
                    case MaterialStressResult.Shear_Cut:
                        if (!IsSoft())
                        {
                            if (IsChip()) gerund = "chipping";
                            else
                            {
                                gerund = "fracturing";
                            }
                        }
                        else
                        {
                            gerund = "tearing";
                        }
                        break;
                    case MaterialStressResult.Shear_CutThrough:
                        gerund = IsSoft() ? "tearing apart" : "shattering";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return gerund;
                //return string.Format("{0} (ContactArea = {1}, WoundArea = {2})", gerund, StrikeResult.ContactArea, StrikeResult.WoundArea);
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
            var maxCa = BodyPart.GetContactArea() * 0.25d;
            return StrikeResult.WoundArea <= maxCa;
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
