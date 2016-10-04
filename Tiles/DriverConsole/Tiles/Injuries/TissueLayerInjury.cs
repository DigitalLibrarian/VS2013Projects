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
        IMaterialStrikeResult StrikeResult { get; }

        IDamageVector GetTotal();
        string GetPhrase();
    }

    class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayerInjuryClass Class { get; private set; }
        public ITissueLayer Layer { get; private set; }

        public IDamageVector Damage { get; private set; }

        public IMaterialStrikeResult StrikeResult { get; private set; }
        public TissueLayerInjury(ITissueLayerInjuryClass injuryClass, ITissueLayer layer, IDamageVector damage, IMaterialStrikeResult strikeResult)
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
            var mom = StrikeResult.Momentum;

            var cost1 = StrikeResult.ShearDentCost;
            var cost2 = StrikeResult.ShearCutCost;
            var cost3= StrikeResult.ShearCutThroughCost;
            if (StrikeResult.StressMode == StressMode.Blunt)
            {
                cost1 = StrikeResult.ImpactDentCost;
                cost2 = StrikeResult.ImpactInitiateFractureCost;
                cost3 = StrikeResult.ImpactCompleteFractureCost;
            }

            return string.Format("{0}({6}, {7}, {2}, {8}) the {1}({3}, {4}, {5}) ", 
                Class.Gerund, 
                Layer.Class.Material.Name, 
                StrikeResult.Stress,
                Normalize(cost1), 
                Normalize(cost2), 
                Normalize(cost3),
                StrikeResult.StressMode.ToString(),
                Normalize(mom),
                StrikeResult.ContactArea
                );
        }

        double Normalize(double d)
        {
            return d;
        }
    }
}
