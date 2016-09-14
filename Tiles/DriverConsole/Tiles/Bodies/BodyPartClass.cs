using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;

namespace Tiles.Bodies
{
    public class BodyPartClass : IBodyPartClass
    {
        public string Name { get; set; }

        public IBodyPartClass Parent { get; set; }
        public ITissueClass Tissue { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Types { get; set; }

        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public bool IsLifeCritical { get; set; }
        public bool CanBeAmputated { get; set; }

        public bool CanGrasp { get; set; }

        public int RelativeSize { get; set; }

        public bool IsNervous { get; set; }
        public bool IsCirculatory { get; set; }
        public bool IsSkeletal { get; set; }

        public bool IsDigit { get; set; }

        public bool IsBreathe { get; set; }
        public bool IsSight { get; set; }

        public bool IsStance { get; set; }
        public bool IsInternal { get; set; }
        public bool IsSmall { get; set; }
        public bool IsEmbedded { get; set; }
        
        public BodyPartClass(
            string name, 
            ITissueClass tissueClass, 
            ArmorSlot armorSlotType, WeaponSlot weaponSlotType, 
            IEnumerable<string> categories,
            IEnumerable<string> types,
            int relSize,
            bool isCritical = false, bool canGrasp = false, bool canBeAmputated = false, 
            bool isNervous = false, bool isCirc = false, bool isSkeletal = false,
            bool isDigit = false, 
            bool isBreathe = false,
            bool isSight = false,
            bool isStance = false,
            bool isInternal = false,
            bool isSmall = false,
            bool isEmbedded = false,
            IBodyPartClass parent = null)
        {
            Name = name;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            Categories = categories;
            Types = types;
            RelativeSize = relSize;
            Parent = parent;

            Tissue = tissueClass;

            IsLifeCritical = isCritical;
            CanBeAmputated = canBeAmputated;
            CanGrasp = canGrasp;
            IsNervous = IsNervous;
            IsCirculatory = isCirc;
            IsSkeletal = isSkeletal;
            IsDigit = isDigit;
            IsBreathe = isBreathe;
            IsSight = isSight;
            IsStance = isStance;
            IsInternal = isInternal;
            IsSmall = isSmall;
            IsEmbedded = isEmbedded;
        }


    }
}