using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyFactory : IBodyFactory
    {
        ITissueFactory TissueFactory { get; set; }

        public BodyFactory(ITissueFactory tissueFactory)
        {
            TissueFactory = tissueFactory;
        }

        public IBody Create(IBodyClass bodyClass)
        {
            int totalBpRelSize = bodyClass.TotalBodyPartRelSize;
            // TODO - this should be based on the instance strength
            var strength = bodyClass.Attributes.Single(x => x.Name.Equals("STRENGTH")).Median;

            var partMap = bodyClass.Parts
                .ToDictionary(x => x, x => Convert(x, bodyClass.Size, totalBpRelSize, strength));

            var parts = new List<IBodyPart>();
            foreach (var bpc in bodyClass.Parts)
            {
                if (bpc.Parent != null)
                {
                    partMap[bpc].Parent = partMap[bpc.Parent];
                }
                parts.Add(partMap[bpc]);
            }
            
            var body = new Body(parts, bodyClass.Size, bodyClass.Moves);

            foreach(var attrClass in bodyClass.Attributes)
            {
                var name = attrClass.Name;
                var value = attrClass.Median;
                body.SetAttribute(name, value);
            }

            return body;
        }

        BodyPart Convert(IBodyPartClass bpClass, double bodySize, int totalBodyPartRelSize, double strength)
        {
            var bpFact = (double)bpClass.RelativeSize / (double)totalBodyPartRelSize;
            var partSizeD = (double)bodySize * bpFact;
            var tissue = TissueFactory.Create(bpClass.Tissue, partSizeD, strength);
            return new BodyPart(bpClass, tissue, partSizeD);
        }
    }
}
