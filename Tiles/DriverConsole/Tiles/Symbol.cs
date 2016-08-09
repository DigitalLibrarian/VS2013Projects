using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles
{
    // TODO - should probably base this off the int code or something
    public enum Symbol
    {
        None = ' ',
        Terrain_Empty = ' ',
        Terrain_Tree = 'o',
        Terrain_Floor = '█',

        Liquid_Light = '░',
        Liquid_Medium = '▒',
        Liquid_Dark = '▓',

        Player = '☺',
        Survivor = '☻',
        Zombie = 'z',

        Wall_BottomLeft_L_Hollow = '╚',
        Wall_BottomRight_L_Hollow = '╝',
        Wall_TopLeft_L_Hollow = '╔',
        Wall_TopRight_L_Hollow = '╗',
        Wall_Vertical_Hollow = '║',
        Wall_Horizontal_Hollow = '═',
        Stone = '*',

        MeleeClub = '|',
        MeleeSword = '/',

        MiscClothing = '&',
        Corpse = 'Ċ',
        CorpseBodyPart = 'c'

    }
}
