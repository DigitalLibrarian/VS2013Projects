using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Bodies;
using Tiles.Materials;
using Df = DwarfFortressNet.RawModels;

namespace DwarfFortressNet.Bridge
{
    public class DfCreaturePallete
    {
        ObjectDb Db { get; set; }

        Dictionary<string, IMaterial> Materials { get; set;}

        public DfCreaturePallete()
        {
            Db = new ObjectDb();
            Materials = new Dictionary<string, IMaterial>();
        }

        public void AddMaterial(string name, IMaterial material)
        {
            Materials[name] = material;
        }
        public IMaterial GetMaterial(string name)
        {
            return Materials[name];
        }
    }

    public class DfBodyConstructor
    {
        ObjectDb Db { get; set;}
        DfMaterialFactory MaterialFactory { get; set;}

        DfCreaturePallete Pallete { get; set; }
        Df.Creature Creature { get; set; }

        string CasteName { get; set; }
        public DfBodyConstructor(ObjectDb db, Df.Creature creature, string casteName)
        {
            Db = db;
            Creature = Creature.FromElement(creature.Element, casteName);
            MaterialFactory = new DfMaterialFactory(db);
            Pallete = new DfCreaturePallete();
            CasteName = casteName;
        }
        
        IMaterial BuildMaterial(string materialTemplateName)
        {
            var mt = Db.Get<MaterialTemplate>(materialTemplateName);
            return MaterialFactory.Create(mt);
        }

        IMaterial BuildTissueMaterial(string tissueTemplateName)
        {
            var tt = Db.Get<TissueTemplate>(tissueTemplateName);
            return MaterialFactory.Create(tt);
        }

        Dictionary<string, int> DefaultTissueThickness = new Dictionary<string, int>();
        void SetDefaultTissueThickness(string name, int thick)
        {
            DefaultTissueThickness[name] = thick;
        }

        Dictionary<string, Dictionary<string, int>> BodyPartTissueThickness = new Dictionary<string, Dictionary<string, int>>();
        void SetBodyPartTissueThickness(string bodyPart, string tissueName, int thick)
        {
            if (BodyPartTissueThickness.ContainsKey(bodyPart))
            {
                BodyPartTissueThickness[bodyPart][tissueName] = thick;
            }
            else
            {
                BodyPartTissueThickness[bodyPart] = new Dictionary<string, int>{
                    {tissueName, thick}
                };
            }
        }

        Dictionary<string, Dictionary<string, string>> BodyPartSideTissue = new Dictionary<string, Dictionary<string, string>>();
        void SetTissueForBodyPart(string bodyPart, string tissueName, string side)
        {
            if (BodyPartSideTissue.ContainsKey(bodyPart))
            {
                BodyPartSideTissue[bodyPart][side] = tissueName;
            }
            else
            {
                BodyPartSideTissue[bodyPart] = new Dictionary<string, string>{
                    {side, tissueName}
                };
            }
        }

        Dictionary<string, List<string>> BodyPartTissueOrder = new Dictionary<string, List<string>>();
        void SetBodyPartTissueOrder(string bodyPart, List<string> tissues)
        {
            BodyPartTissueOrder[bodyPart] = tissues;
        }

