using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryClass : IInjuryClass
    {
        public string Adjective { get; set; }

        public bool IsInstantDeath { get; set; }
        public bool IsBodyPartSpecific { get; set; }
        public bool IsTissueLayerSpecific { get; set; }

        public bool IsPermanant { get; set; }
        public bool CanBeHealed { get; set; }

        public bool CripplesBodyPart { get; set; }

        public bool UsesTtl { get; set; }
        public int Ttl { get; set; }
    }
}
