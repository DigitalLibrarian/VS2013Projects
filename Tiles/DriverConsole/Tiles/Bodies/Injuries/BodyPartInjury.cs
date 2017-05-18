using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Bodies.Injuries
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
            var injuries = TissueLayerInjuries.Where(x => x.StrikeResult.StressResult != Materials.StressResult.None);
            if (Class.IsCompletion)
            {
                return string.Format(" and {0}!", Class.CompletionPhrase);
            }
            else if (injuries.Any())
            {
                var phrases = injuries
                    .Select(injury =>
                    {
                        var remaining = injuries.SkipWhile(x => x != injury);
                        var grouped = remaining.TakeWhile(x => x.Gerund.Equals(injury.Gerund));
                        if (grouped.Last() == injury)
                        {
                            return injury;
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .Where(x => x != null)
                    .Select(x => x.GetPhrase())
                    .ToList();
                if (phrases.Count() > 1)
                {
                    var last = phrases.Last();
                    phrases[phrases.Count() - 1] = string.Format("and {0}", last);
                }
                return string.Format(", {0}!", string.Join(", ", phrases));
            }
            else
            {
                return ", but the attack glances away.";
            }
        }
    }
}
