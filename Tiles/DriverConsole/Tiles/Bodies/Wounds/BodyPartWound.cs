using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Wounds
{
    public interface IBodyPartWound
    {
        IBodyPart Part { get; }
        IEnumerable<ITissueLayerWound> LayerWounds { get; }
        int Age { get; }
    }

    public class BodyPartWound : IBodyPartWound
    {
        public IBodyPart Part { get; set; }
        public IEnumerable<ITissueLayerWound> LayerWounds { get; set; }
        public int Age { get; set; }
    }
}
