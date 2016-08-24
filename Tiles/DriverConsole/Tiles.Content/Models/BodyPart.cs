using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class BodyPart
    {
        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        public BodyPart Parent { get; set; }

        public bool CanBeAmputated { get; set; }
        public bool CanGrasp { get; set; }

        public Tissue Tissue { get; set; }
    }
}
