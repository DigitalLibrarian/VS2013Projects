using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies
{
    public interface IBody
    {
        bool IsGrasping { get; }
        bool IsBeingGrasped { get; }
        bool IsWrestling { get; }

        double Size { get; }
        double StoredFat { get; }

        IList<IBodyPart> Parts { get; }
        IBodyClass Class { get; }

        void Amputate(IBodyPart part);

        IEnumerable<ICombatMoveClass> Moves { get; set; }
        IEnumerable<IBodyPart> GetInternalParts(IBodyPart part);

        void SetAttribute(string name, int value);
        int GetAttribute(string name);
        IEnumerable<string> AttributeNames { get; }

        IDictionary<IBodyPart, int> GetRelations(IBodyPart target, BodyPartRelationType type);
    }
}
