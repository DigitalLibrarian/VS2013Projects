using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Math
{
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public Color(byte r, byte g, byte b, byte a) : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.R == right.R
                && left.G == right.G
                && left.B == right.B
                && left.A == right.A;
        }

        static Color()
        {
            Black = new Color(0, 0, 0, 0xff);
            DarkBlue = new Color(0, 0, 0x8b, 0xff);
            DarkGreen = new Color(0, 0x64, 0, 0xff);
            DarkCyan = new Color(0, 0x8b, 0x8b, 0xff);
            DarkRed = new Color(0x8b, 0, 0, 0xff);
            DarkMagenta = new Color(0x8b, 0, 0x8b, 0xff);
            DarkYellow = new Color(128, 128, 0, 0xff);
            Gray = new Color(0x80, 0x80, 0x80, 0xff);
            DarkGray = new Color(0xa9, 0xa9, 0xa9, 0xff);
            Blue = new Color(0, 0, 0xff, 0xff);
            Green = new Color(0, 0x80, 0, 0xff);
            Cyan = new Color(0, 0xff, 0xff, 0xff);
            Red = new Color(0xff, 0, 0, 0xff);
            Magenta = new Color(0xff, 0, 0xff, 0xff);
            Yellow = new Color(0xff, 0xff, 0, 0xff);
            White = new Color(0xff, 0xff, 0xff, 0xff);

        }
        
        public static Color Black { get; private set; }
        public static Color DarkBlue { get; private set; }
        public static Color DarkGreen { get; private set; }
        public static Color DarkCyan { get; private set; }
        public static Color DarkRed { get; private set; }
        public static Color DarkMagenta { get; private set; }
        public static Color DarkYellow { get; private set; }
        public static Color Gray { get; private set; }
        public static Color DarkGray { get; private set; }
        public static Color Blue { get; private set; }
        public static Color Green { get; private set; }
        public static Color Cyan { get; private set; }
        public static Color Red { get; private set; }
        public static Color Magenta { get; private set; }
        public static Color Yellow { get; private set; }
        public static Color White { get; private set; }

    }
}
