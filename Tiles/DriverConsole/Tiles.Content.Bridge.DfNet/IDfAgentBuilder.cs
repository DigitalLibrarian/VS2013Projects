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
        void SetName(string singular, string plural);
        void SetSymbol(int symbol);
        void SetForegroundColor(Color color);
        void SetBackgroundColor(Color color);

        void AddBody(string name, DfObject bpObject);
        void AddMaterial(string matName, Material material);
        void RemoveMaterial(string matName);

        void AddTissueToBodyPart(string bpName, string tisName);

        void SetBodyPartTissueThickness(string bpName, string tisName, int relThick);
        
        void AddCombatMoveToCategory(CombatMove move, string bpCategory);

        void AddCombatMoveToType(CombatMove move, string type);

        Agent Build();

        void OverrideBodyPartCategorySize(string bpCategory, int size);
    }
}
