using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Body
    {
        public Body()
        {
            Parts = new List<BodyPart>();
            Moves = new List<CombatMove>();
        }
        public List<BodyPart> Parts { get; set; }
        public List<CombatMove> Moves { get; set; }

        public int Size { get; set; }
    }
}
