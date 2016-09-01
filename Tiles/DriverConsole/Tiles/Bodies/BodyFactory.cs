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
            int totalBpRelSize = bodyClass.Parts
                .Select(x => x.RelativeSize)
                .Sum();

            var partMap = bodyClass.Parts
                .ToDictionary(x => x, x => Convert(x, bodyClass.Size, totalBpRelSize));

            var parts = new List<IBodyPart>();
            foreach (var bpc in bodyClass.Parts)
            {
                if (bpc.Parent != null)
                {
                    partMap[bpc].Parent = partMap[bpc.Parent];
                }
                parts.Add(partMap[bpc]);
            }
            return new Body(parts, bodyClass.Size);
        }

        BodyPart Convert(IBodyPartClass bpClass, int bodySize, int totalBodyPartRelSize)
        {
            var tissue = TissueFactory.Create(bpClass.Tissue, bodySize);
            int partSize = (int) ((double)bodySize * ((double)bpClass.RelativeSize / (double)totalBodyPartRelSize));
            return new BodyPart(bpClass, tissue, partSize);
        }
    }
}
