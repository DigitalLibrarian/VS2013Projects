using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Control;
using Tiles.Items;
using Tiles.Math;
using Tiles.Render;
using Tiles.Agents.Combat;

namespace Tiles.ScreensImpl.UI
{
    public class InventoryItemDisplayScreen : CanvasBoxScreen
    {
        IPlayer Player { get; set; }
        IItem Item { get; set; }
        IActionLog Log { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }
        public InventoryItemDisplayScreen(IItem item, IPlayer player, IAgentCommandFactory commandFactory, IActionLog log, ICanvas canvas, Box2 box)
            : base(canvas, box)
        {
            Item = item;
            Player = player;
            PropagateInput = false;
            PropagateUpdate = false;

            Log = log;
            CommandFactory = commandFactory;
        }

        public override void Draw()
        {
            base.Draw();

            var leftColumnLines = new List<string>{
                string.Format("Name: {0}", Item.Class.Name),
                string.Format("Weapon?: {0}", Item.IsWeapon),
                string.Format("Armor?: {0}", Item.IsArmor),
            };

            if (Item.IsArmor)
            {
                leftColumnLines.Add("Armor Slots:");
                foreach (var slot in Item.Class.ArmorClass.RequiredSlots)
                {
                    leftColumnLines.Add(string.Format("  {0}", slot));
                }
            }

            if (Item.IsWeapon)
            {

                leftColumnLines.Add("Weapon Slots:");
                foreach (var slot in Item.Class.WeaponClass.RequiredSlots)
                {
                    leftColumnLines.Add(string.Format("  {0}", slot));
                }

                leftColumnLines.Add(string.Format("Moves:"));
                foreach (var moveClass in Item.Class.WeaponClass.AttackMoveClasses)
                {
                    leftColumnLines.Add(string.Format("  {0}", moveClass.Name));
                }
            }

            var rightColumnLines = new List<string>();

            if (CanWield) rightColumnLines.Add("w\tWield");
            if (CanUnwield) rightColumnLines.Add("u\tUnwield");
            if (CanWear) rightColumnLines.Add("W\tWear");
            if (CanTakeOff) rightColumnLines.Add("T\tTake off");
            if (CanDrop) rightColumnLines.Add("d\tDrop");

            var leftColumnPos = Box.Min;
            var rightColumnPos = Box.Min + new Vector2((int)(Box.Size.X * 0.5), (int)(Box.Size.Y * 0.2));

            Canvas.WriteLineColumn(leftColumnPos, Foreground, Background, leftColumnLines.ToArray());
            Canvas.WriteLineColumn(rightColumnPos, Foreground, Background, rightColumnLines.ToArray());
        }

        bool IsWorn { get { return Player.Agent.Outfit.IsWorn(Item); } }
        bool IsWielded{ get { return Player.Agent.Outfit.IsWielded(Item); } }

        bool CanWield { get { return Player.Agent.Outfit.CanWield(Item); } }
        bool CanWear { get { return Player.Agent.Outfit.CanWear(Item); } }
        bool CanTakeOff { get { return IsWorn; } }
        bool CanUnwield { get { return IsWielded; } }
        bool CanDrop { get { return !IsWorn; } }
                
        void Wield()
        {
            Player.EnqueueCommands(CommandFactory.WieldWeapon(Player.Agent, Item));
            Exit();
        }

        void Unwield()
        {
            Player.EnqueueCommands(CommandFactory.UnwieldWeapon(Player.Agent, Item));
            Exit();
        }

        void Wear()
        {
            Player.EnqueueCommands(CommandFactory.WearArmor(Player.Agent , Item));
            Exit();
        }


        void TakeOff()
        {
            Player.EnqueueCommands(CommandFactory.TakeOffArmor(Player.Agent, Item));
            Exit();
        }

        void Drop()
        {
            Player.EnqueueCommands(CommandFactory.DropInventoryItem(Player.Agent, Item));
            Exit();
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            if (ConsoleKeyCompassMapping.IsCompassKey(args.Key))
            {
                if (ConsoleKeyCompassMapping.ToDirection(args.Key) == CompassDirection.West)
                {
                    Exit();
                }
            }
            else if (args.Key == ConsoleKey.Escape || args.Key == ConsoleKey.Q)
            {
                Exit();
            }


            if (CanWield && args.Key == ConsoleKey.W && !args.Shift) Wield();
            if (CanUnwield && args.Key == ConsoleKey.U && !args.Shift) Unwield();
            if (CanWear && args.Key == ConsoleKey.W && args.Shift) Wear();
            if (CanTakeOff && args.Key == ConsoleKey.T && args.Shift) TakeOff();
            if (CanDrop && args.Key == ConsoleKey.D && !args.Shift) Drop();
        }
    }
}