        //[BODY_DETAIL_PLAN:STANDARD_MATERIALS]
        //[BODY_DETAIL_PLAN:VERTEBRATE_TISSUE_LAYERS:SKIN:FAT:MUSCLE:BONE:CARTILAGE]
        public void Tag_BodyDetailPlan(string name, params string[] args)
        {
            var plan = Db.Get<BodyDetailPlan>().Single(x => x.ReferenceName.Equals(name));

            var tissueOrder = args;
            
            foreach (var addMat in plan.AddMaterials)
            {
                AddMaterialFromMaterialTemplate(addMat.Identifier, addMat.MaterialTemplate);
            }
            foreach (var addTissue in plan.AddTissues)
            {
                AddMaterialFromTissueTemplate(addTissue.Identifier, addTissue.TissueTemplate);
            }

            foreach (var bpLayers in plan.BpLayers)
            {
                if (bpLayers.Strategy == "BY_CATEGORY")
                {
                    foreach (var part in PlannedBodyParts.Values
                        .Where(x => x.Category.Equals(bpLayers.Category)))
                    {
                        var bpTissueOrder = new List<string>();
                        foreach (var tissueRef in bpLayers.Thicknesses)
                        {
                            var tissueName = tissueRef.IsArg
                                ? tissueOrder[tissueRef.Index] : tissueRef.Name;
                            int relThick = tissueRef.Thickness;

                            SetBodyPartTissueThickness(part.ReferenceName,
                                tissueName, relThick);
                            bpTissueOrder.Add(tissueName);
                        }
                        SetBodyPartTissueOrder(
                            part.ReferenceName,
                            bpTissueOrder);
                    }
                }
                else
                {
                    throw new NotImplementedException(string.Format("Unknown BPLAYERS strategy: {0}", bpLayers.Strategy));
                }
            }
            
        }

        void AddMaterialFromMaterialTemplate(string name, string template)
        {
            // creates a material
            var material = BuildMaterial(template);
            Pallete.AddMaterial(name, material);
        }

        void AddMaterialFromTissueTemplate(string name, string template)
        {
            var material = BuildTissueMaterial(template);
            Pallete.AddMaterial(name, material);
            SetDefaultTissueThickness(name,
                Db.Get<TissueTemplate>(template).RelativeThickness
                );

        }
        
	    //[USE_MATERIAL_TEMPLATE:NAIL:NAIL_TEMPLATE]
        public void Tag_UseMaterialTemplate(string name, string template)
        {
            AddMaterialFromMaterialTemplate(name, template);
        }

	    //[USE_TISSUE_TEMPLATE:EYEBROW:EYEBROW_TEMPLATE]
        public void Tag_UseTissueTemplate(string name, string template)
        {
            AddMaterialFromTissueTemplate(name, template);
        }

        //[TISSUE_LAYER:BY_CATEGORY:FINGER:NAIL:FRONT]
        public void Tag_TissueLayer(string strategy, string bodyPart, string tissueName, string side)
        {
            SetTissueForBodyPart(bodyPart, tissueName, side);
        }

        Dictionary<string, Df.BodyPart> PlannedBodyParts = new Dictionary<string, Df.BodyPart>();
        //[BODY:HUMANOID_NECK:2EYES:2EARS:NOSE:2LUNGS:HEART:GUTS:ORGANS:HUMANOID_JOINTS:THROAT:NECK:SPINE:BRAIN:SKULL:5FINGERS:5TOES:MOUTH:TONGUE:FACIAL_FEATURES:TEETH:RIBCAGE]
        public void Tag_Body(params string[] bodyPartSetNames)
        {
            foreach (var bodyPartSetName in bodyPartSetNames)
            {
                var bodyPartSet = Db.Get<Df.BodyPartSet>(bodyPartSetName);
                foreach (var bp in bodyPartSet.BodyParts)
                {
                    PlannedBodyParts.Add(bp.ReferenceName, bp);
                }
            }
        }

