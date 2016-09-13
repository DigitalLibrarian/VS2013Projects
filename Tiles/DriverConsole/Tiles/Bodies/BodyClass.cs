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
        {
            Parts = parts;
            Size = size;
            Moves = moves;
        }
        public IEnumerable<IBodyPartClass> Parts { get; set; }
        public IEnumerable<ICombatMoveClass> Moves { get; set; }
        public double Size { get; set; }
    }
}
