using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using Tiles.Control;
using Tiles.Control.Console;
using Tiles.Items;
using Tiles.Render;
using Tiles.Render.Console;
using Tiles.Math;
using Tiles.Agents;
using Tiles.Bodies;
using Tiles.Agents.Behaviors;
using Tiles.Random;
using Tiles.Structures;
using Tiles.Console;
using Tiles.ScreensImpl;
using Tiles.Gsm;

using Tiles.ScreensImpl.ContentFactories;

namespace DriverConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeConsole();

            var screenManager = new GameScreenManager();
            screenManager.Add(
                new GameSimulationScreen(
                    new GameFactory().SetupGenericZombieWorld(),
                    new ConsoleCanvas(new ConsoleWriter()),
                    new Box(new Vector2(0, 0), new Vector2(80, 24))));

            var source = new ConsoleKeyboardSource(new ConsoleReader());
            source.KeyPressed += screenManager.OnKeyPress;

            while (screenManager.Screens.Any())
            {
                screenManager.Draw();
                if (source.KeyAvailable || screenManager.BlockForInput)
                {
                    source.Pump();
                }
                else
                {
                    if (source.KeyAvailable)
                    {
                        source.Pump();
                    }
                }
                screenManager.Update();
            } 
        }

        #region Console initialize
        static void InitializeConsole()
        {
            System.Console.CursorVisible = false;
        }
        #endregion
    }
}
