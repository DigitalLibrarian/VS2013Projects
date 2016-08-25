﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;

namespace Tiles.Bodies.Health
{
    // the living state of health of an agent
    public interface IHealthState
    {
        bool IsWounded { get; }
        bool IsDead { get; }

        void Add(IInjury injury);

        void Update(int ticks);
    }
}
