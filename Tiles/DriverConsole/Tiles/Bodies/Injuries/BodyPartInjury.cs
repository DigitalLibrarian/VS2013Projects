using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Bodies.Injuries
{
    public interface IBodyPartInjury
    {
        IBodyPart BodyPart { get; }
        IEnumerable<ITissueLayerInjury> TissueLayerInjuries { get; }

        bool IsSever { get; }
    }

    public class BodyPartInjury : IBodyPartInjury
    {
        public IBodyPart BodyPart { get; private set; }
        public IEnumerable<ITissueLayerInjury> TissueLayerInjuries { get; private set; }

        public bool IsSever
        {
            get
            {
                if (BodyPart.Class.IsInternal) return false;

                var connectiveLayers = BodyPart.Tissue.TissueLayers
                    .Where(x => x.Class.IsConnective);

                if (!connectiveLayers.Any())
                {
                    return false;
                }

                if (!connectiveLayers.Any(x => !x.IsPulped()))
                {
                    return true;
                }

                foreach (var layer in connectiveLayers)
                {
                    var layerInjuries = TissueLayerInjuries
                        .Where(x => x.Layer == layer);

                    if (!layerInjuries.Any()) return false;

                    foreach (var injury in layerInjuries)
                    {
                        if (!injury.StrikeResult.IsDefeated
                            || injury.StrikeResult.PenetrationRatio < 1d)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public BodyPartInjury(IBodyPart bodyPart,
            IEnumerable<ITissueLayerInjury> tissueLayerInjuries)
        {
            BodyPart = bodyPart;
            TissueLayerInjuries = tissueLayerInjuries;
        }
    }
}
