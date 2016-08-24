using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    // TODO - should probably base this off the int code or something
    public static class Symbol
    {
        public static readonly int None = ' ';
        public static readonly int Terrain_Empty = ' ';
        public static readonly int Terrain_Tree = 'o';
        public static readonly int Terrain_Floor = (int) ' ';

        public static readonly int Liquid_Light = '░';
        public static readonly int Liquid_Medium = '▒';
        public static readonly int Liquid_Dark = '▓';

        public static readonly int Player = '☺';
        public static readonly int Survivor = '☻';
        public static readonly int Zombie = 'z';

        public static readonly int Wall_BottomLeft_L_Hollow = '╚';
        public static readonly int Wall_BottomRight_L_Hollow = '╝';
        public static readonly int Wall_TopLeft_L_Hollow = '╔';
        public static readonly int Wall_TopRight_L_Hollow = '╗';
        public static readonly int Wall_Vertical_Hollow = '║';
        public static readonly int Wall_Horizontal_Hollow = '═';
        public static readonly int Stone = '*';

        public static readonly int MeleeClub = '|';
        public static readonly int MeleeSword = '/';

        public static readonly int MiscClothing = '&';
        public static readonly int Corpse = 'Ċ';
        public static readonly int CorpseBodyPart = 'c';

    }
}
