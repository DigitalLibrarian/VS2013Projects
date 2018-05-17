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
        void SetBloodMaterial(string matName);
        void SetPusMaterial(string tisName);

        void AddBody(string name, DfObject bpObject);
        void AddMaterial(string matName, Material material);
        void AddTissueTemplate(string matName, DfTissueTemplate tissueTemplate);
        void RemoveMaterial(string matName);

        void AddTissueToBodyPart(string bpName, string tisName);

        void SetBodyPartTissueThickness(string bpName, string tisName, int relThick);
        
        Agent Build();

        void OverrideBodyPartCategorySize(string bpCategory, int size);

        void AddLifeStageSize(int ageYear, int ageDay, double size);

        void AddBodyAttack(DfBodyAttack move);

        void AddAttribute(DfAttributeRange ar);

        void AddBodyPartRelation(string targetStrategy, string targetStrategyParam, BodyPartRelationType relType, string relatedPartStrategy, string relatedPartStrategyParam, int weight);

        void AddTissueLayerOverride(string tissueName, string strategy, string strategyParam, bool hasMajorArteries, string destTissueName);
    }
}
