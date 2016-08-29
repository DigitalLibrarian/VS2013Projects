using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfColorFactory : IDfColorFactory
    {
        static Color[] DullColors = new Color[]
        {
            new Color(0, 0, 0, 0xff),
            new Color(0, 0, 0x8b, 0xff),
            new Color(0, 0x64, 0, 0xff),
            new Color(0, 0x8b, 0x8b, 0xff),
            new Color(0x8b, 0, 0, 0xff),
            new Color(0x8b, 0, 0x8b, 0xff),
            new Color(128, 128, 0, 0xff),
            new Color(0x80, 0x80, 0x80, 0xff)
        };

        static Color[] BrightColors = new Color[]
        {
            new Color(0xa9, 0xa9, 0xa9, 0xff),
            new Color(0, 0, 0xff, 0xff),
            new Color(0, 0x80, 0, 0xff),
            new Color(0, 0xff, 0xff, 0xff),
            new Color(0xff, 0, 0, 0xff),
            new Color(0xff, 0, 0xff, 0xff),
            new Color(0xff, 0xff, 0, 0xff),
            new Color(0xff, 0xff, 0xff, 0xff)
        };

        public Color Create(int c, bool bright)
        {
            if (c > 7) throw new ArgumentException(string.Format("Unknown color index used {0}", c));

            if (bright)
            {
                return BrightColors[c];
            }
            else
            {
                return DullColors[c];
            }
        }
    }
}
