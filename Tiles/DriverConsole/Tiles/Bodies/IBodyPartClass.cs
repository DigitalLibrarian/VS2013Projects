using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

namespace Tiles.Bodies
{
    public interface IBodyPartClass
    {
        IBodyPartClass Parent { get; }
        string Name { get; }

        ArmorSlot ArmorSlot { get; }
        WeaponSlot WeaponSlot { get; }
        bool IsLifeCritical { get; }
        bool CanAmputate { get; }
        bool CanGrasp { get; }

        ITissueClass Tissue { get; }
    }
}
