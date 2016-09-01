using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public interface ITissueLayer
    {
        IMaterial Material { get; }
        /// <summary>
        /// Thickness in m^-5
        /// </summary>
        int Thickness { get; }

        bool CanBeBruised { get; }
        bool CanBeTorn { get; }
        bool CanBePunctured { get; }
    }
}
