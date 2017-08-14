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

        bool IsChip();
        bool IsSoft();
        bool IsVascular();

        double WoundArea { get; }

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

        public double WoundArea
        {
            get
            {
                if (StrikeResult.PenetrationRatio >= 1d)
                {
                    return StrikeResult.ContactArea;
                }
                return 0;
            }
        }

        public bool IsChip()
        {
            var maxCa = BodyPart.GetContactArea() * 0.25d;
            return StrikeResult.ContactArea <= maxCa;
        }

        public bool IsSoft()
        {
            return Layer.Material.IsSoft(StrikeResult.StressMode);
        }

        public bool IsVascular()
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
            return ((long) ((double)d / 10d)) * 10L;
        }
    }
}
