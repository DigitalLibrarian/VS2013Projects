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
            var partMap = bodyClass.Parts
                .ToDictionary(x => x, x => Convert(x));
            var parts = new List<IBodyPart>();
            foreach (var bpc in bodyClass.Parts)
            {
                if (bpc.Parent != null)
                {
                    partMap[bpc].Parent = partMap[bpc.Parent];
                }
                parts.Add(partMap[bpc]);
            }
            return new Body(parts);
        }

        BodyPart Convert(IBodyPartClass bpClass)
        {
            var tissue = TissueFactory.Create(bpClass.Tissue);
            return new BodyPart(bpClass, tissue);
        }
    }
}
