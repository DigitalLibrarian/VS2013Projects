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
            Attributes = new List<Attribute>();
        }
        public List<BodyPart> Parts { get; set; }
        public List<CombatMove> Moves { get; set; }
        public List<Attribute> Attributes { get; set; }

        public double Size { get; set; }

        public Material BloodMaterial { get; set; }
        public Material PusMaterial { get; set; }

        public bool FeelsNoPain { get; set; }
    }
}
