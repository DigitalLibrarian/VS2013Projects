using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface IMaterialStrikeBuilder
    {
        void SetStressMode(StressMode mode);

        void SetStrikeMomentum(double momentum);
        void SetContactArea(int contactArea);

        void SetStrikerMaterial(IMaterial mat);
        void SetStrickenMaterial(IMaterial mat);

        IMaterialStrikeResult Build();
        void Clear();
    }
}
