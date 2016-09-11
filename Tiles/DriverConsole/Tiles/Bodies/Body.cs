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

        public int Size { get; set; }

        public IEnumerable<ICombatMoveClass> Moves { get; set; }
        
        public Body(IList<IBodyPart> parts, int size)
            : this(parts, size, Enumerable.Empty<ICombatMoveClass>())
        {

        }
        public Body(IList<IBodyPart> parts, int size, IEnumerable<ICombatMoveClass> moves)
        {
            Parts = parts;
            Size = size;
            Moves = moves;
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
    }
}
