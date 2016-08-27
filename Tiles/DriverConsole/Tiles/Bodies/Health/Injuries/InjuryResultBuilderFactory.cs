using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryResultBuilderFactory : IInjuryResultBuilderFactory
    {
        IInjuryFactory InjuryFactory { get; set; }
        public InjuryResultBuilderFactory(IInjuryFactory injuryFactory)
        {
            InjuryFactory = injuryFactory;
        }
        public IInjuryResultBuilder Create()
        {
            var damage = new DamageVector();
            return new InjuryResultBuilder(
                InjuryFactory, damage);
        }
    }
}
