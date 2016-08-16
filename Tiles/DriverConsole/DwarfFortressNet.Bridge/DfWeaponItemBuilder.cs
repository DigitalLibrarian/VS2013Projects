using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Items;
using Tiles.Math;
using Df = DwarfFortressNet.RawModels;

namespace DwarfFortressNet.Bridge
{
    public class DfWeaponItemBuilder
    {
        Df.ItemWeapon Weapon { get; set; }
        Df.Inorganic Inorganic { get; set; }
        ObjectDb Db { get; set; }

        public DfWeaponItemBuilder(Df.ItemWeapon weapon, Df.Inorganic inorganic, ObjectDb db)
        {
            Weapon = weapon;
            Inorganic = inorganic;
            Db = db;
        }

        public IItem Build()
        {
            var matBuilder = new DfMaterialBuilder(Inorganic, Db);
            var material = matBuilder.Build();

            var name = Weapon.Name;
            var sprite = new Sprite(Symbol.MeleeSword, Color.White, Color.Black);

            var weaponSlots = new WeaponSlot[]{WeaponSlot.Main};

            var weaponClass = new WeaponClass(name, sprite, weaponSlots, new Tiles.Agents.Combat.ICombatMoveClass[0]);


            var itemClass = new ItemClass
            {
                Name = name,
                Sprite = sprite,
                Material = material,
                WeaponClass = weaponClass
            };

            return new Item
            {
                Class = itemClass
            };
        }


        public static IItem FromDefinition(Df.Inorganic inorg, Df.ItemWeapon weapon, ObjectDb objDb)
        {
            var builder = new DfWeaponItemBuilder(weapon, inorg, objDb);
            return builder.Build();
        }
    }
}
