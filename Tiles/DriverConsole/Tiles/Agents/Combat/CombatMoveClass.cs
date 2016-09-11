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
        public int ContactArea { get; set; }
        public int MaxPenetration { get; set; }
        public int VelocityMultiplier { get; set; }

        public IEnumerable<IBodyPartRequirement> Requirements { get; set; }


        public bool MeetsRequirements(IBody body)
        {
            throw new NotImplementedException();
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
    }

    public class BodyPartRequirement : IBodyPartRequirement
    {
        public BodyPartRequirementType Type { get; set; }
        public IEnumerable<BprConstraint> Constraints { get; set; }
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
