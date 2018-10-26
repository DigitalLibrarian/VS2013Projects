using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Ecs;
using Tiles.Random;

namespace Tiles
{
    public interface IGame
    {
        IEntityManager EntityManager { get; }
        IRandom Random { get; }
        IAtlas Atlas { get; }
        ICamera Camera { get; }
        IActionLog ActionLog { get; }
        IPlayer Player { get; }
        IAttackConductor AttackConductor { get; }

        ITile CameraTile { get; }

        long DesiredFrameLength { get; set; }

        void UpdateBox(Math.Box3 updateBox);
    }
}
