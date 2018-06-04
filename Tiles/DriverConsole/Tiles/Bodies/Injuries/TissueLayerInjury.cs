using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Bodies.Injuries
{
    public interface ITissueLayerInjury
    {
        ITissueLayer Layer { get; }
        IDamageVector Damage { get; }
        MaterialStrikeResult StrikeResult { get; }

        bool IsChip { get; }
        bool IsSoft { get; }
        bool IsVascular { get; }

        double WoundArea { get; }
    }

    class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayer Layer { get; private set; }
        public IBodyPart BodyPart { get; private set; }
        public IDamageVector Damage { get; private set; }

        public MaterialStrikeResult StrikeResult { get; private set; }
        public TissueLayerInjury(IBodyPart bodyPart, ITissueLayer layer, IDamageVector damage, MaterialStrikeResult strikeResult)
        {
            BodyPart = bodyPart;
            Layer = layer;
            StrikeResult = strikeResult;
            Damage = damage;
        }

        public double WoundArea
        {
            get
            {
                if (StrikeResult.PenetrationRatio >= 1d)
                {
                    return StrikeResult.ContactArea;
                }
                return 0;
            }
        }

        public bool IsChip
        {
            get
            {
                return StrikeResult.ContactArea <= BodyPart.ContactArea * 0.25d;
            }
        }

        public bool IsSoft
        {
            get
            {
                return Layer.Material.IsSoft(StrikeResult.StressMode);
            }
        }

        public bool IsVascular
        {
            get
            {
                return Layer.IsVascular;
            }
        }
    }
}
