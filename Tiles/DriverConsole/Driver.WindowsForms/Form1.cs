﻿using System;
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
using System.Reflection;

namespace Driver.Tiles.WindowsForms
{
    public partial class Form1 : Form
    {
        ISpriteFontMap FontMap { get; set; }
        IGameScreenManager ScreenManager { get; set; }
        WindowsFormsKeyboardSource KeyboardSource { get; set; }
        GraphicsCanvas Canvas { get; set; }

        Graphics PanelGraphics;
        
        Queue<TilesControls.KeyPressEventArgs> KeyboardQueue = new Queue<TilesControls.KeyPressEventArgs>();

        public Form1()
        {
            InitializeComponent();

            var imagePath = System.Configuration.ConfigurationManager.AppSettings.Get("FontImageFile");
            var image = Image.FromFile(imagePath);

            var solidGlyphIndex = 219;
            var glyphSize = new Vector2(8, 16);
            FontMap = new SpriteFontMap(image, glyphSize, solidGlyphIndex);

            Canvas = new GraphicsCanvas(gFunc: () => this.PanelGraphics, fontMap: FontMap);
            KeyboardSource = new WindowsFormsKeyboardSource();

            ScreenManager = new GameScreenManager();
            KeyboardSource.KeyPressed += KeyboardSource_KeyPressed;

            this.panelDisplay.Paint += panelDisplay_Paint;
            this.KeyDown += KeyboardSource.InputKeyPressed;

            this.timer1.Interval = 10;
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
                lock (KeyboardQueue)
                {
                    KeyboardQueue.Enqueue(e);
                }
                UpdateGame();
                this.Refresh();
            }
        }

        bool first = true;

        void PaintFrame(PaintEventArgs args)
        {
            PanelGraphics = args.Graphics;
            PanelGraphics.Clear(System.Drawing.Color.Black);

            if (first)
            {
                // TODO - fix this
                // has to be done because some screens draw to canvas during their load 
                // to clear the screen.  This really shouldn't be the screen's problem during Load()
                first = false;
                LoadFirstScreen();
            }

            lock (ScreenManager)
            {
                ScreenManager.Draw();
            }
        }

        void panelDisplay_Paint(object sender, PaintEventArgs args)
        {
            try
            {
                PaintFrame(args);
            }
            catch (Exception ex)
            {
                var brak = 0;
            }
        }

        void UpdateGame()
        {
            lock (KeyboardQueue)
            {
                if (KeyboardQueue.Any())
                {
                    ScreenManager.OnKeyPress(this, KeyboardQueue.Dequeue());
                }
            }

            lock (ScreenManager)
            {
                ScreenManager.Update();
                this.Refresh();

                if (!ScreenManager.BlockForInput && !timer1.Enabled)
                {
                    timer1.Start();
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            lock (ScreenManager)
            {
                UpdateGame();
            }
        }
    }
}
