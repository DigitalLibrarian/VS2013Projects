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

        public ContactType ContactType { get; set; }
        public int ContactArea { get; set; }
        public int MaxPenetration { get; set; }
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
        public List<string> Categories { get; set; }
        public List<string> Types { get; set; }
    }
}
