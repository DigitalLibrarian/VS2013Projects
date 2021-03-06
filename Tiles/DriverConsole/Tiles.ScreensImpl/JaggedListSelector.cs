﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tiles.Math;
using Tiles.Render;

namespace Tiles.ScreensImpl
{
    public class JaggedListSelector
    {
        Vector2 Spacing { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public Color SelectedForeground { get; set; }
        public Color SelectedBackground { get; set; }

        public Vector2 Selected { get; private set; }
        public Box2 ScreenBox { get; private set; }
        public JaggedListSelector(Box2 screenBox)
        {
            ScreenBox = screenBox;
            Spacing = new Vector2(1, 1);
        }

        public void MoveUp() { Selected += Vector2.Truncate(CompassVectors.FromDirection(CompassDirection.North)); }
        public void MoveDown() { Selected += Vector2.Truncate(CompassVectors.FromDirection(CompassDirection.South)); }
        public void MoveRight() { Selected += Vector2.Truncate(CompassVectors.FromDirection(CompassDirection.East)); }
        public void MoveLeft() { Selected += Vector2.Truncate(CompassVectors.FromDirection(CompassDirection.West)); }

        #region Clamping
        public void Update(params int[] listCounts)
        {
            int numLists = listCounts.Count();

            int x = CircleClamp(Selected.X, numLists);
            int y = 0;

            if (numLists != 0)
            {
                if (listCounts[x] == 0)
                {
                    x = 0;
                    for (int t = 0; t < numLists; t++)
                    {
                        if (listCounts[t] > 0)
                        {
                            x = t;
                            break;
                        }
                    }
                }

                y = CircleClamp(Selected.Y, listCounts[x]);
            }

            Selected = new Vector2(x, y);
        }

        int CircleClamp(int x, int max)
        {
            if (max == 0) return 0;

            if (x >= max)
            {
                x = x % max;
            }
            else if (x < 0)
            {
                x = x + max;
            }
            return x;
        }
        #endregion

        int ColumnsPerPage(Box2 box)
        {
            return box.Size.X / Spacing.X;
        }

        int RowsPerPage(Box2 box)
        {
            return box.Size.Y;
        }

        Vector2 SelectedPageIndex(Box2 box)
        {
            int cols = ColumnsPerPage(box);
            int rows = RowsPerPage(box);

            var screenIndex = new Vector2(Spacing.X * Selected.X, Spacing.Y * Selected.Y);

            return new Vector2(screenIndex.X / cols, screenIndex.Y / rows);
        }

        public void Draw(ICanvas canvas, Vector2 screenPos, params string[][] labels)
        {
            List<int> counts = new List<int>();
            foreach (var label in labels)
            {
                counts.Add(label.Count());
            }
            Update(counts.ToArray());

            // screenPos should be treated like upper bound.  We have the rest of the screen
            var box = new Box2(screenPos, ScreenBox.Max);
            var pageIndex = SelectedPageIndex(box);

            Color fg = Foreground;
            Color bg = Background;
            Vector2 rowDelta = new Vector2(0, Spacing.Y);
            Vector2 columnDelta = new Vector2(Spacing.X, 0);

            var colSize = ColumnsPerPage(box);
            var rowSize = RowsPerPage(box);

            for (int i = pageIndex.X * colSize; i < (pageIndex.X + 1) * colSize; i++)
            {
                for (int j = pageIndex.Y * rowSize; j < (pageIndex.Y + 1) * rowSize; j++)
                {
                    if (labels.Count() > i && labels[i].Count() > j)
                    {
                        if (i == Selected.X && j == Selected.Y)
                        {
                            fg = SelectedForeground;
                            bg = SelectedBackground;
                        }
                        else
                        {
                            fg = Foreground;
                            bg = Background;
                        }
                        canvas.DrawString(labels[i][j], screenPos, fg, bg);
                    }
                    screenPos += rowDelta;
                }
                screenPos += columnDelta;
            }
        }
    }
}
