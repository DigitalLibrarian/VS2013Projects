using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Agents.Combat
{
    public class CombatMoveClass : ICombatMoveClass
    {
        public CombatMoveClass(string name, 
            IVerb meleeVerb, 
            int prepTime, int recoveryTime)
        {
            Name = name;
            Verb = meleeVerb;

            PrepTime = prepTime;
            RecoveryTime = recoveryTime;

            Requirements = new List<IBodyPartRequirement>();
            VelocityMultiplier = 1000;
        }


        // New stuff
        public string Name { get; set; }
        public IVerb Verb { get; set; }
        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }
        
        public BodyStateChange AttackerBodyStateChange { get; set; }

        public bool IsDefenderPartSpecific { get; set; }

        public bool IsMartialArts { get; set; }

        public bool IsStrike { get; set; }

        public bool IsItem { get; set; }

        public StressMode StressMode { get; set; }
        public double ContactArea { get; set; }
        public double MaxPenetration { get; set; }
        public int VelocityMultiplier { get; set; }

        public IEnumerable<IBodyPartRequirement> Requirements { get; set; }

        public bool MeetsRequirements(IBody body)
        {
            return Requirements.All(req => req.IsMet(body));
        }


        public IEnumerable<IBodyPart> GetRelatedBodyParts(IBody body)
        {
            return Requirements.SelectMany(r => r.FindParts(body));
        }
    }

    public enum BodyPartRequirementType
    {
        BodyPart,
        ChildTissueLayerGroup,
        ChildBodyPartGroup
    }

    public interface IBodyPartRequirement
    {
        BodyPartRequirementType Type { get; }
        IEnumerable<BprConstraint> Constraints { get; }

        bool IsMet(IBody body);
        IEnumerable<IBodyPart> FindParts(IBody body);
    }

    public class BodyPartRequirement : IBodyPartRequirement
    {
        public BodyPartRequirementType Type { get; set; }
        public IEnumerable<BprConstraint> Constraints { get; set; }

        bool IsConstraintMatch(BprConstraint con, IBodyPart part)
        {
            var checkSet = new List<string>();
            switch (con.ConstraintType)
            {
                case BprConstraintType.ByCategory:
                    checkSet = part.Class.Categories.ToList();
                    break;
                case BprConstraintType.ByType:
                    checkSet = part.Class.Types.ToList();
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var token in con.Tokens)
            {
                if (!checkSet.Contains(token))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<IBodyPart> FindParts(IBody body)
        {
            BprConstraint pConstraint = null;
            foreach (var part in body.Parts)
            {
                switch (Type)
                {
                    case BodyPartRequirementType.BodyPart:
                        if (Constraints.All(c => IsConstraintMatch(c, part)))
                        {
                            return new List<IBodyPart>() { part };
                        }
                        break;
                    case BodyPartRequirementType.ChildBodyPartGroup:
                        pConstraint = Constraints.First();
                        if (IsConstraintMatch(pConstraint, part))
                        {
                            var children = body.Parts.Where(p => p.Parent == part);
                            return children.Where(p =>
                                Constraints.Skip(1).All(con => IsConstraintMatch(con, p)));
                        }
                        break;
                    case BodyPartRequirementType.ChildTissueLayerGroup:
                        pConstraint = Constraints.First();
                        if (IsConstraintMatch(pConstraint, part))
                        {
                            var children = body.Parts.Where(p => p.Parent == part);
                            return children.Where(p =>
                                Constraints.Skip(1).All(con => IsConstraintMatch(con, p)));
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return Enumerable.Empty<IBodyPart>();
        }

        public bool IsMet(IBody body)
        {
            return FindParts(body).Any();
        }
    }


    public enum BprConstraintType
    {
        ByCategory,
        ByType
    }
    public class BprConstraint
    {
        public BprConstraintType ConstraintType { get; set; }
        public List<string> Tokens { get; set; }

        public BprConstraint(BprConstraintType bacType)
        {
            ConstraintType = bacType;
            Tokens = new List<string>();
        }
    }
}
