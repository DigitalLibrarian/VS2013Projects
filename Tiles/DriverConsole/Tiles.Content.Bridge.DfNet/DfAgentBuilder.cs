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
        List<DfBodyAttack> BodyAttacks { get; set; }
        List<DfAttributeRange> Attributes { get; set; }

        Dictionary<string, int> BpCatSizeOverrides { get; set; }

        Dictionary<string, DfTissueTemplate> MatNameToTT { get; set; }

        List<BpRelationDefn> BpRelationDefns { get; set; }
        List<TlOverrideDefn> TlOverrideDefns { get; set; }

        string Name { get; set; }
        int Symbol { get; set; }

        Color Foreground { get; set; }
        Color Background { get; set; }

        double Size { get; set; }

        Material BloodMaterial { get; set; }
        Material PusMaterial { get; set; }

        public DfAgentBuilder()
        {
            BodyPartsDefn = new Dictionary<string, DfObject>();
            Materials = new Dictionary<string, Material>();
            BodyPartCategoryTissues = new Dictionary<string, List<string>>();
            BodyPartCategoryTissueThickness = new Dictionary<string, Dictionary<string, int>>();
            BpCatSizeOverrides = new Dictionary<string, int>();
            BodyAttacks = new List<DfBodyAttack>();
            Foreground = new Color(255, 255, 255, 255);
            Background = new Color(0, 0, 0, 255);
            Attributes = new List<DfAttributeRange>();

            MatNameToTT = new Dictionary<string, DfTissueTemplate>();

            BpRelationDefns = new List<BpRelationDefn>();
            TlOverrideDefns = new List<TlOverrideDefn>();
        }

        #region Lookups
        List<string> GetBodyPartCategories(DfObject bpObject)
        {
            return bpObject.Tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.CATEGORY))
                .Select(t => t.GetParam(0))
                .Concat(new string[] { "ALL" })
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

        public void AddTissueTemplate(string matName, DfTissueTemplate tissueTemplate)
        {
            MatNameToTT[matName] = tissueTemplate;
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
            if (!Materials.ContainsKey(tisName))
            {
                throw new InvalidOperationException(string.Format("No material defined for tissue {0}", tisName));
            }
            return Materials[tisName];
        }

        bool TlOverride_MajorArteries(string bpName, string tisName, IEnumerable<string> categories)
        {
            foreach (var tlOverride in TlOverrideDefns)
            {
                bool isStrategyHit = false;
                switch(tlOverride.TargetStrategy)
                {
                    case BodyPartRelationStrategy.ByCategory:
                        isStrategyHit = categories.Any(x => x == tlOverride.TargetStrategyParam);
                        break;
                    case BodyPartRelationStrategy.ByToken:
                        isStrategyHit = bpName == tlOverride.TargetStrategyParam;
                        break;
                }

                if(isStrategyHit)
                {
                    if(tlOverride.SourceTissueName == "ALL" || tlOverride.SourceTissueName == tisName)
                    {
                        return tlOverride.HasMajorArteries;
                    }
                }
            }
            return false;   
        }

        string TlOverride_SetTissueName(string bpName, string tisName, IEnumerable<string> categories)
        {
            foreach (var tlOverride in TlOverrideDefns)
            {
                bool isStrategyHit = false;
                switch (tlOverride.TargetStrategy)
                {
                    case BodyPartRelationStrategy.ByCategory:
                        isStrategyHit = categories.Any(x => x == tlOverride.TargetStrategyParam);
                        break;
                    case BodyPartRelationStrategy.ByToken:
                        isStrategyHit = bpName == tlOverride.TargetStrategyParam;
                        break;
                }

                if (isStrategyHit)
                {
                    if (tlOverride.SourceTissueName == "ALL" || tlOverride.SourceTissueName == tisName)
                    {
                        return tlOverride.DestTissueName;
                    }
                }
            }
            return null;
        }
        
        Tissue CreateTissueForPart(string bpName, IEnumerable<string> bpCategories)
        {
            var layers = new List<TissueLayer>();

            foreach (var bpCat in bpCategories)
            {
                if (BodyPartCategoryTissueThickness.ContainsKey(bpCat))
                {
                    foreach(var sourceTissueName in BodyPartCategoryTissueThickness[bpCat].Keys)
                    {
                        var tissueNameOverride = TlOverride_SetTissueName(bpName, sourceTissueName, bpCategories) ?? sourceTissueName;
                        var tissueTemplate = MatNameToTT[tissueNameOverride];
                        var thickness = BodyPartCategoryTissueThickness[bpCat][sourceTissueName];
                        layers.Add(new TissueLayer
                        {
                            Material = GetTissueMaterial(tissueNameOverride),
                            RelativeThickness = thickness,
                            VascularRating = tissueTemplate.VascularRating,
                            HealingRate = tissueTemplate.HealingRate,
                            PainReceptors = tissueTemplate.PainReceptors,
                            IsCosmetic = tissueTemplate.IsCosmetic,
                            IsConnective = tissueTemplate.IsConnective,
                            ThickensOnStrength = tissueTemplate.ThickensOnStrength,
                            ThickensOnEnergyStorage = tissueTemplate.ThickensOnEnergyStorage,
                            HasArteries = tissueTemplate.HasArteries,
                            HasMajorArteries = TlOverride_MajorArteries(bpName, sourceTissueName, bpCategories)
                        });
                    }
                }

                if (BodyPartCategoryTissues.ContainsKey(bpCat))
                {
                    foreach (var sourceTissueName in BodyPartCategoryTissues[bpCat])
                    {
                        var tissueNameOverride = TlOverride_SetTissueName(bpName, sourceTissueName, bpCategories) ?? sourceTissueName;
                        var tissueTemplate = MatNameToTT[tissueNameOverride];
                        layers.Add(new TissueLayer
                        {
                            Material = GetTissueMaterial(tissueNameOverride),
                            RelativeThickness = tissueTemplate.RelativeThickness,
                            VascularRating = tissueTemplate.VascularRating,
                            HealingRate = tissueTemplate.HealingRate,
                            PainReceptors = tissueTemplate.PainReceptors,
                            IsCosmetic = tissueTemplate.IsCosmetic,
                            IsConnective = tissueTemplate.IsConnective,
                            ThickensOnStrength = tissueTemplate.ThickensOnStrength,
                            ThickensOnEnergyStorage = tissueTemplate.ThickensOnEnergyStorage,
                            HasArteries = tissueTemplate.HasArteries,
                            HasMajorArteries = TlOverride_MajorArteries(bpName, sourceTissueName, bpCategories)
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
            var categories = GetBodyPartCategories(defn).ToList();
            var tissue = CreateTissueForPart(defn.Name, categories);

            foreach(var cat in categories.ToList())
            {
                if(BodyPartCategoryTissues.ContainsKey(cat))
                {
                    categories.AddRange(BodyPartCategoryTissues[cat]);
                }
                if (BodyPartCategoryTissueThickness.ContainsKey(cat))
                {
                    categories.AddRange(BodyPartCategoryTissueThickness[cat].Keys);
                }
            }
            
            var part = new BodyPart
            {
                Parent = parent,
                Tissue = tissue,
                TokenId = defn.Name,
                NameSingular = defn.Tags.First().GetParam(1),
                NamePlural = defn.Tags.First().GetParam(2),
                CanGrasp = defn.Tags.Any(t => t.IsSingleWord(DfTags.MiscTags.GRASP)),
                CanBeAmputated = defn.Tags.Any(t =>
                        t.IsSingleWord(DfTags.MiscTags.LIMB)
                     || t.IsSingleWord(DfTags.MiscTags.HEAD)
                     || t.IsSingleWord(DfTags.MiscTags.DIGIT)
                     || t.IsSingleWord(DfTags.MiscTags.GRASP)
                     || t.IsSingleWord(DfTags.MiscTags.STANCE)
                     ),
                Moves = new List<CombatMove>(),
                Categories = categories.Distinct().ToList(),
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
                IsConnector = singleWords.Contains("CONNECTOR"),
                PreventsParentCollapse = singleWords.Contains("PREVENTS_PARENT_COLLAPSE"),
                IsSmall = singleWords.Contains("SMALL"),
                IsEmbedded = singleWords.Contains("EMBEDDED"),
                RelativeSize = GetBpSize(defn.Name),
                WeaponSlot = GetBodyPartCategories(defn).Contains("HAND") ? WeaponSlot.Main : WeaponSlot.None,
                BodyPartRelations = new List<BodyPartRelation>()
                
            };
            return part;
        }


        bool IsConstraintMatch(BaConstraint con, BodyPart part)
        {
            var checkSet = new List<string>();
            switch (con.ConstraintType)
            {
                case BaConstraintType.ByCategory:
                    checkSet = part.Categories;
                    break;
                case BaConstraintType.ByType:
                    checkSet = part.Types;
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var token in con.Tokens)
            {
                if (!checkSet.Contains(token))
                {
                    return false;
                }
            }
            return true;
        }


        IEnumerable<BodyPart> IsReqMatch(DfBodyAttack attack, Body body, BodyPart part)
        {
            BaConstraint pConstraint = null;
            switch (attack.RequirementType)
            {
                case BodyPartRequirementType.BodyPart:
                    if (attack.Constraints.All(c => IsConstraintMatch(c, part)))
                    {
                        return new List<BodyPart>() { part};
                    }
                    break;
                case BodyPartRequirementType.ChildBodyPartGroup:
                    pConstraint = attack.Constraints.First();
                    if (IsConstraintMatch(pConstraint, part))
                    {
                        var children = body.Parts.Where(p => p.Parent == part);
                        return children.Where(p =>
                            attack.Constraints.Skip(1).All(con => IsConstraintMatch(con, p)));
                    }
                    break;
                case BodyPartRequirementType.ChildTissueLayerGroup:
                    // TODO - fix DwarfScratch_ContactArea case here
                    // There really isn't a body part here, so need new return type
                    pConstraint = attack.Constraints.First();
                    if (IsConstraintMatch(pConstraint, part))
                    {
                        var children = body.Parts.Where(p => p.Parent == part);
                        return children.Where(p =>
                            attack.Constraints.Skip(1).All(con => IsConstraintMatch(con, p)));
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Enumerable.Empty<BodyPart>();
        }
        IEnumerable<BodyPart> FindAttackParts(DfBodyAttack attack, Body body)
        {
            foreach (var part in body.Parts)
            {
                var t = IsReqMatch(attack, body, part);
                if(t.Any())
                {
                    foreach (var bp in t) 
                    {
                        yield return bp;
                    }
                }
            }
        }


        IEnumerable<CombatMove> CreateBodyCombatMove(DfBodyAttack attack, Body body)
        {
            var bodySize = body.Size;
            int totalBpRelSize = body.Parts
                .Where(x => !x.IsInternal && !x.IsEmbedded)
                .Select(x => x.RelativeSize).Sum();

            BaConstraint firstConstraint;
            List<BodyPart> principalParts;
            switch (attack.RequirementType)
            {
                case BodyPartRequirementType.BodyPart:
                    // only one combat move per part match
                    var parts = FindAttackParts(attack, body);
                    foreach (var part in parts)
                    {
                        double totalContactArea = 0;
                        double totalPartSize = 0;
                        double maxLength = 1;
                        GetAttackProps(attack, part, totalBpRelSize, out maxLength, out totalPartSize, out totalContactArea);
                        var contactRatio = (double)attack.ContactPercent / 100d;
                        var contactArea = totalContactArea * contactRatio;
                        var maxPen = (((double)attack.PenetrationPercent / 100d) * maxLength);
                        // NOTE - the contact area is < 1 here for the parakeet scratch
                        var combatMove =
                            new CombatMove
                            {
                                Name = attack.ReferenceName,
                                Verb = attack.Verb,
                                PrepTime = attack.PrepTime,
                                RecoveryTime = attack.RecoveryTime,
                                IsDefenderPartSpecific = true,
                                IsStrike = true,
                                IsMartialArts = true,
                                ContactType = attack.ContactType,
                                ContactArea = System.Math.Max(1, contactArea),
                                MaxPenetration = System.Math.Max(1, maxPen),
                                VelocityMultiplier = 1000,
                            };

                        combatMove.Requirements.Add(new BodyPartRequirement
                        {
                            Type = attack.RequirementType,
                            Constraints = attack.Constraints.Select(a => new BprConstraint
                            {
                                ConstraintType = (BprConstraintType)(int)a.ConstraintType,
                                Tokens = a.Tokens.ToList()
                            }).ToList()
                        });
                        yield return combatMove;
                    }
                    break;
                case BodyPartRequirementType.ChildBodyPartGroup:
                    // the first constraint identifies a set of parts.  moves should one-to-one with this set
                    firstConstraint = attack.Constraints.First();
                    principalParts = new List<BodyPart>();
                    switch (firstConstraint.ConstraintType)
                    {
                        case BaConstraintType.ByType:
                            foreach (var part in body.Parts)
                            {
                                if (firstConstraint.Tokens.All(part.Types.Contains))
                                {
                                    principalParts.Add(part);
                                }
                            }
                            break;
                        case BaConstraintType.ByCategory:
                            foreach (var part in body.Parts)
                            {
                                if (firstConstraint.Tokens.All(part.Categories.Contains))
                                {
                                    principalParts.Add(part);
                                }
                            }
                            break;
                    }

                    foreach (var part in principalParts)
                    {
                        double totalContactArea = 0;
                        double totalPartSize = 0;
                        double maxLength = 0;

                        var subParts = IsReqMatch(attack, body, part);
                        foreach (var subPart in subParts)
                        {
                            double partCa = 0;
                            double partSize = 0;
                            double partLength = 0;
                            GetAttackProps(attack, subPart, totalBpRelSize, out partLength, out partSize, out partCa);
                            totalContactArea += partCa;
                            totalPartSize += partSize;
                            maxLength += partLength;
                        }

                        totalContactArea *= 1.5d;
                        totalPartSize /= 2d;
                        var contactRatio = (double)attack.ContactPercent / 100d;
                        var contactArea = totalContactArea * contactRatio;
                        var maxPen = (((double)attack.PenetrationPercent / 100d) * maxLength);

                        var combatMove =
                            new CombatMove
                            {
                                Name = attack.ReferenceName,
                                Verb = attack.Verb,
                                PrepTime = attack.PrepTime,
                                RecoveryTime = attack.RecoveryTime,
                                IsDefenderPartSpecific = true,
                                IsStrike = true,
                                IsMartialArts = true,
                                ContactType = attack.ContactType,
                                ContactArea = System.Math.Max(1, contactArea),
                                MaxPenetration = System.Math.Max(1, maxPen),
                                VelocityMultiplier = 1000,
                            };

                        combatMove.Requirements.Add(new BodyPartRequirement
                        {
                            Type = attack.RequirementType,
                            Constraints = attack.Constraints.Select(a => new BprConstraint
                            {
                                ConstraintType = (BprConstraintType)(int)a.ConstraintType,
                                Tokens = a.Tokens.ToList()
                            }).ToList()
                        });
                        yield return combatMove;
                    }
                    break;
                case BodyPartRequirementType.ChildTissueLayerGroup:

                    // the first constraint identifies a set of parts.  moves should one-to-one with this set
                    firstConstraint = attack.Constraints.First();
                    principalParts = new List<BodyPart>();
                    switch (firstConstraint.ConstraintType)
                    {
                        case BaConstraintType.ByType:
                            foreach (var part in body.Parts)
                            {
                                if (firstConstraint.Tokens.All(part.Types.Contains))
                                {
                                    principalParts.Add(part);
                                }
                            }
                            break;
                        case BaConstraintType.ByCategory:
                            foreach (var part in body.Parts)
                            {
                                if (firstConstraint.Tokens.All(part.Categories.Contains))
                                {
                                    principalParts.Add(part);
                                }
                            }
                            break;
                    }

                    foreach (var part in principalParts)
                    {
                        double totalContactArea = 0;
                        double totalPartSize = 0;
                        double maxLength = 0;

                        var subParts = IsReqMatch(attack, body, part);
                        foreach (var subPart in subParts)
                        {
                            double partCa = 0;
                            double partSize = 0;
                            double partLength = 0;
                            GetAttackProps(attack, subPart, totalBpRelSize, out partLength, out partSize, out partCa);
                            totalContactArea += partCa;
                            totalPartSize += partSize;
                            maxLength += partLength;
                        }


                        totalContactArea /= 2d;
                        totalPartSize /= 2d;
                        //maxLength /= 2d;
                        var contactRatio = (double)attack.ContactPercent / 100d;
                        var contactArea = totalContactArea * contactRatio;
                        var maxPen = (((double)attack.PenetrationPercent / 100d) * maxLength);

                        var combatMove =
                            new CombatMove
                            {
                                Name = attack.ReferenceName,
                                Verb = attack.Verb,
                                PrepTime = attack.PrepTime,
                                RecoveryTime = attack.RecoveryTime,
                                IsDefenderPartSpecific = true,
                                IsStrike = true,
                                IsMartialArts = true,
                                ContactType = attack.ContactType,
                                ContactArea = System.Math.Max(1, contactArea),
                                MaxPenetration = System.Math.Max(1, maxPen),
                                VelocityMultiplier = 1000,
                            };

                        combatMove.Requirements.Add(new BodyPartRequirement
                        {
                            Type = attack.RequirementType,
                            Constraints = attack.Constraints.Select(a => new BprConstraint
                            {
                                ConstraintType = (BprConstraintType)(int)a.ConstraintType,
                                Tokens = a.Tokens.ToList()
                            }).ToList()
                        });
                        yield return combatMove;
                    }
                    break;
            }
        }

        void GetAttackProps(DfBodyAttack attack, BodyPart part, int totalBpRelSize, out double maxLength, out double totalPartSize, out double totalContactArea)
        {
            maxLength = 0;
            totalPartSize = 0;
            totalContactArea = 0;
            double partRatio = ((double)part.RelativeSize / totalBpRelSize);
            var partSize = (partRatio * Size);
            var partLength = System.Math.Pow(partSize, 0.3333d);
            var bpContactArea = System.Math.Pow((partSize), 0.666d);
            
            maxLength += partSize;
            totalPartSize += partSize;
            totalContactArea += bpContactArea;
        }

        void SetMoves(Body body)
        {
            body.Moves = BodyAttacks.SelectMany(attack => CreateBodyCombatMove(attack, body)).ToList();
        }

        public void SetBloodMaterial(string matName)
        {
            BloodMaterial = Materials[matName];
        }

        public void SetPusMaterial(string matName)
        {
            PusMaterial = Materials[matName];
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

            foreach (var bpRelationDefn in BpRelationDefns)
            {
                var targetParts = BpRelationQuery(bpRelationDefn.TargetStrategy, bpRelationDefn.TargetStrategyParam, parts, partToDef);
                foreach (var target in targetParts)
                {
                    var bpRelation = new BodyPartRelation
                    {
                        Type = bpRelationDefn.RelationType,
                        Strategy = bpRelationDefn.RelatedPartStrategy,
                        StrategyParam = bpRelationDefn.RelatedPartStrategyParam,
                        Weight = bpRelationDefn.Weight
                    };

                    target.BodyPartRelations.Add(bpRelation);
                }
            }
            
            var sprite = new Sprite(Symbol, Foreground, Background);

            var required = new string[]{
                "STRENGTH"
            };
            var defaultValues = new List<int> { 200, 700, 900, 1000, 1100, 1300, 2000 };

            foreach (var requiredAttrName in required)
            {
                if (!Attributes.Any(x => x.Name.Equals(requiredAttrName)))
                {
                    Attributes.Add(new DfAttributeRange(requiredAttrName, defaultValues));
                }
            }
            
            var newAttrs = Attributes.Select(att =>
                        new Tiles.Content.Models.Attribute
                        {
                            Name = att.Name,
                            Median = att.Median
                        }).ToList();

            var body = new Body
                {
                    Parts = parts,
                    Size = Size,
                    Attributes = newAttrs,
                    BloodMaterial = BloodMaterial,
                    PusMaterial = PusMaterial
                };

            SetMoves(body);

            var agent = new Agent(
                Name, 
                body,
                sprite);
            return agent;
        }

        private IEnumerable<BodyPart> BpRelationQuery(BodyPartRelationStrategy strategy, string param, List<BodyPart> parts, Dictionary<BodyPart,DfObject> partToDef)
        {
            switch (strategy)
            {
                case BodyPartRelationStrategy.ByToken:
                    return parts.Where(x =>
                    {
                        var defn = partToDef[x];
                        return defn.Name.Equals(param);
                    });

                case BodyPartRelationStrategy.ByCategory:
                    return parts.Where(x => x.Categories.Contains(param));
                default:
                    throw new InvalidOperationException(string.Format("Unknown BpRelation part lookup strategy: '{0}'", strategy));
            }


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
            BodyAttacks.Add(move);
        }


        public void OverrideBodyPartCategorySize(string bpCategory, int size)
        {
            BpCatSizeOverrides[bpCategory] = size;
        }


        public void AddLifeStageSize(int ageYear, int ageDay, double size)
        {
            // TODO - make this part of racial model
            Size = size / 10d;
        }

        public void AddAttribute(DfAttributeRange ar)
        {
            Attributes.Add(ar);
        }


        public void AddBodyPartRelation(string targetStrategy, string targetStrategyParam, BodyPartRelationType relType, string relatedPartStrategy, string relatedPartStrategyParam, int weight)
        {
            BpRelationDefns.Add(new BpRelationDefn
            {
                TargetStrategy = Parse(targetStrategy),
                TargetStrategyParam = targetStrategyParam,
                RelationType = relType, 
                RelatedPartStrategy = Parse(relatedPartStrategy),
                RelatedPartStrategyParam = relatedPartStrategyParam,
                Weight = weight
            });
        }

        public void AddTissueLayerOverride(string tissueName, string strategy, string strategyParam, bool hasMajorArteries, string destTissueName)
        {
            TlOverrideDefns.Add(new TlOverrideDefn
            {
                SourceTissueName = tissueName,
                TargetStrategy = Parse(strategy),
                TargetStrategyParam = strategyParam,
                HasMajorArteries = hasMajorArteries,
                DestTissueName = destTissueName
            });
        }

        BodyPartRelationStrategy Parse(string strat)
        {
            switch (strat)
            {
                case "BY_TOKEN":
                    return BodyPartRelationStrategy.ByToken;
                case "BY_CATEGORY":
                    return BodyPartRelationStrategy.ByCategory;
                default:
                    throw new InvalidOperationException(string.Format("Unknown BpRelation part lookup strategy: '{0}'", strat));
            }
        }

        class BpRelationDefn
        {
            public BodyPartRelationStrategy TargetStrategy { get; set; }
            public string TargetStrategyParam { get; set; }
            public BodyPartRelationType RelationType { get; set; }
            public BodyPartRelationStrategy RelatedPartStrategy { get; set; }
            public string RelatedPartStrategyParam { get; set; }
            public int Weight { get; set; }
        }

        class TlOverrideDefn
        {
            public string SourceTissueName { get; set;}
            public BodyPartRelationStrategy TargetStrategy { get; set; }
            public string TargetStrategyParam { get; set; }
            public bool HasMajorArteries { get; set; }
            public string DestTissueName { get; set; }
        }
    }
}
