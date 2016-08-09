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
        public InventoryItemDisplayScreen(IItem item, IPlayer player, IActionLog log, ICanvas canvas, Box box)
            : base(canvas, box)
        {
            Item = item;
            Player = player;
            PropagateInput = false;
            PropagateUpdate = false;

            Log = log;
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
                leftColumnLines.Add(string.Format("Armor Slot: {0}", Item.Armor.ArmorClass.ArmorSlot));
                foreach (var damageTypeObj in Enum.GetValues(typeof(DamageType)))
                {
                    DamageType damageType = (DamageType)damageTypeObj;
                    leftColumnLines.Add(string.Format("{0} Class Resist: {1}", damageType, Item.Armor.ArmorClass.DamageVector.GetComponent(damageType)));
                    leftColumnLines.Add(string.Format("{0} Instance Resist: {1}", damageType, Item.Armor.GetTypeResistence(damageType)));
                }
            }

            if (Item.IsWeapon)
            {
                leftColumnLines.Add(string.Format("Weapon Slot: {0}", Item.Weapon.WeaponClass.WeaponSlot));
                foreach(var damageTypeObj in Enum.GetValues(typeof(DamageType)))
                {
                    DamageType damageType = (DamageType)damageTypeObj;
                    leftColumnLines.Add(string.Format("{0} Class Dmg: {1}", damageType, Item.Weapon.WeaponClass.DamageVector.GetComponent(damageType)));
                    leftColumnLines.Add(string.Format("{0} Instance Dmg: {1}", damageType, Item.Weapon.GetBaseTypeDamage(damageType)));
                }
            }

            var rightColumnLines = new List<string>();

            if (CanWield) rightColumnLines.Add("w\tWield");
            if (CanWear) rightColumnLines.Add("W\tWear");
            if (CanTakeOff) rightColumnLines.Add("T\tTake off");
            if (CanDrop) rightColumnLines.Add("d\tDrop");

            var leftColumnPos = Box.Min;
            var rightColumnPos = Box.Min + new Vector2((int)(Box.Size.X * 0.5), (int)(Box.Size.Y * 0.2));

            Canvas.WriteLineColumn(leftColumnPos, Foreground, Background, leftColumnLines.ToArray());
            Canvas.WriteLineColumn(rightColumnPos, Foreground, Background, rightColumnLines.ToArray());
        }


        bool CanWield 
        { 
            get {
                return HaveSlot && !IsSomethingWielded && Item.IsWeapon && !OtherWielded;
            } 
        }

        bool CanWear
        {
            get
            {
                if (!Item.IsArmor) return false;
                if (!Player.Agent.EquipmentSlots.HasSlot(Item.Armor.ArmorClass.ArmorSlot)) return false;
                if (Player.Agent.EquipmentSlots.IsSlotFull(Item.Armor.ArmorClass.ArmorSlot)) return false;
                return true;
            }
        }

        bool CanTakeOff
        {
            get { return IsWorn; }
        }

        bool IsWorn
        {
            get { return Player.Inventory.GetWorn().Contains(Item); }
        }
        
        bool CanDrop
        {
            get { return !IsWorn; }
        }

        bool HaveSlot
        {
            get
            {
                return
                    (Item.IsWeapon && Player.Agent.EquipmentSlots.HasSlot(Item.Weapon.WeaponClass.WeaponSlot))
                    || (Item.IsArmor && Player.Agent.EquipmentSlots.HasSlot(Item.Armor.ArmorClass.ArmorSlot));
            }
        }

        bool IsSomethingWielded
        {
            get { return (Item.IsWeapon && HaveSlot && Player.Agent.EquipmentSlots.IsSlotFull(Item.Weapon.WeaponClass.WeaponSlot))
                || (Item.IsArmor && HaveSlot && Player.Agent.EquipmentSlots.IsSlotFull(Item.Armor.ArmorClass.ArmorSlot)); }
        }

        bool IsWielded
        {
            get { return (IsSomethingWielded && !OtherWielded); }
        }

        bool OtherWielded
        {
            get { return Item.IsWeapon && HaveSlot
                && Player.Agent.EquipmentSlots.IsSlotFull(Item.Weapon.WeaponClass.WeaponSlot )
                && Player.Agent.EquipmentSlots.Get(Item.Weapon.WeaponClass.WeaponSlot) != Item.Weapon; }
        }
        
        void Wield()
        {
            Player.EnqueueCommand(new AgentCommand
            {
                CommandType = AgentCommandType.WieldWeapon,
                Item = Item,
                Weapon = Item.Weapon
            });
            Exit();
        }

        void Wear()
        {
            Player.EnqueueCommand(new AgentCommand
            {
                CommandType = AgentCommandType.WearArmor,
                Item = Item,
                Armor = Item.Armor
            });
            Exit();
        }

        void TakeOff()
        {
            if (Item.IsWeapon)
            {
                Player.EnqueueCommand(new AgentCommand
                {
                    CommandType = AgentCommandType.UnwieldWeapon,
                    Item = Item,
                    Weapon = Item.Weapon
                });
                Exit();
            }
            if (Item.IsArmor)
            {
                Player.EnqueueCommand(new AgentCommand
                {
                    CommandType = AgentCommandType.TakeOffArmor,
                    Item = Item,
                    Armor = Item.Armor
                });
                Exit();
            }
        }

        void Drop()
        {
            Player.EnqueueCommand(new AgentCommand
            {
                CommandType = AgentCommandType.DropInventoryItem,
                Item = Item
            });
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
            if (CanWear && args.KeyChar == 'W') Wear();
            if (CanTakeOff && args.KeyChar == 'T') TakeOff();
            if (CanDrop && args.KeyChar == 'd') Drop();
        }
    }
}
