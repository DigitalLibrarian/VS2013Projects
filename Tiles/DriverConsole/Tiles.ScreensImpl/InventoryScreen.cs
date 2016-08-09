using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents;
using Tiles.Control;
using Tiles.Gsm;
using Tiles.Items;
using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl
{
    public class InventoryScreen : CanvasBoxScreen
    {
        IPlayer Player { get; set; }
        IActionLog Log { get; set; }
        
        JaggedListSelector Selector { get; set; }

        public InventoryScreen(IPlayer player, IActionLog log, ICanvas canvas, Box box)
            : base(canvas, box)
        {
            Player = player;
            PropagateInput = false;
            PropagateUpdate = false;

            Log = log;
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

            var items = Player.Inventory.GetItems().ToList();
            var worn = Player.Inventory.GetWorn().ToList();
            Selector.Update(items.Count(), worn.Count);
            
            Color fg = Foreground, bg = Background;
            Canvas.DrawString(string.Format("Inventory (H: {0})", Selector.Selected), Box.Min, fg, bg);
            
            var screenPos = Box.Min + new Vector2(0, 2);
            var slope = new Vector2(0, 1);

            int i;
            for (i = 0; i < items.Count(); i++)
            {
                var item = items[i];
                if (Selector.Selected.X == 0 && Selector.Selected.Y == i)
                {
                    fg = Selector.SelectedForeground;
                    bg = Selector.SelectedBackground;
                }
                else
                {
                    fg = Selector.Foreground;
                    bg = Selector.Background;
                }
                Canvas.DrawString(item.Name, screenPos, fg, bg);
                screenPos += slope;
            }
            screenPos = Box.Min + new Vector2(40, 2);

            for (i = 0; i < worn.Count(); i++)
            {
                var item = worn[i];    
                if (Selector.Selected.X == 1 && Selector.Selected.Y == i)
                {
                    fg = Selector.SelectedForeground;
                    bg = Selector.SelectedBackground;
                }
                else
                {
                    fg = Selector.Foreground;
                    bg = Selector.Background;
                }
                Canvas.DrawString(item.Name, screenPos, fg, bg);
                screenPos += slope;
            }
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            if (args.Key == ConsoleKey.Escape || args.Key == ConsoleKey.Q)
            {
                Exit();
            }
            else if (ConsoleKeyCompassMapping.IsCompassKey(args.Key))
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
                    case CompassDirection.East:
                        ViewSelected();
                        break;
                    case CompassDirection.West:
                        Selector.MoveLeft();
                        break;
                }

            }
            else if (args.Key == ConsoleKey.Enter)
            {
                ViewSelected();
            }
        }


        IItem GetHighlightedItem()
        {
            int index = Selector.Selected.Y;
            if (Selector.Selected.X == 0)
            {
                return Player.Inventory.GetItems().ElementAt(index);
            }
            else
            {
                return Player.Inventory.GetWorn().ElementAt(index);
            }
        }

        private void ViewSelected()
        {
            if (Player.Inventory.GetItems().Count() > 0 || Player.Inventory.GetWorn().Count() > 0)
            {
                var hItem = GetHighlightedItem();
                var itemDisplayScreen = new InventoryItemDisplayScreen(hItem, Player, Log, Canvas, Box);
                ScreenManager.Add(itemDisplayScreen);
                Exit();
            }
        }
    }
}
