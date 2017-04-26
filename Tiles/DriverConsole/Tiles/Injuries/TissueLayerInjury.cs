using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Injuries
{
    public interface ITissueLayerInjury
    {
        ITissueLayer Layer { get; }
        ITissueLayerInjuryClass Class { get; }
        MaterialStrikeResult StrikeResult { get; }

        IDamageVector GetTotal();
        string GetPhrase();
    }

    class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayerInjuryClass Class { get; private set; }
        public ITissueLayer Layer { get; private set; }

        public IDamageVector Damage { get; private set; }

        public MaterialStrikeResult StrikeResult { get; private set; }
        public TissueLayerInjury(ITissueLayerInjuryClass injuryClass, ITissueLayer layer, IDamageVector damage, MaterialStrikeResult strikeResult)
        {
            Class = injuryClass;
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
            /*
            return string.Format("*{0}*(Mode={6}, Mom={7}, Stress={2}, ContactArea={8}, WoundArea={9}, ResultMom={13}, RemPen={14}, Sharp={16}) the {1}(ShearCost1={3}, ShearCost2={4}, ShearCost3={5}, ImpactCost1={10}, ImpactCost2={11}, ImpactCost3={12}, Thick={15}) ", 
                Class.Gerund, 
                Layer.Class.Material.Name, 
                StrikeResult.Stress,
                Normalize(StrikeResult.ShearDentCost),
                Normalize(StrikeResult.ShearCutCost),
                Normalize(StrikeResult.ShearCutThroughCost),
                StrikeResult.StressMode.ToString(),
                Normalize(StrikeResult.Momentum),
                StrikeResult.ContactArea,
                StrikeResult.WoundArea,
                Normalize(StrikeResult.ImpactDentCost),
                Normalize(StrikeResult.ImpactInitiateFractureCost),
                Normalize(StrikeResult.ImpactCompleteFractureCost),
                Normalize(StrikeResult.ResultMomentum),
                Normalize(StrikeResult.RemainingPenetration),
                Normalize(StrikeResult.LayerThickness),
                Normalize(StrikeResult.Sharpness)
                );
            */
            return string.Format("{0} the {1}", Class.Gerund, Layer.Class.Name);
        }

        double Normalize(double d)
        {
            return d;
        }
    }
}
