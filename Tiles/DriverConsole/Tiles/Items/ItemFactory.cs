using System.Collections.Generic;
using System.Linq;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;
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
            IMaterial material = null;
            var layer = part.Tissue.TissueLayers.FirstOrDefault();
            if (layer != null)
            {
                material = layer.Material;
            }

            return Create(new ItemClass(
                name: string.Format("{0}'s {1}", agent.Name, part.Name), 
                sprite: new Sprite(Symbol.CorpseBodyPart, Color.DarkGray, Color.Black), 
                size: part.Size,
                material: material, 
                weaponClass: DefaultWeaponClass, 
                armorClass: null));
        }

        public IItem CreateCorpse(IAgent agent)
        {
            IMaterial material = null;
            var part = agent.Body.Parts.FirstOrDefault();
            if (part != null)
            {
                var layer = part.Tissue.TissueLayers.FirstOrDefault();
                if (layer != null)
                {
                    material = layer.Material;
                }
            }

            return Create(new ItemClass(
                name: string.Format("{0}'s corpse", agent.Name), 
                sprite: new Sprite(Symbol.Corpse, Color.DarkGray, Color.Black), 
                size: agent.Body.Size,
                material: material, 
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
                               prepTime: 1,
                               recoveryTime: 1
                               )
                           {
                               IsMartialArts = true,
                               IsDefenderPartSpecific = true,
                               IsItem = true,
                               IsStrike = true
                           },
                    },
            minimumSize: 10000);
    }
}
