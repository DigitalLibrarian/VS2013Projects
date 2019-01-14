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
    public class CombatVerbPickScreen : CanvasBoxScreen
    {
        IEnumerable<IGameScreen> ParentScreens { get; set; }
        IPlayer Player { get; set; }
        IAttackConductor AttackConductor { get; set; }
        ICombatMoveDiscoverer MoveDisco { get; set; }
        IAgent Target { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }

        JaggedListSelector Selector { get; set; }

        public CombatVerbPickScreen(
            IEnumerable<IGameScreen> parents, 
            IPlayer player, IAgent target, IAgentCommandFactory commandFactory,
            IAttackConductor attackConductor, ICombatMoveDiscoverer moveDisco, ICanvas canvas, Box2 box)
            : base(canvas, box) 
        {
            ParentScreens = parents;
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

        private IEnumerable<string> GetDistinctVerbs()
        {
            var moves = MoveDisco.GetPossibleMoves(Player.Agent, Target);
            return moves.Select(m => m.Class.Verb.Conjugate(VerbConjugation.SecondPerson)).Distinct();
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString("What is your attack move?", Box.Min);

            var lines = new List<string>();
            foreach (var verb in GetDistinctVerbs())
            {
                lines.Add(string.Format("{0}", verb));
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
                var verb2ndPerson = GetDistinctVerbs().ElementAt(Selector.Selected.Y);
                ScreenManager.Add(new CombatTargetBodyPartPickScreen(
                ParentScreens.Concat(new IGameScreen[]{ this }), 
                verb2ndPerson, Player, Target, CommandFactory, AttackConductor, MoveDisco, Canvas, Box));
            }
            else if (args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
    }
}
