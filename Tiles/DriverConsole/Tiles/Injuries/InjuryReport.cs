using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Injuries
{
    public interface IInjuryReport
    {
        IEnumerable<IBodyPartInjury> BodyPartInjuries { get; }
    }

    public class InjuryReport : IInjuryReport
    {
        public IEnumerable<IBodyPartInjury> BodyPartInjuries { get; set; }
        public InjuryReport(IEnumerable<IBodyPartInjury> injuries)
        {
            BodyPartInjuries = injuries;
        }
    }
}
