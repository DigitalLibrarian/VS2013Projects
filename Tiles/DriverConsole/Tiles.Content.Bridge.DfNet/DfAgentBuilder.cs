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
        Dictionary<string, List<DfBodyAttack>> MovesByCategory { get; set; }
        Dictionary<string, List<DfBodyAttack>> MovesByType { get; set; }

        Dictionary<string, int> BpCatSizeOverrides { get; set; }

        string Name { get; set; }
        int Symbol { get; set; }

        Color Foreground { get; set; }
        Color Background { get; set; }

        int Size { get; set; }

        public DfAgentBuilder()
        {
            BodyPartsDefn = new Dictionary<string, DfObject>();
            Materials = new Dictionary<string, Material>();
            BodyPartCategoryTissues = new Dictionary<string, List<string>>();
            BodyPartCategoryTissueThickness = new Dictionary<string, Dictionary<string, int>>();
            BpCatSizeOverrides = new Dictionary<string, int>();
            MovesByCategory = new Dictionary<string, List<DfBodyAttack>>();
            MovesByType = new Dictionary<string, List<DfBodyAttack>>();
            Foreground = new Color(255, 255, 255, 255);
            Background = new Color(0, 0, 0, 255);
        }

        #region Lookups
        List<string> GetBodyPartCategories(DfObject bpObject)
        {
            return bpObject.Tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.CATEGORY))
                .Select(t => t.GetParam(0))
                .ToList();
        }

        List<string> GetBodyPartTypes(DfObject bpObject)
        {
            return bpObject.Tags
                .Where(t => t.IsSingleWord())
                .Select(t => t.Name)
                .ToList();
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
            if (!Materials.ContainsKey(matName))
            {
                Materials[matName] = material;
            }
            else
            {
                var mat = Materials[matName];
                mat.Name = material.Name;
            }
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

        int GetDefaultRelsize(string bpName)
        {
            var defn = BodyPartsDefn[bpName];
            var drTag = defn.Tags.Single(x => x.Name.Equals(DfTags.MiscTags.DEFAULT_RELSIZE));
            return int.Parse(drTag.GetParam(0));
        }

        int GetBpSize(string bpName)
        {
            var defn = BodyPartsDefn[bpName];
            foreach (var cat in GetBodyPartCategories(defn))
            {
                if (BpCatSizeOverrides.ContainsKey(cat))
                {
                    return BpCatSizeOverrides[cat];
                }
            }
            return GetDefaultRelsize(bpName);
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

        BodyPart CreateBodyPart(DfObject defn, BodyPart parent)
        {
            var singleWords = defn.Tags.Where(t => t.IsSingleWord())
                .Select(t => t.Name).ToList();

            var part = new BodyPart
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
                Moves = new List<CombatMove>(),
                Categories = GetBodyPartCategories(defn),
                Types = GetBodyPartTypes(defn),
                IsNervous = 
                        singleWords.Contains("NERVOUS")
                    ||  singleWords.Contains("THOUGHT"),
                IsCirculatory = singleWords.Contains("CIRCULATION"),
                IsSkeletal = singleWords.Contains("SKELETAL"),
                IsDigit = singleWords.Contains("DIGIT"),
                IsBreathe = singleWords.Contains("BREATHE"),
                IsSight = singleWords.Contains("SIGHT"),
                IsStance = singleWords.Contains("STANCE"),
                IsInternal = singleWords.Contains("INTERNAL"),
                RelativeSize = GetBpSize(defn.Name)
            };
            return part;
        }

        void AddBodyPartMoves(DfObject defn, BodyPart part, int totalBpSize)
        {
            var moves = GetMovesForPart(defn);

            var partSizeMm = (int) (((double)part.RelativeSize/totalBpSize) * Size) * 10;
            foreach (var bpMove in moves)
            {
                var combatMove =
                    new CombatMove
                    {
                        Name = bpMove.ReferenceName,
                        Verb = bpMove.Verb,
                        PrepTime = bpMove.PrepTime,
                        RecoveryTime = bpMove.RecoveryTime,
                        IsDefenderPartSpecific = true,
                        IsStrike = true,
                        IsMartialArts = true,
                        ContactType = Models.ContactType.Other,
                        ContactArea = (int)((double)bpMove.ContactPercent / 100d) * partSizeMm,
                        MaxPenetration = (int)((double)bpMove.PenetrationPercent / 100d) * partSizeMm,
                        VelocityMultiplier = 1000,
                    };

                combatMove.Requirements.Add(new BodyPartRequirement
                {
                    Type = bpMove.RequirementType,
                    Types = bpMove.ByTypes.ToList(),
                    Categories = bpMove.ByCategories.ToList()
                });
                part.Moves.Add(combatMove);
            }
        }

        List<DfBodyAttack> GetMovesForPart(DfObject defn)
        {
            var bodyPartAttacks = GetBodyPartCategories(defn).SelectMany(cat =>
                {
                    if (MovesByCategory.ContainsKey(cat))
                    {
                        return MovesByCategory[cat];
                    }
                    else
                    {
                        return Enumerable.Empty<DfBodyAttack>();
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
                            return Enumerable.Empty<DfBodyAttack>();
                        }

                    })
                )
                .ToList();
            return bodyPartAttacks;
        }

        public Agent Build()
        {
            var parts = new List<BodyPart>();
            var rootPartDefn = GetRootPartDefn();
            Dictionary<BodyPart, DfObject> partToDef = new Dictionary<BodyPart,DfObject>();

            var rootPart = CreateBodyPart(rootPartDefn, null);
            parts.Add(rootPart);

            var toLoad = new List<DfObject>();
            var partMap = new Dictionary<BodyPart, DfObject>{{rootPart, rootPartDefn}};
            for (int i = 0; i < parts.Count(); i++)
            {
                var part = parts[i];
                var defn = partMap[part];
                partToDef[part] = defn;
                toLoad.Clear();

                toLoad.AddRange(FindPartsForCategories(defn));
                toLoad.AddRange(FindPartsForTypes(defn));
                toLoad.AddRange(FindPartsForDirectConnection(defn.Name));
                 

                foreach (var partDefn in toLoad)
                {
                    var newPart = CreateBodyPart(partDefn, part);
                    parts.Add(newPart);
                    partMap[newPart] = partDefn;
                }
            }

            int totalBPSize = parts.Select(x => x.RelativeSize).Sum();

            foreach (var part in parts)
            {
                var defn = partToDef[part];
                foreach (var move in GetMovesForPart(defn))
                {
                    AddBodyPartMoves(defn, part, totalBPSize);
                }
            }

            var sprite = new Sprite(Symbol, Foreground, Background);
            var agent = new Agent(
                Name, 
                new Body
                {
                    Parts = parts,
                    Size = Size
                },
                sprite);
            return agent;
        }



        public void SetName(string singular, string plural)
        {
            Name = singular;
        }

        public void SetForegroundColor(Color color)
        {
            Foreground = color;
        }

        public void SetBackgroundColor(Color color)
        {
            Background = color;
        }

        public void SetSymbol(int symbol)
        {
            Symbol = symbol;
        }

        public void AddBodyAttack(DfBodyAttack move)
        {
            foreach (var category in move.ByCategories)
            {
                if (!MovesByCategory.ContainsKey(category))
                {
                    MovesByCategory[category] = new List<DfBodyAttack>();
                }
                MovesByCategory[category].Add(move);
            }

            foreach (var type in move.ByTypes)
            {
                if (!MovesByType.ContainsKey(type))
                {
                    MovesByType[type] = new List<DfBodyAttack>();
                }

                MovesByType[type].Add(move);
            }
        }


        public void OverrideBodyPartCategorySize(string bpCategory, int size)
        {
            BpCatSizeOverrides[bpCategory] = size;
        }


        public void AddLifeStageSize(int ageYear, int ageDay, int size)
        {
            // TODO - make this part of racial model
            Size = size;
        }


    }
}
