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
        public static readonly int None = 32;
        public static readonly int Terrain_Empty = 32;
        public static readonly int Terrain_Tree = 15;
        public static readonly int Terrain_Floor = 32;

        public static readonly int Liquid_Light = 176;
        public static readonly int Liquid_Medium = 177;
        public static readonly int Liquid_Dark = 178;

        public static readonly int Player = 1;
        public static readonly int Survivor = 2;
        public static readonly int Zombie = 128;

        public static readonly int Wall_BottomLeft_L_Hollow = 200;
        public static readonly int Wall_BottomRight_L_Hollow = 188;
        public static readonly int Wall_TopLeft_L_Hollow = 201;
        public static readonly int Wall_TopRight_L_Hollow = 187;
        public static readonly int Wall_Vertical_Hollow = 186;
        public static readonly int Wall_Horizontal_Hollow = 205;

        public static readonly int MeleeClub = 124;
        public static readonly int MeleeSword = 92;

        public static readonly int MiscClothing = 157;
        public static readonly int MiscItem = 232;
        public static readonly int MiscWeapon = 173;
        public static readonly int Corpse = 234;
        public static readonly int CorpseBodyPart = 233;
        
        public static int LiquidDepth(int depth)
        {
            return 0x30 + depth;
        }
    }
}
