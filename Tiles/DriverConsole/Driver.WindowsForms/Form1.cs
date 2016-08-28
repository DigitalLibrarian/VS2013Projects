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

using TilesControls = Tiles.Control;

namespace Driver.Tiles.WindowsForms
{
    public partial class Form1 : Form
    {

        ISpriteFontMap FontMap { get; set; }
        IGameScreenManager ScreenManager { get; set; }
        WindowsFormsKeyboardSource KeyboardSource { get; set; }
        GraphicsCanvas Canvas { get; set; }

        Graphics PanelGraphics;
        Graphics BackBufferGraphics;

        BufferedGraphicsContext CurrentContext;
        BufferedGraphics BackBuffer;

        Queue<TilesControls.KeyPressEventArgs> KeyboardQueue = new Queue<TilesControls.KeyPressEventArgs>();

        public Form1()
        {
            InitializeComponent();

            var imagePath = System.Configuration.ConfigurationManager.AppSettings.Get("FontImageFile");
            var image = Image.FromFile(imagePath);

            var solidGlyphIndex = 219;
            var glyphSize = new Vector2(8, 16);
            FontMap = new SpriteFontMap(image, glyphSize, solidGlyphIndex);

            this.PanelGraphics = panelDisplay.CreateGraphics();
            // Gets a reference to the current BufferedGraphicsContext
            CurrentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with Form1, and with 
            // dimensions the same size as the drawing surface of Form1.
            BackBuffer = CurrentContext.Allocate(PanelGraphics,
               this.DisplayRectangle);

            BackBufferGraphics = BackBuffer.Graphics;


            Canvas = new GraphicsCanvas(gFunc: () => this.BackBufferGraphics, fontMap: FontMap);
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

        void KeyboardSource_KeyPressed(object sender, TilesControls.KeyPressEventArgs e)
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
            BackBufferGraphics.Clear(System.Drawing.Color.Black);

            if (first)
            {
                // TODO - fix this
                // has to be done because some screens draw to canvas during their load 
                // to clear the screen.  This really shouldn't be the screen's problem during Load()
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

            BackBuffer.Render();
            // Renders the contents of the buffer to the specified drawing surface.
            BackBuffer.Render(PanelGraphics);
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
