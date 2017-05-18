using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.ScreensImpl.UI
{
    public interface IGameSimulationViewModel
    {
        IAtlas Atlas { get; set; }
        ICamera Camera { get; set; }
        ITile CameraTile { get; set; }
        bool Looking { get; set; }
        IActionLog ActionLog { get; set; }

        long GlobalTime { get; set; }
        bool BlockingForInput { get; set; }
    }
}
