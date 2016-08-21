using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DfNet.Raws.Interpreting
{
    public class DfCasteApplicator : IContextApplicator
    {
        private string CasteName { get; set; }

        public DfCasteApplicator(string casteName)
        {
            CasteName = casteName;
        }
        public void Apply(IDfObjectStore store, IDfObjectContext context)
        {
            var addCastes = new List<string>() { "ALL" };
            string parseCaste = "ALL";

            var newTags = new List<DfTag>();
            foreach (var tag in context.Source.Tags)
            {
                //[CASTE:<CASTE_NAME>] defines a caste called <CASTE_NAME>. Tags following this affect only this caste.

                //[SELECT_CASTE:ALL] state the following tags affect all Castes

                //[SELECT_CASTE:<CASTE_1>]

                //[SELECT_ADDITIONAL_CASTE:<CASTE_2>]

                //[SELECT_ADDITIONAL_CASTE:<CASTE_3>], etc., is used to specify that tags affect a subset of Castes                

                switch (tag.Name)
                {
                    case DfTags.MiscTags.CASTE:
                        parseCaste = tag.GetParam(0);
                        break;
                    case DfTags.MiscTags.SELECT_CASTE:
                        parseCaste = tag.GetParam(0);
                        break;
                    case DfTags.MiscTags.SELECT_ADDITIONAL_CASTE:
                        addCastes.Add(tag.GetParam(0));
                        break;
                    default:
                        if (parseCaste == CasteName || addCastes.Contains(parseCaste))
                        {
                            newTags.Add(tag.CloneDfTag());
                        }
                        break;
                }

            }

            context.InsertTags(newTags.ToArray());
        }


        public static IEnumerable<string> FindCastes(DfObject dfObj)
        {
            foreach (var tag in dfObj.Tags)
            {
                if (tag.Name.Equals(DfTags.MiscTags.CASTE))
                {
                    yield return tag.GetParam(0);
                }
            }
        }
    }
}
