using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Math;

namespace Tiles.Agents.Combat
{
    public class AttackConductor : IAttackConductor
    {
        IAtlas Atlas { get; set; }
        IActionLog Log { get; set; }

        public AttackConductor(IAtlas atlas, IActionLog log)
        {
            Atlas = atlas;
            Log = log;
        }

        public void Conduct(IAgent attacker, IAgent defender, IAttackMove move)
        {
            var totalDamage = move.CalculatedDamage;
            var shedPart = defender.Body.DamagePart(move.TargetBodyPart, totalDamage);
            var newItems = new List<IItem>();
            if (shedPart != null)
            {
                newItems.AddRange(CreateShedBodyPart(defender, shedPart));
            }

            string severed = "";
            bool defenderDies = defender.IsDead;
            // TODO - this needs to be handled more ubiquitously, even outside the combat case
            if (defenderDies)
            {
                var tile = Atlas.GetTileAtPos(defender.Pos);
                tile.RemoveAgent();
                newItems.AddRange(CreateCorpse(defender));
            }

            if (newItems.Any())
            {
                var tile = Atlas.GetTileAtPos(defender.Pos);
                foreach (var newItem in newItems)
                {
                    tile.Items.Add(newItem);
                }
                severed = " and the severed part drops away";
            }

            var verb = "attacks";
            if (move.Verb != null)
            {
                verb = move.Verb;
            }

            var withWeapon = "";
            if (move.Weapon != null)
            {
                withWeapon = string.Format(" with its {0}", move.Weapon.WeaponClass.Name);
            }

            Log.AddLine(string.Format("The {0} {1} the {2}'s {3}{4} for {5} damage{6}.", attacker.Name, verb, defender.Name, move.TargetBodyPart.Name, withWeapon, totalDamage, severed));

            if (defenderDies)
            {
                Log.AddLine(string.Format("The {0} is killed!", defender.Name));
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

            yield return new Item
            {
                Name = string.Format("{0}'s {1}", defender.Name, shedPart.Name),
                Sprite = new Sprite(Symbol.CorpseBodyPart, Color.DarkGray, Color.Black)
            };

        }
    }
}
