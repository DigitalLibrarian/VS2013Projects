using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Gsm;

namespace Tiles.ScreensImpl
{
    public abstract class CommandScreen : GameScreen
    {
        protected IGame Game { get; private set; }
        public CommandScreen(IGame game)
        {
            Game = game;
            PropagateInput = false;
            PropagateUpdate = false;
            PropagateDraw = true;
        }
        public override void Draw() { }
        public override void Update() { }
    }
}
