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

        bool IsMatch(IBodyPartClass bpClass, BodyPartRelationType relationType);
    }

    public enum BodyPartRelationType
    {
        Around,
        Cleans
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

        public bool IsMatch(IBodyPartClass bpClass, BodyPartRelationType relationType)
        {
            if (Type != relationType) return false;

            switch (Strategy)
            {
                case BodyPartRelationStrategy.ByToken:
                    return bpClass.TokenId == StrategyParam;
                case BodyPartRelationStrategy.ByCategory:
                    return bpClass.Categories.Contains(StrategyParam);
                default:
                    throw new InvalidOperationException(string.Format("Unknown BodyPartRelationStrategy: {0}", Strategy));
            }
        }
    }
}
