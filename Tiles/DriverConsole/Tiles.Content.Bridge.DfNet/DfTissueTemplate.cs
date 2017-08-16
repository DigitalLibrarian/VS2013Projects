using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfTissueTemplate
    {
        public string Name { get; set; }
        public bool IsConnective { get; set; }
        public bool IsCosmetic { get; set; }
        public int VascularRating { get; set; }
        public int PainReceptors { get; set; }
        public int HealingRate { get; set; }
        public int RelativeThickness { get; set; }
        public bool ThickensOnStrength { get; set; }
        public bool ThickensOnEnergyStorage { get; set; }
        public bool HasArteries { get; set; }
    }

    public interface IDfTissueTemplateFactory
    {
        DfTissueTemplate Create(string tissueName);
        DfTissueTemplate CreateFromTissueCreatureInline(string creatureName, string tisName);
    }

    public class DfTissueTemplateFactory : IDfTissueTemplateFactory
    {
        IDfObjectStore Store { get; set; }

        public DfTissueTemplateFactory(IDfObjectStore store)
        {
            Store = store;
        }

        public DfTissueTemplate Create(string tissueName)
        {
            var df = Store.Get(DfTags.TISSUE_TEMPLATE, tissueName);
            return Common(df, tissueName);
        }
        public DfTissueTemplate CreateFromTissueCreatureInline(string creatureName, string tisName)
        {
            var creatureDf = Store.Get(DfTags.CREATURE, creatureName);
            var cTags = creatureDf.Tags.ToList();

            int startIndex = cTags.FindIndex(
                t => t.Name.Equals(DfTags.MiscTags.TISSUE)
                    && t.GetParam(0).Equals(tisName));

            int endIndex = cTags.FindIndex(startIndex,
                t => t.Name.Equals(DfTags.MiscTags.TISSUE_LAYER));

            var matTags = cTags.GetRange(startIndex, endIndex - startIndex).ToList();

            var tisO = new DfObject(matTags.ToArray());
            return Common(tisO, tisName);
        }

        public DfTissueTemplate Common(DfObject df, string tissueName)
        {
            DfTissueTemplate template = new DfTissueTemplate
            {
                Name = tissueName
            };
            foreach (var tag in df.Tags)
            {
                switch (tag.Name)
                {
                    case DfTags.MiscTags.VASCULAR:
                        template.VascularRating = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.PAIN_RECEPTORS:
                        template.PainReceptors = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.HEALING_RATE:
                        template.HealingRate = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.CONNECTS:
                        template.IsConnective = true;
                        break;
                    case DfTags.MiscTags.RELATIVE_THICKNESS:
                        template.RelativeThickness = int.Parse(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.COSMETIC:
                        template.IsCosmetic = true;
                        break;
                    case DfTags.MiscTags.ARTERIES:
                        template.HasArteries = true;
                        break;
                    case DfTags.MiscTags.THICKENS_ON_STRENGTH:
                        template.ThickensOnStrength = true;
                        break;
                    case DfTags.MiscTags.THICKENS_ON_ENERGY_STORAGE:
                        template.ThickensOnEnergyStorage = true;
                        break;
                }
            }
            return template;
        }

    }
}