        //[APPLY_CREATURE_VARIATION:STANDARD_BIPED_GAITS:900:711:521:293:1900:2900] 30 kph
        //[APPLY_CREATURE_VARIATION:STANDARD_CLIMBING_GAITS:5951:5419:4898:1463:6944:8233] 6 kph
        //[APPLY_CREATURE_VARIATION:STANDARD_SWIMMING_GAITS:5951:5419:4898:1463:6944:8233] 6 kph
        //[APPLY_CREATURE_VARIATION:STANDARD_CRAWLING_GAITS:2990:2257:1525:731:4300:6100] 12 kph
        public void Tag_ApplyCreatureVariation(string variation, params string[] args) 
        {
            var cv = Db.Get<CreatureVariation>(variation);

            Creature.Tokens.AddRange(cv.CvNewTags.ToList());
            foreach (var removeTag in cv.CvRemoveTags)
            {
                foreach (var token in Creature.Tokens.Where(x => removeTag == x.Name).ToList())
                {
                    Creature.Tokens.Remove(token);
                }
            }

            foreach (var convertTag in cv.CvConvertTags)
            {
                var paramPattern = convertTag.Target;
                var master = convertTag.Master;
                var targets = Creature.Tokens.Where(x => x.Name.Equals(master));
                if (convertTag.Replacement.Any())
                {
                    foreach (var token in targets)
                    {
                        Creature.Tokens.Remove(token);
                    }
                }
                else
                {
                    foreach (var token in targets)
                    {
                        var newWords = new List<string>();
                        foreach (var word in token.Words)
                        {
                            if (word.Contains(paramPattern))
                            {
                                newWords.AddRange(convertTag.Replacement);
                            }
                            else
                            {
                                newWords.Add(word);
                            }
                        }

                        int index = Creature.Tokens.FindIndex(x => x == token);
                        Creature.Tokens.RemoveAt(index);
                        Creature.Tokens.Add(new Tag { Words = newWords });
                    }
                }
            }
        }


        public Tiles.Bodies.IBody Build()
        {
            var firstPass = Creature.Tokens.ToList();
            foreach (var tag in firstPass)
            {
                if (tag.Name == "APPLY_CREATURE_VARIATION")
                {
                    Tag_ApplyCreatureVariation(tag.Words[1], tag.Words.Skip(2).ToArray());
                }
            }

            foreach (var tag in Creature.Tokens)
            {
                switch (tag.Name)
                {
                    case "BODY_DETAIL_PLAN":
                        Tag_BodyDetailPlan(tag.Words[1], tag.Words.Skip(2).ToArray());
                        break;
                    case "USE_MATERIAL_TEMPLATE":
                        Tag_UseMaterialTemplate(tag.Words[1], tag.Words[2]);
                        break;
                    case "USE_TISSUE_TEMPLATE":
                        Tag_UseTissueTemplate(tag.Words[1], tag.Words[2]);
                        break;
                    case "TISSUE_LAYER":
                        // TODO - figure this command out
                        // I think it allows for different sides of a body part to have different tissues
                        //[TISSUE_LAYER:BY_CATEGORY:HEAD:EYEBROW:ABOVE:BY_CATEGORY:EYE]
                        //Tag_TissueLayer(tag.Words[1], tag.Words[2], tag.Words[3], tag.Words[4]);
                        break;
                    case "BODY":
                        Tag_Body(tag.Words.Skip(1).ToArray());
                        break;
                }
            }

            var bBuilder = new DfBodyBuilder();

            foreach (var part in PlannedBodyParts.Values)
            {
                var layers = new List<DfBodyBuilder.TissueLayer>();
                foreach (var tissueName in GetBodyPartTissueOrder(part.ReferenceName))
                {
                    if (!tissueName.Equals("NONE"))
                    {
                        layers.Add(new DfBodyBuilder.TissueLayer
                        {
                            RelativeThickness = GetBodyPartTissueThickness(part.ReferenceName, tissueName),
                            Material = GetBodyPartTissueMaterial(part.ReferenceName, tissueName)
                        });
                    }
                }

                bBuilder.AddPart(part, layers);
            }


            return bBuilder.Build();
        }

        private IEnumerable<string> GetBodyPartTissueOrder(string bodyPartName)
        {
            if (BodyPartTissueOrder.ContainsKey(bodyPartName))
            {
                if (BodyPartTissueThickness.ContainsKey(bodyPartName))
                {
                    return BodyPartTissueOrder[bodyPartName];
                }
            }
            return Enumerable.Empty<string>();
        }

        private IMaterial GetBodyPartTissueMaterial(string p, string tissueName)
        {
            return Pallete.GetMaterial(tissueName);
        }

