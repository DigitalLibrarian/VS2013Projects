using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

namespace Tiles.Bodies
{
    public class BodyPart : IBodyPart
    {
        public string Name { get; private set; }
        public IBodyPart Parent { get; set; }
        public HealthVector Health { get; private set; }
        public ArmorSlot ArmorSlot { get; private set; }
        public WeaponSlot WeaponSlot { get; private set; }
        public bool IsLifeCritical { get; private set; }
        public bool CanAmputate { get; private set; }

        private bool _canGrasp;
        public bool CanGrasp { get { return _canGrasp && !IsGrasping && Weapon == null; } }
        public IBodyPart Grasped { get; private set; }
        public IBodyPart Grasper { get; set; }
        public bool IsGrasping { get { return Grasped != null; } }
        public bool IsWrestling { get { return IsGrasping || IsBeingGrasped; } }
        public bool IsBeingGrasped { get { return Grasper != null; } }
        public IItem Weapon { get; set; }
        public IItem Armor { get; set; }
        public BodyPart(string name, bool isCritical, bool canAmputate, bool canGrasp,
            ArmorSlot armorSlotType, WeaponSlot weaponSlotType, IBodyPart parent)
        {
            Name = name;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            Parent = parent;
            IsLifeCritical = isCritical;
            CanAmputate = canAmputate;
            _canGrasp = canGrasp;

            Health = HealthVector.Create();
        }

        public BodyPart(string name, bool isCritical, bool canAmputate, bool canGrasp, ArmorSlot armorSlotType, WeaponSlot weaponSlotType)
            : this(name, isCritical,canAmputate, canGrasp, armorSlotType, weaponSlotType, null)
        {

        }


        public void StartGrasp(IBodyPart part)
        {
            Grasped = part;
            part.Grasper = part;
        }

        public void StopGrasp(IBodyPart part)
        {
            Grasped = null;
            part.Grasper = null;
        }
    }
}
