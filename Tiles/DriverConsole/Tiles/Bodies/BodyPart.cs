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
        public bool CanBeAmputated { get { return Class.CanAmputate; } }
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

        public ITissue Tissue { get; private set; }

        public BodyPart(IBodyPartClass bodyPartClass, ITissue tissue, IBodyPart parent) 
        {
            Class = bodyPartClass;
            Tissue = tissue;
            Parent = parent;

            Health = HealthVector.Create();
        }

        public BodyPart(IBodyPartClass bodyPartClass, ITissue tissue) : this(bodyPartClass, tissue, null) { }
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
