using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class Body : IBody
    {
        public bool IsGrasping
        {
            get { return Parts.Any(x => x.IsGrasping); }
        }
        public bool IsWrestling { get {  return Parts.Any(x => x.IsWrestling);} }
        public IList<IBodyPart> Parts { get; private set; }
        public Body(IList<IBodyPart> parts)
        {
            Parts = parts;
        }

        public IBodyPart DamagePart(IBodyPart part, uint dmg)
        {
            // TODO - handle foreign parts
            part.Health.TakeDamage(dmg);
            if (part.Health.OutOfHealth)
            {
                if (part.CanAmputate)
                {
                    foreach (var subPart in Parts.ToList())
                    {
                        if (subPart.Parent == part || subPart == part)
                        {
                            Parts.Remove(subPart);
                        }
                    }
                    return part;
                }
            }
            return null;
        }
    }
}
