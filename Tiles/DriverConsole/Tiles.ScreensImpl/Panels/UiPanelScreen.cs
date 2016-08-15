using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl.Panels
{
    public abstract class UiPanelScreen : CanvasBoxScreen
    {
        protected IGameSimulationViewModel ViewModel { get; private set; }

        public UiPanelScreen(IGameSimulationViewModel viewModel, ICanvas canvas, Box2 box)
            : base(canvas, box)
        {
            ViewModel = viewModel;

            PropagateInput = true;
            PropagateDraw = true;
            PropagateUpdate = true;
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {

        }
    }
}
