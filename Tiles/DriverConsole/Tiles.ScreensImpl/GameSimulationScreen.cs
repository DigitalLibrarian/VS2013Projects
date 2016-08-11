﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render;
using Tiles.ScreensImpl.Panels;

namespace Tiles.ScreensImpl
{
    public class GameSimulationScreen : CanvasBoxScreen
    {
        IGame Game { get; set; }
        
        bool Paused { get; set; }
        bool UnpauseOnUpdate { get; set; }

        Vector2 UpdateBoxHalfSize { get; set; }

        IAgentCommandFactory CommandFactory { get; set; }

        public GameSimulationScreen(IGame game, ICanvas canvas, Box box)
            : base(canvas, box)
        {
            Game = game;
            Paused = false;

            CommandFactory = new AgentCommandFactory();
            
            var padV = new Vector2(1,1);
            var simDisplayBox = new Box(padV, padV + new Vector2(23, 23));
            var actionLogPanelWidth = Box.Size.X - (simDisplayBox.Size.X + (padV.X * 3));
            var actionLogPanelHeight = game.ActionLog.MaxLines;
            var infoPanelOrigin = new Vector2(simDisplayBox.Max.X + padV.X, simDisplayBox.Min.Y);
            var infoPanelWidth = actionLogPanelWidth;
            var infoPanelHeight = Box.Max.Y - actionLogPanelHeight - padV.Y - padV.Y;
            var infoPanelBox = new Box(
                infoPanelOrigin,
                infoPanelOrigin + new Vector2(infoPanelWidth, infoPanelHeight)
                );
            var actionLogPanelOrigin = new Vector2(infoPanelBox.Min.X, infoPanelBox.Max.Y + padV.Y);
            var actionLogPanelBox = new Box(
                actionLogPanelOrigin,
                actionLogPanelOrigin + new Vector2(actionLogPanelWidth, actionLogPanelHeight)
                );

            ViewModel = new GameSimulationViewModel();

            var lookBox = new Box(
                infoPanelBox.Min,
                infoPanelBox.Max + new Vector2(0, Box.Max.Y - padV.Y - actionLogPanelHeight - padV.Y)
                );
            var lookExaminePanel = new LookUiPanelScreen(ViewModel, canvas, lookBox);
            SimDisplayScreen = new SimDisplayUiPanelScreen(ViewModel, canvas, simDisplayBox);
            ActionLogScreen = new ActionLogUiPanelScreen(ViewModel, canvas, actionLogPanelBox);
            LookingScreen = new LookingCommandScreen(Game, lookExaminePanel);
            InventoryScreen = new InventoryScreen(Game.Player, CommandFactory, Game.ActionLog, Canvas, Box);

            UpdateBoxHalfSize = game.Atlas.SiteSize;

        }

        IGameSimulationViewModel ViewModel { get; set; }
        SimDisplayUiPanelScreen SimDisplayScreen { get; set; }
        ActionLogUiPanelScreen ActionLogScreen { get; set; }
        LookingCommandScreen LookingScreen { get; set; }
        InventoryScreen InventoryScreen { get; set; }

        public override void Load()
        {
            base.Load();
            ScreenManager.Add(SimDisplayScreen);
            ScreenManager.Add(ActionLogScreen);
            BlockForInput = true;// block on initial draw for user input
        }

        public override void Unload()
        {
            base.Unload();
            ScreenManager.Remove(SimDisplayScreen);
            ScreenManager.Remove(ActionLogScreen);
        }

        void UpdateViewModel()
        {
            ViewModel.Camera = Game.Camera;
            ViewModel.CameraTile = Game.CameraTile;
            ViewModel.Atlas = Game.Atlas;
            ViewModel.Looking = Paused;
            ViewModel.ActionLog = Game.ActionLog;
        }

        public override void Draw()
        {
            if (!Paused)
            {
                Game.Camera.Pos = Game.Player.Pos;
            }

            UpdateViewModel();
        }

        public override void Update()
        {
            if (!Paused)
            {
                var updatedAgents = new List<IAgent>();
                var camPos = Game.Camera.Pos;

                var updateBox = new Box(camPos - UpdateBoxHalfSize, camPos + UpdateBoxHalfSize);
                // TODO - limit this to a "working set" of sites
                foreach (var tile in Game.Atlas.GetTiles(updateBox).ToList()) 
                {
                    if (tile.HasAgent && !updatedAgents.Contains(tile.Agent))
                    {
                        var cTile = Game.Atlas.GetTileAtPos(tile.Agent.Pos);
                        updatedAgents.Add(tile.Agent);
                        tile.Agent.Update(Game);
                    }
                }
            }
            
            if (UnpauseOnUpdate)
            {
                Paused = false;
                UnpauseOnUpdate = false;
            }

            if (Game.Player.Agent.IsDead)
            {
                ScreenManager.Add(new YouDiedScreen(Game.Random, Canvas, Box));
                Exit();
            }

            BlockForInput = Game.Player.Agent.AgentBehavior.Context.Executed;
        }

        #region Controls
        public override void OnKeyPress(KeyPressEventArgs args)
        {
            Key_Quit(args);
            Key_Move(args);
            Key_Inventory(args);
            Key_Look(args);
            Key_Get(args);
        }

        void Key_Inventory(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.I)
            {
                ShowInventoryScreen();
            }
        }
        void Key_Quit(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.Q)
            {
                Exit();
            }
        }
        void Key_Look(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.X)
            {
                StartLooking();
            }
        }
        void Key_Move(KeyPressEventArgs args)
        {
            if (ConsoleKeyCompassMapping.IsCompassDirection(args.Key))
            {
                var delta = CompassVectors.FromDirection(ConsoleKeyCompassMapping.ToDirection(args.Key));
                var newTile = Game.Atlas.GetTileAtPos(Game.Player.Pos + delta);
                if (newTile.HasAgent && !newTile.Agent.IsDead)
                {
                    ScreenManager.Add(
                        new CombatScreen(
                            Game.Player, 
                            newTile.Agent, 
                            CommandFactory,
                            Game.AttackConductor, 
                            new AttackMoveFactory(new AttackMoveBuilder(new DamageCalc())), Canvas, Box)
                        );
                }
                else
                {
                    Game.Player.EnqueueCommand(CommandFactory.MoveDirection(Game.Player.Agent, delta));
                }
            }

            if (args.Key == ConsoleKey.NumPad5)
            {
                Game.Player.EnqueueCommand(CommandFactory.Nothing(Game.Player.Agent));
            }
        }
        void Key_Get(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.G)
            {
                Game.Player.EnqueueCommand(CommandFactory.PickUpItemsOnAgentTile(Game.Player.Agent));
            }
        }
        #endregion

        void ShowInventoryScreen()
        {
            ScreenManager.Add(InventoryScreen);
        }

        void StartLooking()
        {
            ScreenManager.Add(LookingScreen);
            Paused = true;
            UnpauseOnUpdate = true;
        }
    }
}
