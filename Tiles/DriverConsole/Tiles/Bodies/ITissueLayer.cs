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
        int RelativeThickness { get; }

        bool CanBeBruised { get; }
        bool CanBeTorn { get; }
        bool CanBePunctured { get; }
    }
}
