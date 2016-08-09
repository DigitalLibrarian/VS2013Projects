using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;

namespace Tiles.Gsm
{
    public class GameScreenManager : IGameScreenManager
    {
        public IList<IGameScreen> Screens { get; private set; }

        public GameScreenManager()
        {
            Screens = new List<IGameScreen>();
        }

        public void Add(IGameScreen screen)
        {
            Screens.Add(screen);
            screen.OnEnter(this);
            screen.Load();
        }

        public void Remove(IGameScreen screen)
        {
            screen.Unload();
            screen.OnExit();
            Screens.Remove(screen);
        }

        public void Draw()
        {
            bool notFound = true;
            int i = 0;
            var totalScreens = Screens.Count();
            int drawStart = totalScreens - 1;
            TopDownTraverse(screen =>
            {
                if (notFound && !screen.PropagateDraw)
                {
                    drawStart = i;
                    notFound = false;
                }
                i++;
            });

            drawStart = (totalScreens - 1) - drawStart;
            for (i = drawStart; i < Screens.Count(); i++)
            {
                Screens[i].Draw();
            }
        }

        public void Update()
        {
            var updateBlocked = false;
            TopDownTraverse(screen => {
                if (!updateBlocked)
                {
                    if (!screen.PropagateUpdate)
                    {
                        updateBlocked = true;
                    }
                    screen.Update();
                }
            });
        }

        public void OnKeyPress(object sender, KeyPressEventArgs args)
        {
            bool otherScreenHasFocus = false;

            TopDownTraverse(screen =>
            {
                if (screen.State == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.OnKeyPress(args);
                        if (!screen.PropagateInput)
                            otherScreenHasFocus = true;
                    }
                }
            });
        }

        void TopDownTraverse(Action<IGameScreen> action)
        {
            var screensToUpdate = new List<IGameScreen>(Screens); // TODO - make member to avoid garbage generation
            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                action(screen);
            }
        }


        public bool BlockForInput
        {
            get { return Screens.Any(x => x.BlockForInput); }
        }
    }
}
