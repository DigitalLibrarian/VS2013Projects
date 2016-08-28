using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.Render.WindowsForms
{
    public class GraphicsCanvas : ICanvas
    {
        Func<Graphics> GFunc { get; set; }
        public GraphicsCanvas(Func<Graphics> gFunc)
        {
            GFunc = gFunc;
        }

        #region Pens and Brushes
        private Pen GetPen(Tiles.Math.Color foregroundColor, Tiles.Math.Color backgroundColor)
        {
            throw new NotImplementedException();
        }

        private Pen GetPen()
        {
            throw new NotImplementedException();
        }
        #endregion


        void _DrawString(string s, Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {

        }


        public void DrawSprite(Tiles.ISprite sprite, Tiles.Math.Vector2 screenPos)
        {
            throw new NotImplementedException();
        }

        public void DrawSymbol(int s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
            throw new NotImplementedException();
        }

        public void FillBox(int s, Tiles.Math.Vector2 topLeft, Tiles.Math.Vector2 size, Tiles.Math.Color foregroundColor, Tiles.Math.Color backgroundColor)
        {
            var pen = GetPen(foregroundColor, backgroundColor);
            GFunc().DrawRectangle(pen, topLeft.X, topLeft.Y, size.X, size.Y);
        }
        
        public void DrawString(string s, Tiles.Math.Vector2 screenPos)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, int width)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
            throw new NotImplementedException();
        }

        public void WriteLinesInALine(Tiles.Math.Vector2 point, Tiles.Math.Vector2 slope, params string[] lines)
        {
            throw new NotImplementedException();
        }

        public void WriteLineColumn(Tiles.Math.Vector2 topLeft, params string[] lines)
        {
            throw new NotImplementedException();
        }

        public void WriteLineColumn(Tiles.Math.Vector2 topLeft, Tiles.Math.Color fg, Tiles.Math.Color bg, params string[] lines)
        {
            throw new NotImplementedException();
        }
    }
}
