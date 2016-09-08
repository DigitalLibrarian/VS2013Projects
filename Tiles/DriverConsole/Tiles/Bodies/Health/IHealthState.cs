using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Bodies.Health
{
    // the living state of health of an agent
    public interface IHealthState
    {
        void BindBody(IBody body);

        bool IsWounded { get; }
        bool IsDead { get; }

        void Add(IInjury injury);

        void Update(int ticks);
    }
}
