using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Bodies.Health
{
    // a living injury with dynamic state, configured by IInjuryClass injection
    public interface IInjury
    {
        string Adjective { get; }
        
        bool IsInstantDeath { get; }
        bool IsPermanant { get; }
        bool CanBeHealed { get; }

        bool IsOver { get; }
        bool CripplesBodyPart { get; }

        void Update(int ticks);

        string GetDisplayLabel();
    }
}
