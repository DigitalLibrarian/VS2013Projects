using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

namespace Tiles.Bodies
{
    public class BodyPartClass : IBodyPartClass
    {
        public string Name { get; set; }

        public IBodyPartClass Parent { get; set; }
        public ITissueClass Tissue { get; set; }

        public ArmorSlot ArmorSlot { get; set; }
        public WeaponSlot WeaponSlot { get; set; }
        public bool IsLifeCritical { get; set; }
        public bool CanAmputate { get; set; }

        public bool CanGrasp { get; set; }

        public BodyPartClass(
            string name, 
            bool isCritical, bool canAmputate, bool canGrasp,
            ITissueClass tissueClass,
            ArmorSlot armorSlotType, WeaponSlot weaponSlotType, 
            IBodyPartClass parent = null)
        {
            Name = name;
            IsLifeCritical = isCritical;
            CanAmputate = canAmputate;
            CanGrasp = canGrasp;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            Parent = parent;

            Tissue = tissueClass;
        }
    }
}
