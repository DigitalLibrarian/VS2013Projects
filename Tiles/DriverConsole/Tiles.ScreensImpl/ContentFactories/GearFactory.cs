using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Items;
using Tiles.Math;
using Tiles.Random;
using Tiles.Agents.Combat;
using Tiles.Materials;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class GearFactory
    {
        #region static defns
        static IMaterial Leather = new Material("leather", "leather");
        static IMaterial Plastic = new Material("plastic", "plastic");
        static IMaterial Steel = new Material("steel", "steel");
        static IMaterial Wood = new Material("wood", "wood");
        static IMaterial Cotton = new Material("cotton", "cotton");
        private static IList<IItemClass> _ItemClasses = new List<IItemClass>
        {
            new ItemClass(
                name: "plastic flyswatter", 
                sprite: new Sprite(Symbol.MeleeClub, Color.White, Color.Black), 
                size: 100,
                material: Plastic,
                weaponClass:
                new WeaponClass(
                   slots: new WeaponSlot[] { WeaponSlot.Main },
                   attackMoveClasses: new ICombatMoveClass[] {
                       new CombatMoveClass(
                           name: "Swat",
                           meleeVerb: new Verb(
                                new Dictionary<VerbConjugation, string>()
                                {
                                    { VerbConjugation.FirstPerson, "swat"},
                                    { VerbConjugation.SecondPerson, "swat"},
                                    { VerbConjugation.ThirdPerson, "swats"},
                                }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Blunt, 1 }
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           ){
                           IsDefenderPartSpecific = true,
                           IsMartialArts = true,
                           IsStrike = true,
                           IsItem = true,
                       },
                       new CombatMoveClass(
                           name: "Swish",
                           meleeVerb: new Verb(
                                new Dictionary<VerbConjugation, string>()
                                {
                                    { VerbConjugation.FirstPerson, "swish"},
                                    { VerbConjugation.SecondPerson, "swish"},
                                    { VerbConjugation.ThirdPerson, "swishes"},
                                }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Slash, 1 }
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           ){
                           IsDefenderPartSpecific = true,
                           IsMartialArts = true,
                           IsStrike = true,
                           IsItem = true,
                       },
                   }
                )),
            new ItemClass(
               name: "steel sword", 
               sprite: new Sprite(Symbol.MeleeSword, Color.White, Color.Black), 
               material: Steel,
               size: 300,
               weaponClass: 
                new WeaponClass(
                   slots: new WeaponSlot[] { WeaponSlot.Main },
                   attackMoveClasses: new ICombatMoveClass[] {
                       new CombatMoveClass(
                           name: "Slash",
                           meleeVerb: new Verb(
                                new Dictionary<VerbConjugation, string>()
                                {
                                    { VerbConjugation.FirstPerson, "slash"},
                                    { VerbConjugation.SecondPerson, "slash"},
                                    { VerbConjugation.ThirdPerson, "slashes"},
                                }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Slash, 55 }
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           ){
                           IsDefenderPartSpecific = true,
                           IsMartialArts = true,
                           IsStrike = true,
                           IsItem = true,
                       },
                       new CombatMoveClass(
                           name: "Stab",
                           meleeVerb: new Verb(
                                new Dictionary<VerbConjugation, string>()
                                {
                                    { VerbConjugation.FirstPerson, "stab"},
                                    { VerbConjugation.SecondPerson, "stab"},
                                    { VerbConjugation.ThirdPerson, "stabs"},
                                }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Pierce, 63 },
                                        { DamageType.Slash, 11 }
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           ){
                           IsDefenderPartSpecific = true,
                           IsMartialArts = true,
                           IsStrike = true,
                           IsItem = true,
                       },
                   }
                )),
            new ItemClass(
                name: "baseball bat", 
                sprite: new Sprite(Symbol.MeleeClub, Color.White, Color.Black),
                size: 300,
                material: Wood,
                weaponClass:
                new WeaponClass(
                    slots: new WeaponSlot[] { WeaponSlot.Main },
                    attackMoveClasses: new ICombatMoveClass[] {
                       new CombatMoveClass(
                           name: "Bash",
                           meleeVerb: new Verb(
                                new Dictionary<VerbConjugation, string>()
                                {
                                    { VerbConjugation.FirstPerson, "bash"},
                                    { VerbConjugation.SecondPerson, "bash"},
                                    { VerbConjugation.ThirdPerson, "bashes"},
                                }, true),
                           damage: new DamageVector(
                                    new Dictionary<DamageType,uint>{
                                        { DamageType.Blunt, 25 }
                                    }
                               ),
                          prepTime: 1,
                          recoveryTime: 1
                           ){
                           IsDefenderPartSpecific = true,
                           IsMartialArts = true,
                           IsStrike = true,
                           IsItem = true,
                       }
                   }
                )),
            new ItemClass(
                name: "cotton hat",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Cotton,
                armorClass: new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 25},
                            { DamageType.Pierce, 1},
                            { DamageType.Blunt, 1},
                        }),
                        ArmorSlot.Head
                    )),
            new ItemClass(
                name: "leather arm pad (L)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass:
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 80},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.LeftArm
                    )),
            new ItemClass(
                name: "leather arm pad (R)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass:
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 80},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.RightArm
                    )),
            new ItemClass(
                name: "leather leg pad (L)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass:
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 80},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.LeftLeg
                    )),
            new ItemClass(
                name: "leather leg pad (R)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass:
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 80},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.RightLeg
                    )),
            new ItemClass(
                name: "leather vest",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 95},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.Torso
                    )),
            new ItemClass(
                name: "leather cap",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 75},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.Head
                    )),
            new ItemClass(
                name: "leather shoe (R)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 90},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.RightFoot
                    )),
            new ItemClass(
                name: "leather shoe (L)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 90},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.LeftFoot
                    )),
            new ItemClass(
                name: "leather glove (R)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
                        new DamageVector(new Dictionary<DamageType,uint>{
                            { DamageType.Slash, 45},
                            { DamageType.Pierce, 7},
                            { DamageType.Blunt, 2},
                        }),
                        ArmorSlot.RightHand
                    )),
            new ItemClass(
                name: "leather glove (L)",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                new ArmorClass(    
                    new DamageVector(new Dictionary<DamageType,uint>{
                        { DamageType.Slash, 45},
                        { DamageType.Pierce, 7},
                        { DamageType.Blunt, 2},
                    }),
                    ArmorSlot.LeftHand
                )),
            new ItemClass(
                name: "leather trenchcoat",
                sprite: new Sprite(Symbol.MiscClothing, Color.White, Color.Black),
                size: 10,
                material: Leather,
                armorClass: 
                    new ArmorClass(
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
                    )),
        };
        private IRandom Random;

        public GearFactory(IRandom Random)
        {
            // TODO: Complete member initialization
            this.Random = Random;
        }
        #endregion

        IItemFactory ItemFactory = new ItemFactory();
        
        

        static IWeaponClass DefaultWeaponClass = new WeaponClass(
            slots: new WeaponSlot[] {  WeaponSlot.Main},
            attackMoveClasses: new ICombatMoveClass[] { 
                   new CombatMoveClass(
                       name: "Strike",
                       meleeVerb: new Verb(
                            new Dictionary<VerbConjugation, string>()
                            {
                                { VerbConjugation.FirstPerson, "strike"},
                                { VerbConjugation.SecondPerson, "strike"},
                                { VerbConjugation.ThirdPerson, "strikes"},
                            }, true),
                       damage: new DamageVector(
                                new Dictionary<DamageType,uint>{
                                    { DamageType.Slash, 1 },
                                    { DamageType.Pierce, 1 },
                                    { DamageType.Blunt, 1 }
                                }
                           ),
                          prepTime: 1,
                          recoveryTime: 1
                       )
                        {
                       IsDefenderPartSpecific = true,
                       IsMartialArts = true,
                       IsStrike = true,
                       IsItem = true,
                   },

            });

        IItem CreateItem(
            string name, ISprite sprite, IItemClass itemClass)
        {
            return ItemFactory.Create(itemClass);
        }
        public IItem CreateRandomItem()
        {
            return ItemFactory.Create(Random.NextElement(_ItemClasses));
        }
    }
}
