using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfAgentBuilder
    {
        void AddBody(string name, DfObject bpObject);

        void AddMaterialFromTemplate(string matName, DfObject matObj);
        void RemoveMaterial(string matName);
        void AddTissueToBodyPart(string bpName, string tisName);

        void SetBodyPartTissueThickness(string bpName, string tisName, int relThick);

        Agent Build();
    }
}
