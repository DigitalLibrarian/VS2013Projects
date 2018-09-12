using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ILayeredMaterialStrikeResultBuilder
    {
        void AddLayer(IMaterial mat);
        void AddLayer(IMaterial mat, double thick, double volume, object tag);

        void SetStrikerMaterial(IMaterial mat);

        /// <summary>
        /// Set the total amount of momentum going in
        /// </summary>
        /// <param name="momentum"></param>
        void SetMomentum(double momentum);

        void SetStrikerContactArea(double contactArea);
        void SetStrickenContactArea(double contactArea);
        void SetStrikerSharpness(double sharpness);
        void SetMaxPenetration(double maxPenetration);
        void SetStressMode(StressMode mode);
        void SetImplementWasSmall(bool wasSmall);
        void SetImplementSize(double size);

        ILayeredMaterialStrikeResult Build();

        void Clear();
    }
}