        private int GetBodyPartTissueThickness(string p, string tissueName)
        {
            if (BodyPartTissueThickness.ContainsKey(p))
            {
                if (BodyPartTissueThickness[p].ContainsKey(tissueName))
                {
                    return BodyPartTissueThickness[p][tissueName];
                }
            }

            if (DefaultTissueThickness.ContainsKey(tissueName))
            {
                return DefaultTissueThickness[tissueName];
            }
            return 0;
        }
    }

    public class DfBodyBuilder
    {
        public class TissueLayer
        {
            public int RelativeThickness { get; set; }
            public IMaterial Material { get; set; }
        }

        Dictionary<Df.BodyPart, List<TissueLayer>> BpLayers = new Dictionary<Df.BodyPart, List<TissueLayer>>();

        public void AddPart(Df.BodyPart bp, List<TissueLayer> layers) 
        {
            BpLayers[bp] = layers;
        }

        bool IsRoot(Df.BodyPart bodyPart)
        {
            return bodyPart.Con == null && bodyPart.ConType == null && bodyPart.ConCat == null;
        }

        ICollection<Df.BodyPart> BodyPartDefns { get { return BpLayers.Keys; } }
        
        public IBody Build()
        {
            var parts = new List<IBodyPart>();
            if (BodyPartDefns.Any())
            {
                var defn = BodyPartDefns.Single(IsRoot);

                var part = Create(defn);
                parts.Add(part);
                var partMap = new Dictionary<IBodyPart, Df.BodyPart>{{part, defn}};
                // dependency resolve from root part
                for (int i = 0; i < parts.Count(); i++)
                {
                    // we just created part, and are looking for things that connect to it
                    part = parts[i];
                    defn = partMap[part];

                    var toLoad = new List<Df.BodyPart>();
                    // if we have a category, then we look for parts that connect to that category
                    if (defn.Category != null)
                    {
                        toLoad.AddRange(BodyPartDefns.Where(x => x.ConCat == defn.Category));
                    }

                    // Look for any that connect to use via a ConType
                    toLoad.AddRange(
                        BodyPartDefns.Where(
                            x => x.ConType != null 
                                && defn.Tokens.Any(t => t.IsSingleWord(x.ConType))));

                    // Look for any that are directly connected to the part
                    toLoad.AddRange(BodyPartDefns.Where(x => x.Con == defn.ReferenceName));

                    foreach (var partDefn in toLoad)
                    {
                        var newPart = Create(part, partDefn);
                        parts.Add(newPart);
                        partMap.Add(newPart, partDefn);
                    }
                }
            }

            return new Body(parts);
        }

        IBodyPart Create(IBodyPart parent, Df.BodyPart partDefn)
        {
            return CreatePart(partDefn, parent);
        }

        IBodyPart Create(Df.BodyPart partDefn)
        {
            return CreatePart(partDefn);
        }

        IBodyPart CreatePart(Df.BodyPart partDefn, IBodyPart parentBP = null)
        {
            return new Tiles.Bodies.BodyPart(
                    new BodyPartClass(
                        name: partDefn.Name,
                        isCritical: partDefn.Tokens.Any(x => x.IsSingleWord("THOUGHT")),
                        canAmputate: false,
                        canGrasp: partDefn.Tokens.Any(token => token.IsSingleWord("GRASP")),
                        armorSlotType: Tiles.Items.ArmorSlot.None,
                        weaponSlotType: Tiles.Items.WeaponSlot.None
                        ),
                    tissue: CreateTissue(partDefn),
                    parent: parentBP
                    ); ;
        }

        ITissue CreateTissue(Df.BodyPart partDefn)
        {
            var layers = BpLayers[partDefn].Select<TissueLayer, ITissueLayer>(x => new Tiles.Bodies.TissueLayer(x.Material, x.RelativeThickness)).ToList();
            return new Tissue(layers);
        }
    }
}
