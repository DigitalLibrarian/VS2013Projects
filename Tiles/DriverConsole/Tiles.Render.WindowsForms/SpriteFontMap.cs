﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tiles.Math;

namespace Tiles.Render.WindowsForms
{
    public interface ISpriteFontMap
    {
        int SolidGlyphIndex { get; }
        Vector2 GlyphSize { get; }
        Image Get(int charCode);
    }

    public class SpriteFontMap : ISpriteFontMap
    {
        Bitmap Image { get; set; }
        public Vector2 GlyphSize { get; private set; }
        public int SolidGlyphIndex { get; private set; }
        Vector2 GlyphDim { get; set; }

        List<Image> Map { get; set; }

        public SpriteFontMap(Image image, Vector2 glyphSize, int solidGlyphIndex)
        {
            Image = new Bitmap(image);
            SolidGlyphIndex = solidGlyphIndex;
            GlyphSize = glyphSize;
            GlyphDim = new Vector2(image.Width / glyphSize.X, image.Height / glyphSize.Y);

            Map = new List<Image>();
            for (int y = 0; y < GlyphDim.Y; y++) 
            { 
                for(int x = 0; x < GlyphDim.X;x++)
                {
                    var cropArea = new Rectangle(x * glyphSize.X, y * glyphSize.Y, glyphSize.X, glyphSize.Y);
                    var glyphImage = Image.Clone(cropArea, Image.PixelFormat);
                    Map.Add(glyphImage);
                }
            }
        }

        public Image Get(int charCode)
        {
            return Map[charCode];
        }
    }
}
