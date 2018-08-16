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
        IBodyPartClass Class { get; }

        string Name { get; }
        IBodyPart Parent { get; }

        ITissue Tissue { get; }

        bool IsLifeCritical { get; }
        bool CanBeAmputated { get; }
        bool CanGrasp { get; }

        bool IsNervous { get; }
        bool IsCirculatory { get; }
        bool IsSkeletal { get; }

        bool IsDigit { get; }

        bool IsBreathe { get; }
        bool IsSight { get; }

        bool IsStance { get; }
        bool IsInternal { get; }
        bool IsConnector { get; }

        bool PreventsParentCollapse { get; }

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

        double Size { get; }

        bool IsDamaged { get; }
        bool IsEffectivelyPulped { get; }

        double Mass { get; }
        double ContactArea { get; }
        double Thickness { get; }

        //double GetMass();
        //double GetContactArea();
        //double GetThickness();

        bool IsLeft { get; }

        bool IsRight { get; }
    }
}
