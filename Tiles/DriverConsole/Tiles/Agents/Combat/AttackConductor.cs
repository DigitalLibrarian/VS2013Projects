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
            if (move.AttackMoveClass.IsMeleeStrike)
            {
                ConductMelee(attacker, defender, move);
            }
            else if (attacker.Body.IsGrasping)
            {
                ConductWrestling(attacker, defender, move);
            }

            if(move.AttackMoveClass.IsGraspPart)
            {
                ConductGrasp(attacker, defender, move);
            }

        }

        private void ConductWrestling(IAgent attacker, IAgent defender, IAttackMove move)
        {
            var dmg = move.PredictedDamage;
            move.DefenderBodyPart.Health.TakeDamage(dmg);

            var limbMessage = ".";
            var shedPart = defender.Body.DamagePart(move.DefenderBodyPart, dmg);
            if (shedPart != null)
            {
                var newItems = CreateShedBodyPart(defender, shedPart);
                move.AttackerBodyPart.StopGrasp(shedPart);
                HandleDamageProducts(attacker, defender, newItems, move);
                limbMessage = " and it comes off!";
            }

            if (defender.IsDead)
            {
                HandleDeath(attacker, defender);
            }

            if (move.AttackMoveClass.IsReleasePart)
            {
                move.AttackerBodyPart.StopGrasp(move.DefenderBodyPart);
            }

            Log.AddLine(string.Format("{0} {1} the {2}'s {3}{4}", 
                AttackerName(attacker),
                Verb(attacker, move.AttackMoveClass.Verb),
                defender.Name,
                move.DefenderBodyPart.Name,
                limbMessage
                ));
        }


        private void ConductGrasp(IAgent attacker, IAgent defender, IAttackMove move)
        {
            var attackerName = AttackerName(attacker);
            var attackerPoss = Possessive(attacker);
            var defenderPoss = Possessive(defender);
            if (CompassVectors.IsCompassVector(attacker.Pos - defender.Pos) && !move.DefenderBodyPart.IsWrestling)
            {
                move.AttackerBodyPart.StartGrasp(move.DefenderBodyPart);
                var verb = Verb(attacker, move.AttackMoveClass.Verb);
                Log.AddLine(string.Format("{0} {1} the {2}'s {3}.", attackerName, verb, move.Defender.Name, move.DefenderBodyPart.Name));
            }
            else
            {
                Log.AddLine(string.Format("{0} lunges for the {1} but misses.", attackerName, move.Defender.Name));
            }
        }

        void ConductMelee(IAgent attacker, IAgent defender, IAttackMove move)
        {
            var totalDamage = move.PredictedDamage;
            var shedPart = defender.Body.DamagePart(move.DefenderBodyPart, totalDamage);
            var newItems = new List<IItem>();
            if (shedPart != null)
            {
                if (shedPart.IsGrasping)
                {
                    shedPart.StopGrasp(shedPart.Grasped);
                }
                newItems.AddRange(CreateShedBodyPart(defender, shedPart));
            }

            string severed = "";
            bool defenderDies = defender.IsDead;
            // TODO - this needs to be handled more ubiquitously, even outside the combat case
            // perhaps an AgentReaper that visits every agent at death, for housekeeping.
            if (defenderDies)
            {
                HandleDeath(attacker, defender);
            }

            if (newItems.Any())
            {
                HandleDamageProducts(attacker, defender, newItems, move);
                severed = " and the severed part drops away";
            }

            bool secondPerson = attacker.IsPlayer;

            var verb = move.AttackMoveClass.Verb.Conjugate(secondPerson ? VerbConjugation.SecondPerson : VerbConjugation.ThirdPerson);

            var attackerPronoun = "its";
            var attackerName = string.Format("The {0}", attacker.Name);
            if (secondPerson)
            {
                attackerName = "You";
                attackerPronoun = "your";
            }

            var withWeapon = "";
            if (move.Weapon != null)
            {
                withWeapon = string.Format(" with {0} {1}",attackerPronoun, move.Weapon.Name);
            }

            Log.AddLine(string.Format("{0} {1} the {2}'s {3}{4} for {5} damage{6}.", attackerName, verb, defender.Name, move.DefenderBodyPart.Name, withWeapon, totalDamage, severed));

            if (defenderDies)
            {
                Log.AddLine(string.Format("The {0} is killed!", defender.Name));
            }
        }


        void HandleDeath(IAgent attacker, IAgent defender)
        {
            foreach (var part in defender.Body.Parts)
            {
                if (part.IsGrasping)
                {
                    part.StopGrasp(part.Grasped);
                }

                foreach (var attPart in attacker.Body.Parts.Where(x => x.Grasped == part))
                {
                    attPart.StopGrasp(part);
                }
            }
            var tile = Atlas.GetTileAtPos(defender.Pos);
            tile.RemoveAgent();
            foreach (var item in CreateCorpse(defender))
            {
                tile.Items.Add(item);
            }
        }

        void HandleDamageProducts(IAgent attacker, IAgent defender, IEnumerable<IItem> items, IAttackMove move)
        {
            var tile = Atlas.GetTileAtPos(defender.Pos);
            foreach (var newItem in items)
            {
                if (move.AttackMoveClass.TakeDamageProducts)
                {
                    attacker.Inventory.AddItem(newItem);
                }
                else
                {
                    tile.Items.Add(newItem);
                }
            }
        }


        string AttackerName(IAgent attacker)
        {
            if (attacker.IsPlayer)
            {
                return "You";
            }
            else
            {
                return string.Format("The {0}", attacker.Name);
            }
        }


        string Possessive(IAgent attacker)
        {
            if (attacker.IsPlayer)
            {
                return "your";
            }
            else
            {
                return "its";
            }
        }

        string Verb(IAgent a, IVerb verb)
        {
            return verb.Conjugate(a.IsPlayer ? VerbConjugation.SecondPerson : VerbConjugation.ThirdPerson);
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
                    attackMoveClasses: new IAttackMoveClass[] { 
                           new AttackMoveClass(
                               name: "Strike",
                               meleeVerb: new Verb(
                                   firstPerson: "strike",
                                   secondPerson: "strike",
                                   thirdPerson: "strikes"
                                   ),
                               damage: new DamageVector(
                                        new Dictionary<DamageType,uint>{
                                            { DamageType.Slash, 1 },
                                            { DamageType.Pierce, 1 },
                                            { DamageType.Blunt, 1 },
                                            { DamageType.Burn, 1 }
                                        }
                                   )
                               ),

                    });

    }
}
