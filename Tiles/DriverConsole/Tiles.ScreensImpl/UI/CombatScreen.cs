using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Agents.Combat;
using Tiles.Control;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl.UI
{
    public class CombatScreen : CanvasBoxScreen
    {
        IPlayer Player { get; set; }
        IAttackConductor AttackConductor { get; set; }
        ICombatMoveDiscoverer MoveDisco { get; set; }
        IAgent Target { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }

        JaggedListSelector Selector { get; set; }

        public CombatScreen(IPlayer player, IAgent target, IAgentCommandFactory commandFactory,
            IAttackConductor attackConductor, ICombatMoveDiscoverer moveDisco, ICanvas canvas, Box2 box)
            : base(canvas, box) 
        {
            Player = player;
            Target = target;
            CommandFactory = commandFactory;
            AttackConductor = attackConductor;
            MoveDisco = moveDisco;

            PropagateInput = false;
            PropagateUpdate = false;
        }

        public override void Load()
        {
            base.Load();

            Selector = new JaggedListSelector()
            {
                Foreground = this.Foreground,
                Background = this.Background,
                SelectedBackground = Color.Blue,
                SelectedForeground = Color.White
            };
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("What is your attack move?", Box.Min);

            var lines = new List<string>();
            var moves = MoveDisco.GetPossibleMoves(Player.Agent, Target);
            foreach (var move in moves)
            {
                lines.Add(string.Format("{0} ({1} dmg)", move.Name, move.PredictedDamage));
            }
            lines.Add("Don't attack");

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
                var moves = MoveDisco.GetPossibleMoves(Player.Agent, Target).ToList();
                if (Selector.Selected.Y < moves.Count())
                {
                    Player.EnqueueCommands(CommandFactory.MeleeAttack(Player.Agent, Target, moves.ElementAt(Selector.Selected.Y)));
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
