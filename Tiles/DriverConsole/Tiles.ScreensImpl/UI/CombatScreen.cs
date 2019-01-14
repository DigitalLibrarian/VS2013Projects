using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl.UI
{
    public class CombatScreen : CanvasBoxScreen
    {
        IGame Game { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }
        ICombatTargetDiscoverer TargetDisco { get; set; }
        JaggedListSelector Selector { get; set; }

        public CombatScreen(IGame game, IAgentCommandFactory commandFactory, ICombatTargetDiscoverer targetDisco, ICanvas canvas, Box2 box)
            : base(canvas, box)
        {
            Game = game;
            CommandFactory = commandFactory;
            TargetDisco = targetDisco;
        }

        public override void Load()
        {
            base.Load();

            Selector = new JaggedListSelector(Box)
            {
                Foreground = this.Foreground,
                Background = this.Background,
                SelectedBackground = Color.Blue,
                SelectedForeground = Color.White
            };
        }

        IEnumerable<IAgent> GetAgents()
        {
            return TargetDisco.GetPossibleTargets(Game.Player.Agent, Game.Atlas);
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("Select your target:", Box.Min);

            var lines = new List<string>();
            foreach (var agent in GetAgents())
            {
                lines.Add(string.Format("{0}", agent.Name));
            }

            Selector.Draw(Canvas, Box.Min + new Vector2(1, 2), lines.ToArray());
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            if (ConsoleKeyCompassMapping.IsCompassKey(args.Key))
            {
                var comDir = ConsoleKeyCompassMapping.ToDirection(args.Key);
                switch (comDir)
                {
                    case CompassDirection.North:
                        Selector.MoveUp();
                        break;
                    case CompassDirection.South:
                        Selector.MoveDown();
                        break;
                }
            }
            else if (args.Key == ConsoleKey.Enter)
            {
                var agent = GetAgents().ElementAt(Selector.Selected.Y);
                ScreenManager.Add(
                    new CombatVerbPickScreen(
                        new IGameScreen[] { this },
                        Game.Player,
                        agent,
                        CommandFactory,
                        Game.AttackConductor,
                        new CombatMoveDiscoverer(new CombatMoveFactory()), Canvas, Box)
                    );
            }
            else if (args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
    }
}
