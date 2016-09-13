using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfBodyPartAttackFactory
    {
        DfBodyAttack Create(DfObject attackDf);
    }

    public class DfBodyAttackFactory : IDfBodyPartAttackFactory
    {
        public DfBodyAttack Create(DfObject attackDf)
        {
            DfBodyAttack attack = new DfBodyAttack();
            attack.ContactType = ContactType.Blunt;

            foreach (var tag in attackDf.Tags)
            {
                switch (tag.Name)
                {
                    case DfTags.MiscTags.ATTACK:
                        attack.ReferenceName = tag.GetParam(0);
                        var reqTypeStr = tag.GetParam(1);
                        attack.RequirementType = GetReqType(reqTypeStr);
                        int numParams = tag.GetParams().Count();
                        BaConstraint constraint = null;
                        for (int i = 2; i < numParams;i++ )
                        {
                            switch (tag.GetParam(i))
                            {
                                case DfTags.MiscTags.BY_CATEGORY:
                                    constraint = new BaConstraint(BaConstraintType.ByCategory);
                                    for (int j = i+1; j < numParams; j++)
                                    {
                                        var p = tag.GetParam(j);
                                        if (p.StartsWith("BY"))
                                        {
                                            break;
                                        }
                                        if (!p.Equals("ALL")) constraint.Tokens.Add(p);
                                    }
                                    attack.Constraints.Add(constraint);
                                    break;
                                case DfTags.MiscTags.BY_TYPE:
                                    constraint = new BaConstraint(BaConstraintType.ByType);
                                    for (int j = i+1; j < numParams; j++)
                                    {
                                        var p = tag.GetParam(j);
                                        if (p.StartsWith("BY"))
                                        {
                                            break;
                                        }
                                        constraint.Tokens.Add(p);
                                    }
                                    attack.Constraints.Add(constraint);
                                    break;
                            }
                        }
                        break;
                    case DfTags.MiscTags.ATTACK_VERB:
                        attack.Verb = new Verb
                        {
                            SecondPerson = tag.GetParam(0),
                            ThirdPerson = tag.GetParam(1)
                        };
                        break;
                    case DfTags.MiscTags.ATTACK_CONTACT_PERC:
                        attack.ContactPercent = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.ATTACK_PENETRATION_PERC:
                        attack.PenetrationPercent = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.ATTACK_PREPARE_AND_RECOVER:
                        attack.PrepTime = int.Parse(tag.GetParam(0));
                        attack.RecoveryTime = int.Parse(tag.GetParam(1));
                        break;
                    case DfTags.MiscTags.ATTACK_FLAG_EDGE:
                        attack.ContactType = ContactType.Edge;
                        break;
                }
            }
            return attack;
        }

        BodyPartRequirementType GetReqType(string reqType)
        {
            switch (reqType)
            {
                case "BODYPART":
                    return BodyPartRequirementType.BodyPart;
                case "CHILD_TISSUE_LAYER_GROUP":
                    return BodyPartRequirementType.ChildTissueLayerGroup;
                case "CHILD_BODYPART_GROUP":
                    return BodyPartRequirementType.ChildBodyPartGroup;
                default:
                    throw new NotImplementedException();

            }
        }
    }
}
