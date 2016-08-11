using System;
using System.Collections.Generic;
using Tiles.Bodies;
using Tiles.Items;
namespace Tiles.Agents.Combat
{
    public interface IAttackMove
    {
        IAgent Attacker { get; set; }
        uint CalculatedDamage { get; set; }
        IAgent Defender { get; set; }
        bool IsCritical { get; set; }
        string Name { get; set; }
        IBodyPart TargetBodyPart { get; set; }
        IWeapon Weapon { get; set; }

        IAttackMoveClass AttackMoveClass { get; }
    }

    public enum VerbConjugation
    {
        FirstPerson,
        SecondPerson,
        ThirdPerson
    }
    public interface IVerb
    {
        string Conjugate(VerbConjugation con);
    }

    public class Verb : IVerb
    {
        Dictionary<VerbConjugation, string> Conjugations { get; set; }

        public Verb(string firstPerson, string secondPerson, string thirdPerson)
        {
            Conjugations = new Dictionary<VerbConjugation, string>
            {
                { VerbConjugation.FirstPerson , firstPerson},
                { VerbConjugation.SecondPerson , secondPerson },
                { VerbConjugation.ThirdPerson , thirdPerson }
            };
        }

        public string Conjugate(VerbConjugation vc)
        {
            if (Conjugations.ContainsKey(vc))
            {
                return Conjugations[vc];
            }
            throw new MissingVerbConjugationException(this, vc);
        }
    }

    public class MissingVerbConjugationException : Exception
    {
        public IVerb Verb { get; private set; }
        public VerbConjugation VerbConjugation { get; private set; }

        public MissingVerbConjugationException(IVerb verb, VerbConjugation vc) : base(string.Format("Missing verb conjugation {0} for {1}", vc, verb))
        {
            Verb = verb;
            VerbConjugation = vc;
        }
    }

    public interface IAttackMoveClass
    {
        string Name { get; }
        IVerb MeleeVerb { get; }
        DamageVector DamageVector { get; }
    }

    public class AttackMoveClass : IAttackMoveClass
    {
        public string Name { get; private set; }
        public IVerb MeleeVerb { get; private set; }
        public DamageVector DamageVector { get; private set; }

        public AttackMoveClass(string name, IVerb meleeVerb, DamageVector damage)
        {
            Name = name;
            MeleeVerb = meleeVerb;
            DamageVector = damage;
        }
    }


}
