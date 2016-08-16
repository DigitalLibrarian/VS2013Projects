using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    
	//[ADD_MATERIAL:MUSCLE:MUSCLE_TEMPLATE]
    public class BodyDetailPlan_AddMaterial
    {
        public const string TokenName = "ADD_MATERIAL";
        public string Identifier { get; set; }
        public string MaterialTemplate { get; set; }

        public static BodyDetailPlan_AddMaterial FromTag(Tag tag)
        {
            return new BodyDetailPlan_AddMaterial
            {
                Identifier = tag.Words[1],
                MaterialTemplate = tag.Words[2]
            };
        }

        
    }

    public class BodyDetailPlan_AddTissue
    {
        public const string TokenName = "ADD_TISSUE";
        public string Identifier { get; set; }
        public string TissueTemplate { get; set; }

        public static BodyDetailPlan_AddTissue FromTag(Tag tag)
        {
            return new BodyDetailPlan_AddTissue
            {
                Identifier = tag.Words[1],
                TissueTemplate = tag.Words[2]
            };
        }
    }

    //[BP_LAYERS:BY_CATEGORY:ARM_UPPER:ARG4:25:ARG3:25:ARG2:5:ARG1:1]
    public class BodyDetailPlan_BpLayers
    {
        public const string TokenName = "BP_LAYERS";

        public string Strategy { get; set; }
        public string Category { get; set; }
        public Dictionary<string, int> ArgumentThickness { get; set; }

        public static BodyDetailPlan_BpLayers FromTag(Tag tag)
        {
            var tokenName = tag.Words[0];
            var strategy = tag.Words[1];
            var category = tag.Words[2];

            var bpLayers = new BodyDetailPlan_BpLayers
            {
                Strategy = strategy,
                Category = category,
                ArgumentThickness = new Dictionary<string,int>()
            };
            var leftOvers = tag.Words.Skip(3).ToList();
            // The mod 2 is hack.  These parameter lists might have 2 or 3 element sublists, with no syntactic way to tell. I just ignore 3s until it is a problem.
            while (leftOvers.Any() && leftOvers.Count() % 2 == 0)
            {
                var argName = leftOvers.ElementAt(0);
                var argValue = int.Parse(leftOvers.ElementAt(1));
                leftOvers.RemoveRange(0, 2);
                bpLayers.ArgumentThickness.Add(argName, argValue);
            }

            return bpLayers;
        }
    }

    //[BP_POSITION:BY_CATEGORY:SHELL:TOP]
    public class BodyDetailPlan_BpPosition
    {
        public const string TokenName = "BP_POSITION";
        public string Strategy { get; set; }
        public string Category { get; set; }
        public string Side { get; set; }

        public static BodyDetailPlan_BpPosition FromTag(Tag tag)
        {
            return new BodyDetailPlan_BpPosition
            {
                Strategy = tag.Words[1],
                Category = tag.Words[2],
                Side = tag.Words[3]
            };
        }

    }
    
	//[BP_RELSIZE:BY_CATEGORY:SPINE:100]
    public class BodyDetailPlan_BpRelSize
    {
        public const string TokenName = "BP_RELSIZE";

        public string Strategy { get; set; }
        public string Category { get; set; }
        public int RelativeSize { get; set;}

        public static BodyDetailPlan_BpRelSize FromTag(Tag tag)
        {
            return new BodyDetailPlan_BpRelSize
            {
                Strategy = tag.Words[1],
                Category = tag.Words[2],
                RelativeSize = int.Parse(tag.Words[3])
            };
        }
    }

    //[BP_RELATION:BY_CATEGORY:LIP:AROUND:BY_CATEGORY:TEETH:100]
    //[BP_RELATION:BY_TOKEN:L_EYELID:AROUND:BY_TOKEN:LEYE:50]
    public class BodyDetailPlan_BpRelation
    {
        public const string TokenName = "BP_RELATION";

        public string LeftStrategy { get; set; }
        public string RightStrategy { get; set; }

        public string LeftCategory { get; set; }
        public string LeftToken { get; set; }

        public string RightCategory { get; set; }
        public string RightToken { get; set; }

        public string Relationship { get; set; }
        public int Extent { get; set; }

        public static BodyDetailPlan_BpRelation FromTag(Tag tag)
        {
            var rel = new BodyDetailPlan_BpRelation{
                LeftStrategy = tag.Words[1],
                RightStrategy = tag.Words[4],
                Relationship = tag.Words[3],
                Extent = int.Parse(tag.Words[6])
            };

            switch(rel.LeftStrategy)
            {
                case "BY_CATEGORY":
                    rel.LeftCategory = tag.Words[2];
                    break;
                case "BY_TOKEN":
                    rel.LeftCategory = tag.Words[2];
                    break;
                default:
                    throw new Exception("WTF DF");
            }

            switch (rel.RightStrategy)
            {
                case "BY_CATEGORY":
                    rel.RightCategory = tag.Words[5];
                    break;
                case "BY_TOKEN":
                    rel.RightCategory = tag.Words[5];
                    break;
                default:
                    throw new Exception("WTF DF");
            }

            return rel;
        }

    }

    public class BodyDetailPlan
    {
        public const string TokenName = "BODY_DETAIL_PLAN";

        public string ReferenceName { get; set; }
        public List<BodyDetailPlan_AddMaterial> AddMaterials { get; set; }
        public List<BodyDetailPlan_AddTissue> AddTissues { get; set; }
        public List<BodyDetailPlan_BpLayers> BpLayers { get; set; }
        public List<BodyDetailPlan_BpPosition> BpPositions { get; set; }
        public List<BodyDetailPlan_BpRelation> BpRelations { get; set; }
        public List<BodyDetailPlan_BpRelSize> BpRelSizes { get; set; }

        public static BodyDetailPlan FromElement(Element ele)
        {
            var plan = new BodyDetailPlan
            {
                ReferenceName = ele.Tags.First().Words[1],
                AddMaterials = new List<BodyDetailPlan_AddMaterial>(),
                AddTissues = new List<BodyDetailPlan_AddTissue>(),
                BpLayers = new List<BodyDetailPlan_BpLayers>(),
                BpPositions = new List<BodyDetailPlan_BpPosition>(),
                BpRelations = new List<BodyDetailPlan_BpRelation>(),
                BpRelSizes = new List<BodyDetailPlan_BpRelSize>()
            };

            foreach (var tag in ele.Tags)
            {
                switch (tag.Name)
                {
                    case BodyDetailPlan_AddMaterial.TokenName:
                        plan.AddMaterials.Add(BodyDetailPlan_AddMaterial.FromTag(tag));
                        break;
                    case BodyDetailPlan_AddTissue.TokenName:
                        plan.AddTissues.Add(BodyDetailPlan_AddTissue.FromTag(tag));
                        break;
                    case BodyDetailPlan_BpLayers.TokenName:
                        plan.BpLayers.Add(BodyDetailPlan_BpLayers.FromTag(tag));
                        break;
                    case BodyDetailPlan_BpPosition.TokenName:
                        plan.BpPositions.Add(BodyDetailPlan_BpPosition.FromTag(tag));
                        break;
                    case BodyDetailPlan_BpRelation.TokenName:
                        plan.BpRelations.Add(BodyDetailPlan_BpRelation.FromTag(tag));
                        break;
                    case BodyDetailPlan_BpRelSize.TokenName:
                        plan.BpRelSizes.Add(BodyDetailPlan_BpRelSize.FromTag(tag));
                        break;
                }
            }
            return plan;
        }
    }
}
