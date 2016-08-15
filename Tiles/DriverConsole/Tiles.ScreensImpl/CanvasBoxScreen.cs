using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl
{
    public abstract class CanvasBoxScreen : CanvasScreen
    {
        protected Color Foreground { get; set; }
        protected Color Background { get; set; }
        public Box2 Box { get; private set; }
        public CanvasBoxScreen(ICanvas canvas, Box2 box)
            : base(canvas)
        {
            Box = box;
            Foreground = Color.White;
            Background = Color.Black;

            PropagateDraw = false;
            BlockForInput = false;
        }

        public override void Draw() { }

        public override void Update() { }

        // TODO - using load/unload as a screen invalidator

        public override void Load()
        {
            Canvas.FillBox(Symbol.None, Box.Min, Box.Size, Color.White, Color.Black);
        }

        public override void Unload()
        {
            // this is kinda gross
            Canvas.FillBox(Symbol.None, Box.Min, Box.Size, Color.White, Color.Black);
        }
    }

    
}
