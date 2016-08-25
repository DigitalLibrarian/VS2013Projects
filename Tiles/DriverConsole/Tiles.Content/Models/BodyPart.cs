using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class BodyPart
    {
        public BodyPart() 
        {
            Tissue = new Tissue();
        }

        public BodyPart Parent { get; set; }

        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        
        public bool CanBeAmputated { get; set; }
        public bool CanGrasp { get; set; }

        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeapnSlot { get; set; }

        public Tissue Tissue { get; set; }
    }
}
