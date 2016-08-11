﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Control;
using Tiles.Render;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Random;
using Tiles.Agents.Behaviors;

namespace Tiles
{
    public class Game : IGame
    {
        public IAtlas Atlas { get; private set; }
        public ICamera Camera { get; private set; }
        public IActionLog ActionLog { get; private set; }
        public IPlayer Player { get; private set; }
        public IAttackConductor AttackConductor { get; private set;}
        public IRandom Random { get; private set; }

        public IActiveAgentCommandSet ActiveCommands { get; private set; }

        public ITile CameraTile { get { return Atlas.GetTileAtPos(Camera.Pos); } }

        public Game(IAtlas atlas, IPlayer player, ICamera camera, IActionLog log, IActiveAgentCommandSet activeCommands, IRandom random)
        {
            Atlas = atlas;
            Player = player;
            Camera = camera;
            ActionLog = log;

            ActiveCommands = activeCommands;
            Random = random;

            AttackConductor = new AttackConductor(atlas, log);

            Atlas.GetTileAtPos(player.Agent.Pos).SetAgent(player.Agent);
        }


        public long TicksPerUpdate
        {
            get { return ActiveCommands.GetShortestTimeRemaining(); }// TODO - this should be dynamic based on current player action
        }


    }
}
