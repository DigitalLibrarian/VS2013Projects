using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;

namespace Tiles.Bodies
{
    public interface IBodyPart
    {
        string Name { get; }
        IBodyPart Parent { get; }
        HealthVector Health { get; }

        bool IsLifeCritical { get; }
        bool CanAmputate { get; }
        bool CanGrasp { get; }

        ArmorSlot ArmorSlot { get; }
        IItem Armor { get; set; }
        WeaponSlot WeaponSlot { get; }
        IItem Weapon { get; set; }

        IBodyPart GraspedBy { get; set; }
        IBodyPart Grasped { get; }
        bool IsGrasping { get; }
        bool IsBeingGrasped { get; }

        void StartGrasp(IBodyPart part);
        void StopGrasp(IBodyPart part);


        bool IsWrestling { get; }
    }
}
