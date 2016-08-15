﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Items
{
    public class ItemClass : IItemClass
    {
        public string Name { get; set; }
        public ISprite Sprite { get; set; }
        public IWeaponClass WeaponClass { get; set; }
        public IArmorClass ArmorClass { get; set; }
    }
}