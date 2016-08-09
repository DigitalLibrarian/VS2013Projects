using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Console;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.Render.Console
{
    public class ConsoleCanvas : ICanvas
    {
        IConsoleWriter Writer { get; set; }
        public ConsoleCanvas(IConsoleWriter writer)
        {
            Writer = writer;
        }

        public void DrawSprite(ISprite sprite, Vector2 screenPos)
        {
            DrawSymbol(sprite.Symbol, screenPos, sprite.ForegroundColor, sprite.BackgroundColor);
        }

        public void DrawSymbol(Tiles.Symbol s, Vector2 screenPos, Color fg, Color bg)
        {
            SetCursorPosition(screenPos);
            SetColors(fg, bg);

            Writer.Write(ToChar(s));
        }

        public void DrawString(string s, Vector2 screenPos)
        {
            SetCursorPosition(screenPos);
            Writer.Write(s);
        }

        public void DrawString(string s, Vector2 screenPos, Color fg, Color bg)
        {
            SetColors(fg, bg);
            DrawString(s, screenPos);
        }

        public void DrawString(string s, Vector2 screenPos, int width)
        {
            DrawString(string.Join("", Enumerable.Repeat(ToChar(Symbol.None), width)), screenPos);
            DrawString(s, screenPos);
        }

        protected ConsoleColor ToConsoleColor(Color c)
        {
            return (ConsoleColor)(int)c;
        }

        void SetCursorPosition(Vector2 pos)
        {
            Writer.SetCursorPosition(pos.X, pos.Y);
        }

        void SetColors(Color fg, Color bg)
        {
            Writer.SetColors(ToConsoleColor(fg), ToConsoleColor(bg));
        }

        char ToChar(Symbol s)
        {
            return (char)s;
        }

        public void WriteLinesInALine(Vector2 point, Vector2 slope, params string[] lines)
        {
            foreach (var line in lines)
            {
                DrawString(line, point);
                point += slope;
            }
        }

        public void WriteLineColumn(Vector2 topLeft, params string[] lines)
        {
            WriteLinesInALine(topLeft, new Vector2(0, 1), lines);
        }


        public void FillBox(Symbol s, Vector2 topLeft, Vector2 size, Color foregroundColor, Color backgroundColor)
        {
            SetColors(foregroundColor, backgroundColor);

            for (int x = topLeft.X; x < topLeft.X + size.X; x++)
            {
                for (int y = topLeft.Y; y < topLeft.Y + size.Y; y++)
                {
                    SetCursorPosition(new Vector2(x, y));
                    Writer.Write(ToChar(s));
                }
            }
        }


        public void WriteLineColumn(Vector2 topLeft, Color fg, Color bg, params string[] lines)
        {
            SetColors(fg, bg);
            WriteLineColumn(topLeft, lines);
        }
    }
}
