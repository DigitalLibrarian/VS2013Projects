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
    public class CombatDodgeDirectionPickScreen : CanvasBoxScreen
    {
        IEnumerable<IGameScreen> ParentScreens { get; set; }
        IGame Game { get; set; }
        IAgent Other { get; set; }
        IDodgeAgentCommandDiscoverer DodgeDisco { get; set; }

        JaggedListSelector Selector { get; set; }

        public CombatDodgeDirectionPickScreen(
            IEnumerable<IGameScreen> parentScreens, 
            IGame game, 
            IAgent other,
            IDodgeAgentCommandDiscoverer dodgeDisco,
            ICanvas canvas, 
            Box2 box)
            : base(canvas, box)
        {
            ParentScreens = parentScreens;
            Game = game;
            Other = other;
            DodgeDisco = dodgeDisco;

            PropagateInput = false;
            PropagateUpdate = false;
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

        IEnumerable<IAgentCommand> GetDodges()
        {
            return DodgeDisco.GetPossibleDodges(Game.Player.Agent, Other, Game.Atlas);
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("Dodging - Which direction?", Box.Min);

            var lines = new List<string>();
            foreach (var command in GetDodges().Where(c => c.CommandType == AgentCommandType.Dodge))
            {
                var v = command.Direction;
                var dir = CompassVectors.GetCompassDirection(v);
                lines.Add(dir.ToString());
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
                var dCount = -1;
                var groups = new List<List<IAgentCommand>>();
                foreach (var c in GetDodges())
                {
                    if (c.CommandType == AgentCommandType.Dodge)
                    {
                        groups.Add(new List<IAgentCommand>());
                        dCount++;
                    }
                    groups[dCount].Add(c);
                }

                var commands = groups[Selector.Selected.Y];
                Game.Player.EnqueueCommands(commands);
                foreach (var screen in ParentScreens)
                {
                    ScreenManager.Remove(screen);
                }
                Exit();
            }
            else if (args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
        
    }
}
