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

namespace Tiles.ScreensImpl
{
    public class InventoryItemDisplayScreen : CanvasBoxScreen
    {
        IPlayer Player { get; set; }
        IItem Item { get; set; }
        IActionLog Log { get; set; }
        IAgentCommandFactory CommandFactory { get; set; }
        public InventoryItemDisplayScreen(IItem item, IPlayer player, IAgentCommandFactory commandFactory, IActionLog log, ICanvas canvas, Box box)
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
                string.Format("Name: {0}", Item.Name),
                string.Format("Weapon?: {0}", Item.IsWeapon),
                string.Format("Armor?: {0}", Item.IsArmor),
            };

            if (Item.IsArmor)
            {
                leftColumnLines.Add("Armor Slots:");
                foreach (var slot in Item.ArmorClass.RequiredSlots)
                {
                    leftColumnLines.Add(string.Format("  {0}", slot));
                }

                foreach (var damageTypeObj in Enum.GetValues(typeof(DamageType)))
                {
                    DamageType damageType = (DamageType)damageTypeObj;
                    leftColumnLines.Add(string.Format("   -{0}: {1}%", damageType, Item.ArmorClass.ResistVector.GetComponent(damageType)));
                }
            }

            if (Item.IsWeapon)
            {

                leftColumnLines.Add("Weapon Slots:");
                foreach (var slot in Item.WeaponClass.RequiredSlots)
                {
                    leftColumnLines.Add(string.Format("  {0}", slot));
                }

                leftColumnLines.Add(string.Format("Moves:"));
                foreach (var moveClass in Item.WeaponClass.AttackMoveClasses)
                {
                    leftColumnLines.Add(string.Format("  {0}", moveClass.Name));
                    foreach (var damageTypeObj in Enum.GetValues(typeof(DamageType)))
                    {
                        DamageType damageType = (DamageType)damageTypeObj;

                        uint dmg = moveClass.DamageVector.GetComponent(damageType);
                        if (dmg > 0)
                        {
                            leftColumnLines.Add(string.Format("   +{0}: {1}", damageType, dmg));
                        }
                    }
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
            Player.EnqueueCommand(CommandFactory.WieldWeapon(Player.Agent, Item));
            Exit();
        }

        void Unwield()
        {
            Player.EnqueueCommand(CommandFactory.UnwieldWeapon(Player.Agent, Item));
            Exit();
        }

        void Wear()
        {
            Player.EnqueueCommand(CommandFactory.WearArmor(Player.Agent , Item));
            Exit();
        }


        void TakeOff()
        {
            Player.EnqueueCommand(CommandFactory.TakeOffArmor(Player.Agent, Item));
            Exit();
        }

        void Drop()
        {
            Player.EnqueueCommand(CommandFactory.DropInventoryItem(Player.Agent, Item));
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


            if (CanWield && args.KeyChar == 'w') Wield();
            if (CanUnwield && args.KeyChar == 'u') Unwield();
            if (CanWear && args.KeyChar == 'W') Wear();
            if (CanTakeOff && args.KeyChar == 'T') TakeOff();
            if (CanDrop && args.KeyChar == 'd') Drop();
        }
    }
}
