using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents
{
    public class AgentReaper : IAgentReaper
    {
        IAtlas Atlas { get; set; }

        public IEnumerable<IItem> Reap(IAgent agent)
        {
            foreach (var bodyPart in agent.Body.Parts)
            {
                ClearGrasps(bodyPart);
            }
            Atlas.GetTileAtPos(agent.Pos).RemoveAgent();
            return CreateCorpse(agent);
        }

        public IEnumerable<IItem> Reap(IAgent agent, IBodyPart bodyPart)
        {
            ClearGrasps(bodyPart);
            return CreateShedBodyPart(agent, bodyPart);
        }

        void ClearGrasps(IBodyPart part)
        {
            if (part.IsBeingGrasped)
            {
                part.Grasper.StopGrasp(part);
            }

            if (part.IsGrasping)
            {
                part.StopGrasp(part.Grasped);
            }
        }

        IEnumerable<IItem> CreateCorpse(IAgent defender)
        {
            var items = defender.Inventory.GetItems().Concat(defender.Inventory.GetWorn()).ToList();
            foreach (var item in items)
            {
                defender.Inventory.RemoveItem(item);
                yield return item;
            }

            yield return new Item
            {
                Name = string.Format("{0}'s corpse", defender.Name),
                Sprite = new Sprite(Symbol.Corpse, Color.DarkGray, Color.Black)
            };
        }
        IEnumerable<IItem> CreateShedBodyPart(IAgent defender, IBodyPart shedPart)
        {
            if (shedPart.Weapon != null)
            {
                var weaponItem = defender.Inventory.GetWorn(shedPart.Weapon);
                if (weaponItem != null)
                {
                    defender.Inventory.RemoveItem(weaponItem);
                    yield return weaponItem;
                }
            }

            if (shedPart.Armor != null)
            {
                var armorItem = defender.Inventory.GetWorn(shedPart.Armor);
                if (armorItem != null)
                {
                    defender.Inventory.RemoveItem(armorItem);
                    yield return armorItem;
                }
            }

            yield return CreateShedLimbItem(defender, shedPart);

        }

        IItem CreateShedLimbItem(IAgent defender, IBodyPart part)
        {
            return new Item
            {
                Name = string.Format("{0}'s {1}", defender.Name, part.Name),
                Sprite = new Sprite(Symbol.CorpseBodyPart, Color.DarkGray, Color.Black),
                WeaponClass = DefaultWeaponClass
            };
        }

        // TODO - create factory interface
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
                               IsMartialArts = true
                           },

                    });

    }
}
