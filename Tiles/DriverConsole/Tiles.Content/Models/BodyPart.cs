using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class BodyPart
    {
        public BodyPart Parent { get; set; }
        
        public List<CombatMove> Moves { get; set; }

        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        
        public bool CanBeAmputated { get; set; }
        public bool CanGrasp { get; set; }

        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeapnSlot { get; set; }

        public Tissue Tissue { get; set; }

        public bool IsNervous { get; set; }
        public bool IsCirculatory { get; set; }
        public bool IsSkeletal { get; set; }

        public bool IsDigit { get; set; }

        public bool IsBreathe { get; set; }
        public bool IsSight { get; set; }

        public bool IsStanding { get; set; }
        public bool IsInternal { get; set; }
    }

}
