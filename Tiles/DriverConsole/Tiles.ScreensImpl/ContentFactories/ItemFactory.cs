﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;
using Tiles.Math;
using Tiles.Random;
using Tiles.Agents.Combat;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class ItemFactory
    {
        #region static defns
        private static IList<IWeaponClass> _WeaponClasses = new List<IWeaponClass>
        {
            new WeaponClass("baseball bat", 
                new Sprite(Symbol.MeleeClub, Color.White, Color.Black), 
                new DamageVector(
                    new Dictionary<DamageType,uint>{
                        { DamageType.Blunt, 25 }
                    }
               ),
               "bashes",
                WeaponSlot.Main
            ),
            new WeaponClass("iron scimitar", 
                new Sprite(Symbol.MeleeSword, Color.White, Color.Black), 
                new DamageVector(
                    new Dictionary<DamageType,uint>{
                        { DamageType.Slash, 36},
                        { DamageType.Pierce, 22}
                    }
                ),
                "slashes",
                WeaponSlot.Main
            ),
        };

        private static IList<IArmorClass> _ArmorClasses = new List<IArmorClass>
        {
            new ArmorClass("cotton hat",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 25},
                    { DamageType.Pierce, 1},
                    { DamageType.Blunt, 1},
                }),
                ArmorSlot.Head
            ),
            new ArmorClass("leather arm pad (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.LeftArm
            ),
            new ArmorClass("leather arm pad (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.RightArm
            ),
            new ArmorClass("leather leg pad (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.LeftLeg
            ),
            new ArmorClass("leather leg pad (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.RightLeg
            ),            
            new ArmorClass("leather vest",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 95},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.Torso
            ),
            new ArmorClass("leather cap",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 75},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.Head
            ),
            new ArmorClass("leather shoe (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 90},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.RightFoot
            ),
            new ArmorClass("leather shoe (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 90},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.LeftFoot
            ),
            new ArmorClass("leather glove (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 45},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.RightHand
            ),
            new ArmorClass("leather glove (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 45},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.LeftHand
            ),
            new ArmorClass("leather trenchcoat",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 45},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                }),
                ArmorSlot.Torso,
                ArmorSlot.LeftArm,
                ArmorSlot.RightArm,
                ArmorSlot.LeftLeg,
                ArmorSlot.RightLeg
            ),
        };
        #endregion
        
        public IEnumerable<IItem> CreateAllWeapons()
        {
            foreach (var weaponClass in _WeaponClasses)
            {
                yield return new WeaponItem(weaponClass);
            }
        }

        public IEnumerable<IItem> CreateAllArmors()
        {
            foreach (var armorClass in _ArmorClasses)
            {
                yield return new ArmorItem(armorClass);
            }
        }


        public IItem CreateRandomItem(IRandom random)
        {
            var wc = _WeaponClasses.Count();
            var ac = _ArmorClasses.Count();
            var tot = wc + ac;

            var index = random.Next(tot);

            if (index < wc)
            {
                return new WeaponItem(_WeaponClasses[index]);
            }
            else
            {
                return new ArmorItem(_ArmorClasses[index - wc]);
            }
        }
    }
}
