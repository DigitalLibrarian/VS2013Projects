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
        public string Name { get { return Class.Name; } }
        public ArmorSlot ArmorSlot { get { return Class.ArmorSlot; } }
        public WeaponSlot WeaponSlot { get { return Class.WeaponSlot; } }
        public bool IsLifeCritical { get { return Class.IsLifeCritical; } }
        public bool CanAmputate { get { return Class.CanAmputate; } }
        public bool CanGrasp { get { return Class.CanGrasp && !IsGrasping && Weapon == null; } }

        public IBodyPart Grasped { get; private set; }
        public IBodyPart GraspedBy { get; set; }
        public bool IsGrasping { get { return Grasped != null; } }
        public bool IsWrestling { get { return IsGrasping || IsBeingGrasped; } }
        public bool IsBeingGrasped { get { return GraspedBy != null; } }

        public IItem Weapon { get; set; }
        public IItem Armor { get; set; }

        public IBodyPartClass Class { get; private set; }
        public IBodyPart Parent { get; set; }
        public HealthVector Health { get; private set; }

        public BodyPart(
            IBodyPartClass bodyPartClass,
            /*string name, bool isCritical, bool canAmputate, bool canGrasp,
            ArmorSlot armorSlotType, WeaponSlot weaponSlotType, */
             IBodyPart parent) 
        {
            /*
            Name = name;
            ArmorSlot = armorSlotType;
            WeaponSlot = weaponSlotType;
            IsLifeCritical = isCritical;
            CanAmputate = canAmputate;
            _canGrasp = canGrasp;

            */
            Class = bodyPartClass;
            Parent = parent;

            Health = HealthVector.Create();
        }

        public BodyPart(IBodyPartClass bodyPartClass) : this(bodyPartClass, null) { }


        /*
        public BodyPart(string name, bool isCritical, bool canAmputate, bool canGrasp, ArmorSlot armorSlotType, WeaponSlot weaponSlotType)
            : this(name, isCritical, canAmputate, canGrasp, armorSlotType, weaponSlotType, null)
        {

        }
         * */


        public void StartGrasp(IBodyPart part)
        {
            Grasped = part;
            part.GraspedBy = this;
        }

        public void StopGrasp(IBodyPart part)
        {
            Grasped = null;
            part.GraspedBy = null;
        }
    }
}
