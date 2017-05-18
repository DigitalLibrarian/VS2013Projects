using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.ScreensImpl.UI
{
    public class GameSimulationViewModel : IGameSimulationViewModel
    {
        public IAtlas Atlas { get; set; }
        public ICamera Camera { get; set; }
        public ITile CameraTile { get; set; }
        public bool Looking { get; set; }
        public IActionLog ActionLog { get; set; }
        public long GlobalTime { get; set; }
        public bool BlockingForInput { get; set; }
    }
}
