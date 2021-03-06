﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Items
{
    public interface IItemClass
    {
        string Name { get; }
        /// <summary>
        /// Volume in cm^3
        /// </summary>
        double Size { get; }
        Sprite Sprite { get; }
        IWeaponClass WeaponClass { get; }
        IArmorClass ArmorClass { get; }

        IMaterial Material{ get;}
    }
}
