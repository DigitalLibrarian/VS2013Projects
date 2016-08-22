using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws.Interpreting.TagInterpreters
{
    public class TagInterpreter_AddMaterial : TagInterpreter_UseMaterialTemplate
    {
        public override string TagName
        {
            get { return DfTags.MiscTags.ADD_MATERIAL; }
        }
    }
}
