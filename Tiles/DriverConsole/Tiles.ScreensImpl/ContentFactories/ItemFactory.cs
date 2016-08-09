using System;
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
                WeaponSlot.Main, 
                new DamageVector(
                    new Dictionary<DamageType,uint>{
                        { DamageType.Blunt, 25 }
                    }
               ),
               "bashes"
            ),
            new WeaponClass("iron scimitar", 
                new Sprite(Symbol.MeleeSword, Color.White, Color.Black),
                WeaponSlot.Main, 
                new DamageVector(
                    new Dictionary<DamageType,uint>{
                        { DamageType.Slash, 36},
                        { DamageType.Pierce, 22}
                    }
                ),
                "slashes"
            ),
        };

        private static IList<IArmorClass> _ArmorClasses = new List<IArmorClass>
        {
            new ArmorClass("cotton hat",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.Head,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 25},
                    { DamageType.Pierce, 1},
                    { DamageType.Blunt, 1},
                })
            ),
            new ArmorClass("leather arm pad (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.LeftArm,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather arm pad (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.RightArm,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather leg pad (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.LeftLeg,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather leg pad (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.RightLeg,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 80},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),            
            new ArmorClass("leather vest",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.Torso,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 95},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather cap",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.Head,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 75},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather shoe (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.RightFoot,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 90},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather shoe (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.LeftFoot,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 90},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather glove (R)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.RightHand,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 45},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
            ),
            new ArmorClass("leather glove (L)",
                new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                ArmorSlot.LeftHand,
                new DamageVector(new Dictionary<DamageType,uint>{
                    { DamageType.Slash, 45},
                    { DamageType.Pierce, 7},
                    { DamageType.Blunt, 2},
                })
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
