using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl
{
    public abstract class CanvasScreen : GameScreen
    {
        protected ICanvas Canvas { get; private set; }
        public CanvasScreen(ICanvas canvas)
        {
            Canvas = canvas;
        }
    }

    // TODO - this might be useful to set control paging for long lists of things
    class TranslatingCanvas : ICanvas
    {
        public Vector2 Translation { get; set; }
        ICanvas Inner { get; set; }

        public TranslatingCanvas(ICanvas inner)
        {
            Inner = inner;
        }

        Vector2 Translate(Vector2 v)
        {
            return v + Translation;
        }

        public void DrawSprite(ISprite sprite, Vector2 screenPos)
        {
            Inner.DrawSprite(sprite, Translate(screenPos));
        }

        public void DrawSymbol(int s, Vector2 screenPos, Color fg, Color bg)
        {
            Inner.DrawSymbol(s, Translate(screenPos), fg, bg);
        }

        public void FillBox(int s, Vector2 topLeft, Vector2 size, Color foregroundColor, Color backgroundColor)
        {
            Inner.FillBox(s, Translate(topLeft), size, foregroundColor, backgroundColor);
        }

        public void DrawString(string s, Vector2 screenPos)
        {
            Inner.DrawString(s, Translate(screenPos));
        }

        public void DrawString(string s, Vector2 screenPos, int width)
        {
            Inner.DrawString(s, Translate(screenPos), width);
        }

        public void DrawString(string s, Vector2 screenPos, Color fg, Color bg)
        {
            Inner.DrawString(s, Translate(screenPos), fg, bg);
        }

        public void WriteLinesInALine(Vector2 point, Vector2 slope, params string[] lines)
        {
            Inner.WriteLinesInALine(Translate(point), slope, lines);
        }

        public void WriteLineColumn(Vector2 topLeft, params string[] lines)
        {
            Inner.WriteLineColumn(Translate(topLeft), lines);
        }

        public void WriteLineColumn(Vector2 topLeft, Color fg, Color bg, params string[] lines)
        {
            Inner.WriteLineColumn(Translate(topLeft), fg, bg, lines);
        }
    }
}
