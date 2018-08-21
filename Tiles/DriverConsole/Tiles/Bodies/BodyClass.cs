using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyClass : IBodyClass
    {
        public BodyClass(double size, IEnumerable<IBodyPartClass> parts)
            : this(size, parts, new List<ICombatMoveClass>())
        {
        }
        
        public BodyClass(double size, IEnumerable<IBodyPartClass> parts, IEnumerable<ICombatMoveClass> moves)
            : this(size, parts, moves, new List<IAttributeClass>(), null, null)
        {

        }

        public BodyClass(double size, IEnumerable<IBodyPartClass> parts, IEnumerable<ICombatMoveClass> moves, IEnumerable<IAttributeClass> attrs, IMaterial bloodMaterial, IMaterial pusMaterial)
        {
            Parts = parts;
            Size = size;
            Moves = moves;
            Attributes = attrs;

            BloodMaterial = bloodMaterial;
            PusMaterial = pusMaterial;
        }
        public IEnumerable<IBodyPartClass> Parts { get; set; }
        public IEnumerable<ICombatMoveClass> Moves { get; set; }
        public IEnumerable<IAttributeClass> Attributes { get; set; }
        public double Size { get; set; }

        public IMaterial BloodMaterial { get; set; }
        public IMaterial PusMaterial { get; set; }
        public bool FeelsNoPain { get; set; }

        public int TotalBodyPartRelSize
        {
            get
            {
                return Parts
                .Where(x => !x.IsInternal && !x.IsEmbedded)
                .Select(x => x.RelativeSize).Sum();
            }
        }


        public int GetAttribute(string name)
        {
            return Attributes.Single(a => a.Name.Equals(name))
                .Median;
        }
    }
}
