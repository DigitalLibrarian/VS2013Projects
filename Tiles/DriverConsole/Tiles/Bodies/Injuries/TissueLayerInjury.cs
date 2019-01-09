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

        StressResult StressResult { get; }

        bool IsDefeated { get; }
        bool IsChip { get; }
        bool IsSoft { get; }
        bool IsVascular { get; }

        double WoundArea { get; }
        double ContactArea { get; }
        double ContactAreaRatio { get; }
        double PenetrationRatio { get; }

        int PainContribution { get;  }
        int BleedingContribution { get; }
        bool ArteryOpened { get; }
    }

    public class TissueLayerInjury : ITissueLayerInjury
    {
        public ITissueLayer Layer { get; private set; }
        public IBodyPart BodyPart { get; private set; }
        public IDamageVector Damage { get; private set; }

        public StressResult StressResult { get; private set; }

        public bool IsDefeated { get; private set; }
        public bool IsChip { get; private set; }
        public bool IsSoft { get; private set; }
        public bool IsVascular { get; private set; }

        public double WoundArea { get; private set; }
        public double ContactArea { get; private set; }
        public double ContactAreaRatio { get; private set; }
        public double PenetrationRatio { get; private set; }

        public int PainContribution { get; private set; }
        public int BleedingContribution { get; private set; }
        public bool ArteryOpened { get; private set; }

        public TissueLayerInjury(IBodyPart bodyPart, ITissueLayer layer, StressResult stressResult, IDamageVector damage, 
            double woundArea, double contactArea, double contactAreaRatio, double penetrationRatio, 
            int painContribution, int bleedingContribution, bool arteryOpened,
            bool isDefeated, bool isChip, bool isSoft, bool isVascular)
        {
            BodyPart = bodyPart;
            Layer = layer;
            Damage = damage;
            StressResult = stressResult;

            WoundArea = woundArea;
            ContactArea = contactArea;
            ContactAreaRatio = contactAreaRatio;
            PenetrationRatio = penetrationRatio;
            PainContribution = painContribution;
            BleedingContribution = bleedingContribution;
            ArteryOpened = arteryOpened;
            IsDefeated = isDefeated;
            IsChip = isChip;
            IsSoft = isSoft;
            IsVascular = isVascular;
        }
    }
}
