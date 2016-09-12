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
            return string.Format("{0} the {1}", Class.Gerund, Layer.Class.Material.Name);
        }
    }
}
