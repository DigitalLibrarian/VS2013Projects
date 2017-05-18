using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Bodies.Injuries
{
    public interface ITissueLayerInjury
    {
        ITissueLayer Layer { get; }
        //ITissueLayerInjuryClass Class { get; }
        MaterialStrikeResult StrikeResult { get; }

        string Gerund { get; }

        IDamageVector GetTotal();
        string GetPhrase();
    }

    class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayer Layer { get; private set; }
        public IBodyPart BodyPart { get; private set; }

        public IDamageVector Damage { get; private set; }

        public MaterialStrikeResult StrikeResult { get; private set; }
        public TissueLayerInjury(IBodyPart bodyPart, ITissueLayer layer, IDamageVector damage, MaterialStrikeResult strikeResult)
        {
            BodyPart = bodyPart;
            Layer = layer;
            Damage = damage;
            StrikeResult = strikeResult;
        }

        public IDamageVector GetTotal()
        {
            return Damage;
        }

        public string GetPhrase()
        {
            return string.Format("{0} the {1}", Gerund, Layer.Class.Name);
        }

        public string Gerund
        {
            get
            {
                var gerund = "";
                switch (StrikeResult.StressResult)
                {
                    case StressResult.None:
                        gerund = "stopping at";
                        break;
                    case StressResult.Impact_Dent:
                        gerund = IsVascular() ? "bruising" : "denting";
                        break;
                    case StressResult.Impact_Bypass:
                        gerund = IsVascular() ? "bruising" : "denting";
                        break;
                    case StressResult.Impact_InitiateFracture:
                        if (IsChip()) gerund = "chipping";
                        else
                        {
                            gerund = IsSoft() ? "tearing" : "fracturing";
                        }
                        break;
                    case StressResult.Impact_CompleteFracture:
                        if (IsChip()) gerund = "chipping";
                        else
                        {
                            gerund = IsSoft() ? "tearing apart" : "shattering";
                        }
                        break;
                    case StressResult.Shear_Dent:
                        gerund = "denting";
                        break;
                    case StressResult.Shear_Cut:
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
                    case StressResult.Shear_CutThrough:

                        if (IsSoft())
                        {
                            gerund = "tearing apart";
                        }
                        else
                        {
                            gerund = IsChip() ? "tearing through" : "shattering";
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return gerund;
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
            return StrikeResult.ContactArea <= maxCa;
        }
    }
}
