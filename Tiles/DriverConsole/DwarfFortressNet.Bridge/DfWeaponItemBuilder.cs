using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Agents.Combat;
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

            var moves = Weapon.Attacks.Select(attack => {

                var verb = new Verb(new Dictionary<VerbConjugation, string>{
                    {VerbConjugation.SecondPerson, attack.VerbSecondPerson},
                    {VerbConjugation.ThirdPerson, attack.VerbThirdPerson}
                }, false);
                var damageVector = new DamageVector();
                if (attack.AttackType == "BLUNT")
                {
                    damageVector.SetComponent(DamageType.Blunt, 20);
                }
                else if (attack.AttackType == "EDGE")
                {
                    damageVector.SetComponent(DamageType.Slash, 10);
                    damageVector.SetComponent(DamageType.Pierce, 10);
                }
                var moveClass = new CombatMoveClass(
                    attack.VerbSecondPerson, 
                    verb, 
                    damageVector,
                    attack.PrepTime,
                    attack.RecoveryTime)
                {
                    IsStrike = true,
                    IsMartialArts = true,
                    IsItem = true,
                    IsDefenderPartSpecific = true
                };
                return moveClass;
            });

            var weaponClass = new WeaponClass(name, sprite, weaponSlots, moves.ToArray());
            var itemClass = new ItemClass
            {
                Name = string.Format("{0} {1}",Inorganic.AllSolidAdjective, name),
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
