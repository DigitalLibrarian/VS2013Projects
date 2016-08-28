using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Render;

using Drawing = System.Drawing;
namespace Tiles.Render.WindowsForms
{
    public class GraphicsCanvas : ICanvas
    {
        Func<Graphics> GFunc { get; set; }
        ISpriteFontMap FontMap { get; set; }
        Dictionary<Tiles.Math.Color, ColorMatrix> CCMatrix { get; set; }

        Dictionary<Tiles.Math.Color, Drawing.Color> ColorMap = new Dictionary<Tiles.Math.Color, Drawing.Color>
        {
            {Tiles.Math.Color.Black, Drawing.Color.Black},
            {Tiles.Math.Color.Blue, Drawing.Color.Blue},
            {Tiles.Math.Color.Cyan, Drawing.Color.Cyan},
            {Tiles.Math.Color.DarkBlue, Drawing.Color.DarkBlue},
            {Tiles.Math.Color.DarkCyan, Drawing.Color.DarkCyan},
            {Tiles.Math.Color.DarkGray, Drawing.Color.DarkGray},
            {Tiles.Math.Color.DarkGreen, Drawing.Color.DarkGreen},
            {Tiles.Math.Color.DarkMagenta, Drawing.Color.DarkMagenta},
            {Tiles.Math.Color.DarkRed, Drawing.Color.DarkRed},
            {Tiles.Math.Color.DarkYellow, Drawing.Color.DarkGoldenrod},
            {Tiles.Math.Color.Gray, Drawing.Color.Gray},
            {Tiles.Math.Color.Green, Drawing.Color.Green},
            {Tiles.Math.Color.Magenta, Drawing.Color.Magenta},
            {Tiles.Math.Color.Red, Drawing.Color.Red},
            {Tiles.Math.Color.White, Drawing.Color.White},
            {Tiles.Math.Color.Yellow, Drawing.Color.Yellow}
        };

        public GraphicsCanvas(Func<Graphics> gFunc, ISpriteFontMap fontMap)
        {
            GFunc = gFunc;
            FontMap = fontMap;

            CCMatrix = new Dictionary<Tiles.Math.Color, ColorMatrix>();
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

        #region Primitive mapping
        Point Point(Vector2  v)
        {
            return new Point(v.X * FontMap.GlyphSize.X, v.Y * FontMap.GlyphSize.Y);
        }

        ColorMatrix ColorMatrix(Tiles.Math.Color cc)
        {
            if (CCMatrix.ContainsKey(cc))
            {
                return CCMatrix[cc];
            }

            var dc = ColorMap[cc];

            var rComp = (float)(dc.R) / (float)byte.MaxValue;
            var bComp = (float)(dc.B) / (float)byte.MaxValue;
            var gComp = (float)(dc.G) / (float)byte.MaxValue;

            float[][] colorMatrixElements = { 
               new float[] {1f,  0,  0,  0, 0},        // red scaling factor
               new float[] {0,  1f,  0,  0, 0},        // green scaling factor
               new float[] {0,  0,  1f,  0, 0},        // blue scaling factor
               new float[] {0,  0,  0,  1f, 0},        // alpha scaling factor 
               new float[] {rComp -1f, bComp - 1f,gComp -1f, 0, 1}};


            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            CCMatrix[cc] = colorMatrix;
            return colorMatrix;
        }

        #endregion


        void _DrawString(string s, Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {

        }


        public void DrawSprite(ISprite sprite, Tiles.Math.Vector2 screenPos)
        {
            var image = FontMap.Get(sprite.Symbol);
            var screenPoint = Point(screenPos);
            // Presumably I can use the sprite color information to roduce a ColorMatrix

            var imageAttr = new ImageAttributes();
            var colorMatrix = ColorMatrix(sprite.ForegroundColor);
            imageAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var destRect = new Rectangle(screenPoint, new Size(FontMap.GlyphSize.X, FontMap.GlyphSize.Y));

            GFunc().DrawImage(image, destRect, 0, 0, FontMap.GlyphSize.X, FontMap.GlyphSize.Y, GraphicsUnit.Pixel, imageAttr);
        }

        public void DrawSymbol(int s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
        }

        public void FillBox(int s, Tiles.Math.Vector2 topLeft, Tiles.Math.Vector2 size, Tiles.Math.Color foregroundColor, Tiles.Math.Color backgroundColor)
        {
            /*
            var pen = GetPen(foregroundColor, backgroundColor);
            GFunc().DrawRectangle(pen, topLeft.X, topLeft.Y, size.X, size.Y);
             * */
        }
        
        public void DrawString(string s, Tiles.Math.Vector2 screenPos)
        {
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, int width)
        {
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
        }

        public void WriteLinesInALine(Tiles.Math.Vector2 point, Tiles.Math.Vector2 slope, params string[] lines)
        {
        }

        public void WriteLineColumn(Tiles.Math.Vector2 topLeft, params string[] lines)
        {
        }

        public void WriteLineColumn(Tiles.Math.Vector2 topLeft, Tiles.Math.Color fg, Tiles.Math.Color bg, params string[] lines)
        {
        }
    }
}
