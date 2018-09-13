using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Injuries;

namespace Tiles.Bodies.Wounds
{
    public interface IBodyPartWoundFactory 
    {
        IBodyPartWound Create(IBodyPartInjury injury);
    }

    public class BodyPartWoundFactory : IBodyPartWoundFactory
    {
        public IBodyPartWound Create(IBodyPartInjury injury)
        {
            return new BodyPartWound
            {
                Part = injury.BodyPart,
                LayerWounds = injury.TissueLayerInjuries.Select(Create),
                Age = 0
            };
        }

        private ITissueLayerWound Create(ITissueLayerInjury injury)
        {
            return new TissueLayerWound
            {
                Layer = injury.Layer,
                Pain = injury.PainContribution
            };
        }
    }
}
