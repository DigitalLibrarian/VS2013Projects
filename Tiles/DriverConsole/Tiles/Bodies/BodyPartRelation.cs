using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IBodyPartRelation
    {
        BodyPartRelationType Type { get; }
        BodyPartRelationStrategy Strategy { get; }
        string StrategyParam { get; }
        
        int Weight { get; }
    }
    public enum BodyPartRelationStrategy
    {
        ByToken,
        ByCategory
    }

    public class BodyPartRelation : IBodyPartRelation
    {
        public BodyPartRelationType Type { get; set; }
        public BodyPartRelationStrategy Strategy { get; set; }
        public string StrategyParam { get; set; }

        public int Weight { get; set; }
    }
}
