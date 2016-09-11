using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfBodyAttack
    {
        public DfBodyAttack()
        {
            Constraints = new List<BaConstraint>();
        }

        public string ReferenceName { get; set; }
        public BodyPartRequirementType RequirementType { get; set; }
        
        public List<BaConstraint> Constraints { get; set; }

        public Verb Verb { get; set; }

        public int ContactPercent { get; set; }
        public int PenetrationPercent { get; set; }

        public int PrepTime { get; set; }
        public int RecoveryTime { get; set; }
    }

    public enum BaConstraintType
    {
        ByCategory,
        ByType
    }
    public class BaConstraint
    {
        public BaConstraintType ConstraintType { get; set; }
        public List<string> Tokens { get; set; }

        public BaConstraint(BaConstraintType bacType)
        {
            ConstraintType = bacType;
            Tokens = new List<string>();
        }
    }
}
