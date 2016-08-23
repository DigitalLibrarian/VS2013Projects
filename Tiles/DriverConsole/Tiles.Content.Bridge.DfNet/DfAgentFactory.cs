using DfNet.Raws;
using DfNet.Raws.Interpreting;
using DfNet.Raws.Interpreting.Applicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfAgentFactory : IDfAgentFactory
    {
        IDfObjectStore Store { get; set; }
        IDfAgentBuilderFactory BuilderFactory { get; set; }

        public DfAgentFactory(IDfObjectStore store, IDfAgentBuilderFactory builderFactory)
        {
            Store = store;
            BuilderFactory = builderFactory;
        }

        public Agent Create(string name, string caste)
        {
            var creatureDf = Store.Get(DfTags.CREATURE, name);
            // this shit needs to be refactored
            var context = new DfObjectContext(creatureDf);
            ApplyPass(new DfCreatureApplicator(creatureDf), context);

            if (caste != null)
            {
                ApplyPass(new DfCasteApplicator(caste), context);
            }
            
            creatureDf = context.Create();
            // now we have a creature definition that :
            // 1. has had creature variations applied
            // 2. GoTo/CopyFrom has been applied
            // 3. CvConvert,New,andRemove tags have been applied
            // 4. Body and body detail plans have been replaced with 
            // paramed content

            // we need to :
            // 2. Create the tissues
            // 3. Create the materials
            // 4. Return as nested Agent model
            string strategy, bpName, tisName;
            var agentContext = BuilderFactory.Create();
            foreach (var tag in creatureDf.Tags)
            {
                switch (tag.Name)
                {
                    case DfTags.MiscTags.USE_MATERIAL_TEMPLATE:
                        var matObj = GetMaterialFromTemplate(tag, DfTags.MATERIAL_TEMPLATE);
                        agentContext.AddMaterialFromTemplate(
                            tag.GetParam(0), 
                            matObj);
                        break;
                    case DfTags.MiscTags.REMOVE_MATERIAL:
                        agentContext.RemoveMaterial(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.USE_TISSUE_TEMPLATE:
                        var tisObj = GetMaterialFromTemplate(tag, DfTags.TISSUE_TEMPLATE);
                        agentContext.AddMaterialFromTemplate(
                            tag.GetParam(0),
                            tisObj);
                        break;
                    case DfTags.MiscTags.REMOVE_TISSUE:
                        agentContext.RemoveMaterial(tag.GetParam(0));
                        break;
                    case DfTags.MiscTags.TISSUE_LAYER:
                        strategy = tag.GetParam(0);
                        if (!strategy.Equals(DfTags.MiscTags.BY_CATEGORY))
                        {
                            throw new NotImplementedException();
                        }
                        bpName = tag.GetParam(1);
                        tisName = tag.GetParam(2);
                        agentContext.AddTissueToBodyPart(bpName, tisName);
                        break;
                    case DfTags.MiscTags.BP_LAYERS: 
                        strategy = tag.GetParam(0);
                        if (!strategy.Equals(DfTags.MiscTags.BY_CATEGORY))
                        {
                            throw new NotImplementedException();
                        }
                        bpName = tag.GetParam(1);
                        tisName = tag.GetParam(2);
                        int relThick = int.Parse(tag.GetParam(3));
                        agentContext.SetBodyPartTissueThickness(bpName, tisName, relThick);

                        break;
                    case DfTags.BODY:
                        foreach (var bodyName in tag.GetParams())
                        {
                            agentContext.AddBody(bodyName, Store.Get(DfTags.BODY, bodyName));
                        }
                        break;


                    //case DfTags.MiscTags.BP_RELSIZE:
                    //    break;
                    //case DfTags.MiscTags.BP_POSITION:
                    //    break;
                    //case DfTags.MiscTags.BP_RELATION:
                    //    break;

                    // TODO - these add tags to the various tissue
                    // not currently needed for Tiles sime
                    //case DfTags.MiscTags.SELECT_TISSUE_LAYER:

                    //    break;
                    //case DfTags.MiscTags.PLUS_TISSUE_LAYER:

                    //    break;

                    // TODO - these are unarmed attacks built-in to the body
                    //case DfTags.MiscTags.ATTACK:
                    //    break;
                }
            }

            return agentContext.Build();
        }
                
        private DfObject GetMaterialFromTemplate(DfTag tag, string type)
        {
            var tempName = tag.GetParam(1);
            var matObj = Store.Get(type, tempName);
            return matObj.CloneDfObject();
        }
        
        void ApplyPass(IContextApplicator app, IDfObjectContext context)
        {
            context.StartPass();
            app.Apply(Store, context);
            context.EndPass();
        }
    }
}
