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
    public class CombatTargetBodyPartPickScreen : CanvasBoxScreen
    {
        IPlayer Player { get; set; }
        IAttackConductor AttackConductor { get; set; }
        ICombatMoveDiscoverer MoveDisco { get; set; }
        IAgent Target { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }

        JaggedListSelector Selector { get; set; }

        IGameScreen ParentScreen { get; set; }
        string Verb2ndPerson { get; set; }

        public CombatTargetBodyPartPickScreen(IGameScreen parent, string verb2ndPerson, IPlayer player, IAgent target, IAgentCommandFactory commandFactory,
            IAttackConductor attackConductor, ICombatMoveDiscoverer moveDisco, ICanvas canvas, Box2 box)
            : base(canvas, box)
        {

            ParentScreen = parent;
            Verb2ndPerson = verb2ndPerson;

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

            Selector = new JaggedListSelector(Box)
            {
                Foreground = this.Foreground,
                Background = this.Background,
                SelectedBackground = Color.Blue,
                SelectedForeground = Color.White
            };
        }

        private IEnumerable<ICombatMove> GetMoves()
        {
            return MoveDisco.GetPossibleMoves(Player.Agent, Target).Where(m => m.Class.Verb.Conjugate(VerbConjugation.SecondPerson) == Verb2ndPerson);
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("Where would you like to target your attack?", Box.Min);

            var lines = new List<string>();
            foreach (var move in GetMoves())
            {
                lines.Add(string.Format("{0}", move.Name));
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
                var moves = GetMoves();
                if (Selector.Selected.Y < moves.Count())
                {
                    Player.EnqueueCommands(CommandFactory.MeleeAttack(Player.Agent, Target, moves.ElementAt(Selector.Selected.Y)));
                    ScreenManager.Remove(ParentScreen);
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
