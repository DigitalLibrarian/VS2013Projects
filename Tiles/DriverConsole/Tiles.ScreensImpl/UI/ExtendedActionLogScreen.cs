using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Math;
using Tiles.Render;
using Tiles.StringManipulation;


namespace Tiles.ScreensImpl.UI
{
    public class ExtendedActionLogScreen : CanvasBoxScreen
    {
        IActionLog ActionLog { get; set; }
        JaggedListSelector Selector { get; set; }

        public ExtendedActionLogScreen(ICanvas canvas, Box2 box, IActionLog actionLog)
            : base(canvas, box)
        {
            ActionLog = actionLog;

            PropagateInput = false;
            PropagateUpdate = false;
        }

        public override void Load()
        {
            base.Load();

            Selector = new JaggedListSelector(Box)
            {
                Foreground = this.Foreground,
                Background = this.Background,
                SelectedBackground = Color.Blue,
                SelectedForeground = Color.White
            };
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("Action Log", Box.Min);

            var lines = ActionLog.GetLines();
            lines = lines.WrapText(Box.Size.X - 1);
            Selector.Draw(Canvas, Box.Min + new Vector2(1, 2), lines.ToArray());
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
            else if (args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
    }
}
