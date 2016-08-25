using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health
{
    // these get configured via content
    public interface IInjuryClass
    {
        string Adjective { get; }

        bool IsInstantDeath { get; }
        bool IsBodyPartSpecific { get; }
        bool IsTissueLayerSpecific { get; }

        bool IsPermanant { get; }
        bool CanBeHealed { get; }

        bool CripplesBodyPart { get; set; }

        bool UsesTtl { get; }
        int Ttl { get; }
    }
}
