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
        MaterialStrikeResult StrikeResult { get; }

        string Gerund { get; }
        string GetPhrase();

        IDamageVector GetDamage();
    }

    class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayer Layer { get; private set; }
        public IBodyPart BodyPart { get; private set; }

        public MaterialStrikeResult StrikeResult { get; private set; }
        public TissueLayerInjury(IBodyPart bodyPart, ITissueLayer layer, MaterialStrikeResult strikeResult)
        {
            BodyPart = bodyPart;
            Layer = layer;
            StrikeResult = strikeResult;
        }

        #region Language Generation
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
        #endregion

        public bool IsChip()
        {
            var maxCa = BodyPart.GetContactArea() * 0.25d;
            return StrikeResult.ContactArea <= maxCa;
        }

        private bool IsSoft()
        {
            return Layer.IsSoft(StrikeResult.StressMode);
        }

        private bool IsVascular()
        {
            return Layer.IsVascular();
        }

        public IDamageVector GetDamage()
        {
            var damage = new DamageVector();
            switch (StrikeResult.StressResult)
            {
                case StressResult.Impact_Bypass:
                    damage.EffectFraction.Numerator = Round(StrikeResult.ContactAreaRatio * (double) damage.EffectFraction.Denominator);
                    break;
                case StressResult.Impact_CompleteFracture:
                    damage.CutFraction.Numerator = Round(StrikeResult.PenetrationRatio * StrikeResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(StrikeResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_Dent:
                    damage.DentFraction.Numerator = Round(StrikeResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_Cut:
                    damage.CutFraction.Numerator = Round(StrikeResult.PenetrationRatio * StrikeResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(StrikeResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                case StressResult.Shear_CutThrough:
                    damage.CutFraction.Numerator = Round(StrikeResult.PenetrationRatio * StrikeResult.ContactAreaRatio * (double)damage.CutFraction.Denominator);
                    damage.DentFraction.Numerator = Round(StrikeResult.ContactAreaRatio * (double)damage.DentFraction.Denominator);
                    break;
                default:
                    break;
            }
            return damage;
        }

        private long Round(double d)
        {
            return ((long) ((double)d / 100d)) * 100L;
        }
    }
}
