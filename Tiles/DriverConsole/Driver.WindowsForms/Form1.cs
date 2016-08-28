using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiles.Gsm;
using Tiles.Math;
using Tiles.Render.WindowsForms;
using Tiles.ScreenImpl.UI;

namespace Driver.WindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.panelDisplay.Paint += panelDisplay_Paint;

            Canvas = new GraphicsCanvas(gFunc: () => this.G);
            KeyboardSource = new WindowsFormsKeyboardSource();
            this.KeyDown += KeyboardSource.InputKeyPressed;
            KeyboardSource.KeyPressed += KeyboardSource_KeyPressed;

            var screenBox = new Box2(new Vector2(0, 0), new Vector2(80, 24));

            var screenManager = new GameScreenManager();
            screenManager.Add(new ScreenLoadingMenuScreen(Canvas, screenBox));

            KeyboardSource.KeyPressed += screenManager.OnKeyPress;
        }

        List<char> Input = new List<char>();
        void KeyboardSource_KeyPressed(object sender, Tiles.Control.KeyPressEventArgs e)
        {
            Input.Add(e.KeyChar);
            this.Refresh();
        }

        WindowsFormsKeyboardSource KeyboardSource { get; set; }
        GraphicsCanvas Canvas { get; set; }

        Graphics G;
        void panelDisplay_Paint(object sender, PaintEventArgs e)
        {
            G = e.Graphics;
            G.Clear(System.Drawing.Color.Black);
            var bounds = G.VisibleClipBounds;

            int symbolWidth = 10;
            var symbolsWide = bounds.Width / symbolWidth;

            var b = Brushes.BurlyWood;
            var font = new Font("Console", symbolWidth);
            G.DrawString(string.Format("symbols wide : {0}", symbolsWide),
                font, b, 0, 0);
            G.DrawString(string.Join("", Input),
                font, b, 0, symbolWidth);
        }
    }
}
