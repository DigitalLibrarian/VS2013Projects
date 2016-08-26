using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfAgentBuilder : IDfAgentBuilder
    {
        Dictionary<string, DfObject> BodyPartsDefn { get; set; }
        Dictionary<string, Material> Materials { get; set; }
        Dictionary<string, List<string>> BodyPartCategoryTissues { get; set; }
        Dictionary<string, Dictionary<string, int>> BodyPartCategoryTissueThickness { get; set; }
        Dictionary<string, List<CombatMove>> MovesByCategory { get; set; }
        Dictionary<string, List<CombatMove>> MovesByType { get; set; }

        public DfAgentBuilder()
        {
            BodyPartsDefn = new Dictionary<string, DfObject>();
            Materials = new Dictionary<string, Material>();
            BodyPartCategoryTissues = new Dictionary<string, List<string>>();
            BodyPartCategoryTissueThickness = new Dictionary<string, Dictionary<string, int>>();
            MovesByCategory = new Dictionary<string, List<CombatMove>>();
            MovesByType = new Dictionary<string, List<CombatMove>>();
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


        private IEnumerable<DfObject> FindPartsForTypes(DfObject defn)
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


        public void AddMaterial(string matName, Material material)
        {
            Materials[matName] = material;
        }

        public void RemoveMaterial(string matName)
        {
            Materials.Remove(matName);
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
            return Materials[tisName];
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

        BodyPart CreateBodyPart(DfObject defn, BodyPart parent, List<CombatMove> moves)
        {
            var singleWords = defn.Tags.Where(t => t.IsSingleWord())
                .Select(t => t.Name).ToList();

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
                     ),
                Moves = moves.ToList(),
                IsNervous = 
                        singleWords.Contains("NERVOUS")
                    ||  singleWords.Contains("THOUGHT"),
                IsCirculatory = singleWords.Contains("CIRCULATION"),
                IsSkeletal = singleWords.Contains("SKELETAL"),
                IsDigit = singleWords.Contains("DIGIT"),
                IsBreathe = singleWords.Contains("BREATHE"),
                IsSight = singleWords.Contains("SIGHT"),
                IsStance = singleWords.Contains("STANCE"),
                IsInternal = singleWords.Contains("INTERNAL")
            };
        }

        List<CombatMove> GetMovesForPart(DfObject defn)
        {
            return GetBodyPartCategories(defn).SelectMany(cat =>
                {
                    if (MovesByCategory.ContainsKey(cat))
                    {
                        return MovesByCategory[cat];
                    }
                    else
                    {
                        return Enumerable.Empty<CombatMove>();
                    }
                })
                .Concat(
                    defn.Tags.Where(t => t.IsSingleWord())
                    .Select(t=> t.Name)
                    .SelectMany(type =>
                    {
                        if (MovesByType.ContainsKey(type))
                        {
                            return MovesByType[type];
                        }
                        else
                        {
                            return Enumerable.Empty<CombatMove>();
                        }

                    })
                )
                .ToList();
        }

        public Agent Build()
        {
            var parts = new List<BodyPart>();
            var rootPartDefn = GetRootPartDefn();

            var rootPart = CreateBodyPart(rootPartDefn, null, GetMovesForPart(rootPartDefn));
            parts.Add(rootPart);

            var partMoves = new List<CombatMove>();
            var toLoad = new List<DfObject>();
            var partMap = new Dictionary<BodyPart, DfObject>{{rootPart, rootPartDefn}};
            for (int i = 0; i < parts.Count(); i++)
            {
                var part = parts[i];
                var defn = partMap[part];
                toLoad.Clear();
                partMoves.Clear();

                toLoad.AddRange(FindPartsForCategories(defn));
                toLoad.AddRange(FindPartsForTypes(defn));
                toLoad.AddRange(FindPartsForDirectConnection(defn.Name));
                                
                foreach (var move in GetMovesForPart(defn))
                {
                    partMoves.Add(move);
                }

                foreach (var partDefn in toLoad)
                {
                    var newPart = CreateBodyPart(partDefn, part, partMoves);
                    parts.Add(newPart);
                    partMap[newPart] = partDefn;
                }
            }

            var agent = new Agent(Name, new Body
            {
                Parts = parts
            });
            return agent;
        }



        string Name { get; set; }
        public void SetName(string singular, string plural)
        {
            Name = singular;
        }

        public void AddCombatMoveToCategory(CombatMove move, string category)
        {
            if (!MovesByCategory.ContainsKey(category))
            {
                MovesByCategory[category] = new List<CombatMove>();
            }
            MovesByCategory[category].Add(move);
        }


        public void AddCombatMoveToType(CombatMove move, string type)
        {
            if (!MovesByType.ContainsKey(type))
            {
                MovesByType[type] = new List<CombatMove>();
            }

            MovesByType[type].Add(move);
        }
    }
}
