using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;
using Tiles.Render;
using Tiles.ScreensImpl.UI;
using Tiles.StringManipulation;

namespace Tiles.ScreensImpl.Panels
{
    public class ActionLogUiPanelScreen : UiPanelScreen
    {
        public ActionLogUiPanelScreen(IGameSimulationViewModel viewModel,
            ICanvas canvas, Box2 box)
            : base(viewModel, canvas, box)
        {

        }
        public override void Draw()
        {
            var lines = ViewModel.ActionLog.GetLines();
            lines = lines.WrapText(Box.Size.X - 1);
            lines = lines.Reverse().Take(Box.Size.Y).Reverse();
            lines = lines.Select(x => x.PadRight(Box.Size.X - 1));
            Canvas.WriteLineColumn(Box.Min, Color.White, Color.Black, lines.ToArray());
        }
    }
}
