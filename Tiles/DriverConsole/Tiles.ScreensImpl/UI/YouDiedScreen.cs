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

        long frameCount = 0;
        public override void Draw()
        {
            if (frameCount % 1000 == 0)
            {
                Canvas.DrawString(Message, Random.Next(Box.Max - new Vector2(Message.Length, 0)));
            }
            frameCount++;
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
