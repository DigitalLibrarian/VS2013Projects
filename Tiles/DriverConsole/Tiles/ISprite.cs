using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles
{
    public interface ISprite
    {
        Symbol Symbol { get; set; }
        Color ForegroundColor { get; set; }
        Color BackgroundColor { get; set; }
    }

    public class Sprite : ISprite
    {
        public Symbol Symbol { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public Sprite(Symbol symbol, Color foregroundColor, Color backgroundColor)
        {
            Symbol = symbol;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }
}
