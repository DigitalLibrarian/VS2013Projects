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
    public interface IAttackConductor
    {
        void Conduct(IAgent attacker, IAgent defender, IAttackMove move);
    }

    public enum BodyStateChange
    {
        None,
        StartHold,
        ReleaseHold,
        BreakHold
    }

    public interface ICombatMoveClass
    {
        BodyStateChange AttackerBodyStateChange { get; }

        bool IsDefenderPartSpecific { get; }
        bool IsMartialArts { get; }
        bool IsStrike { get; }
        bool IsItem { get; }

        IVerb Verb { get; }

        //bool IsRanged { get; }
        //bool IsOffensive { get; }
        //bool IsDefensive { get; }

        DamageVector Damage { get; }
    }

    public interface ICombatMove
    {
        ICombatMoveClass Class { get; }

        string Name { get; set; }

        IAgent Attacker { get; set; }
        IAgent Defender { get; set; }

        IBodyPart AttackerBodyPart { get; set; }
        IBodyPart DefenderBodyPart { get; set; }
        IItem Item { get; set; }

        uint PredictedDamage { get; set; }
    }

    public class AttackConductor_New
    {
        IList<ICombatEvolution> Evolutions { get; set; }

        public AttackConductor_New(IList<ICombatEvolution> evolutions)
        {
            Evolutions = evolutions;
        }

        public void Conduct(IAgent attacker, IAgent defender, ICombatMove move)
        {
            var context = new CombatSession{
                Move = move,
                Attacker = attacker,
                Defender = defender
            };

            foreach (var evo in Evolutions)
            {
                evo.Evolve(context);
            }
        }
    }

    public interface IAgentReaper
    {
        IEnumerable<IItem> Reap(IAgent agent);
        IEnumerable<IItem> Reap(IAgent agent, IBodyPart bodyPart);
    }

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
                    attackMoveClasses: new IAttackMoveClass[] { 
                           new AttackMoveClass(
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
                               ),

                    });

    }

    public interface IActionReport
    {
        Vector2 WorldPos { get; }
        string Message { get; }
    }
    public class ActionReport : IActionReport
    {
        public Vector2 WorldPos { get; set; }
        public string Message { get; set; }
    }


    public interface IActionReporter
    {
        void ReportGrabStartBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportGrabMiss(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportGrabReleaseBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee);
        void ReportMeleeItemStrikeBodyPart(IAgent attacker, IAgent defender, IVerb verb, IItem item, IBodyPart bodyPart, uint dmg, bool targetPartWasShed);
        void ReportMeleeStrikeBodyPart(IAgent attacker, IAgent defender, IVerb verb, IBodyPart bodyPart, uint dmg, bool targetPartWasShed);
    }

    public class ActionReporter : IActionReporter
    {
        public IActionLog Log { get; private set; }
        public ActionReporter(IActionLog log)
        {
            Log = log;
        }

        public void ReportGrabStartBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            throw new NotImplementedException();
        }

        public void ReportGrabMiss(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            throw new NotImplementedException();
        }

        public void ReportGrabReleaseBodyPart(ICombatMoveContext session, IVerb verb, IBodyPart grasper, IBodyPart graspee)
        {
            throw new NotImplementedException();
        }

        public void ReportMeleeItemStrikeBodyPart(IAgent attacker, IAgent defender, IVerb verb, IItem item, IBodyPart bodyPart, uint dmg, bool targetPartWasShed)
        {
            throw new NotImplementedException();
        }

        public void ReportMeleeStrikeBodyPart(IAgent attacker, IAgent defender, IVerb verb, IBodyPart bodyPart, uint dmg, bool targetPartWasShed)
        {
            throw new NotImplementedException();
        }

    }
}
