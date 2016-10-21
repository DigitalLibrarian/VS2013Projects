using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Random;
using Tiles.Render;

namespace Tiles.ScreensImpl.UI
{
    public class YouDiedScreen : CanvasBoxScreen
    {
        IRandom Random { get; set; }
        string Message { get; set; }

        public YouDiedScreen(IRandom random, ICanvas canvas, Box2 box) : base(canvas, box)
        {
            Random = random;
            Message = "You died!";
            BlockForInput = false;
        }

        public override void Draw()
        {
            Canvas.DrawString(Message, Box.Min + (Box.Size*0.5) - new Vector2(Message.Length/2, 0));
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.Q || args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
    }
}
