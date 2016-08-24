using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfAgentBuilder : IDfAgentBuilder
    {
        Dictionary<string, DfObject> BodyPartsDefn { get; set; }
        Dictionary<string, DfObject> MaterialDefns { get; set; }
        Dictionary<string, List<string>> BodyPartCategoryTissues { get; set; }
        Dictionary<string, Dictionary<string, int>> BodyPartCategoryTissueThickness { get; set; }


        public DfAgentBuilder()
        {
            BodyPartsDefn = new Dictionary<string, DfObject>();
            MaterialDefns = new Dictionary<string, DfObject>();
            BodyPartCategoryTissues = new Dictionary<string, List<string>>();
            BodyPartCategoryTissueThickness = new Dictionary<string, Dictionary<string, int>>();
        }

        #region Lookups
        IEnumerable<string> GetBodyPartCategories(DfObject bpObject)
        {
            return bpObject.Tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.CATEGORY))
                .Select(t => t.GetParam(0));
        }

        DfObject GetRootPartDefn()
        {
            var parts =  BodyPartsDefn.Values.Where(o => !o.Tags.Any(
                t => t.Name.Equals(DfTags.MiscTags.CONTYPE)
                || t.Name.Equals(DfTags.MiscTags.CON)
                || t.Name.Equals(DfTags.MiscTags.CON_CAT)
                ));

            return parts.Single();
        }

        private bool ConnectsToCategory(DfObject o, string cat)
        {
            return o.Tags.Any(t => t.Name.Equals(DfTags.MiscTags.CON_CAT)
                && t.GetParam(0).Equals(cat));
        }

        IEnumerable<DfObject> FindPartsForDirectConnection(string partName)
        {
            return BodyPartsDefn.Values.Where(o => o.Tags.Any(
                t => t.Name.Equals(DfTags.MiscTags.CON)
                    && t.GetParam(0).Equals(partName)));
        }

        private IEnumerable<DfObject> FindPartsForConnTypes(DfObject defn)
        {
            return BodyPartsDefn.Values.Where(
                o => o.Tags.Any(
                    t => t.Name.Equals(DfTags.MiscTags.CONTYPE)
                    && defn.Tags.Any(dT => dT.IsSingleWord(t.GetParam(0)))));
        }

        private IEnumerable<DfObject> FindPartsForCategories(DfObject defn)
        {
            return GetBodyPartCategories(defn)
                .SelectMany(cat => BodyPartsDefn.Values.Where(o => ConnectsToCategory(o, cat)));
        }
        #endregion


        public void AddBody(string name, DfObject bpObject)
        {
            var tags = bpObject.Tags.Skip(1);

            var bodyPartDefns = tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.BP))
                .Select<DfTag, List<DfTag>>(t =>
                {
                    var index = bpObject.Tags.IndexOf(t);
                    var count = 1;
                    DfTag next = bpObject.Next(t);
                    while (next != null && !next.Name.Equals(DfTags.MiscTags.BP))
                    {
                        count++;
                        next = bpObject.Next(next);
                    }
                    return bpObject.Tags.ToList().GetRange(index, count);
                })
                .Select(tagList => new DfObject(tagList.ToList()));

            foreach (var bodyPartDefn in bodyPartDefns)
            {
                BodyPartsDefn[bodyPartDefn.Name] = bodyPartDefn;
            }
        }

        

        public void AddMaterialFromTemplate(string matName, DfObject matTemplateObj)
        {
            MaterialDefns[matName] =  matTemplateObj;
        }

        public void RemoveMaterial(string matName)
        {
            MaterialDefns.Remove(matName);
            foreach (var bpName in BodyPartCategoryTissueThickness.Keys.ToList())
            {
                if (BodyPartCategoryTissueThickness[bpName].ContainsKey(matName))
                {
                    BodyPartCategoryTissueThickness[bpName].Remove(matName);
                }
            }
        }

        public void AddTissueToBodyPart(string bpCategory, string tisName)
        {
            if (!BodyPartCategoryTissues.ContainsKey(bpCategory))
            {
                BodyPartCategoryTissues[bpCategory] = new List<string>();
            }
            BodyPartCategoryTissues[bpCategory].Add(tisName);

        }

        public void SetBodyPartTissueThickness(string bpCategory, string tisName, int relThick)
        {
            if (!BodyPartCategoryTissueThickness.ContainsKey(bpCategory))
            {
                BodyPartCategoryTissueThickness[bpCategory] = new Dictionary<string, int>();
            }
            BodyPartCategoryTissueThickness[bpCategory][tisName] = relThick;
        }

        string GetMaterialAdj(DfObject matDefn)
        {
            var adjTag = matDefn.Tags.SingleOrDefault(t => t.Name.Equals(DfTags.MiscTags.STATE_NAME_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID));

            string adj = null;
            if (adjTag != null)
            {
                adj = adjTag.GetParam(1);
            }

            if (adj == null)
            {
                adj = matDefn.Tags.SingleOrDefault(t => t.Name.Equals(DfTags.MiscTags.TISSUE_NAME))
                        .GetParam(0);
            }

            return adj;
        }

        Material GetTissueMaterial(string tisName)
        {
            var matDefn = MaterialDefns[tisName];
            return new Material
            {
                Adjective = GetMaterialAdj(matDefn)
            };
        }

        Tissue CreateTissueForPart(string bpName)
        {
            var layers = new List<TissueLayer>();

            foreach (var bpCat in GetBodyPartCategories(BodyPartsDefn[bpName]))
            {

                if (BodyPartCategoryTissueThickness.ContainsKey(bpCat))
                {
                    foreach(var tisName in BodyPartCategoryTissueThickness[bpCat].Keys)
                    {
                        var thickness = BodyPartCategoryTissueThickness[bpCat][tisName];
                        layers.Add(new TissueLayer
                        {
                            Material = GetTissueMaterial(tisName),
                            RelativeThickness = thickness
                        });
                    }

                }

                if (BodyPartCategoryTissues.ContainsKey(bpCat))
                {
                    foreach (var tisName in BodyPartCategoryTissues[bpCat])
                    {
                        layers.Add(new TissueLayer
                        {
                            Material = GetTissueMaterial(tisName),
                            RelativeThickness = 1
                        });
                    }
                }
            }
            return new Tissue
            {
                Layers = layers
            };
        }

        BodyPart CreateBodyPart(DfObject defn, BodyPart parent)
        {
            return new BodyPart
            {
                Parent = parent,
                Tissue = CreateTissueForPart(defn.Name),
                NameSingular = defn.Tags.First().GetParam(1),
                NamePlural = defn.Tags.First().GetParam(2),
                CanGrasp = defn.Tags.Any(t => t.IsSingleWord(DfTags.MiscTags.GRASP)),
                CanBeAmputated = defn.Tags.Any(t =>
                        t.IsSingleWord(DfTags.MiscTags.LIMB)
                     || t.IsSingleWord(DfTags.MiscTags.HEAD)
                     || t.IsSingleWord(DfTags.MiscTags.DIGIT)
                     )
            };
        }


        public Agent Build()
        {
            var parts = new List<BodyPart>();
            var rootPartDefn = GetRootPartDefn();

            var rootPart = CreateBodyPart(rootPartDefn, null);
            parts.Add(rootPart);

            var toLoad = new List<DfObject>();
            var partMap = new Dictionary<BodyPart, DfObject>{{rootPart, rootPartDefn}};
            for (int i = 0; i < parts.Count(); i++)
            {
                var part = parts[i];
                var defn = partMap[part];
                toLoad.Clear();

                toLoad.AddRange(FindPartsForCategories(defn));
                toLoad.AddRange(FindPartsForConnTypes(defn));
                toLoad.AddRange(FindPartsForDirectConnection(defn.Name));

                foreach (var partDefn in toLoad)
                {
                    var newPart = CreateBodyPart(partDefn, part);
                    parts.Add(newPart);
                    partMap[newPart] = partDefn;
                }
            }

            return new Agent
            {
                Body = new Body
                {
                    Parts = parts
                }
            };
        }



    }
}
