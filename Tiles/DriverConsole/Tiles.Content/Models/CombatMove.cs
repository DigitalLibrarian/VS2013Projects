﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class CombatMove
    {
        public CombatMove()
        {
            Verb = new Verb();
            Requirements = new List<BodyPartRequirement>();
        }
        public string Name { get; set; }
        public Verb Verb { get; set; }

        public BodyStateChange BodyStateChange { get; set; }

        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }

        public bool IsDefenderPartSpecific { get; set; }
        public bool IsMartialArts { get; set; }
        public bool IsStrike { get; set; }
        public bool IsItem { get; set; }
        public bool IsEdged { get; set; }

        public bool CanLatch { get; set; }

        public ContactType ContactType { get; set; }
        public double ContactArea { get; set; }
        public double MaxPenetration { get; set; }
        public int VelocityMultiplier { get; set; }

        public List<BodyPartRequirement> Requirements { get; set; }
    }


    public enum BodyPartRequirementType
    {
        BodyPart,
        ChildTissueLayerGroup,
        ChildBodyPartGroup
    }

    public class BodyPartRequirement 
    {
        public BodyPartRequirementType Type { get; set; }
        public List<BprConstraint> Constraints { get; set; }
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
    }
}
