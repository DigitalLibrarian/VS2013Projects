using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Math;

namespace Tiles.Items
{

    public interface IItemFactory
    {
        IItem Create(IItemClass itemClass);
        IItem CreateShedLimb(IAgent agent, IBodyPart part);
        IItem CreateCorpse(IAgent agent);
    }

    public class ItemFactory : IItemFactory
    {
        public IItem Create(IItemClass itemClass)
        {
            return new Item
            {
                Class = itemClass
            };
        }

        public IItem CreateShedLimb(IAgent agent, IBodyPart part)
        {
            return Create(new ItemClass
            {
                Name = string.Format("{0}'s {1}", agent.Name, part.Name),
                Sprite = new Sprite(Symbol.CorpseBodyPart, Color.DarkGray, Color.Black),
                WeaponClass = DefaultWeaponClass
            });
        }

        private static IWeaponClass DefaultWeaponClass = new WeaponClass(
                    name: "Strike",
                    sprite: null,
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
                                            { DamageType.Blunt, 1 },
                                            { DamageType.Burn, 1 }
                                        }
                                   )
                               )
                           {
                               IsMartialArts = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true,
                               IsStrike = true
                           },

                    });


        public IItem CreateCorpse(IAgent agent)
        {
            return Create(new ItemClass
            {
                Name = string.Format("{0}'s corpse", agent.Name),
                Sprite = new Sprite(Symbol.Corpse, Color.DarkGray, Color.Black)
            });
        }
    }
}
