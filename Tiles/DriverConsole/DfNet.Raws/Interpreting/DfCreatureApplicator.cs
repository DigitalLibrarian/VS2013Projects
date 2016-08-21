using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting
{
    public class DfCreatureApplicator : IContextApplicator
    {
        DfObject CDefn { get; set; }
        IDfObjectInterpreter Interpreter { get; set; }
        public DfCreatureApplicator(DfObject cDefn)
        {
            CDefn = cDefn;

            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_CopyTagsFrom(),
                new TagInterpreter_GoToStart(),
                new TagInterpreter_GoToEnd(),
                new TagInterpreter_GoToTag(),
                new TagInterpreter_ApplyCreatureVariation(),
                new TagInterpreter_CvConvertTag()
                //new TagInterpreter_CreatureBodyPartInclude(),
                //new TagInterpreter_BodyDetailPlanInclude()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context,  context.Source.Tags, true);
        }
    }


    public class DfBodyApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }
        public DfBodyApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_CreatureBodyPartInclude(),
                new TagInterpreter_BodyDetailPlanInclude()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);
        }
    }



    public class DfMaterialApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }

        
        public DfMaterialApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_UseMaterialTemplate(),
                new TagInterpreter_AddMaterial(),
                new TagInterpreter_RemoveMaterial()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);
            


        }
    }



    public class DfTissueApplicator : IContextApplicator
    {
        IDfObjectInterpreter Interpreter { get; set; }


        public DfTissueApplicator()
        {
            Interpreter = new DfObjectInterpreter(
                new TagInterpreter_UseTissueTemplate(),
                new TagInterpreter_AddTissue(),
                new TagInterpreter_RemoveTissue()
                );
        }

        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            Interpreter.Interpret(store, context, context.Source.Tags, true);



        }
    }
}
