using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render;
using Tiles.ScreensImpl;
using Tiles.ScreensImpl.ContentFactories;

namespace Tiles.ScreenImpl
{
    public class ScreenLoadingMenuScreen : CanvasBoxScreen
    {
        IDictionary<string, Func<IGameScreen>> Factories { get; set; }
        JaggedListSelector Selector { get; set; }
        public ScreenLoadingMenuScreen(ICanvas canvas, Box box): base(canvas, box)
        {
            Factories = new Dictionary<string, Func<IGameScreen>>{
                {
                    "Zombie World", () => new GameSimulationScreen(
                        new GameFactory().SetupGenericZombieWorld(),
                        canvas,
                        box)
                },
                {
                    "Arena", () => new GameSimulationScreen(
                        new GameFactory().SetupArenaWorld(),
                        canvas,
                        box)
                },

            };
            Selector = new JaggedListSelector()
            {
                Foreground = Foreground,
                Background = Background,
                SelectedBackground = Color.Blue,
                SelectedForeground = Color.White
            };
            Selector.Update(Factories.Count());

            BlockForInput = true;
        }
        
        public override void Draw()
        {
            Color fg = Foreground, bg = Background;
            Canvas.DrawString(string.Format("Screen Launcher (H: {0})", Selector.Selected), Box.Min, fg, bg);
            Selector.Draw(Canvas, new Vector2(1, 2), Factories.Keys.ToArray());
        }

        public override void Update()
        {

        }

        public override void OnKeyPress(Control.KeyPressEventArgs args)
        {
            if (ConsoleKeyCompassMapping.IsCompassKey(args.Key))
            {
                var comDir = ConsoleKeyCompassMapping.ToDirection(args.Key);
                switch (comDir)
                {
                    case CompassDirection.North:
                        Selector.MoveUp();
                        break;
                    case CompassDirection.South:
                        Selector.MoveDown();
                        break;
                }
            }
            else if (args.Key == ConsoleKey.Enter)
            {
                LaunchSelected();
            }
        }

        private void LaunchSelected()
        {
            var selectedKey = Factories.Keys.ToArray()[Selector.Selected.Y];
            var screen = Factories[selectedKey]();
            ScreenManager.Add(screen);
            Exit();
        }
    }
}
