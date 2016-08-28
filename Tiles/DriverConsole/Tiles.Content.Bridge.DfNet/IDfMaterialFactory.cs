using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfMaterialFactory
    {
        Material CreateInorganic(string name);
        Material CreateTissue(string tissueTemplate);
        Material CreateFromMaterialTemplate(string materialTemplate);

        Material CreateFromTissueCreatureInline(string creatureName, string tisName);
    }
}
