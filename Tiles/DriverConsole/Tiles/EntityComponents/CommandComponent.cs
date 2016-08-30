﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public interface ICommandComponent : IComponent
    {
        IAgentBehavior Behavior { get; }
    }

    public class CommandComponent : ICommandComponent
    {
        public int Id { get { return ComponentTypes.Command; } }

        public IAgentBehavior Behavior { get; set; }
    }
}
