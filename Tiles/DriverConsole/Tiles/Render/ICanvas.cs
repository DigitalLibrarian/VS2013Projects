using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Render
{
    public interface ICanvas
    {
        void DrawSprite(Sprite sprite, Vector2 screenPos);
        void DrawSymbol(int s, Vector2 screenPos, Color fg, Color bg);

        void FillBox(int s, Vector2 topLeft, Vector2 size, Color foregroundColor, Color backgroundColor);

        void DrawString(string s, Vector2 screenPos);
        void DrawString(string s, Vector2 screenPos, int width); 
        void DrawString(string s, Vector2 screenPos, Color fg, Color bg);

        void WriteLinesInALine(Vector2 point, Vector2 slope, params string[] lines);
        void WriteLineColumn(Vector2 topLeft, params string[] lines);
        void WriteLineColumn(Vector2 topLeft, Color fg, Color bg, params string[] lines);
    }
}
