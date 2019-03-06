using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterialStrikeResultBuilder
    {
        void SetStressMode(StressMode mode);

        void SetStrikeMomentum(double momentum);
        void SetStrikerContactArea(double contactArea);
        void SetStrickenContactArea(double contactArea);
        void SetStrikerSharpness(double sharpness);

        void SetStrikerMaterial(IMaterial mat);
        void SetStrickenMaterial(IMaterial mat);
        void SetLayerVolume(double vol);
        void SetLayerThickness(double p);

        void SetRemainingPenetration(double penetrationLeft);
        void SetImplementIsSmall(bool isSmall);
        void SetImplementSize(double size);
        void SetImplementIsEdged(bool isEdged);

        void SetMaxPenetration(double maxPenetration);

        MaterialStrikeResult Build();
        void Clear();
    }
}
