﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render;
using Tiles.ScreensImpl.Panels;

namespace Tiles.ScreensImpl.UI
{
    public class LookingCommandScreen : CommandScreen
    {
        LookUiPanelScreen InfoPanel { get; set; }
        public LookingCommandScreen(IGame game, LookUiPanelScreen infoPanel) : base(game) 
        {
            InfoPanel = infoPanel;
        }

        public override void Load()
        {
            base.Load();
            ScreenManager.Add(InfoPanel);
        }

        public override void Unload()
        {
            ScreenManager.Remove(InfoPanel);
            base.Unload();
        }

        public override void OnKeyPress(Control.KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.Q || args.Key == ConsoleKey.Escape || args.Key == ConsoleKey.X)
            {
                Exit();
            }
            else if (ConsoleKeyCompassMapping.IsCompassDirection(args.Key))
            {
                var dir2 = CompassVectors.FromDirection(ConsoleKeyCompassMapping.ToDirection(args.Key));
                Game.Camera.Move(new Vector3(dir2.X, dir2.Y, 0));
            }
            else
            {
                InfoPanel.OnKeyPress(args);
            }
        }
    }
}