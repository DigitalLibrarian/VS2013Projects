using DwarfFortressNet.RawModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class DfAgentBuilder
    {
        ObjectDb Db { get; set;}
        DfMaterialFactory MaterialFactory { get; set;}

        DfCreaturePallete Pallete { get; set; }
        Df.Creature Creature { get; set; }
        public DfAgentBuilder(ObjectDb db, Df.Creature creature)
        {
            Db = db;
            Creature = creature;
            MaterialFactory = new DfMaterialFactory(db);
            Pallete = new DfCreaturePallete();
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
                        foreach (var tissueRef in bpLayers.Thicknesses)
                        {
                            var tissueName = tissueRef.IsArg
                                ? tissueOrder[tissueRef.Index] : tissueRef.Name;
                            int relThick = tissueRef.Thickness;

                            SetBodyPartTissueThickness(part.ReferenceName,
                                tissueName, relThick);
                            SetBodyPartTissueOrder(
                                part.ReferenceName,
                                tissueOrder.ToList());
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
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

        public Tiles.Agents.IAgent Build()
        {
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
                    layers.Add(new DfBodyBuilder.TissueLayer
                    {
                        RelativeThickness = GetBodyPartTissueThickness(part.ReferenceName, tissueName),
                        Material = GetBodyPartTissueMaterial(part.ReferenceName, tissueName)
                    });
                }

                bBuilder.AddPart(part, layers);
                Console.WriteLine(part.ReferenceName + "\t" + layers.Count());
            }


            return null;
        }

        private IEnumerable<string> GetBodyPartTissueOrder(string p)
        {
            if (BodyPartTissueOrder.ContainsKey(p))
            {
                return BodyPartTissueOrder[p];
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

        public void AddPart(Df.BodyPart bp, List<TissueLayer> layers) {}
    }


    public class DfCreatureBodyBuilder
    {
        Dictionary<string, List<Df.BodyPart>> PartDefsByToken { get; set; }

        Dictionary<string, List<IBodyPart>> CreatedPartsByToken { get; set; }
        Dictionary<string, IBodyPart> CreatedPartsByName { get; set; }

        IList<IBodyPart> BodyParts { get; set; }

        Creature Creature { get; set; }
        ObjectDb Db { get; set; }
        public DfCreatureBodyBuilder(Creature creature, ObjectDb objDb)
        {
            Creature = creature;
            Db = objDb;

            Clear();
        }

        public void Clear()
        {
            PartDefsByToken = new Dictionary<string, List<Df.BodyPart>>();
            CreatedPartsByName = new Dictionary<string, IBodyPart>();
            CreatedPartsByToken = new Dictionary<string, List<IBodyPart>>();

            BodyParts = new List<IBodyPart>();
        }

        #region Data View
        public IEnumerable<Df.BodyPart> AllPartDefns
        {
            get
            {
                return Creature.BodyPartSets.Select(refName => Db.Get<Df.BodyPartSet>(refName)).SelectMany(set => set.BodyParts);
            }
        }

        public IEnumerable<Df.BodyPart> RootPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.Con == null && part.ConType == null);
            }
        }

        public IEnumerable<Df.BodyPart> SpecificallyConnectedPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.Con != null);
            }
        }

        public IEnumerable<Df.BodyPart> TokenConnectedPartDefns
        {
            get
            {
                return AllPartDefns.Where(part => part.ConType != null);
            }
        }

        #endregion

        void RecordPart(Df.BodyPart dfPart, IBodyPart part)
        {
            var refName = dfPart.ReferenceName;
            CreatedPartsByName.Add(refName, part);

            foreach (var tokenName in dfPart.Tokens.Where(x => x.IsSingleWord()).Select(x => x.Words.First()))
            {
                if (CreatedPartsByToken.ContainsKey(tokenName))
                {
                    CreatedPartsByToken[tokenName].Add(part);
                }
                else
                {
                    CreatedPartsByToken[tokenName] = new List<IBodyPart> { part };
                }
            }
        }

        public void AddRootParts()
        {
            foreach (var part in RootPartDefns)
            {
                var bp = CreatePart(part);

                RecordPart(part, bp);
                BodyParts.Add(bp);
            }
        }

        public void AddSpecificallyConnectedParts()
        {
            foreach (var part in SpecificallyConnectedPartDefns)
            {
                var parentBP = CreatedPartsByName[part.Con];

                var bp = CreatePart(part, parentBP);

                RecordPart(part, bp);
                BodyParts.Add(bp);
            }
        }

        IBodyPart CreatePart(Df.BodyPart part, IBodyPart parentBP = null)
        {
            return new Tiles.Bodies.BodyPart(
                    new BodyPartClass(
                        name: part.Name,
                        isCritical: part.Tokens.Any(x => x.IsSingleWord("THOUGHT")),
                        canAmputate: false,
                        canGrasp: part.Tokens.Any(token => token.IsSingleWord("GRASP")),
                        armorSlotType: Tiles.Items.ArmorSlot.None,
                        weaponSlotType: Tiles.Items.WeaponSlot.None
                        ),
                    parent: parentBP
                    ); ;
        }

        public void AddTokenConnectedParts()
        {
            foreach (var part in TokenConnectedPartDefns)
            {
                foreach (var targetParent in CreatedPartsByToken[part.ConType])
                {
                    var bp = CreatePart(part, targetParent);

                    BodyParts.Add(bp);
                }
            }
        }

        public IBody Build()
        {
            return new Body(BodyParts);
        }
        

        public static IBody FromCreatureDefinition(Creature c, ObjectDb objDb)
        {
            var builder = new DfCreatureBodyBuilder(c, objDb);
            builder.AddRootParts();
            builder.AddSpecificallyConnectedParts();
            builder.AddTokenConnectedParts();
            return builder.Build();
        }
    }
}
