using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Bodies.Health.Injuries
{
    // a living injury with dynamic state, configured by IInjuryClass injection
    public interface IInjury
    {
        IInjuryClass Class { get; }

        string Adjective { get; }
        
        bool IsInstantDeath { get; }
        bool IsPermanant { get; }
        bool CanBeHealed { get; }

        bool IsOver { get; }
        bool CripplesBodyPart { get; }

        void Update(int ticks);

        string GetDisplayLabel();

        bool RemovesBodyPart { get; }
    }
}
