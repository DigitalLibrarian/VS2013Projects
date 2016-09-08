using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Bodies.Health
{
    public class HealthState : IHealthState
    {
        IBody Body { get; set; }
        public bool InstantDeath { get; private set; }
        List<IInjury> Injuries { get; set; }
        public HealthState()
        {
            Injuries = new List<IInjury>();
        }

        public void Add(IInjury injury)
        {
            Injuries.Add(injury);
            // TODO - clean up
            injury.BodyPart.Damage.Add(injury.Damage);
        }

        public bool IsWounded { get { return Injuries.Any(); } }

        public bool IsDead
        {
            get { return InstantDeath; }
        }

        public void Update(int ticks)
        {
            foreach (var injury in Injuries.ToArray())
            {
                injury.Update(ticks);

                CheckInstantDeath(injury);

                if (injury.IsOver)
                {
                    Injuries.Remove(injury);
                }
            }
        }

        void CheckInstantDeath(IInjury injury)
        {
            InstantDeath = InstantDeath
                || injury.IsInstantDeath;
        }

        public IEnumerable<IInjury> GetInjuries()
        {
            return Injuries;
        }
    }
}
