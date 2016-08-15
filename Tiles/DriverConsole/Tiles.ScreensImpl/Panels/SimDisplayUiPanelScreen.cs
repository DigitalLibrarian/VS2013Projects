using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl.Panels
{
    public class SimDisplayUiPanelScreen : UiPanelScreen
    {
        Color NullTileForegroundColor = Color.Blue;
        Color NullTileBackgroundColor = Color.White;

        public SimDisplayUiPanelScreen(IGameSimulationViewModel viewModel, ICanvas canvas, Box2 box) : base(viewModel, canvas, box) { }
                
        public override void Draw()
        {
            var camPos = ViewModel.Camera.Pos;
            var atlas = ViewModel.Atlas;
            var reticule = ViewModel.Looking;

            int leftX = camPos.X - (Box.Size.X / 2);
            int topY = camPos.Y - (Box.Size.Y / 2);
            Vector3 topLeftWorld = new Vector3(leftX, topY, camPos.Z);

            Canvas.DrawString(string.Format("Time: {0}", ViewModel.GlobalTime), Vector2.Zero, Color.White, Color.Black);
            for (int x = 0; x < Box.Size.X; x++)
            {
                for (int y = 0; y < Box.Size.Y; y++)
                {
                    var worldPos = topLeftWorld + new Vector3(x, y, 0);
                    var tile = atlas.GetTileAtPos(worldPos);
                    if (tile != null)
                    {
                        DrawTile(tile, Box.Min.X + x, Box.Min.Y + y);
                    }
                    else
                    {
                        Canvas.DrawSymbol(Symbol.None, new Vector2(Box.Min.X + x, Box.Min.Y + y), NullTileForegroundColor, NullTileBackgroundColor);
                    }
                }
            }

            if (reticule)
            {
                ISprite s = PickDisplaySprite(atlas.GetTileAtPos(camPos));
                var screenPos = AtlasToScreen(camPos, Box.Min, Box.Size, camPos);
                Canvas.DrawSymbol(s.Symbol, screenPos, Color.White, Color.Blue);
            }
        }

        protected ISprite PickDisplaySprite(ITile tile)
        {
            var obj = tile.GetTopItem();
            ISprite s;
            if (tile.HasAgent)
            {
                s = tile.Agent.Sprite;
            }
            else if (obj != null)
            {
                s = obj.Class.Sprite;
            }
            else if (tile.HasStructureCell)
            {
                s = tile.StructureCell.Sprite;
            }
            else
            {
                s = tile.TerrainSprite;
            }
            return s;
        }

        protected Vector2 AtlasToScreen(Vector3 atlasPos, Vector2 atlasScreenOrigin, Vector2 displaySize, Vector3 camPos)
        {
            var camPos2d = new Vector2(camPos.X, camPos.Y);
            var topLeftWorld = camPos2d - (displaySize * 0.5);
            return atlasScreenOrigin + new Vector2(atlasPos.X, atlasPos.Y) - topLeftWorld;
        }

        private void DrawTile(ITile tile, int screenX, int screenY)
        {
            ISprite s = PickDisplaySprite(tile);
            Canvas.DrawSprite(s, new Vector2(screenX, screenY));
        }
    }
}
