using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;
using Tiles.Math;

namespace Tiles.Bodies
{
    public interface ITissueLayer
    {
        ITissueLayerClass Class { get; }
        IMaterial Material { get; }
        /// <summary>
        /// Thickness in m^-5
        /// </summary>
        double Thickness { get; }
        double Volume { get; }

        string Name { get; }

        bool IsPulped();
        bool IsVascular();
        bool IsSoft(StressMode stressMode);
    }
}
