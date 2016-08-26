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
        public IEnumerable<ICombatMoveClass> Moves { get; set; }

        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public bool IsLifeCritical { get; set; }
        public bool CanAmputate { get; set; }

        public bool CanGrasp { get; set; }


        public bool IsNervous { get; set; }
        public bool IsCirculatory { get; set; }
        public bool IsSkeletal { get; set; }

        public bool IsDigit { get; set; }

        public bool IsBreathe { get; set; }
        public bool IsSight { get; set; }

        public bool IsStanding { get; set; }
        public bool IsInternal { get; set; }

        public BodyPartClass(
            string name, 
            ITissueClass tissueClass, 
            ArmorSlot armorSlotType, WeaponSlot weaponSlotType, 
            IEnumerable<ICombatMoveClass> moves, 
            bool isCritical = false, bool canGrasp = false, bool canAmputate = false, 
            bool isNervous = false, bool isCirc = false, bool IsSkeletal = false,
            bool isDigit = false, 
            bool isBreathe = false,
            bool isSight = false,
            bool IsStance = false,
            bool IsInternal = false,
            IBodyPartClass parent = null)
        {
            Name = name;
            IsLifeCritical = isCritical;
            CanAmputate = canAmputate;
            CanGrasp = canGrasp;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            Moves = moves;
            Parent = parent;

            Tissue = tissueClass;
        }
    }
}
