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
        public bool CanBeAmputated { get { return Class.CanBeAmputated; } }
        public bool CanGrasp { get { return Class.CanGrasp && !IsGrasping && Weapon == null; } }
        public int RelativeSize { get { return Class.RelativeSize; } }

        public bool IsNervous { get { return Class.IsNervous; } }
        public bool IsCirculatory { get { return Class.IsCirculatory; } }
        public bool IsSkeletal { get { return Class.IsSkeletal; } }

        public bool IsDigit { get { return Class.IsDigit; } }

        public bool IsBreathe { get { return Class.IsBreathe; } }
        public bool IsSight { get { return Class.IsSight; } }

        public bool IsStance { get { return Class.IsStance; } }
        public bool IsInternal { get { return Class.IsInternal; } }

        public IBodyPart Grasped { get; private set; }
        public IBodyPart GraspedBy { get; set; }
        public bool IsGrasping { get { return Grasped != null; } }
        public bool IsWrestling { get { return IsGrasping || IsBeingGrasped; } }
        public bool IsBeingGrasped { get { return GraspedBy != null; } }

        public IItem Weapon { get; set; }
        public IItem Armor { get; set; }

        public IBodyPartClass Class { get; private set; }
        public IBodyPart Parent { get; set; }

        public ITissue Tissue { get; private set; }
        public double Size { get; private set; }

        public IDamageVector Damage { get; private set; }

        public BodyPart(IBodyPartClass bodyPartClass, ITissue tissue, double size, IBodyPart parent) 
        {
            Class = bodyPartClass;
            Tissue = tissue;
            Size = size;
            Parent = parent;
            Damage = new DamageVector();
        }

        public BodyPart(IBodyPartClass bodyPartClass, ITissue tissue, double size) 
            : this(bodyPartClass, tissue, size, null) { }

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

        public double GetMass()
        {
            // body part mass just uses the inner most layer of tissue's density.  Kinda silly
            var dense = Tissue.TissueLayers.First();
            var density = (double)dense.Material.SolidDensity/100d;
            return density * Size;
        }
        
        public double GetContactArea()
        {
            return (int)System.Math.Pow((Size), 0.666d);
        }
        public double GetThickness()
        {
            return (int)System.Math.Pow((Size * 10000d), 0.333d);
        }


        public bool IsPulped()
        {
            return Damage.GetTypes().Any(dt => Damage.GetFraction(dt).AsDouble() >= 1d);
        }
    }
}
