using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
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

            var imagePath = System.Configuration.ConfigurationManager.AppSettings.Get("FontImageFile");
            var image = Image.FromFile(imagePath);

            var glyphSize = new Vector2(8, 16);
            var fontMap = new SpriteFontMap(image, glyphSize);

            Canvas = new GraphicsCanvas(gFunc: () => this.G, fontMap: fontMap);
            KeyboardSource = new WindowsFormsKeyboardSource();
            this.KeyDown += KeyboardSource.InputKeyPressed;
            KeyboardSource.KeyPressed += KeyboardSource_KeyPressed;

            var screenBox = new Box2(new Vector2(0, 0), new Vector2(80, 24));

            ScreenManager = new GameScreenManager();
            ScreenManager.Add(new ScreenLoadingMenuScreen(Canvas, screenBox));

            KeyboardSource.KeyPressed += ScreenManager.OnKeyPress;
        }

        List<char> Input = new List<char>();
        void KeyboardSource_KeyPressed(object sender, Tiles.Control.KeyPressEventArgs e)
        {
            Input.Add(e.KeyChar);
            this.Refresh();
        }
        IGameScreenManager ScreenManager { get; set; }
        WindowsFormsKeyboardSource KeyboardSource { get; set; }
        GraphicsCanvas Canvas { get; set; }

        Graphics G;
        void panelDisplay_Paint(object sender, PaintEventArgs e)
        {
            G = e.Graphics;
            G.Clear(System.Drawing.Color.Black);
            try
            {
                ScreenManager.Draw();
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
            }
            /*
            var bounds = G.VisibleClipBounds;

            int symbolWidth = 10;
            var symbolsWide = bounds.Width / symbolWidth;

            var b = Brushes.BurlyWood;
            var font = new Font("Console", symbolWidth);
            G.DrawString(string.Format("symbols wide : {0}", symbolsWide),
                font, b, 0, 0);
            G.DrawString(string.Join("", Input),
                font, b, 0, symbolWidth);
             * */
        }
    }
}
