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

        public IEnumerable<ICombatMoveClass> Moves { get; set; }

        Dictionary<string, int> Attributes { get; set; }

        public Body(IList<IBodyPart> parts, double size)
            : this(parts, size, Enumerable.Empty<ICombatMoveClass>())
        {

        }
        public Body(IList<IBodyPart> parts, double size, IEnumerable<ICombatMoveClass> moves)
        {
            Parts = parts;
            Size = size;
            Moves = moves;
            Attributes = new Dictionary<string, int>();
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
    
    }
}
