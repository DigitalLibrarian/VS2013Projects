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

        public Tissue Tissue { get; set; }
        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeaponSlot { get; set; }

        public List<CombatMove> Moves { get; set; }

        public List<string> Categories { get; set; }
        public List<string> Types { get; set; }

        public string NameSingular { get; set; }
        public string NamePlural { get; set; }
        
        public bool CanBeAmputated { get; set; }
        public bool CanGrasp { get; set; }

        public bool IsNervous { get; set; }
        public bool IsCirculatory { get; set; }
        public bool IsSkeletal { get; set; }

        public bool IsDigit { get; set; }

        public bool IsBreathe { get; set; }
        public bool IsSight { get; set; }

        public bool IsStance { get; set; }
        // TODO - implement this : 
        //[INTERNAL] - Marks the lungs as being internal - which places it inside any tissue layers of the attached body part, meaning that attacks have to go through the upper body's tissues first, and prevents it from being severed like an external body part.
        public bool IsInternal { get; set; }

        public bool IsSmall { get; set; }
        public bool IsEmbedded { get; set; }

        public int RelativeSize { get; set; }

        public List<BodyPartRelation> BodyPartRelations { get; set; }


        public string TokenId { get; set; }
    }

    public enum BodyPartRelationType
    {
        Around,
        Cleans
    }

    public enum BodyPartRelationStrategy
    {
        ByToken,
        ByCategory
    }

    public class BodyPartRelation
    {
        public BodyPartRelationType Type { get; set; }
        public BodyPartRelationStrategy Strategy { get; set; }
        public string StrategyParam { get; set; }

        public int Weight { get; set; }
    }
}
