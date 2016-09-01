using System.Collections.Generic;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Items
{
    public class ItemFactory : IItemFactory
    {
        public IItem Create(IItemClass itemClass)
        {
            return new Item(itemClass);
        }

        public IItem CreateShedLimb(IAgent agent, IBodyPart part)
        {
            return Create(new ItemClass(
                name: string.Format("{0}'s {1}", agent.Name, part.Name), 
                sprite: new Sprite(Symbol.CorpseBodyPart, Color.DarkGray, Color.Black), 
                size: part.Size,
                material: null, weaponClass: DefaultWeaponClass, 
                armorClass: null));
        }

        public IItem CreateCorpse(IAgent agent)
        {
            return Create(new ItemClass(
                name: string.Format("{0}'s corpse", agent.Name), 
                sprite: new Sprite(Symbol.Corpse, Color.DarkGray, Color.Black), 
                size: agent.Body.Size,
                material: null, 
                weaponClass: DefaultWeaponClass, 
                armorClass: null));
        }

        private static IWeaponClass DefaultWeaponClass = new WeaponClass(
            slots: new WeaponSlot[] { WeaponSlot.Main },
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
                               IsMartialArts = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true,
                               IsStrike = true
                           },

                    });
    }
}
