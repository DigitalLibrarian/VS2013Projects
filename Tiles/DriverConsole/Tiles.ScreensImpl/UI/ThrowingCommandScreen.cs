using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Items;

namespace Tiles.ScreensImpl.UI
{
    // TODO - this sucks
    public class ThrowingCommandScreen : CommandScreen
    {
        IGameSimulationViewModel ViewModel { get; set; }
        IItem Item { get; set; }

        public ThrowingCommandScreen(IGame game, IGameSimulationViewModel viewModel) : base(game)
        {
            ViewModel = viewModel;
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.Enter)
            {
                if (CanThrow())
                {

                }
            }
        }

        bool CanThrow() { return true; }
    }
}
