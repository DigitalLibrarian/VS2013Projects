using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    class Injury
    {
        string Adjective { get; set; }

        bool IsInstantDeath { get; set; }
        bool IsBodyPartSpecific { get; set; }
        bool IsTissueLayerSpecific { get; set; }

        bool IsPermanant { get; set; }
        bool CanBeHealed { get; set; }

        bool CripplesBodyPart { get; set; }

        bool UsesTtl { get; set; }
        int Ttl { get; set; }
    }
}
