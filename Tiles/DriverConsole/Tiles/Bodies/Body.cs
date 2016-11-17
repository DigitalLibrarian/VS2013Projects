using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiles.Agents.Combat;

namespace Tiles.Bodies
{
    public class Body : IBody
    {
        public bool IsGrasping { get { return Parts.Any(x => x.IsGrasping); } }
        public bool IsBeingGrasped { get { return Parts.Any(x => x.IsBeingGrasped); } }
        public bool IsWrestling { get {  return Parts.Any(x => x.IsWrestling);} }
        public IList<IBodyPart> Parts { get; private set; }

        public double Size { get; set; }
        public double StoredFat { get { return 500000d; } }
        public IBodyClass Class { get; private set; }
        public IEnumerable<ICombatMoveClass> Moves { get; set; }

        Dictionary<string, int> Attributes { get; set; }

        public Body(IBodyClass bodyClass, IList<IBodyPart> parts, double size)
            : this(bodyClass, parts, size, Enumerable.Empty<ICombatMoveClass>())
        {

        }
        public Body(IBodyClass bodyClass, IList<IBodyPart> parts, double size, IEnumerable<ICombatMoveClass> moves)
        {
            Parts = parts;
            Size = size;
            Moves = moves;
            Attributes = new Dictionary<string, int>();
            Class = bodyClass;
        }

        public void Amputate(IBodyPart part)
        {
            foreach (var subPart in Parts.ToList())
            {
                if (subPart.Parent == part)
                {
                    Amputate(subPart);
                }

                if(subPart == part)
                {
                    Parts.Remove(subPart);
                }
            }
        }

        public IEnumerable<IBodyPart> GetInternalParts(IBodyPart part)
        {
            return Parts.Where(p => p.Parent == part && p.IsInternal).Reverse();
        }

        public void SetAttribute(string name, int value)
        {
            Attributes[name] = value;
        }
        public int GetAttribute(string name)
        {
            return Attributes[name];
        }
        public IEnumerable<string> AttributeNames { get { return Attributes.Keys; } }

        public IDictionary<IBodyPart, int> GetRelations(IBodyPart target, BodyPartRelationType type)
        {
            var d = new Dictionary<IBodyPart, int>();
            foreach (var bpRel in target.Class.BodyPartRelations.Where(r => r.Type == type))
            {
                foreach (var relatedPart in BpRelationQuery(bpRel.Strategy, bpRel.StrategyParam))
                {
                    d.Add(relatedPart, bpRel.Weight);
                }
            }
            return d;
        }
        
        private IEnumerable<IBodyPart> BpRelationQuery(BodyPartRelationStrategy strategy, string param)
        {
            switch (strategy)
            {
                case BodyPartRelationStrategy.ByToken:
                    return Parts.Where(x => x.Class.TokenId.Equals(param));

                case BodyPartRelationStrategy.ByCategory:
                    return Parts.Where(x => x.Class.Categories.Contains(param));
            }

            return Enumerable.Empty<IBodyPart>();
        }
    }
}
