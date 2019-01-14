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
        private static readonly string Dodge = "dodge";

        IEnumerable<IGameScreen> ParentScreens { get; set; }
        IGame Game { get; set; }
        IDodgeAgentCommandDiscoverer DodgeDisco { get; set; }
        ICombatMoveDiscoverer MoveDisco { get; set; }
        IAgent Target { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }

        JaggedListSelector Selector { get; set; }

        public CombatVerbPickScreen(
            IEnumerable<IGameScreen> parents, 
            IGame game, IAgent target, IAgentCommandFactory commandFactory,
            ICombatMoveDiscoverer moveDisco, 
            IDodgeAgentCommandDiscoverer dodgeDisco,
            ICanvas canvas, Box2 box)
            : base(canvas, box) 
        {
            ParentScreens = parents;
            Game = game;
            Target = target;
            CommandFactory = commandFactory;
            DodgeDisco = dodgeDisco;
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

        IEnumerable<string> GetDistinctVerbs()
        {
            // standard attacks
            foreach (var moveVerb in MoveDisco.GetPossibleMoves(Game.Player.Agent, Target)
                .Select(m => m.Class.Verb.Conjugate(VerbConjugation.SecondPerson)).Distinct())
            {
                yield return moveVerb;
            }

            // dodges
            if (DodgeDisco.GetPossibleDodges(Game.Player.Agent, Target, Game.Atlas).Any())
            {
                yield return Dodge;
            }
        }

        public override void Draw()
        {
            base.Draw();

            Canvas.DrawString(string.Format("Targeting {0} - What do you want to do?", Target.Name), Box.Min);

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
                switch (ConsoleKeyCompassMapping.ToDirection(args.Key))
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
                if (verb2ndPerson == Dodge)
                {
                    ScreenManager.Add(
                        new CombatDodgeDirectionPickScreen(
                            ParentScreens.Concat(new IGameScreen[] { this }),
                            Game,
                            Target,
                            new DodgeAgentCommandDiscoverer(CommandFactory),
                            Canvas, Box));
                }
                else
                {
                    ScreenManager.Add(
                        new CombatTargetBodyPartPickScreen(
                            ParentScreens.Concat(new IGameScreen[] { this }),
                            verb2ndPerson,
                            Game.Player,
                            Target,
                            CommandFactory,
                            Game.AttackConductor,
                            MoveDisco, Canvas, Box));
                }
            }
            else if (args.Key == ConsoleKey.Escape)
            {
                Exit();
            }
        }
    }
}
