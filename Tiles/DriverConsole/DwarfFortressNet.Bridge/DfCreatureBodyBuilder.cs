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

        public Tiles.Bodies.IBody Build()
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
            var defn = BodyPartDefns.Single(IsRoot);

            var parts = new List<IBodyPart>();
            var part = Create(defn);
            parts.Add(part);
            var partMap = new Dictionary<IBodyPart, Df.BodyPart>();
            partMap.Add( part, defn);

            for (int i = 0; i < parts.Count(); i++)
            {
                part = parts[i];
                defn = partMap[part];

                var toLoad = new List<Df.BodyPart>();
                if (defn.Category != null)
                {
                    toLoad.AddRange(BodyPartDefns.Where(x => x.ConCat == defn.Category));
                }

                toLoad.AddRange(BodyPartDefns.Where(x => x.ConType != null && defn.Tokens.Any(t => t.IsSingleWord(x.ConType))));
                toLoad.AddRange(BodyPartDefns.Where(x => x.Con == defn.ReferenceName));

                foreach (var partDefn in toLoad)
                {
                    var newPart = Create(part, partDefn);
                    parts.Add(newPart);
                    partMap.Add(newPart, partDefn);
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
    }
}
