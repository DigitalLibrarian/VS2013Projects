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
        private const int PenWidth = 2;

        Func<Graphics> GFunc { get; set; }
        ISpriteFontMap FontMap { get; set; }
        Dictionary<Tiles.Math.Color, ImageAttributes> ImageAttrs { get; set; }

        Graphics G { get { return GFunc(); } }

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

            ImageAttrs = new Dictionary<Tiles.Math.Color, ImageAttributes>();
            DefaultPen = new Pen(Drawing.Color.White, PenWidth);
            DefaultFont = new Font("Console", FontMap.GlyphSize.X);
        }

        #region Pens and Brushes
        Dictionary<Tiles.Math.Color, Pen> ColorPens { get; set; }
        Pen DefaultPen { get; set; }
        Font DefaultFont { get; set; }
        private Pen GetPen(Tiles.Math.Color foregroundColor)
        {
            if (ColorPens.ContainsKey(foregroundColor))
            {
                return ColorPens[foregroundColor];
            }

            var pen = new Pen(ColorMap[foregroundColor], PenWidth);
            ColorPens[foregroundColor] = pen;
            return pen;
        }

        #endregion

        #region Primitive mapping
        Point Point(Vector2  v)
        {
            return new Point(v.X * FontMap.GlyphSize.X, v.Y * FontMap.GlyphSize.Y);
        }

        ImageAttributes GetColorMatrix(Tiles.Math.Color cc)
        {
            if (ImageAttrs.ContainsKey(cc))
            {
                return ImageAttrs[cc];
            }
            var dc = ColorMap[cc];
            /*
            var rComp = (float)(dc.R) / (float)byte.MaxValue;
            var bComp = (float)(dc.B) / (float)byte.MaxValue;
            var gComp = (float)(dc.G) / (float)byte.MaxValue;

            float[][] colorMatrixElements = { 
               new float[] {1f,  0,  0,  0, 0},        // red scaling factor
               new float[] {0,  1f,  0,  0, 0},        // green scaling factor
               new float[] {0,  0,  1f,  0, 0},        // blue scaling factor
               new float[] {0,  0,  0,  1f, 0},        // alpha scaling factor
               new float[] {rComp -1f, bComp - 1f,gComp -1f, 0, 1}
                                            };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            */
            var imageAttr = new ImageAttributes();
            //imageAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Drawing.Color.White; 
            colorMap.NewColor = dc;

            ColorMap[] remapTable = { colorMap };
            imageAttr.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
            //CCMatrix[cc] = colorMatrix;
            //return colorMatrix;

            ImageAttrs[cc] = imageAttr;
            return imageAttr;
        }

        Rectangle GlyphRect(Point p)
        {
            return new Rectangle(p, new Size(FontMap.GlyphSize.X, FontMap.GlyphSize.Y));
        }
        #endregion
        void DrawGlyph(int g, Rectangle screenRect, Tiles.Math.Color fg)
        {
            //var colorMatrix = GetColorMatrix(fg);
            //using (var imageAttr = new ImageAttributes())
            {
                var imageAttr = GetColorMatrix(fg);
                //imageAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
                
                var image = FontMap.Get(g);
                var graphics = G;

                graphics.DrawImage(image, screenRect, 0, 0, FontMap.GlyphSize.X, FontMap.GlyphSize.Y, GraphicsUnit.Pixel, imageAttr);
            }
        }

        public void DrawSprite(ISprite sprite, Tiles.Math.Vector2 screenPos)
        {
            var screenPoint = Point(screenPos);
            var destRect = GlyphRect(screenPoint);
            DrawGlyph(FontMap.SolidGlyphIndex, destRect, sprite.BackgroundColor);
            DrawGlyph(sprite.Symbol, destRect, sprite.ForegroundColor);
        }

        public void DrawSymbol(int s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
            var screenPoint = Point(screenPos);
            var destRect = GlyphRect(screenPoint);
            DrawGlyph(FontMap.SolidGlyphIndex, destRect, bg);
            DrawGlyph(s, destRect, fg);
        }

        
        public void DrawString(string s, Tiles.Math.Vector2 screenPos)
        {
            var screenPoint = Point(screenPos);
            G.DrawString(s, DefaultFont, SystemBrushes.Window, screenPoint.X, screenPoint.Y, StringFormat.GenericDefault);
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, int width)
        {
            var p = Point(screenPos);
            var size = G.MeasureString(s, DefaultFont, width * (int) DefaultFont.Size);

            var destRect = new Rectangle(p, new Size((int)size.Width, (int)size.Height));
            DrawGlyph(FontMap.SolidGlyphIndex, destRect, Tiles.Math.Color.Black);
            G.DrawString(s, DefaultFont, SystemBrushes.Window, p.X, p.Y, StringFormat.GenericDefault);
        }

        public void DrawString(string s, Tiles.Math.Vector2 screenPos, Tiles.Math.Color fg, Tiles.Math.Color bg)
        {
            var p = Point(screenPos);
            var size = G.MeasureString(string.Format("{0}{1}", s, s), DefaultFont);

            var destRect = new Rectangle(p, new Size((int)size.Width, (int)size.Height));
            DrawGlyph(FontMap.SolidGlyphIndex, destRect, bg);
            G.DrawString(s, DefaultFont, SystemBrushes.Window, p.X, p.Y);
        }


        public void FillBox(int s, Vector2 topLeft, Vector2 size, Tiles.Math.Color foregroundColor, Tiles.Math.Color backgroundColor)
        {
            return;
            //SetColors(foregroundColor, backgroundColor);

            for (int x = topLeft.X; x < topLeft.X + size.X; x++)
            {
                for (int y = topLeft.Y; y < topLeft.Y + size.Y; y++)
                {
                    //SetCursorPosition(new Vector2(x, y));
                    //Writer.Write(ToChar(s));

                    var destRect = GlyphRect(new Point(x * FontMap.GlyphSize.X, y * FontMap.GlyphSize.Y));

                    DrawGlyph(FontMap.SolidGlyphIndex, destRect, backgroundColor);
                    DrawGlyph(s, destRect, foregroundColor);
                }
            }
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

        public void WriteLineColumn(Vector2 topLeft, Tiles.Math.Color fg, Tiles.Math.Color bg, params string[] lines)
        {
            //SetColors(fg, bg);
            WriteLineColumn(topLeft, lines);
        }
    }
}
