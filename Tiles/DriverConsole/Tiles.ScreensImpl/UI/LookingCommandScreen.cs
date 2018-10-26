using System;
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
                if (args.Shift)
                {
                    dir2 *= 10;
                }
                Game.Camera.Move(new Vector3(dir2.X, dir2.Y, 0));
            }
            else if (args.Key == ConsoleKey.OemComma && args.Shift)
            {
                Game.Camera.Move(new Vector3(0, 0, 1));
            }
            else if (args.Key == ConsoleKey.OemPeriod && args.Shift)
            {
                Game.Camera.Move(new Vector3(0, 0, -1));
            }
            else if (args.Key == ConsoleKey.M && args.Shift)
            {
                var tilePos = Game.Camera.Pos;
                var tile = Game.Atlas.GetTileAtPos(tilePos);
                tile.IsTerrainPassable = true;
                tile.TerrainSprite = new Sprite(Symbol.None, Color.Black, Color.Black);
            }
            else if (args.Key == ConsoleKey.F && args.Shift)
            {
                var tilePos = Game.Camera.Pos;
                var tile = Game.Atlas.GetTileAtPos(tilePos);
                if (!tile.HasAgent)
                {
                    tile.IsTerrainPassable = false;
                    tile.LiquidDepth = 0;
                    tile.TerrainSprite = new Sprite(Symbol.Terrain_Floor, Color.White, Color.White);
                }
            }
            else
            {
                InfoPanel.OnKeyPress(args);
            }
        }
    }
}
