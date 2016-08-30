using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Behaviors;
using Tiles.Ecs;
using Tiles.EntityComponents;

namespace Tiles.EntitySystems
{
    public class CommandSystem : BaseSystem
    {
        public CommandSystem()
            : base(ComponentTypes.Command)
        {

        }

        protected override void UpdateEntity(IEntity entity, IGame game)
        {
            var commandComp = entity.GetComponent<ICommandComponent>();

            long ticks = game.DesiredFrameLength;

            // TODO - port agent command behavior here
        }

    }
}
