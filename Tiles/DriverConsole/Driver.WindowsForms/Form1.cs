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

        ISpriteFontMap FontMap { get; set; }
        IGameScreenManager ScreenManager { get; set; }
        WindowsFormsKeyboardSource KeyboardSource { get; set; }
        GraphicsCanvas Canvas { get; set; }

        Graphics G;

        public Form1()
        {
            InitializeComponent();


            var imagePath = System.Configuration.ConfigurationManager.AppSettings.Get("FontImageFile");
            var image = Image.FromFile(imagePath);

            var solidGlyphIndex = 219;
            var glyphSize = new Vector2(8, 16);
            FontMap = new SpriteFontMap(image, glyphSize, solidGlyphIndex);

            this.G = panelDisplay.CreateGraphics();
            Canvas = new GraphicsCanvas(gFunc: () => this.G, fontMap: FontMap);
            KeyboardSource = new WindowsFormsKeyboardSource();

            ScreenManager = new GameScreenManager();
            KeyboardSource.KeyPressed += KeyboardSource_KeyPressed;

            this.panelDisplay.Paint += panelDisplay_Paint;
            this.KeyDown += KeyboardSource.InputKeyPressed;
        }

        void LoadFirstScreen()
        {
            var sgW = panelDisplay.Width / FontMap.GlyphSize.X;
            var sgH = panelDisplay.Height / FontMap.GlyphSize.Y;
            var screenBox = new Box2(new Vector2(0, 0), new Vector2(sgW, sgH));
            ScreenManager.Add(new ScreenLoadingMenuScreen(Canvas, screenBox));
        }

        Queue<Tiles.Control.KeyPressEventArgs> KeyboardQueue = new Queue<Tiles.Control.KeyPressEventArgs>();
        void KeyboardSource_KeyPressed(object sender, Tiles.Control.KeyPressEventArgs e)
        {
            if (ScreenManager.BlockForInput)
            {
                KeyboardQueue.Enqueue(e);
                UpdateGame();
                this.Refresh();
            }

        }

        bool first = true;

        void panelDisplay_Paint(object sender, PaintEventArgs e)
        {
            G.Clear(System.Drawing.Color.Black);

            if (first)
            {
                first = false;
                LoadFirstScreen();
            }

            try
            {
                ScreenManager.Draw();
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
            }
        }

        void UpdateGame()
        {
            if (KeyboardQueue.Any())
            {
                ScreenManager.OnKeyPress(this, KeyboardQueue.Dequeue());
            }

            ScreenManager.Update();

            while(!ScreenManager.BlockForInput)
            {
                UpdateGame();
            }
        }
    }
}
