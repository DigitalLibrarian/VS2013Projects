using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;

namespace Tiles.Bodies
{
    public interface IBodyPartClass
    {
        IBodyPartClass Parent { get; }
        string TokenId { get; }
        string Name { get; }
        int RelativeSize { get; }

        ArmorSlot ArmorSlot { get; }
        WeaponSlot WeaponSlot { get; }
        bool IsLifeCritical { get; }
        bool CanBeAmputated { get; }
        bool CanGrasp { get; }

        ITissueClass Tissue { get; }
        
        bool IsNervous { get; }
        bool IsCirculatory { get; }
        bool IsSkeletal { get;}

        bool IsDigit { get; }

        bool IsBreathe { get; }
        bool IsSight { get; }

        bool IsStance { get; }
        bool IsLeft { get; set; }
        bool IsRight { get; set; }
        bool IsInternal { get; }
        bool IsSmall { get; }
        bool IsEmbedded { get; }
        bool IsConnector { get; }
        bool PreventsParentCollapse { get; }

        IEnumerable<string> Categories { get; }
        IEnumerable<string> Types { get; }
        IEnumerable<IBodyPartRelation> BodyPartRelations { get; set; }
    }
}
