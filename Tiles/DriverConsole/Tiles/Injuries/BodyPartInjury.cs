using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Injuries
{
    public interface IBodyPartInjury
    {
        IBodyPart BodyPart { get; }
        IBodyPartInjuryClass Class { get; }
        IEnumerable<ITissueLayerInjury> TissueLayerInjuries { get; }

        IDamageVector GetTotal();
        string GetResultPhrase();
    }

    public class BodyPartInjury : IBodyPartInjury
    {
        public IBodyPart BodyPart { get; private set; }
        public IBodyPartInjuryClass Class { get; private set; }
        public IEnumerable<ITissueLayerInjury> TissueLayerInjuries { get; private set; }

        public BodyPartInjury(IBodyPartInjuryClass injuryClass,
            IBodyPart bodyPart,
            IEnumerable<ITissueLayerInjury> tissueLayerInjuries)
        {
            BodyPart = bodyPart;
            Class = injuryClass;
            TissueLayerInjuries = tissueLayerInjuries;
        }


        public IDamageVector GetTotal()
        {
            var d = new DamageVector();
            foreach (var tissueInjury in TissueLayerInjuries)
            {
                d.Add(tissueInjury.GetTotal());
            }
            return d;
        }
        
        public string GetResultPhrase()
        {
            if (Class.IsCompletion)
            {
                return string.Format(" and {0}!", Class.CompletionPhrase);
            }
            else if (TissueLayerInjuries.Any())
            {
                return string.Format(", {0}!",
                    string.Join(", ",
                    TissueLayerInjuries.Select(x => x.GetPhrase())));
            }
            else
            {
                return ", but the attack glances away.";
            }
        }
    }
}
