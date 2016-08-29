using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Sprite
    {
        public int Symbol { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }

        public Sprite(int symbol, Color fg, Color bg)
        {
            Symbol = symbol;
            Foreground = fg;
            Background = bg;
        }
    }
}
