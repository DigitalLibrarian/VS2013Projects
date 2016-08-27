using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Control;
using Tiles.Math;
using Tiles.Render;
using Tiles.ScreensImpl.UI;

namespace Tiles.ScreensImpl.Panels
{
    public class LookUiPanelScreen : UiPanelScreen
    {
        enum InfoPanelState { None, Agent, Tile, Structure, Item };
        InfoPanelState InfoState { get; set; }

        Vector2 InfoLinesOffset = new Vector2(1, 2);
        public LookUiPanelScreen(IGameSimulationViewModel viewModel, ICanvas canvas, Box2 box)
            : base(viewModel, canvas, box)
        {
            PropagateDraw = true;
        }

        public override void Load()
        {
            base.Load();
            InfoState = InfoPanelState.Item;
            Canvas.FillBox(Symbol.None, Box.Min, Box.Size, Color.White, Color.Black);
        }


        public override void Unload()
        {
            // this is kinda gross
            Canvas.FillBox(Symbol.None, Box.Min, Box.Size, Color.White, Color.Black);
        }

        public override void Draw()
        {
            Canvas.FillBox(Symbol.None, Box.Min, Box.Size, Color.White, Color.Black);

            if (!ViewModel.Looking) return;
            DrawHeader();
            switch (InfoState)
            {
                case InfoPanelState.None:
                    Draw_None(ViewModel);
                    break;
                case InfoPanelState.Agent:
                    Draw_Agent(ViewModel);
                    break;
                case InfoPanelState.Tile:
                    Draw_Tile(ViewModel);
                    break;
                case InfoPanelState.Structure:
                    Draw_Structure(ViewModel);
                    break;
                case InfoPanelState.Item:
                    Draw_Items(ViewModel);
                    break;
            }
        }

        private void Draw_None(IGameSimulationViewModel viewModel) { }
        private void Draw_Agent(IGameSimulationViewModel viewModel)
        {
            var agent = viewModel.CameraTile.Agent;
            var lines = new List<string>();

            if (agent == null)
            {
                lines.Add("No agent");
            }
            else
            {
                lines.Add(string.Format("Name: {0}", agent.Name));
                if (agent.Body.IsWrestling)
                {
                    if (agent.Body.IsGrasping)
                    {
                        lines.Add("Grasping");
                    }
                    if (agent.Body.IsBeingGrasped)
                    {
                        lines.Add("Being grasped");
                    }
                }
                foreach (var bodyPart in agent.Body.Parts)
                {
                    // TODO - wound display
                    lines.Add(string.Format("{0}", bodyPart.Name));

                    foreach (var item in agent.Outfit.GetItems(bodyPart))
                    {
                        lines.Add(string.Format(" {0}", item.Class.Name));
                    }
                }
            }
            DrawInfoLines(lines.ToArray());
        }
        private void Draw_Tile(IGameSimulationViewModel viewModel)
        {
            var tile = viewModel.CameraTile;
            DrawInfoLines(
                string.Format("Tile Index: {0}", tile.Index),
                string.Format("Terrain Type : {0}", tile.Terrain),
                string.Format("Is Passable?: {0}", tile.IsTerrainPassable),
                string.Format("Has Structure: {0}", tile.HasStructureCell),
                string.Format("Symbol: {0}", tile.TerrainSprite.Symbol),
                string.Format("Agent: {0}", tile.HasAgent ? string.Format("{0} ({1})", tile.Agent.Name, tile.Agent.Pos) : "n/a"),
                string.Format("Items: {0}", tile.Items.Count())
                );
        }
        private void Draw_Structure(IGameSimulationViewModel viewModel)
        {
            var lines = new List<string>();
            var tile = viewModel.CameraTile;

            if (!tile.HasStructureCell)
            {
                lines.Add("No structure");
            }
            else
            {
                var cell = tile.StructureCell;
                lines.Add(string.Format("Structure Name: {0}", cell.Structure.Name));
                lines.Add(string.Format("Cell Type: {0}", cell.Type));
                lines.Add(string.Format("Can Pass: {0}", cell.CanPass));
                lines.Add(string.Format("Can Open: {0}", cell.CanOpen));
                lines.Add(string.Format("Can Close: {0}", cell.CanClose));
                lines.Add(string.Format("Is Open: {0}", cell.IsOpen));
            }
            DrawInfoLines(lines.ToArray());
        }

        private void Draw_Items(IGameSimulationViewModel viewModel)
        {
            var lines = new List<string>();
            var items = viewModel.CameraTile.Items;
            if (!items.Any())
            {
                lines.Add("No items");
            }
            else
            {
                foreach (var item in items)
                {
                    lines.Add(item.Class.Name);
                }
            }
            DrawInfoLines(lines.ToArray());
        }

        private void DrawInfoLines(params string[] lines)
        {
            Canvas.WriteLineColumn(Box.Min + InfoLinesOffset, Color.White, Color.Black, lines);
        }

        private void DrawHeader()
        {
            Vector2 pos = Box.Min;
            Color subfg = Color.White;
            Color highFg = Color.Yellow;
            Color fg;
            Color bg = Color.Black;
            foreach (var stateObj in Enum.GetValues(typeof(InfoPanelState)))
            {
                InfoPanelState state = (InfoPanelState)stateObj;
                if (state == InfoPanelState.None) continue;
                if (InfoState == state)
                {
                    fg = highFg;
                }
                else
                {
                    fg = subfg;
                }
                var label = state.ToString();
                char c = label[0];
                label = label.Substring(1);

                Canvas.DrawString(string.Format("({0}){1}", c, label), pos, fg, bg);
                pos += new Vector2(label.Count() + 4, 0);
            }
            Canvas.DrawString(ViewModel.CameraTile.Index.ToString(), pos, subfg, bg);
        }

        public override void OnKeyPress(KeyPressEventArgs args)
        {
            switch (args.Key)
            {
                case ConsoleKey.T:
                    InfoState = InfoPanelState.Tile;
                    break;
                case ConsoleKey.A:
                    InfoState = InfoPanelState.Agent;
                    break;
                case ConsoleKey.S:
                    InfoState = InfoPanelState.Structure;
                    break;
                case ConsoleKey.I:
                    InfoState = InfoPanelState.Item;
                    break;
            }
        }
    }
}
