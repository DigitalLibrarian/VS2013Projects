using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;
namespace Tiles.Content.Bridge.DfNet
{
    public class DfMaterialFactory : IDfMaterialFactory
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialBuilderFactory BuilderFactory { get; set; }

        public DfMaterialFactory(IDfObjectStore store, IDfMaterialBuilderFactory builderFactory)
        {
            Store = store;
            BuilderFactory = builderFactory;
        }

        public Material CreateInorganic(string name)
        {
            var df = _CreateInorganicDf(name);
            return Common(df);
        }

        DfObject _CreateInorganicDf(string name)
        {
            var df = Store.Get(DfTags.INORGANIC, name);

            var includeTag = df.Tags.FirstOrDefault(t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE));

            if (includeTag != null)
            {
                var matTempDf = Store.Get(DfTags.MATERIAL_TEMPLATE, includeTag.GetParam(0));
                var newTags = new List<DfTag>();
                newTags.Add(df.Tags.First());
                newTags.AddRange(matTempDf.Tags.Skip(1));
                newTags.AddRange(df.Tags.Skip(1).Where(t => t != includeTag));
                df = new DfObject(newTags);
            }
            return df;
        }

        public Material CreateTissue(string tissueTemplate)
        {
            var df = Store.Get(DfTags.TISSUE_TEMPLATE, tissueTemplate);
            return Common(df);
        }

        public bool IsCosmeticTissue(string tissueTemplate)
        {
            var df = Store.Get(DfTags.TISSUE_TEMPLATE, tissueTemplate);
            return df.Tags.Any(t => t.IsSingleWord("COSMETIC"));
        }

        public Material CreateFromTissueCreatureInline(string creatureName, string tisName)
        {
            var creatureDf = Store.Get(DfTags.CREATURE, creatureName);
            var cTags = creatureDf.Tags.ToList();

            int startIndex = cTags.FindIndex(
                t => t.Name.Equals(DfTags.MiscTags.TISSUE)
                    && t.GetParam(0).Equals(tisName));

            int endIndex = cTags.FindIndex(startIndex,
                t => t.Name.Equals(DfTags.MiscTags.TISSUE_LAYER));

            var matTags = cTags.GetRange(startIndex, endIndex - startIndex).ToList();

            foreach(var createMatTag in matTags.Where(t => t.Name.Equals(DfTags.MiscTags.TISSUE_MATERIAL)).ToList())
            {
                var matStrat = createMatTag.GetParam(0);
                //[TISSUE_MATERIAL:LOCAL_CREATURE_MAT:WOOD]
                if (matStrat.Equals("LOCAL_CREATURE_MAT"))
                {
                    var matName = createMatTag.GetParam(1);
                    var cMatTempTag = creatureDf.Tags.Single(
                        t => t.Name.Equals(DfTags.MiscTags.USE_MATERIAL_TEMPLATE)
                                && t.GetParam(0).Equals(matName));
                    var matTempName = cMatTempTag.GetParam(1);
                    var matTempDf = Store.Get(DfTags.MATERIAL_TEMPLATE, matTempName);
                    var matTempTags = matTempDf.Tags.Skip(1).ToList();
                    matTags.InsertRange(0, matTempTags);
                } else if(matStrat.Equals("INORGANIC"))
                {
                    var inorgName = createMatTag.GetParam(1);
                    var inorgDf = _CreateInorganicDf(inorgName);
                    matTags.InsertRange(0, inorgDf.Tags.Skip(1));
                }
                else if(matStrat.Equals("MUD"))
                {
                    /// grr
                    matTags.InsertRange(0, GetDefaultMaterialPropertyTags());
                }
                else
                {
                    throw new NotImplementedException(string.Format("Unknown tissue inclusion strategy {0}", createMatTag.ToString()));
                }
            }

            var tisO = new DfObject(matTags.ToArray());
            return Common(tisO);
        }

        DfTag[] GetDefaultMaterialPropertyTags()
        {
            return new DfTag[]{
                        new DfTag(DfTags.MiscTags.IMPACT_YIELD, "1"),
                        new DfTag(DfTags.MiscTags.IMPACT_FRACTURE, "1"),
                        new DfTag(DfTags.MiscTags.IMPACT_STRAIN_AT_YIELD, "1"),
                        new DfTag(DfTags.MiscTags.SHEAR_YIELD, "1"),
                        new DfTag(DfTags.MiscTags.SHEAR_FRACTURE, "1"),
                        new DfTag(DfTags.MiscTags.SHEAR_STRAIN_AT_YIELD, "1")
                    };
        }

        public Material CreateFromMaterialTemplate(string materialTemplate)
        {
            var df = Store.Get(DfTags.MATERIAL_TEMPLATE, materialTemplate);

            if (df.Name.Equals("FLAME_TEMPLATE"))
            {
                var matTempTags = df.Tags.ToList();
                matTempTags.AddRange(GetDefaultMaterialPropertyTags());
                df = new DfObject(matTempTags);
            }
            return Common(df);
        }

        private Material Common(DfObject df)
        {
            var b = BuilderFactory.Create();

            // TODO - this whole concept of "picking one string" needs to go away, 
            // and we should just model the descriptive words as given.
            var name = GetMaterialName(df);
            var adjective = GetMaterialAdj(df);
            b.SetName(name);
            b.SetAdjective(adjective);

            string state, value;
            foreach (var tag in df.Tags)
            {
                switch (tag.Name)
                {
                    case DfTags.MiscTags.IMPACT_YIELD:
                        b.SetImpactYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.IMPACT_FRACTURE:
                        b.SetImpactFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.IMPACT_STRAIN_AT_YIELD:
                        b.SetImpactStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.SHEAR_YIELD:
                        b.SetShearYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.SHEAR_FRACTURE:
                        b.SetShearFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.SHEAR_STRAIN_AT_YIELD:
                        b.SetShearStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.COMPRESSIVE_YIELD:
                        b.SetCompressiveYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.COMPRESSIVE_FRACTURE:
                        b.SetCompressiveFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.COMPRESSIVE_STRAIN_AT_YIELD:
                        b.SetCompressiveStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TENSILE_YIELD:
                        b.SetTensileYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TENSILE_FRACTURE:
                        b.SetTensileFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TENSILE_STRAIN_AT_YIELD:
                        b.SetTensileStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TORSION_YIELD:
                        b.SetTorsionYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TORSION_FRACTURE:
                        b.SetTorsionFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.TORSION_STRAIN_AT_YIELD:
                        b.SetTorsionStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.BENDING_YIELD:
                        b.SetBendingYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.BENDING_FRACTURE:
                        b.SetBendingFracture(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.BENDING_STRAIN_AT_YIELD:
                        b.SetBendingStrainAtYield(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.SOLID_DENSITY:
                        b.SetSolidDensity(int.Parse(tag.GetParam(0)));
                        break;
                    case DfTags.MiscTags.MAX_EDGE:
                        double sharp = ((double)int.Parse(tag.GetParam(0))) / 10000d;
                        if (sharp == 0)
                        {
                            sharp = 0.01d;
                        }
                        b.SetSharpnessMultiplier(sharp);
                        break;
                    case DfTags.MiscTags.STATE_NAME_ADJ:
                            state = tag.GetParam(0);
                            value = tag.GetParam(1);
                            b.AddStatePropertyValue("NAME", state, value);
                            b.AddStatePropertyValue("ADJ", state, value);
                        break;
                    default:
                        if (tag.Name.StartsWith("STATE_"))
                        {
                            var propName = tag.Name.Replace("STATE_", "");
                            state = tag.GetParam(0);
                            value = tag.GetParam(1);
                            b.AddStatePropertyValue(propName, state, value);
                        }
                        break;
                }
            }

            return b.Build();
        }


        string GetMaterialAdj(DfObject matDefn)
        {
            var checks = new Dictionary<Predicate<DfTag>, Func<DfTag, string>>
            {
                {t => t.Name.Equals(DfTags.MiscTags.TISSUE_NAME), t => t.GetParam(0)},
                {t => t.Name.Equals(DfTags.MiscTags.STATE_NAME_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID), t => t.GetParam(1)},
                {t => t.Name.Equals(DfTags.MiscTags.STATE_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID), t => t.GetParam(1)},
                {t => t.Name.Equals(DfTags.MiscTags.STATE_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.SOLID), t => t.GetParam(1)},
                {t => t.Name.Equals(DfTags.MiscTags.STATE_NAME_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL), t => t.GetParam(1)},

                {t => t.Name.Equals(DfTags.MiscTags.STATE_NAME), t => t.GetParam(1)},
                {t => t.Name.Equals(DfTags.MiscTags.IS_GEM), t => t.GetParam(0)}
            };

            foreach (var check in checks.Keys)
            {
                var adjTag = matDefn.Tags.LastOrDefault(t => check(t));
                var valueGetter = checks[check];
                if (adjTag != null)
                {
                    return valueGetter(adjTag);
                }
            }

            throw new InvalidOperationException("Could not come up with adjective for " + matDefn.Tags.First().ToString());
        }

        string GetMaterialName(DfObject matDefn)
        {

            var checks = new Dictionary<Predicate<DfTag>, Func<DfTag, string>>
            {
                {t => t.Name.Equals(DfTags.MiscTags.STATE_NAME_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID), t => t.GetParam(1)},
                {t => t.Name.Equals(DfTags.MiscTags.IS_GEM), t => t.GetParam(0)}
            };

            foreach (var check in checks.Keys)
            {
                var adjTag = matDefn.Tags.LastOrDefault(t => check(t));
                var valueGetter = checks[check];
                if (adjTag != null)
                {
                    var v =  valueGetter(adjTag);
                    if (!v.Equals("n/a"))
                    {
                        return v;
                    }
                }
            }

            return GetMaterialAdj(matDefn);
        }
    }
}
